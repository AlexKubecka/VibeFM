using System;
using System.Collections.Generic;
using FootballManager.Enums;
using FootballManager.Models;

namespace FootballManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a team of 11 players with realistic attributes for their positions
            List<Player> team = new List<Player>
            {
                // Goalkeeper
                new Player("John Doe", 25, Position.Goalkeeper, "English")
                {
                    Handling = 85, Reflexes = 90, Communication = 80, AerialAbility = 75, JumpingReach = 78
                },

                // Defenders
                new Player("Jane Smith", 28, Position.RightBack, "Spanish")
                {
                    Marking = 80, Tackling = 85, Heading = 75, Positioning = 82, Strength = 78, Speed = 80
                },
                new Player("Carlos Ruiz", 24, Position.CenterBack, "Mexican")
                {
                    Marking = 85, Tackling = 88, Heading = 80, Positioning = 85, Strength = 80
                },
                new Player("Lucas Martinez", 21, Position.CenterBack, "Argentinian")
                {
                    Marking = 83, Tackling = 86, Heading = 82, Positioning = 84, Strength = 78
                },
                new Player("Ella Wilson", 29, Position.LeftBack, "German")
                {
                    Marking = 78, Tackling = 80, Heading = 75, Positioning = 80, Strength = 76, Speed = 82
                },

                // Midfielders
                new Player("Emily Johnson", 22, Position.DefensiveMidfielder, "American")
                {
                    Passing = 85, Vision = 80, Teamwork = 88, WorkRate = 90, Tackling = 85
                },
                new Player("Liam Brown", 26, Position.CentralMidfielder, "Australian")
                {
                    Passing = 88, Vision = 85, Teamwork = 90, WorkRate = 85, Stamina = 80
                },
                new Player("Sophia Lee", 23, Position.CentralMidfielder, "South Korean")
                {
                    Passing = 87, Vision = 84, Teamwork = 85, WorkRate = 83, Stamina = 78
                },

                // Forwards
                new Player("Oliver Davis", 27, Position.RightWinger, "Canadian")
                {
                    Finishing = 85, Dribbling = 88, Crossing = 80, Speed = 90, Composure = 78
                },
                new Player("Mia Taylor", 24, Position.LeftWinger, "French")
                {
                    Finishing = 83, Dribbling = 85, Crossing = 82, Speed = 88, Composure = 80
                },
                new Player("Noah White", 30, Position.Striker, "Dutch")
                {
                    Finishing = 90, Dribbling = 85, Shooting = 88, Composure = 85, Heading = 80
                }
            };

            // Print the team
            Console.WriteLine("Team of 11 Players:");
            Console.WriteLine("--------------------");
            foreach (var player in team)
            {
                Console.WriteLine($"Name: {player.Name}, Nationality: {player.Nationality}, Position: {player.Position}, Overall Rating: {player.CalculateOverallRating()}");
            }
        }
    }
}