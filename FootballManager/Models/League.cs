using System;
using System.Collections.Generic;
using System.Linq;
using FootballManager.Models;
using FootballManager.Utilities;

namespace FootballManager.Models
{
    public class League
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nation { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();

        public League(string name, string nation)
        {
            Name = name;
            Nation = nation;
        }

        // Simulate matches between all teams (home and away)
        public void SimulateLeague()
        {
            Logger.Log($"Simulating league: {Name}");
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

                        Logger.Log($"Match simulated: {team1.Name} vs {team2.Name} (Home: {homeResult}, Away: {awayResult})");
                    }
                }
            }

            Logger.Log($"League simulation completed for {Name}");
            LogLeagueTable(points);
        }

        // Simulate a match between two teams
        private int SimulateMatch(Team homeTeam, Team awayTeam)
        {
            double homeRating = homeTeam.CalculateTeamOverallRating();
            double awayRating = awayTeam.CalculateTeamOverallRating();

            Logger.Log($"Simulating match: {homeTeam.Name} (Rating: {homeRating}) vs {awayTeam.Name} (Rating: {awayRating})");

            // Simple formula: higher rating wins, draw if ratings are close
            if (homeRating > awayRating + 5)
            {
                Logger.Log($"{homeTeam.Name} wins at home.");
                return 3; // Home team wins
            }
            if (awayRating > homeRating + 5)
            {
                Logger.Log($"{awayTeam.Name} wins away.");
                return 0; // Away team wins
            }

            Logger.Log($"Match ends in a draw.");
            return 1; // Draw
        }

        // Log the league table (for debugging purposes)
        private void LogLeagueTable(Dictionary<Team, int> points)
        {
            Logger.Log($"League Table: {Name}");
            Logger.Log("-----------------------------------");
            Logger.Log("Team\t\tPoints");
            Logger.Log("-----------------------------------");

            foreach (var team in points.OrderByDescending(p => p.Value))
            {
                Logger.Log($"{team.Key.Name}\t\t{team.Value}");
            }
        }

        // Print the league table (dedicated printing function)
        public void PrintLeagueTable(Dictionary<Team, int> points)
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