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
                .AsNoTracking()
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
                    Wins = t.Wins,
                    Draws = t.Draws,
                    Losses = t.Losses,
                    GoalsFor = t.GoalsFor,
                    GoalsAgainst = t.GoalsAgainst,
                    Players = _dbContext.Players.Where(p => p.TeamId == t.Id).ToList()
                })
                .ToList();

            return View(league);
        }

        [HttpPost]
        public IActionResult SimulateSeason(int leagueId)
        {
            // Get league name by id (index in list)
            var leagueNames = _dbContext.Teams
                .Select(t => t.LeagueName)
                .Distinct()
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();
            if (leagueId < 1 || leagueId > leagueNames.Count)
                return NotFound();
            var leagueName = leagueNames[leagueId - 1];

            // Get all teams in this league
            var teams = _dbContext.Teams.Where(t => t.LeagueName == leagueName).ToList();
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
