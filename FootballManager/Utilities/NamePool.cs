using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FootballManager.Utilities
{
    public class NamePool
    {
        public List<string> TeamNames { get; private set; } = new List<string>();
        public List<string> PlayerNames { get; private set; } = new List<string>();

        public NamePool(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<NamePoolData>(json);

            if (data != null)
            {
                TeamNames = data.TeamNames ?? new List<string>();
                PlayerNames = data.PlayerNames ?? new List<string>();
            }
        }

        private class NamePoolData
        {
            public List<string>? TeamNames { get; set; }
            public List<string>? PlayerNames { get; set; }
        }
    }
}