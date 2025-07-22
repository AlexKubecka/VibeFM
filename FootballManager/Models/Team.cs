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
        // Basic Attributes
        public string Name { get; set; }
        public string Nationality { get; set; }
        public Staff TeamStaff { get; set; } = new Staff(); // Reference to the Staff class
        public List<Player> Players { get; set; } = new List<Player>();

        // Value and Reputation Attributes
        public double Value { get; set; } // Team's monetary value
        public double Reputation { get; set; } // Team's reputation score
        
        // Stadium Attributes
        public int StadiumCapacity { get; set; } // Capacity of the team's stadium
        public string StadiumName { get; set; } // Name of the team's stadium

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
                Console.WriteLine($"Cannot add player {player.Name}. The team already has 25 players.");
                return;
            }
            Players.Add(player);
            Console.WriteLine($"Player {player.Name} added to the team.");
        }

        // Method to Remove a Player
        public void RemovePlayer(string playerName)
        {
            var player = Players.FirstOrDefault(p => p.Name == playerName);
            if (player != null)
            {
                Players.Remove(player);
                Console.WriteLine($"Player {player.Name} removed from the team.");
            }
            else
            {
                Console.WriteLine($"Player {playerName} not found in the team.");
            }
        }

        // Method to Display Team Info
        public void DisplayTeam()
        {
            Console.WriteLine($"Team: {Name} ({Nationality})");
            Console.WriteLine($"Stadium: {StadiumName} (Capacity: {StadiumCapacity})");
            Console.WriteLine($"Value: ${Value:N2} million");
            Console.WriteLine($"Reputation: {Reputation:N2}");
            Console.WriteLine("Staff Members:");
            foreach (var staffMember in TeamStaff.Members)
            {
                Console.WriteLine($"- {staffMember.Name}, Job: {EnumHelper.GetDescription(staffMember.Job)}");
            }

            Console.WriteLine("Players:");
            foreach (var player in Players)
            {
                Console.WriteLine($"- {player.GetFormattedInfo()}");
            }
        }

        // Method to Calculate Average Overall Rating
        public double CalculateTeamOverallRating()
        {
            if (Players.Count == 0) return 0;

            // Calculate average player rating
            double playerRating = Players.Average(p => p.CalculateOverallRating());

            // Get manager and assistant manager ratings
            double managerRating = TeamStaff.Members
                .Where(m => m.Job == Job.Manager)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            double assistantManagerRating = TeamStaff.Members
                .Where(m => m.Job == Job.AssistantManager)
                .Select(m => m.CalculateOverallRating())
                .FirstOrDefault();

            // Combine ratings (weighted average)
            double overallRating = Math.Round((playerRating * 0.7) + (managerRating * 0.2) + (assistantManagerRating * 0.1), 2);

            return overallRating;
        }
    }
}