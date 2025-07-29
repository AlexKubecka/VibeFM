using System;

namespace FootballManager.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int LeagueId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    // Navigation for Game Details view (not mapped to DB)
    public Team? HomeTeam { get; set; }
    public Team? AwayTeam { get; set; }
    public List<Player> HomePlayers { get; set; } = new List<Player>();
    public List<Player> AwayPlayers { get; set; } = new List<Player>();
    }
}
