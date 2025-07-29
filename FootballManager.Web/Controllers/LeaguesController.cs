using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FootballManager.Models;

namespace FootballManager.Web.Controllers
{
    public class LeaguesController : Controller
    {
        private readonly FootballManager.Data.FootballManagerDbContext _dbContext;

        public LeaguesController(FootballManager.Data.FootballManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            // Get all leagues from the Leagues table
            var leagues = _dbContext.Leagues
                .Include(l => l.Teams)
                .ToList();
            return View(leagues);
        }

        public IActionResult Details(int id)
        {
            // Find league by LeagueId
            var league = _dbContext.Leagues
                .Include(l => l.Teams)
                .ThenInclude(t => t.Players)
                .FirstOrDefault(l => l.Id == id);
            if (league == null)
                return NotFound();

            // Get all matches for this league
            var matches = _dbContext.Matches
                .Where(m => m.LeagueId == id)
                .OrderByDescending(m => m.Date)
                .ToList();

            ViewBag.Matches = matches;
            return View(league);
        }


        [HttpPost]
        public IActionResult SimulateMatchweek(int leagueId)
        {
            // Find league by LeagueId
            var league = _dbContext.Leagues.Include(l => l.Teams).ThenInclude(t => t.Players).FirstOrDefault(l => l.Id == leagueId);
            if (league == null)
                return NotFound();
            var teams = league.Teams.OrderBy(t => t.Id).ToList();
            var rand = new Random();

            int numTeams = teams.Count;
            var firstMatchDate = new DateTime(2025, 8, 10);
            // Generate double round-robin schedule (home/away for each pair)
            var schedule = new List<(int HomeId, int AwayId, DateTime Date)>();
            int totalRounds = (numTeams - 1) * 2;
            int gamesPerWeek = numTeams / 2;
            // Berger tables for first round (home/away)
            var teamIds = teams.Select(t => t.Id).ToList();
            if (numTeams % 2 != 0) teamIds.Add(-1); // bye if odd
            int n = teamIds.Count;
            for (int round = 0; round < n - 1; round++)
            {
                for (int i = 0; i < n / 2; i++)
                {
                    int t1 = teamIds[i];
                    int t2 = teamIds[n - 1 - i];
                    if (t1 != -1 && t2 != -1)
                    {
                        var date1 = firstMatchDate.AddDays(7 * round);
                        var date2 = firstMatchDate.AddDays(7 * (round + n - 1));
                        schedule.Add((t1, t2, date1)); // first half
                        schedule.Add((t2, t1, date2)); // second half (reverse fixture)
                    }
                }
                // rotate
                var temp = teamIds[n - 1];
                for (int i = n - 1; i > 1; i--)
                    teamIds[i] = teamIds[i - 1];
                teamIds[1] = temp;
            }

            // Find the next matchweek to simulate (first week with unplayed matches)
            var playedMatches = _dbContext.Matches.Where(m => m.LeagueId == leagueId).ToList();
            var playedSet = new HashSet<string>(playedMatches.Select(m => $"{m.HomeTeamId}_{m.AwayTeamId}_{m.Date.Date:yyyy-MM-dd}"));
            var scheduleWeeks = schedule.GroupBy(s => s.Date).OrderBy(g => g.Key).ToList();
            var nextWeek = scheduleWeeks.FirstOrDefault(week => week.Any(fix => !playedSet.Contains($"{fix.HomeId}_{fix.AwayId}_{fix.Date.Date:yyyy-MM-dd}")));
            if (nextWeek == null)
                return RedirectToAction("Details", new { id = leagueId }); // All weeks played

            foreach (var fixture in nextWeek)
            {
                // Only simulate if not already played
                string matchKey = $"{fixture.HomeId}_{fixture.AwayId}_{fixture.Date.Date:yyyy-MM-dd}";
                if (playedSet.Contains(matchKey)) continue;
                var home = teams.First(t => t.Id == fixture.HomeId);
                var away = teams.First(t => t.Id == fixture.AwayId);
                int homeGoals = rand.Next(0, 5);
                int awayGoals = rand.Next(0, 5);
                home.GoalsFor += homeGoals;
                home.GoalsAgainst += awayGoals;
                away.GoalsFor += awayGoals;
                away.GoalsAgainst += homeGoals;
                home.GamesPlayed++;
                away.GamesPlayed++;

                // Save match to database
                var match = new FootballManager.Models.Match
                {
                    LeagueId = leagueId,
                    HomeTeamId = home.Id,
                    AwayTeamId = away.Id,
                    HomeGoals = homeGoals,
                    AwayGoals = awayGoals,
                    Date = fixture.Date
                };
                _dbContext.Matches.Add(match);
                _dbContext.SaveChanges(); // To get match.Id for PlayerMatchStat

                // --- Realistic Player stats update (per match) ---
                List<Player> SelectStarters(List<Player> players)
                {
                    var positions = new[] {
                        "goalkeeper",
                        "left back", "center back", "center back", "right back",
                        "central midfielder", "central midfielder", "central midfielder",
                        "left winger", "striker", "right winger"
                    };
                    var selected = new List<Player>();
                    var used = new HashSet<int>();
                    foreach (var pos in positions)
                    {
                        var best = players
                            .Where(p => p.Position != null && p.Position.ToLower().Contains(pos) && !used.Contains(p.Id))
                            .OrderByDescending(p => p.MarketValue)
                            .FirstOrDefault();
                        if (best != null)
                        {
                            selected.Add(best);
                            used.Add(best.Id);
                        }
                        else
                        {
                            var fallback = players.Where(p => !used.Contains(p.Id)).OrderByDescending(p => p.MarketValue).FirstOrDefault();
                            if (fallback != null)
                            {
                                selected.Add(fallback);
                                used.Add(fallback.Id);
                            }
                        }
                    }
                    return selected;
                }
                var homeStarters = SelectStarters(home.Players);
                var awayStarters = SelectStarters(away.Players);
                var homeSubs = home.Players.Except(homeStarters).OrderByDescending(p => p.MarketValue).Take(5).ToList();
                var awaySubs = away.Players.Except(awayStarters).OrderByDescending(p => p.MarketValue).Take(5).ToList();

                // --- Realistic goal/assist distribution ---
                List<Player> PickScorers(List<Player> starters, int goals)
                {
                    var forwards = starters.Where(p => p.Position.ToLower().Contains("striker") || p.Position.ToLower().Contains("winger")).ToList();
                    var mids = starters.Where(p => p.Position.ToLower().Contains("midfielder")).ToList();
                    var defs = starters.Where(p => p.Position.ToLower().Contains("back")).ToList();
                    var all = starters.ToList();
                    var scorers = new List<Player>();
                    for (int g = 0; g < goals; g++)
                    {
                        double r = rand.NextDouble();
                        Player scorer = null;
                        if (r < 0.6 && forwards.Count > 0) scorer = forwards[rand.Next(forwards.Count)];
                        else if (r < 0.9 && mids.Count > 0) scorer = mids[rand.Next(mids.Count)];
                        else if (defs.Count > 0) scorer = defs[rand.Next(defs.Count)];
                        else scorer = all[rand.Next(all.Count)];
                        scorers.Add(scorer);
                    }
                    return scorers;
                }
                var homeScorers = PickScorers(homeStarters, homeGoals);
                var homeAssisters = new List<Player>();
                for (int g = 0; g < homeGoals; g++)
                {
                    var possibleAssists = homeStarters.Where(p => p != homeScorers[g]).ToList();
                    if (possibleAssists.Count > 0)
                        homeAssisters.Add(possibleAssists[rand.Next(possibleAssists.Count)]);
                }
                var awayScorers = PickScorers(awayStarters, awayGoals);
                var awayAssisters = new List<Player>();
                for (int g = 0; g < awayGoals; g++)
                {
                    var possibleAssists = awayStarters.Where(p => p != awayScorers[g]).ToList();
                    if (possibleAssists.Count > 0)
                        awayAssisters.Add(possibleAssists[rand.Next(possibleAssists.Count)]);
                }

                // Clean sheets for goalkeepers (from starters)
                var homeGk = homeStarters.FirstOrDefault(p => p.Position != null && p.Position.ToLower().Contains("goalkeeper"));
                var awayGk = awayStarters.FirstOrDefault(p => p.Position != null && p.Position.ToLower().Contains("goalkeeper"));
                bool homeCleanSheet = (awayGoals == 0 && homeGk != null);
                bool awayCleanSheet = (homeGoals == 0 && awayGk != null);

                // --- Cards and injuries ---
                Dictionary<int, (int yellow, int red, int inj)> AssignCardsAndInjuries(List<Player> starters, List<Player> subs)
                {
                    var dict = new Dictionary<int, (int, int, int)>();
                    foreach (var p in starters)
                    {
                        int y = rand.NextDouble() < 0.12 ? 1 : 0;
                        int r = rand.NextDouble() < 0.02 ? 1 : 0;
                        int inj = rand.NextDouble() < 0.01 ? 1 : 0;
                        dict[p.Id] = (y, r, inj);
                    }
                    foreach (var p in subs)
                    {
                        int y = rand.NextDouble() < 0.05 ? 1 : 0;
                        int r = rand.NextDouble() < 0.01 ? 1 : 0;
                        int inj = rand.NextDouble() < 0.005 ? 1 : 0;
                        dict[p.Id] = (y, r, inj);
                    }
                    return dict;
                }
                var homeCardDict = AssignCardsAndInjuries(homeStarters, homeSubs);
                var awayCardDict = AssignCardsAndInjuries(awayStarters, awaySubs);

                // --- Realistic match ratings ---
                double GetMatchRating(Player p, int goalsFor, int goalsAgainst, int goals, int assists, int yellow, int red, int inj, bool cleanSheet)
                {
                    double perf = 6.5;
                    if (p.Position.ToLower().Contains("goalkeeper"))
                    {
                        if (cleanSheet) perf += 1.0;
                    }
                    if (p.Position.ToLower().Contains("striker") || p.Position.ToLower().Contains("winger"))
                    {
                        perf += 0.6 * goals + 0.3 * assists;
                    }
                    else if (p.Position.ToLower().Contains("midfielder"))
                    {
                        perf += 0.4 * goals + 0.5 * assists;
                    }
                    else if (p.Position.ToLower().Contains("back"))
                    {
                        perf += 0.2 * goals + 0.2 * assists;
                        if (cleanSheet) perf += 0.3;
                    }
                    perf -= 0.3 * yellow;
                    perf -= 1.0 * red;
                    if (inj > 0) perf -= 0.5;
                    perf = Math.Max(4.0, Math.Min(10.0, perf + rand.NextDouble() * 0.5 - 0.25));
                    return perf;
                }

                // --- Create PlayerMatchStat for all players ---
                void CreatePlayerMatchStats(List<Player> starters, List<Player> subs, int teamId, int goalsFor, int goalsAgainst, List<Player> scorers, List<Player> assisters, Dictionary<int, (int yellow, int red, int inj)> cardDict, bool cleanSheet)
                {
                    foreach (var p in starters)
                    {
                        int goals = scorers.Count(x => x.Id == p.Id);
                        int assists = assisters.Count(x => x.Id == p.Id);
                        int yellow = cardDict.ContainsKey(p.Id) ? cardDict[p.Id].yellow : 0;
                        int red = cardDict.ContainsKey(p.Id) ? cardDict[p.Id].red : 0;
                        int inj = cardDict.ContainsKey(p.Id) ? cardDict[p.Id].inj : 0;
                        int mins = 90;
                        double rating = GetMatchRating(p, goalsFor, goalsAgainst, goals, assists, yellow, red, inj, cleanSheet);
                        var stat = new FootballManager.Models.PlayerMatchStat
                        {
                            PlayerId = p.Id,
                            MatchId = match.Id,
                            TeamId = teamId,
                            Goals = goals,
                            Assists = assists,
                            YellowCards = yellow,
                            RedCards = red,
                            Injuries = inj,
                            MinutesPlayed = mins,
                            Starter = true,
                            MatchRating = rating,
                            CleanSheets = cleanSheet ? 1 : 0
                        };
                        _dbContext.PlayerMatchStats.Add(stat);
                    }
                    foreach (var p in subs)
                    {
                        int goals = 0, assists = 0, yellow = 0, red = 0, inj = 0;
                        int mins = rand.Next(10, 30);
                        if (cardDict.ContainsKey(p.Id)) { yellow = cardDict[p.Id].yellow; red = cardDict[p.Id].red; inj = cardDict[p.Id].inj; }
                        double rating = GetMatchRating(p, goalsFor, goalsAgainst, goals, assists, yellow, red, inj, cleanSheet);
                        var stat = new FootballManager.Models.PlayerMatchStat
                        {
                            PlayerId = p.Id,
                            MatchId = match.Id,
                            TeamId = teamId,
                            Goals = goals,
                            Assists = assists,
                            YellowCards = yellow,
                            RedCards = red,
                            Injuries = inj,
                            MinutesPlayed = mins,
                            Starter = false,
                            MatchRating = rating,
                            CleanSheets = 0
                        };
                        _dbContext.PlayerMatchStats.Add(stat);
                    }
                }
                CreatePlayerMatchStats(homeStarters, homeSubs, home.Id, homeGoals, awayGoals, homeScorers, homeAssisters, homeCardDict, homeCleanSheet);
                CreatePlayerMatchStats(awayStarters, awaySubs, away.Id, awayGoals, homeGoals, awayScorers, awayAssisters, awayCardDict, awayCleanSheet);

                // Update season stats as before (optional, for season totals)
                foreach (var p in homeStarters) { p.GamesPlayed++; p.MinutesPlayed += 90; }
                foreach (var p in awayStarters) { p.GamesPlayed++; p.MinutesPlayed += 90; }
                foreach (var p in homeSubs) { p.GamesPlayed++; p.MinutesPlayed += 15; }
                foreach (var p in awaySubs) { p.GamesPlayed++; p.MinutesPlayed += 15; }
                foreach (var scorer in homeScorers) scorer.Goals++;
                foreach (var scorer in awayScorers) scorer.Goals++;
                foreach (var assister in homeAssisters) assister.Assists++;
                foreach (var assister in awayAssisters) assister.Assists++;
                if (homeCleanSheet && homeGk != null) homeGk.CleanSheets++;
                if (awayCleanSheet && awayGk != null) awayGk.CleanSheets++;

                // --- Team results ---
                if (homeGoals > awayGoals)
                {
                    home.Wins++;
                    away.Losses++;
                    home.RecentResults.Insert(0, 'W');
                    away.RecentResults.Insert(0, 'L');
                }
                else if (homeGoals < awayGoals)
                {
                    away.Wins++;
                    home.Losses++;
                    home.RecentResults.Insert(0, 'L');
                    away.RecentResults.Insert(0, 'W');
                }
                else
                {
                    home.Draws++;
                    away.Draws++;
                    home.RecentResults.Insert(0, 'D');
                    away.RecentResults.Insert(0, 'D');
                }
                if (home.RecentResults.Count > 5) home.RecentResults.RemoveAt(5);
                if (away.RecentResults.Count > 5) away.RecentResults.RemoveAt(5);
                _dbContext.SaveChanges();
            }

            _dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = leagueId });
        }

        [HttpPost]
        public IActionResult SimulateSeason(int leagueId)
        {
            // Find league by LeagueId
            var league = _dbContext.Leagues.Include(l => l.Teams).FirstOrDefault(l => l.Id == leagueId);
            if (league == null)
                return NotFound();
            var teams = league.Teams.ToList();
            var rand = new Random();

            // Reset stats
            foreach (var team in teams)
            {
                team.Wins = 0;
                team.Draws = 0;
                team.Losses = 0;
                team.GoalsFor = 0;
                team.GoalsAgainst = 0;
            }

            // Simulate each pair of teams playing each other twice (home/away)
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = 0; j < teams.Count; j++)
                {
                    if (i == j) continue;
                    var home = teams[i];
                    var away = teams[j];
                    int homeGoals = rand.Next(0, 5);
                    int awayGoals = rand.Next(0, 5);
                    home.GoalsFor += homeGoals;
                    home.GoalsAgainst += awayGoals;
                    away.GoalsFor += awayGoals;
                    away.GoalsAgainst += homeGoals;
                    if (homeGoals > awayGoals)
                    {
                        home.Wins++;
                        away.Losses++;
                    }
                    else if (homeGoals < awayGoals)
                    {
                        away.Wins++;
                        home.Losses++;
                    }
                    else
                    {
                        home.Draws++;
                        away.Draws++;
                    }
                }
            }

            _dbContext.SaveChanges();
            return RedirectToAction("Details", new { id = leagueId });
        }
    }
}
