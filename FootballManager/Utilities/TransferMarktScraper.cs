using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

public static class TransfermarktScraper
    {
    public static async Task<Dictionary<string, TeamInfo>> GetPremierLeagueTeamsAndPlayersAsync()
    {
        var url = "https://www.transfermarkt.com/premier-league/startseite/wettbewerb/GB1";
        var baseUrl = "https://www.transfermarkt.com";
        var teamsData = new Dictionary<string, TeamInfo>();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

        // Scrape team market values from the main table
        var html = await httpClient.GetStringAsync(url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var teamValueDict = new Dictionary<string, string>();
        var teamRows = doc.DocumentNode.SelectNodes("//table[contains(@class,'items')]/tbody/tr");
        if (teamRows != null)
        {
            foreach (var row in teamRows)
            {
                var nameNode = row.SelectSingleNode("td[2]//a[@title]");
                var valueNode = row.SelectSingleNode("td[last()]//a|td[last()]");
                var teamName = nameNode?.GetAttributeValue("title", "").Trim();
                var marketValue = valueNode?.InnerText.Trim() ?? "";
                // Filter: skip if already added, or if market value is empty, '0', or just a currency symbol
                bool isValidValue = !string.IsNullOrEmpty(marketValue) && marketValue != "0" && marketValue != "?" && marketValue != "€" && marketValue != "$";
                if (!string.IsNullOrEmpty(teamName) && isValidValue && !teamValueDict.ContainsKey(teamName))
                {
                    teamValueDict[teamName] = marketValue;
                    Console.WriteLine($"Found team: {teamName}, Market Value: {marketValue}");
                }
            }
        }


        // Teams are in <a class="hauptlink no-border-links" ...>
        var nodes = doc.DocumentNode.SelectNodes("//td[contains(@class, 'hauptlink') and contains(@class, 'no-border-links')]/a[@title]");
        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                var teamName = node.GetAttributeValue("title", "");
                var href = node.GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(teamName) && !string.IsNullOrEmpty(href) && !teamsData.ContainsKey(teamName))
                {
                    // Build the full team URL (squad page)
                    var teamUrl = baseUrl + href;
                    var teamInfo = await GetTeamDataFromTeamPageAsync(teamUrl, httpClient);
                    if (teamValueDict.TryGetValue(teamName, out var teamMarketValue))
                        teamInfo.TotalMarketValue = teamMarketValue;
                    teamsData[teamName] = teamInfo;
                    Console.WriteLine($"Scraped {teamInfo.Players.Count} players for {teamName} (Stadium: {teamInfo.StadiumName}, Capacity: {teamInfo.StadiumCapacity}, Value: {teamInfo.TotalMarketValue})");
                }
            }
        }
        else
        {
            Console.WriteLine("No team nodes found.");
        }

        return teamsData;
    }

    // Scrape stadium name, capacity, and players from a team page
    public static async Task<TeamInfo> GetTeamDataFromTeamPageAsync(string teamUrl, HttpClient httpClient)
    {
        await Task.Delay(10000);
        var html = await httpClient.GetStringAsync(teamUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Manager info
        string manager = null;
        var managerNode = doc.DocumentNode.SelectSingleNode("//a[contains(@href, '/profil/trainer/') and contains(@class, 'name')]");
        if (managerNode != null)
        {
            manager = managerNode.InnerText.Trim();
        }
        
        // Stadium info
        string stadiumName = "Unknown";
        int stadiumCapacity = 0;
        var stadiumNode = doc.DocumentNode.SelectSingleNode("//li[contains(.,'Stadium:') and contains(@class,'data-header__label')]");
        if (stadiumNode != null)
        {
            var stadiumNameNode = stadiumNode.SelectSingleNode(".//a");
            if (stadiumNameNode != null)
                stadiumName = stadiumNameNode.InnerText.Trim();
            var capacityNode = stadiumNode.SelectSingleNode(".//span[contains(@class,'tabellenplatz')]");
            if (capacityNode != null)
            {
                var capText = capacityNode.InnerText.Trim().Replace("Seats", "").Replace(".", "").Trim();
                int.TryParse(capText, out stadiumCapacity);
            }
        }

        // Players
        var players = new List<PlayerInfo>();
        var rows = doc.DocumentNode.SelectNodes("//table[contains(@class, 'items')]/tbody/tr");
        if (rows != null)
        {
            foreach (var row in rows)
            {
                var numberNode = row.SelectSingleNode("td[1]//div[@class='rn_nummer']");
                var number = numberNode?.InnerText.Trim() ?? "";
                var nameNode = row.SelectSingleNode("td[2]//td[@class='hauptlink']/a");
                var name = nameNode?.InnerText.Trim() ?? "";
                var dobAgeNode = row.SelectSingleNode("td[3]");
                var dobAge = dobAgeNode?.InnerText.Trim() ?? "";
                var natNode = row.SelectSingleNode("td[4]/img[1]");
                var nationality = natNode?.GetAttributeValue("title", "") ?? "";
                var mvNode = row.SelectSingleNode("td[5]/a");
                var marketValue = mvNode?.InnerText.Trim() ?? "";
                // Player position: after name, in the same cell, e.g. "Ederson Goalkeeper"
                var posNode = row.SelectSingleNode("td[2]//table//tr/td[2] | td[2]");
                string position = null;
                if (posNode != null)
                {
                    var posText = posNode.InnerText.Trim();
                    // Try to extract position from the text after the name
                    if (!string.IsNullOrEmpty(name) && posText.StartsWith(name))
                    {
                        position = posText.Substring(name.Length).Trim();
                    }
                }
                if (!string.IsNullOrEmpty(name))
                {
                    players.Add(new PlayerInfo
                    {
                        Number = number,
                        Name = name,
                        DobAndAge = dobAge,
                        Nationality = nationality,
                        MarketValue = marketValue,
                        Position = position
                    });
                }
                await Task.Delay(2000);
            }
        }
        return new TeamInfo
        {
            StadiumName = stadiumName,
            StadiumCapacity = stadiumCapacity,
            Players = players,
            Manager = manager
        };
    }

    private static async Task<List<PlayerInfo>> GetPlayersDataFromTeamPageAsync(string teamUrl, HttpClient httpClient)
    {
        var players = new List<PlayerInfo>();
        await Task.Delay(10000);
        var html = await httpClient.GetStringAsync(teamUrl);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Each player is a <tr> in the squad table
        var rows = doc.DocumentNode.SelectNodes("//table[contains(@class, 'items')]/tbody/tr");
        if (rows != null)
        {
            foreach (var row in rows)
            {
                // Number
                var numberNode = row.SelectSingleNode("td[1]//div[@class='rn_nummer']");
                var number = numberNode?.InnerText.Trim() ?? "";

                // Name
                var nameNode = row.SelectSingleNode("td[2]//td[@class='hauptlink']/a");
                var name = nameNode?.InnerText.Trim() ?? "";

                // DOB and Age
                var dobAgeNode = row.SelectSingleNode("td[3]");
                var dobAge = dobAgeNode?.InnerText.Trim() ?? "";

                // First Nationality
                var natNode = row.SelectSingleNode("td[4]/img[1]");
                var nationality = natNode?.GetAttributeValue("title", "") ?? "";

                // Market Value
                var mvNode = row.SelectSingleNode("td[5]/a");
                var marketValue = mvNode?.InnerText.Trim() ?? "";

                if (!string.IsNullOrEmpty(name))
                {
                    players.Add(new PlayerInfo
                    {
                        Number = number,
                        Name = name,
                        DobAndAge = dobAge,
                        Nationality = nationality,
                        MarketValue = marketValue
                    });
                }
                await Task.Delay(2000);
            }
        }
        return players;
    }


    // DTO for team info
    public class TeamInfo
    {
        public string StadiumName { get; set; }
        public int StadiumCapacity { get; set; }
        public string TotalMarketValue { get; set; } // New: team total market value
        public List<PlayerInfo> Players { get; set; }
        public string Manager { get; set; } // New: manager name
    }

    // Helper class for player info
    public class PlayerInfo
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string DobAndAge { get; set; }
        public string Nationality { get; set; }
        public string MarketValue { get; set; }
        public string Position { get; set; } // New: player position
    }


    
    // Save all team, stadium, and player data into a single JSON file
    public static void SaveAllTeamDataToJson(Dictionary<string, TeamInfo> data, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, json);
    }
}