using System;

namespace FootballManager.Web.Models
{
    public class PlayerSimState
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public bool Starter { get; set; }
        public int Goals { get; set; } = 0;
        public int Assists { get; set; } = 0;
        public int YellowCards { get; set; } = 0;
        public int RedCards { get; set; } = 0;
        public int MinutesPlayed { get; set; } = 0;
        public double MatchRating { get; set; } = 6.5;
        // Add more fields as needed (injuries, substitutions, etc.)
    }
}
