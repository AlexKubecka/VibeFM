using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FootballManager.Enums;
using FootballManager.Data;
using FootballManager.Models;

namespace FootballManager.Utilities
{
    public static class JsonDbSeeder
    {
        // PlayerInfo class matching the JSON structure
        public class PlayerInfo
        {
            public string Number { get; set; }
            public string Name { get; set; }
            public string DobAndAge { get; set; }
            public string Nationality { get; set; }
            public string MarketValue { get; set; }
            public string Position { get; set; }
        }

        // Helper to parse age from "Aug 17, 1993 (31)"
        private static int ParseAge(string dobAndAge)
        {
            if (string.IsNullOrEmpty(dobAndAge)) return 0;
            var start = dobAndAge.IndexOf('(');
            var end = dobAndAge.IndexOf(')');
            if (start >= 0 && end > start)
            {
                var ageStr = dobAndAge.Substring(start + 1, end - start - 1);
                if (int.TryParse(ageStr, out int age))
                    return age;
            }
            return 0;
        }

        // Seed from combined_team_data.json (Dictionary<string, TeamInfo>)
        public static void SeedFromCombinedTeamData(FootballManagerDbContext dbContext, string jsonPath)
        {
            if (!File.Exists(jsonPath))
            {
                Console.WriteLine($"JSON file not found: {jsonPath}");
                return;
            }

            var json = File.ReadAllText(jsonPath);
            var wrapper = System.Text.Json.JsonSerializer.Deserialize<WrappedLeagueData>(json);
            if (wrapper == null || wrapper.Teams == null)
            {
                Console.WriteLine("Failed to deserialize wrapped JSON data.");
                return;
            }

            // Clear existing data to avoid FK constraint errors
            dbContext.Players.RemoveRange(dbContext.Players);
            dbContext.StaffMembers.RemoveRange(dbContext.StaffMembers);
            dbContext.Teams.RemoveRange(dbContext.Teams);
            dbContext.Leagues.RemoveRange(dbContext.Leagues);
            dbContext.SaveChanges();

            string leagueName = wrapper.LeagueName ?? "Unknown";
            string leagueNation = wrapper.LeagueNation ?? "Unknown";

            // Create or get League entity
            var league = dbContext.Leagues.FirstOrDefault(l => l.Name == leagueName && l.Nation == leagueNation);
            if (league == null)
            {
                league = new League
                {
                    Name = leagueName,
                    Nation = leagueNation
                };
                dbContext.Leagues.Add(league);
                dbContext.SaveChanges();
            }

            foreach (var kvp in wrapper.Teams)
            {
                var teamName = kvp.Key;
                var teamInfo = kvp.Value;

                // Extract manager info if present (from TeamInfoCombined)
                string managerName = null;
                string managerStartDate = null;
                if (teamInfo is TeamInfoCombined teamCombined && teamCombined.Manager != null)
                {
                    managerName = teamCombined.Manager.Name;
                    managerStartDate = teamCombined.Manager.StartDate;
                }

                // Add team if not exists
                var team = dbContext.Teams.FirstOrDefault(t => t.Name == teamName);
                if (team == null)
                {
                    team = new Team
                    {
                        Name = teamName,
                        Nationality = leagueNation, // Use LeagueNation as Nationality
                        StadiumName = teamInfo.StadiumName ?? "Default Stadium",
                        StadiumCapacity = teamInfo.StadiumCapacity > 0 ? teamInfo.StadiumCapacity : 50000,
                        Value = ParseMarketValue(teamInfo.TotalMarketValue),
                        LeagueName = leagueName,
                        LeagueId = league.Id, // Assign LeagueId
                        Wins = 0,
                        Draws = 0,
                        Losses = 0,
                        GoalsFor = 0,
                        GoalsAgainst = 0
                    };
                    dbContext.Teams.Add(team);
                    dbContext.SaveChanges();
                }
                else
                {
                    // Ensure LeagueId is set for existing teams
                    if (team.LeagueId != league.Id)
                    {
                        team.LeagueId = league.Id;
                        dbContext.SaveChanges();
                    }
                }
                // Add or update manager as StaffMember
                if (!string.IsNullOrWhiteSpace(managerName))
                {
                    // Remove any existing manager for this team
                    var existingManagers = dbContext.StaffMembers.Where(s => s.TeamId == team.Id && s.Job == FootballManager.Enums.Job.Manager).ToList();
                    if (existingManagers.Any())
                    {
                        dbContext.StaffMembers.RemoveRange(existingManagers);
                        dbContext.SaveChanges();
                    }
                    DateTime? parsedStartDate = null;
                    if (!string.IsNullOrWhiteSpace(managerStartDate) && DateTime.TryParse(managerStartDate, out var dt))
                        parsedStartDate = dt;
                    var manager = new StaffMember(managerName, FootballManager.Enums.Job.Manager)
                    {
                        TeamId = team.Id,
                        StartDate = parsedStartDate
                    };
                    dbContext.StaffMembers.Add(manager);
                    dbContext.SaveChanges();
                }

                // Add players
                if (teamInfo.Players != null)
                {
                    foreach (var p in teamInfo.Players)
                    {
                        if (!dbContext.Players.Any(pl => pl.Name == p.Name && pl.TeamId == team.Id))
                        {
                            dbContext.Players.Add(new Player
                            {
                                Name = p.Name,
                                Age = ParseAge(p.DobAndAge),
                                Nationality = p.Nationality,
                                TeamId = team.Id,
                                MarketValue = ParseMarketValue(p.MarketValue),
                                Position = GetPositionDescription(ParsePosition(p.Position)),
                                Morale = 0.5, // Default morale
                                GamesPlayed = 0,
                                Goals = 0,
                                Assists = 0,
                                CleanSheets = 0,
                                PerformanceRating = 0
                            });
                        }
                    }
                    dbContext.SaveChanges();
                }
            }
        }

