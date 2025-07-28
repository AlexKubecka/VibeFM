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

                // --- Player stats update ---
                void UpdateGamesPlayed(List<Player> players)
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
                    var subs = players.Where(p => !used.Contains(p.Id)).OrderByDescending(p => p.MarketValue).Take(5).ToList();
                    foreach (var p in selected.Concat(subs)) p.GamesPlayed++;
                }
                UpdateGamesPlayed(home.Players);
                UpdateGamesPlayed(away.Players);

                // Randomly assign goals and assists for home team (from starting 11 only)
                var homeStarters = home.Players.OrderByDescending(p => p.MarketValue).Take(11).ToList();
                var homeScorers = homeStarters.OrderBy(_ => rand.Next()).Take(homeGoals).ToList();
                foreach (var scorer in homeScorers) scorer.Goals++;
                for (int g = 0; g < homeGoals; g++)
                {
                    var possibleAssists = homeStarters.Where(p => !homeScorers.Skip(g).Take(1).Contains(p)).ToList();
                    if (possibleAssists.Count > 0)
                        possibleAssists[rand.Next(possibleAssists.Count)].Assists++;
                }

                // Randomly assign goals and assists for away team (from starting 11 only)
                var awayStarters = away.Players.OrderByDescending(p => p.MarketValue).Take(11).ToList();
                var awayScorers = awayStarters.OrderBy(_ => rand.Next()).Take(awayGoals).ToList();
                foreach (var scorer in awayScorers) scorer.Goals++;
                for (int g = 0; g < awayGoals; g++)
                {
                    var possibleAssists = awayStarters.Where(p => !awayScorers.Skip(g).Take(1).Contains(p)).ToList();
                    if (possibleAssists.Count > 0)
                        possibleAssists[rand.Next(possibleAssists.Count)].Assists++;
                }

                // Clean sheets for goalkeepers (from starters)
                var homeGk = homeStarters.FirstOrDefault(p => p.Position != null && p.Position.ToLower().Contains("goalkeeper"));
                var awayGk = awayStarters.FirstOrDefault(p => p.Position != null && p.Position.ToLower().Contains("goalkeeper"));
                if (awayGoals == 0 && homeGk != null) homeGk.CleanSheets++;
                if (homeGoals == 0 && awayGk != null) awayGk.CleanSheets++;

                foreach (var p in home.Players)
                {
                    double perf = 6.5 + 0.5 * p.Goals + 0.3 * p.Assists;
                    if (p.Position != null && p.Position.ToLower().Contains("goalkeeper") && awayGoals == 0) perf += 1;
                    p.AddMatchRating(perf);
                }
                foreach (var p in away.Players)
                {
                    double perf = 6.5 + 0.5 * p.Goals + 0.3 * p.Assists;
                    if (p.Position != null && p.Position.ToLower().Contains("goalkeeper") && homeGoals == 0) perf += 1;
                    p.AddMatchRating(perf);
                }

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
