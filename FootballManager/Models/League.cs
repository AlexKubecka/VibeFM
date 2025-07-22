using System;
using System.Collections.Generic;
using System.Linq;
using FootballManager.Models;

namespace FootballManager.Models
{
    public class League
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();

        public League(string name)
        {
            Name = name;
        }

        // Simulate matches between all teams (home and away)
        public void SimulateLeague()
        {
            Dictionary<Team, int> points = Teams.ToDictionary(team => team, team => 0);

            foreach (var team1 in Teams)
            {
                foreach (var team2 in Teams)
                {
                    if (team1 != team2)
                    {
                        // Simulate home match
                        int homeResult = SimulateMatch(team1, team2);
                        points[team1] += homeResult;

                        // Simulate away match
                        int awayResult = SimulateMatch(team2, team1);
                        points[team2] += awayResult;
                    }
                }
            }

            // Display league table
            DisplayLeagueTable(points);
        }

        // Simulate a match between two teams
        private int SimulateMatch(Team homeTeam, Team awayTeam)
        {
            double homeRating = homeTeam.CalculateTeamOverallRating();
            double awayRating = awayTeam.CalculateTeamOverallRating();

            // Simple formula: higher rating wins, draw if ratings are close
            if (homeRating > awayRating + 5) return 3; // Home team wins
            if (awayRating > homeRating + 5) return 0; // Away team wins
            return 1; // Draw
        }

        // Display the league table
        private void DisplayLeagueTable(Dictionary<Team, int> points)
        {
            Console.WriteLine($"League Table: {Name}");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Team\t\tPoints");
            Console.WriteLine("-----------------------------------");

            foreach (var team in points.OrderByDescending(p => p.Value))
            {
                Console.WriteLine($"{team.Key.Name}\t\t{team.Value}");
            }
        }
    }
}