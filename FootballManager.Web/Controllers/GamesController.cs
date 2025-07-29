using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FootballManager.Models;
using System.Linq;

namespace FootballManager.Web.Controllers
{
    public class GamesController : Controller
    {
        private readonly FootballManager.Data.FootballManagerDbContext _dbContext;
        public GamesController(FootballManager.Data.FootballManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Details(int id)
        {
            var match = _dbContext.Matches.FirstOrDefault(m => m.Id == id);
            if (match == null) return NotFound();

            // Get teams
            var home = _dbContext.Teams.Include(t => t.Players).FirstOrDefault(t => t.Id == match.HomeTeamId);
            var away = _dbContext.Teams.Include(t => t.Players).FirstOrDefault(t => t.Id == match.AwayTeamId);
            match.HomeTeam = home;
            match.AwayTeam = away;
            match.HomePlayers = home?.Players?.ToList() ?? new List<Player>();
            match.AwayPlayers = away?.Players?.ToList() ?? new List<Player>();

            // Load simulation state from session if present
            var simState = HttpContext.Session.GetObjectFromJson<FootballManager.Web.Models.GameSimulationState>($"sim_match_{id}");
            if (simState != null)
            {
                ViewBag.CurrentMinute = simState.CurrentMinute;
                ViewBag.SimulationEvents = simState.Events;
            }
            return View(match);
        }

        [HttpPost]
        public IActionResult SimulateMinute(int id)
        {
            // Load or create simulation state
            var simState = HttpContext.Session.GetObjectFromJson<FootballManager.Web.Models.GameSimulationState>($"sim_match_{id}") ?? new FootballManager.Web.Models.GameSimulationState { MatchId = id };
            if (simState.CurrentMinute >= 90)
            {
                TempData["Message"] = "Match already finished.";
                return RedirectToAction("Details", new { id });
            }
            // Simulate one minute (for demo, random event or nothing)
            var rand = new Random();
            string ev = null;
            if (rand.NextDouble() < 0.07) // 7% chance of event
            {
                var eventTypes = new[] { "Goal", "Yellow Card", "Red Card", "Injury", "Chance Missed" };
                var type = eventTypes[rand.Next(eventTypes.Length)];
                ev = $"Minute {simState.CurrentMinute + 1}: {type}!";
                simState.Events.Add(ev);
            }
            simState.CurrentMinute++;
            // Save back to session
            HttpContext.Session.SetObjectAsJson($"sim_match_{id}", simState);
            return RedirectToAction("Details", new { id });
        }
    }
}
