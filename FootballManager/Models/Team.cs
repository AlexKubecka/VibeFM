using System;
using System.Collections.Generic;
using System.Linq;
using FootballManager.Enums;
using FootballManager.Models;
using FootballManager.Utilities;

namespace FootballManager.Models
{
    public class Team
    {
        // Db Attribute
        public int Id { get; set; } // Primary key

        // Basic Attributes
        public string Name { get; set; }
        public string Nationality { get; set; }
        public List<StaffMember> StaffMembers { get; set; } = new List<StaffMember>(); // Direct collection of staff members
        public List<Player> Players { get; set; } = new List<Player>();

        // Value and Reputation Attributes
        public double Value { get; set; } // Team's monetary value
        public double Reputation { get; set; } // Team's reputation score

        // League
        public int LeagueId { get; set; } // Foreign key to League
        public League League { get; set; } // Navigation property
        public string LeagueName { get; set; } // (Optional: for display, can be removed after migration)
        
        // Recent Results
        // Store last N match results (e.g., 'W', 'D', 'L') and performance ratings (0-10)
        public List<char> RecentResults { get; set; } = new List<char>(); // e.g., ['W','D','L','W','W']

        // Squad Morale (average of all player morales)
        public double SquadMorale => Players.Count > 0 ? Players.Average(p => p.Morale) : 0.5;

        public string GetSquadMoraleDescription()
        {
            double morale = SquadMorale;
            if (morale <= 0.15) return "Awful";
            if (morale <= 0.3) return "Very Bad";
            if (morale <= 0.45) return "Bad";
            if (morale <= 0.6) return "Neutral";
            if (morale <= 0.75) return "Good";
            if (morale <= 0.9) return "Very Good";
            return "Excellent";
        }

        // Helper to get recent results as a qualitative form description (e.g., "Awful", "Bad", "Neutral", "Good", "Perfect")
        public string GetFormDescription()
        {
            if (RecentResults == null || RecentResults.Count == 0) return "No recent results";
            int total = RecentResults.Count;
            int wins = RecentResults.Count(r => r == 'W');
            double winRate = (double)wins / total;
            if (winRate == 1.0) return "Perfect";
            if (winRate >= 0.75) return "Good";
            if (winRate >= 0.4) return "Neutral";
            if (winRate >= 0.2) return "Bad";
            return "Awful";
        }

        // League Table Stats
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;
        public int Points => Wins * 3 + Draws;

        // Stadium Attributes
        public int StadiumCapacity { get; set; } // Capacity of the team's stadium
        public string StadiumName { get; set; } // Name of the team's stadium

        // Parameterless constructor for EF and seeding
        public Team() { }

        // Constructor
        public Team(string name, string nationality, string stadiumName, int stadiumCapacity)
        {
            Name = name;
            Nationality = nationality;
            StadiumName = stadiumName;
            StadiumCapacity = stadiumCapacity;
        }

        // Method to Add a Player
        public void AddPlayer(Player player)
        {
            if (Players.Count >= 25)
            {
                Logger.Log($"Cannot add player {player.Name}. The team already has 25 players.");
                return;
            }
            Players.Add(player);
            player.Team = this; // Assign the team to the player
            Logger.Log($"Player {player.Name} added to the team.");
        }

        // Method to Remove a Player
        public void RemovePlayer(string playerName)
        {
            var player = Players.FirstOrDefault(p => p.Name == playerName);
            if (player != null)
            {
                Players.Remove(player);
                player.Team = null; // Remove the team association from the player
                Logger.Log($"Player {player.Name} removed from the team.");
            }
            else
            {
                Logger.Log($"Player {playerName} not found in the team.");
            }
        }

        // Log Team Info (for debugging purposes)
        public void LogTeamInfo()
        {
            Logger.Log($"Team: {Name} ({Nationality})");
            Logger.Log($"Stadium: {StadiumName} (Capacity: {StadiumCapacity})");
            Logger.Log($"Value: ${Value:N2} million");
            Logger.Log($"Reputation: {Reputation:N2}");
            Logger.Log("Staff Members:");
            foreach (var staffMember in StaffMembers)
            {
                Logger.Log($"- {staffMember.Name}, Job: {EnumHelper.GetDescription(staffMember.Job)}");
            }

            Logger.Log("Players:");
            foreach (var player in Players)
            {
                Logger.Log($"- {player.Name}: {player.CalculateOverallRating()}");
            }
        }

        // Print Team Info (dedicated printing function)
        public void PrintTeamInfo()
        {
            Console.WriteLine($"Team: {Name} ({Nationality})");
            Console.WriteLine($"Stadium: {StadiumName} (Capacity: {StadiumCapacity})");
            Console.WriteLine($"Value: ${Value:N2} million");
            Console.WriteLine($"Reputation: {Reputation:N2}");
            Console.WriteLine("Staff Members:");
            foreach (var staffMember in StaffMembers)
            {
                Console.WriteLine($"- {staffMember.Name}, Job: {EnumHelper.GetDescription(staffMember.Job)}");
            }

            Console.WriteLine("Players:");
            foreach (var player in Players)
            {
                Console.WriteLine($"- {player.Name}: {player.CalculateOverallRating()}");
            }
        }

        // Method to Calculate Average Overall Rating
        public double CalculateTeamOverallRating()
        {
            if (Players.Count == 0) return 0;

            // Calculate average player rating
            double playerRating = Players.Average(p => p.CalculateOverallRating());

            // Get ratings for all staff roles
            double managerRating = StaffMembers
                .Where(m => m.Job == Job.Manager)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            double assistantManagerRating = StaffMembers
                .Where(m => m.Job == Job.AssistantManager)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            double coachRating = StaffMembers
                .Where(m => m.Job == Job.Coach)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            double fitnessCoachRating = StaffMembers
                .Where(m => m.Job == Job.FitnessCoach)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            double goalkeepingCoachRating = StaffMembers
                .Where(m => m.Job == Job.GoalkeepingCoach)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            double physioRating = StaffMembers
                .Where(m => m.Job == Job.Physio)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            double scoutRating = StaffMembers
                .Where(m => m.Job == Job.Scout)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            double analystRating = StaffMembers
                .Where(m => m.Job == Job.Analyst)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            // Combine ratings (weighted average)
            return Math.Round((playerRating * 0.5) + (managerRating * 0.2) + (assistantManagerRating * 0.1) +
                            (coachRating * 0.05) + (fitnessCoachRating * 0.05) + (goalkeepingCoachRating * 0.05) +
                            (physioRating * 0.025) + (scoutRating * 0.025) + (analystRating * 0.025), 2);
        }
    }
}