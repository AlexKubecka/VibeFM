using FootballManager.Web.Models;
        
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballManager.Models;
using System;
using System.Linq;

namespace FootballManager.Web.Controllers
{
    public class DevController : Controller
    {
        private readonly FootballManager.Data.FootballManagerDbContext _dbContext;
        public DevController(FootballManager.Data.FootballManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult SimulateGame()
        {
            ViewBag.Teams = _dbContext.Teams.OrderBy(t => t.Name).ToList();
            return View();
        }


        [HttpPost]
        public IActionResult StartMinuteSimulation(int homeTeamId, int awayTeamId, DateTime? date)
        {
            if (homeTeamId == awayTeamId) return BadRequest("Teams must be different");
            var home = _dbContext.Teams.Include(t => t.Players).FirstOrDefault(t => t.Id == homeTeamId);
            var away = _dbContext.Teams.Include(t => t.Players).FirstOrDefault(t => t.Id == awayTeamId);
            if (home == null || away == null) return NotFound();
            // Select starters (same logic as before)
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

            // Initialize simulation state
            var simState = new GameSimulationState
            {
                HomeTeamId = home.Id,
                AwayTeamId = away.Id,
                HomeTeamName = home.Name,
                AwayTeamName = away.Name,
                CurrentMinute = 0,
                HomePlayers = homeStarters.Select(p => new PlayerSimState { PlayerId = p.Id, Name = p.Name, Position = p.Position, Starter = true }).ToList()
                    .Concat(homeSubs.Select(p => new PlayerSimState { PlayerId = p.Id, Name = p.Name, Position = p.Position, Starter = false })).ToList(),
                AwayPlayers = awayStarters.Select(p => new PlayerSimState { PlayerId = p.Id, Name = p.Name, Position = p.Position, Starter = true }).ToList()
                    .Concat(awaySubs.Select(p => new PlayerSimState { PlayerId = p.Id, Name = p.Name, Position = p.Position, Starter = false })).ToList(),
                Events = new List<string>(),
                HomeScore = 0,
                AwayScore = 0
            };
            HttpContext.Session.SetObjectAsJson("dev_sim_state", simState);
            return RedirectToAction("MinuteSimulation");
        }

        [HttpGet]
        public IActionResult MinuteSimulation()
        {
            var simState = HttpContext.Session.GetObjectFromJson<GameSimulationState>("dev_sim_state");
            if (simState == null)
            {
                return RedirectToAction("SimulateGame");
            }
            ViewBag.SimState = simState;
            return View();
        }

        [HttpPost]
        public IActionResult SimulateNextMinute()
        {
            var simState = HttpContext.Session.GetObjectFromJson<GameSimulationState>("dev_sim_state");
            if (simState == null) return RedirectToAction("SimulateGame");
            if (simState.CurrentMinute >= 90)
            {
                TempData["Message"] = "Match already finished.";
                return RedirectToAction("MinuteSimulation");
            }
            var rand = new Random();
            // Example: 5% chance of goal, 3% yellow, 1% red, 1% injury
            string ev = null;
            if (rand.NextDouble() < 0.05)
            {
                // Random team scores
                bool homeGoal = rand.Next(2) == 0;
                var scorerList = homeGoal ? simState.HomePlayers.Where(p => p.Starter).ToList() : simState.AwayPlayers.Where(p => p.Starter).ToList();
                var scorer = scorerList[rand.Next(scorerList.Count)];
                scorer.Goals++;
                if (homeGoal) simState.HomeScore++; else simState.AwayScore++;
                ev = $"Minute {simState.CurrentMinute + 1}: GOAL! {scorer.Name} ({(homeGoal ? simState.HomeTeamName : simState.AwayTeamName)})";
            }
            else if (rand.NextDouble() < 0.03)
            {
                // Yellow card
                bool home = rand.Next(2) == 0;
                var playerList = home ? simState.HomePlayers : simState.AwayPlayers;
                var player = playerList[rand.Next(playerList.Count)];
                player.YellowCards++;
                ev = $"Minute {simState.CurrentMinute + 1}: Yellow card for {player.Name} ({(home ? simState.HomeTeamName : simState.AwayTeamName)})";
            }
            else if (rand.NextDouble() < 0.01)
            {
                // Red card
                bool home = rand.Next(2) == 0;
                var playerList = home ? simState.HomePlayers : simState.AwayPlayers;
                var player = playerList[rand.Next(playerList.Count)];
                player.RedCards++;
                ev = $"Minute {simState.CurrentMinute + 1}: Red card for {player.Name} ({(home ? simState.HomeTeamName : simState.AwayTeamName)})";
            }
            else if (rand.NextDouble() < 0.01)
            {
                // Injury
                bool home = rand.Next(2) == 0;
                var playerList = home ? simState.HomePlayers : simState.AwayPlayers;
                var player = playerList[rand.Next(playerList.Count)];
                ev = $"Minute {simState.CurrentMinute + 1}: Injury for {player.Name} ({(home ? simState.HomeTeamName : simState.AwayTeamName)})";
            }
            if (ev != null) simState.Events.Add(ev);
            // Update ratings for all players
            foreach (var p in simState.HomePlayers)
            {
                p.MatchRating = 6.5 + p.Goals * 0.5 + p.Assists * 0.3 - p.RedCards * 1.0 - p.YellowCards * 0.2;
                if (p.Position != null && p.Position.ToLower().Contains("goalkeeper") && simState.AwayScore == 0)
                    p.MatchRating += 0.5; // bonus for clean sheet
            }
            foreach (var p in simState.AwayPlayers)
            {
                p.MatchRating = 6.5 + p.Goals * 0.5 + p.Assists * 0.3 - p.RedCards * 1.0 - p.YellowCards * 0.2;
                if (p.Position != null && p.Position.ToLower().Contains("goalkeeper") && simState.HomeScore == 0)
                    p.MatchRating += 0.5;
            }
            simState.CurrentMinute++;
            HttpContext.Session.SetObjectAsJson("dev_sim_state", simState);
            return RedirectToAction("MinuteSimulation");
        }

        [HttpPost]
        public IActionResult SimulateFullGame()
        {
            var simState = HttpContext.Session.GetObjectFromJson<GameSimulationState>("dev_sim_state");
            if (simState == null) return RedirectToAction("SimulateGame");
            var rand = new Random();
            while (simState.CurrentMinute < 90)
            {
                string ev = null;
                if (rand.NextDouble() < 0.05)
                {
                    bool homeGoal = rand.Next(2) == 0;
                    var scorerList = homeGoal ? simState.HomePlayers.Where(p => p.Starter).ToList() : simState.AwayPlayers.Where(p => p.Starter).ToList();
                    var scorer = scorerList[rand.Next(scorerList.Count)];
                    scorer.Goals++;
                    if (homeGoal) simState.HomeScore++; else simState.AwayScore++;
                    ev = $"Minute {simState.CurrentMinute + 1}: GOAL! {scorer.Name} ({(homeGoal ? simState.HomeTeamName : simState.AwayTeamName)})";
                }
                else if (rand.NextDouble() < 0.03)
                {
                    bool home = rand.Next(2) == 0;
                    var playerList = home ? simState.HomePlayers : simState.AwayPlayers;
                    var player = playerList[rand.Next(playerList.Count)];
                    player.YellowCards++;
                    ev = $"Minute {simState.CurrentMinute + 1}: Yellow card for {player.Name} ({(home ? simState.HomeTeamName : simState.AwayTeamName)})";
                }
                else if (rand.NextDouble() < 0.01)
                {
                    bool home = rand.Next(2) == 0;
                    var playerList = home ? simState.HomePlayers : simState.AwayPlayers;
                    var player = playerList[rand.Next(playerList.Count)];
                    player.RedCards++;
                    ev = $"Minute {simState.CurrentMinute + 1}: Red card for {player.Name} ({(home ? simState.HomeTeamName : simState.AwayTeamName)})";
                }
                else if (rand.NextDouble() < 0.01)
                {
                    bool home = rand.Next(2) == 0;
                    var playerList = home ? simState.HomePlayers : simState.AwayPlayers;
                    var player = playerList[rand.Next(playerList.Count)];
                    ev = $"Minute {simState.CurrentMinute + 1}: Injury for {player.Name} ({(home ? simState.HomeTeamName : simState.AwayTeamName)})";
                }
                if (ev != null) simState.Events.Add(ev);
                // Update ratings for all players
                foreach (var p in simState.HomePlayers)
                {
                    p.MatchRating = 6.5 + p.Goals * 0.5 + p.Assists * 0.3 - p.RedCards * 1.0 - p.YellowCards * 0.2;
                    if (p.Position != null && p.Position.ToLower().Contains("goalkeeper") && simState.AwayScore == 0)
                        p.MatchRating += 0.5;
                }
                foreach (var p in simState.AwayPlayers)
                {
                    p.MatchRating = 6.5 + p.Goals * 0.5 + p.Assists * 0.3 - p.RedCards * 1.0 - p.YellowCards * 0.2;
                    if (p.Position != null && p.Position.ToLower().Contains("goalkeeper") && simState.HomeScore == 0)
                        p.MatchRating += 0.5;
                }
                simState.CurrentMinute++;
            }
            // Save to session for display
            HttpContext.Session.SetObjectAsJson("dev_sim_state", simState);

            // At the end, save stats to the database
            // Create a Match entity
            var match = new Match
            {
                LeagueId = 0, // Dev tool, not linked to a league
                HomeTeamId = simState.HomeTeamId,
                AwayTeamId = simState.AwayTeamId,
                HomeGoals = simState.HomeScore,
                AwayGoals = simState.AwayScore,
                Date = DateTime.Now
            };
            _dbContext.Matches.Add(match);
            _dbContext.SaveChanges();

            // Save PlayerMatchStat for each player
            foreach (var p in simState.HomePlayers)
            {
                var stat = new PlayerMatchStat
                {
                    PlayerId = p.PlayerId,
                    MatchId = match.Id,
                    TeamId = simState.HomeTeamId,
                    Goals = p.Goals,
                    Assists = p.Assists,
                    YellowCards = p.YellowCards,
                    RedCards = p.RedCards,
                    MinutesPlayed = 90,
                    Starter = p.Starter,
                    MatchRating = 6.5 + p.Goals * 0.5 - p.RedCards * 1.0 - p.YellowCards * 0.2, // Simple rating
                    CleanSheets = (simState.AwayScore == 0 && p.Position.ToLower().Contains("goalkeeper")) ? 1 : 0
                };
                _dbContext.PlayerMatchStats.Add(stat);
            }
            foreach (var p in simState.AwayPlayers)
            {
                var stat = new PlayerMatchStat
                {
                    PlayerId = p.PlayerId,
                    MatchId = match.Id,
                    TeamId = simState.AwayTeamId,
                    Goals = p.Goals,
                    Assists = p.Assists,
                    YellowCards = p.YellowCards,
                    RedCards = p.RedCards,
                    MinutesPlayed = 90,
                    Starter = p.Starter,
                    MatchRating = 6.5 + p.Goals * 0.5 - p.RedCards * 1.0 - p.YellowCards * 0.2,
                    CleanSheets = (simState.HomeScore == 0 && p.Position.ToLower().Contains("goalkeeper")) ? 1 : 0
                };
                _dbContext.PlayerMatchStats.Add(stat);
            }
            _dbContext.SaveChanges();

            TempData["Message"] = "Full game simulated and saved.";
            return RedirectToAction("MinuteSimulation");
        }
    }
}
