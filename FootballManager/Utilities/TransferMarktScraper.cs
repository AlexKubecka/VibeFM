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
    public static async Task<Dictionary<string, List<PlayerInfo>>> GetPremierLeagueTeamsAndPlayersAsync()
    {
        var url = "https://www.transfermarkt.com/premier-league/startseite/wettbewerb/GB1";
        var baseUrl = "https://www.transfermarkt.com";
        var teamsAndPlayers = new Dictionary<string, List<PlayerInfo>>();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

        var html = await httpClient.GetStringAsync(url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Teams are in <a class="hauptlink no-border-links" ...>
        var nodes = doc.DocumentNode.SelectNodes("//td[contains(@class, 'hauptlink') and contains(@class, 'no-border-links')]/a[@title]");
        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                var teamName = node.GetAttributeValue("title", "");
                var href = node.GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(teamName) && !string.IsNullOrEmpty(href) && !teamsAndPlayers.ContainsKey(teamName))
                {
                    // Build the full team URL (squad page)
                    var teamUrl = baseUrl + href;
                    var players = await GetPlayersDataFromTeamPageAsync(teamUrl, httpClient);
                    teamsAndPlayers[teamName] = players;
                    Console.WriteLine($"Scraped {players.Count} players for {teamName}");
                }
            }
        }
        else
        {
            Console.WriteLine("No team nodes found.");
        }

        return teamsAndPlayers;
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

    // Helper class for player info
    public class PlayerInfo
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string DobAndAge { get; set; }
        public string Nationality { get; set; }
        public string MarketValue { get; set; }
    }

    public static async Task<List<PlayerInfo>> ScrapeSingleTeamAsync(string teamUrl, string teamName, string outputFile)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

        var players = await GetPlayersDataFromTeamPageAsync(teamUrl, httpClient);

        // Save to JSON
        var dict = new Dictionary<string, List<PlayerInfo>> { [teamName] = players };
        SaveTeamsAndPlayersToJson(dict, outputFile);

        Console.WriteLine($"Saved {players.Count} players for {teamName} to {outputFile}");
        return players;
    }

    // Call this after scraping
    public static void SaveTeamsAndPlayersToJson(Dictionary<string, List<PlayerInfo>> data, string filePath)
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