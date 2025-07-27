using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            // Get all unique league names from teams
            var leagueNames = _dbContext.Teams
                .Select(t => t.LeagueName)
                .Distinct()
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            // For each league name, get the first team's Nationality as the Nation
            var leagues = leagueNames.Select((name, idx) => {
                var nation = _dbContext.Teams.FirstOrDefault(t => t.LeagueName == name)?.Nationality ?? "";
                return new League(name, nation) { Id = idx + 1 };
            }).ToList();
            return View(leagues);
        }

        public IActionResult Details(int id)
        {
            // Get league name by id (index in list)
            var leagueNames = _dbContext.Teams
                .Select(t => t.LeagueName)
                .Distinct()
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();
            if (id < 1 || id > leagueNames.Count)
                return NotFound();
            var leagueName = leagueNames[id - 1];

            // Get the nation from the first team in this league
            var nation = _dbContext.Teams.FirstOrDefault(t => t.LeagueName == leagueName)?.Nationality ?? "";

            var league = new League(leagueName, nation) { Id = id };
            league.Teams = _dbContext.Teams
                .Where(t => t.LeagueName == leagueName)
                .Select(t => new Team
                {
                    Id = t.Id,
                    Name = t.Name,
                    Nationality = t.Nationality,
                    StadiumName = t.StadiumName,
                    StadiumCapacity = t.StadiumCapacity,
                    Value = t.Value,
                    LeagueName = t.LeagueName,
                    Players = _dbContext.Players.Where(p => p.TeamId == t.Id).ToList()
                })
                .ToList();

            return View(league);
        }
    }
}
