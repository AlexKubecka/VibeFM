using System; // For basic system functionality
using System.Linq; // For LINQ operations like OrderByDescending
using FootballManager.Enums; // For enums like Job and Position
using FootballManager.Models; // For League, Team, Player, and Staff classes
using FootballManager.Utilities; // For LeagueFactory and other utility classes
using FootballManager.Data; // For FootballManagerDbContext


namespace FootballManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // Enable debug mode
            Logger.DebugMode = false;
            using (var dbContext = new FootballManagerDbContext())
{
    ClearDatabase(dbContext);
    
    Console.WriteLine("Adding sample data to the database...");

                // Add Teams
                if (!dbContext.Teams.Any())
                {
                    dbContext.Teams.AddRange(
                        new Team("Manchester United", "English", "Old Trafford", 75000) { Id = 1, Value = 1000, Reputation = 85 },
                        new Team("Liverpool", "English", "Anfield", 54000) { Id = 2, Value = 950, Reputation = 80 }
                    );
                    dbContext.SaveChanges();
                    Console.WriteLine("Teams added.");
                }

                // Add Players
                if (!dbContext.Players.Any())
                {
                    dbContext.Players.AddRange(
                        new Player { Name = "John Doe", Age = 25, Position = Position.Striker, Nationality = "English", TeamId = 1 },
                        new Player { Name = "Michael Smith", Age = 22, Position = Position.Goalkeeper, Nationality = "English", TeamId = 1 },
                        new Player { Name = "David Brown", Age = 28, Position = Position.CentralMidfielder, Nationality = "English", TeamId = 2 },
                        new Player { Name = "FREE AGENT", Age = 88, Position = Position.CentralMidfielder, Nationality = "English"}
                    );
                    dbContext.SaveChanges();
                    Console.WriteLine("Players added.");
                }

                // Add Staff Members
                if (!dbContext.StaffMembers.Any())
                {
                    dbContext.StaffMembers.AddRange(
                        new StaffMember("Manager Manchester United", Job.Manager) { TacticalKnowledge = 90, Leadership = 85, DecisionMaking = 80, TeamId = 1},
                        new StaffMember("Assistant Manager Liverpool", Job.AssistantManager) { Leadership = 80, Communication = 75, TrainingKnowledge = 70, TeamId = 2 },
                        new StaffMember("FREE AGENT STAFF", Job.Coach) { Leadership = 80, Communication = 75, TrainingKnowledge = 70 }
                    );
                    dbContext.SaveChanges();
                    Console.WriteLine("Staff members added.");
                }

    Console.WriteLine("Testing database retrieval...");

    // Query all teams
    var teams = dbContext.Teams.ToList();
    Console.WriteLine($"Teams Count: {teams.Count}");
    foreach (var team in teams)
    {
        Console.WriteLine($"Team: {team.Name}, Reputation: {team.Reputation}, Value: {team.Value}");
    }

    // Query all players
    var players = dbContext.Players.ToList();
    Console.WriteLine($"Players Count: {players.Count}");
    foreach (var player in players)
    {
        Console.WriteLine($"Player: {player.Name}, Age: {player.Age}, TeamId: {player.TeamId}");
    }

    // Query all staff members
    var staffMembers = dbContext.StaffMembers.ToList();
    Console.WriteLine($"Staff Members Count: {staffMembers.Count}");
    foreach (var staff in staffMembers)
    {
        Console.WriteLine($"Staff: {staff.Name}, Job: {staff.Job}, TeamId: {staff.TeamId}");
    }
}
            // Create a league using the LeagueFactory
            // LeagueFactory leagueFactory = new LeagueFactory();
            // League league = leagueFactory.CreateLeague("Premier League");

            // // Rank teams by overall ratings
            // RankTeamsByOverallRatings(league);
        }

        private static void RankTeamsByOverallRatings(League league)
        {
            Console.WriteLine($"League Rankings: {league.Name}");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Team\t\tOverall Rating\tValue\tReputation");
            Console.WriteLine("-----------------------------------");

            foreach (var team in league.Teams.OrderByDescending(t => t.CalculateTeamOverallRating()))
            {
                Console.WriteLine($"{team.Name}\t\t{team.CalculateTeamOverallRating()}\t${team.Value:N2}M\t{team.Reputation:N2}");
                Console.WriteLine("Manager Rating: " + team.StaffMembers
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

        private static void ClearDatabase(FootballManagerDbContext dbContext)
        {
            Console.WriteLine("Clearing database...");

            // Clear Players
            if (dbContext.Players.Any())
            {
                dbContext.Players.RemoveRange(dbContext.Players);
                Console.WriteLine("Players cleared.");
            }

            // Clear Staff Members
            if (dbContext.StaffMembers.Any())
            {
                dbContext.StaffMembers.RemoveRange(dbContext.StaffMembers);
                Console.WriteLine("Staff members cleared.");
            }

            // Clear Teams
            if (dbContext.Teams.Any())
            {
                dbContext.Teams.RemoveRange(dbContext.Teams);
                Console.WriteLine("Teams cleared.");
            }

            // Save changes
            dbContext.SaveChanges();
            Console.WriteLine("Database cleared successfully.");
        }
    }
}