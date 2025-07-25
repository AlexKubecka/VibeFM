using System; // For basic system functionality
using System.Linq; // For LINQ operations like OrderByDescending
using FootballManager.Enums; // For enums like Job and Position
using FootballManager.Models; // For League, Team, Player, and Staff classes
using FootballManager.Utilities; // For LeagueFactory and other utility classes

namespace FootballManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // Enable debug mode
            Logger.DebugMode = false;

            // Create a league using the LeagueFactory
            LeagueFactory leagueFactory = new LeagueFactory();
            League league = leagueFactory.CreateLeague("Premier League");

            // Rank teams by overall ratings
            RankTeamsByOverallRatings(league);
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