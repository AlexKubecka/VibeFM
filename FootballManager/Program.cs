using System;
using FootballManager.Enums; // Add this directive
using FootballManager.Models;

namespace FootballManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a league
            League league = new League("Premier League");

            // Random generator for player attributes
            Random random = new Random();

            // Populate the league with 10 teams
            for (int i = 1; i <= 10; i++)
            {
                // Randomize reputation and stadium size for the team
                double teamReputation = random.Next(50, 100); // Reputation score
                int stadiumCapacity = random.Next(20000, 80000); // Stadium capacity

                // Calculate team value based on reputation and stadium size
                double teamValue = CalculateTeamValue(teamReputation, stadiumCapacity);

                Team team = new Team($"Team {i}", "International", $"Stadium {i}", stadiumCapacity)
                {
                    Value = teamValue,
                    Reputation = teamReputation
                };

                // Add staff members
                team.TeamStaff.AddStaffMember(new StaffMember($"Manager {i}", Job.Manager)
                {
                    TacticalKnowledge = random.Next(70, 100),
                    Leadership = random.Next(70, 100),
                    DecisionMaking = random.Next(70, 100)
                });

                team.TeamStaff.AddStaffMember(new StaffMember($"Assistant Manager {i}", Job.AssistantManager)
                {
                    Leadership = random.Next(60, 90),
                    Communication = random.Next(60, 90),
                    TrainingKnowledge = random.Next(60, 90)
                });

                // Add 11 players with attributes based on their positions and team quality
                for (int j = 1; j <= 11; j++)
                {
                    Position position = GetRandomPosition(random);
                    team.AddPlayer(CreatePlayer($"Player {j} (Team {i})", position, random, team.Value, team.Reputation));
                }

                league.Teams.Add(team);
            }

            // Rank teams by overall ratings
            RankTeamsByOverallRatings(league);
        }

        // Helper method to calculate team value based on reputation and stadium size
        private static double CalculateTeamValue(double reputation, int stadiumCapacity)
        {
            // Formula: Value is influenced by reputation and stadium capacity
            return Math.Round(Math.Pow((2 * reputation * reputation) + (stadiumCapacity / 100), 2), 2);
        }

        // Helper method to create a player with attributes based on their position and team quality
        private static Player CreatePlayer(string name, Position position, Random random, double teamValue, double teamReputation)
        {
            // Adjust ranges based on team value and reputation
            int minAttribute = (int)Math.Max(50, teamReputation); // Minimum attribute value
            int maxAttribute = (int)Math.Min(100, teamValue / 10 + 50); // Maximum attribute value

            Player player = new Player(name, random.Next(18, 35), position, "International");

            switch (position)
            {
                case Position.Goalkeeper:
                    player.Handling = random.Next(minAttribute, maxAttribute);
                    player.Reflexes = random.Next(minAttribute, maxAttribute);
                    player.Communication = random.Next(minAttribute, maxAttribute);
                    player.AerialAbility = random.Next(minAttribute, maxAttribute);
                    player.JumpingReach = random.Next(minAttribute, maxAttribute);
                    break;

                case Position.RightBack:
                case Position.LeftBack:
                    player.Marking = random.Next(minAttribute, maxAttribute);
                    player.Tackling = random.Next(minAttribute, maxAttribute);
                    player.Positioning = random.Next(minAttribute, maxAttribute);
                    player.Speed = random.Next(minAttribute, maxAttribute);
                    player.Crossing = random.Next(minAttribute, maxAttribute);
                    break;

                case Position.CenterBack:
                    player.Marking = random.Next(minAttribute, maxAttribute);
                    player.Tackling = random.Next(minAttribute, maxAttribute);
                    player.Heading = random.Next(minAttribute, maxAttribute);
                    player.Positioning = random.Next(minAttribute, maxAttribute);
                    player.Strength = random.Next(minAttribute, maxAttribute);
                    break;

                case Position.DefensiveMidfielder:
                    player.Passing = random.Next(minAttribute, maxAttribute);
                    player.Vision = random.Next(minAttribute, maxAttribute);
                    player.Teamwork = random.Next(minAttribute, maxAttribute);
                    player.WorkRate = random.Next(minAttribute, maxAttribute);
                    player.Tackling = random.Next(minAttribute, maxAttribute);
                    break;

                case Position.CentralMidfielder:
                    player.Passing = random.Next(minAttribute, maxAttribute);
                    player.Vision = random.Next(minAttribute, maxAttribute);
                    player.Teamwork = random.Next(minAttribute, maxAttribute);
                    player.WorkRate = random.Next(minAttribute, maxAttribute);
                    player.Dribbling = random.Next(minAttribute, maxAttribute);
                    break;

                case Position.RightWinger:
                case Position.LeftWinger:
                    player.Dribbling = random.Next(minAttribute, maxAttribute);
                    player.Crossing = random.Next(minAttribute, maxAttribute);
                    player.Speed = random.Next(minAttribute, maxAttribute);
                    player.Finishing = random.Next(minAttribute, maxAttribute);
                    player.Composure = random.Next(minAttribute, maxAttribute);
                    break;

                case Position.Striker:
                    player.Finishing = random.Next(minAttribute, maxAttribute);
                    player.Dribbling = random.Next(minAttribute, maxAttribute);
                    player.Shooting = random.Next(minAttribute, maxAttribute);
                    player.Heading = random.Next(minAttribute, maxAttribute);
                    player.Composure = random.Next(minAttribute, maxAttribute);
                    break;
            }

            return player;
        }

        // Helper method to get a random position
        private static Position GetRandomPosition(Random random)
        {
            Array positions = Enum.GetValues(typeof(Position));
            return (Position)positions.GetValue(random.Next(positions.Length));
        }

        // Helper method to rank teams by overall ratings
        private static void RankTeamsByOverallRatings(League league)
        {
            Console.WriteLine($"League Rankings: {league.Name}");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Team\t\tOverall Rating\tValue\tReputation");
            Console.WriteLine("-----------------------------------");

            foreach (var team in league.Teams.OrderByDescending(t => t.CalculateTeamOverallRating()))
            {
                Console.WriteLine($"{team.Name}\t\t{team.CalculateTeamOverallRating()}\t${team.Value:N2}M\t{team.Reputation:N2}");
                Console.WriteLine("Manager Rating: " + team.TeamStaff.Members
                    .Where(m => m.Job == Job.Manager)
                    .Select(m => m.CalculateOverallRating())
                    .FirstOrDefault());
                Console.WriteLine("Player Ratings:");
                foreach (var player in team.Players)
                {
                    Console.WriteLine($"- {player.Name}: {player.CalculateOverallRating()}");
                }
                Console.WriteLine("-----------------------------------");
            }
        }
    }
}