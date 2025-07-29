using System;
using System.Collections.Generic;

namespace FootballManager.Web.Models
{
    public class GameSimulationState
    {
        public int MatchId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public int HomeScore { get; set; } = 0;
        public int AwayScore { get; set; } = 0;
        public int CurrentMinute { get; set; } = 0;
        public List<string> Events { get; set; } = new List<string>();
        public List<PlayerSimState> HomePlayers { get; set; } = new List<PlayerSimState>();
        public List<PlayerSimState> AwayPlayers { get; set; } = new List<PlayerSimState>();
        // You can add more fields here for advanced simulation (e.g. player stamina, injuries, etc.)
    }
}