        // Wrapper class for new JSON structure
        public class WrappedLeagueData
        {
            public string LeagueName { get; set; }
            public string LeagueNation { get; set; }
            public Dictionary<string, TeamInfoCombined> Teams { get; set; }
        }

        // Parse market value string like "€1.34bn" or "€39.37m" to double (in millions)
        public static double ParseMarketValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            value = value.Trim().ToLower();
            // Remove currency symbols and question marks
            value = value.Replace("€", "").Replace("$", "").Replace("?", "").Trim();
            // Replace comma with dot for decimal separator if needed
            value = value.Replace(",", ".");
            double multiplier = 1;
            if (value.EndsWith("bn")) { multiplier = 1000; value = value.Replace("bn", ""); }
            else if (value.EndsWith("m")) { multiplier = 1; value = value.Replace("m", ""); }
            else if (value.EndsWith("k")) { multiplier = 0.001; value = value.Replace("k", ""); }
            // Remove any remaining non-numeric, non-dot, non-minus, non-e characters
            value = System.Text.RegularExpressions.Regex.Replace(value, "[^0-9.eE-]", "");
            if (double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double num))
                return num * multiplier;
            return 0;
        }

        // Parse position string to Position enum
        public static FootballManager.Enums.Position ParsePosition(string pos)
        {
            if (string.IsNullOrWhiteSpace(pos)) return FootballManager.Enums.Position.CentralMidfielder;
            var normalized = pos.Trim().ToLower().Replace("-", " ").Replace("_", " ");
            // Strict, normalized matching
            switch (normalized)
            {
                case "goalkeeper":
                    return FootballManager.Enums.Position.Goalkeeper;
                case "right back":
                    return FootballManager.Enums.Position.RightBack;
                case "center back":
                case "centre back":
                    return FootballManager.Enums.Position.CenterBack;
                case "left back":
                    return FootballManager.Enums.Position.LeftBack;
                case "defensive midfielder":
                    return FootballManager.Enums.Position.DefensiveMidfielder;
                case "central midfielder":
                    return FootballManager.Enums.Position.CentralMidfielder;
                case "right winger":
                    return FootballManager.Enums.Position.RightWinger;
                case "left winger":
                    return FootballManager.Enums.Position.LeftWinger;
                case "striker":
                case "forward":
                    return FootballManager.Enums.Position.Striker;
            }
            // Fallback: substring matching for rare/unknown cases
            if (normalized.Contains("goalkeeper")) return FootballManager.Enums.Position.Goalkeeper;
            if (normalized.Contains("right back")) return FootballManager.Enums.Position.RightBack;
            if (normalized.Contains("left back")) return FootballManager.Enums.Position.LeftBack;
            if (normalized.Contains("center back") || normalized.Contains("centre back")) return FootballManager.Enums.Position.CenterBack;
            if (normalized.Contains("defensive")) return FootballManager.Enums.Position.DefensiveMidfielder;
            if (normalized.Contains("midfield")) return FootballManager.Enums.Position.CentralMidfielder;
            if (normalized.Contains("right winger")) return FootballManager.Enums.Position.RightWinger;
            if (normalized.Contains("left winger")) return FootballManager.Enums.Position.LeftWinger;
            if (normalized.Contains("striker") || normalized.Contains("forward")) return FootballManager.Enums.Position.Striker;
            return FootballManager.Enums.Position.CentralMidfielder;
        }

        // Helper to get readable string from Position enum
        public static string GetPositionDescription(FootballManager.Enums.Position pos)
        {
            var type = typeof(FootballManager.Enums.Position);
            var memInfo = type.GetMember(pos.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attrs.Length > 0)
                    return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
            }
            return pos.ToString();
        }

        public class TeamInfoCombined
        {
            public string StadiumName { get; set; }
            public int StadiumCapacity { get; set; }
            public string TotalMarketValue { get; set; }
            public List<PlayerInfo> Players { get; set; }
            public ManagerInfo Manager { get; set; } // Add this property
        }

        public class ManagerInfo
        {
            public string Name { get; set; }
            public string StartDate { get; set; }
        }
    }
}
