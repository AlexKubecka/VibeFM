using System.ComponentModel.DataAnnotations.Schema;

namespace FootballManager.Models
{
    public class PlayerMatchStat
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int MinutesPlayed { get; set; }
        public bool Starter { get; set; }
        public double MatchRating { get; set; }
        public int CleanSheets { get; set; }
        public int Injuries { get; set; }
        // Navigation
        [NotMapped]
        public Player? Player { get; set; }
    }
}
