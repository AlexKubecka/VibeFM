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
        //public List<PlayerMatchStat> PlayerStats { get; set; }
    }
}
