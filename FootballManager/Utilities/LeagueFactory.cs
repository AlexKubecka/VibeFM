using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FootballManager.Enums;
using FootballManager.Models;

namespace FootballManager.Utilities
{
    public class LeagueFactory
    {
        private readonly List<string> _teamNames;
        private readonly List<string> _playerNames;
        private readonly List<string> _firstNames;
        private readonly List<string> _lastNames;
        private readonly HashSet<string> _usedNames = new HashSet<string>(); // Tracks used names to avoid duplicates
        private readonly TeamFactory _teamFactory = new TeamFactory();
        private readonly PlayerFactory _playerFactory = new PlayerFactory();
        private readonly Random _random = new Random();

        public LeagueFactory()
        {
            Logger.Log("Initializing LeagueFactory...");

            // Load names from JSON file or use defaults
            var namePool = LoadNamePool("Data/names.json");
            _teamNames = namePool.TeamNames;
            _playerNames = namePool.PlayerNames;
            _firstNames = namePool.FirstNames;
            _lastNames = namePool.LastNames;

            Logger.Log($"Loaded {_teamNames.Count} team names and {_playerNames.Count} player names.");
        }

        public League CreateLeague(string leagueName, string leagueNation)
        {
            Logger.Log($"Creating league: {leagueName}");
            League league = new League(leagueName, leagueNation);

            for (int i = 0; i < _teamNames.Count; i++)
            {
                // Generate random reputation and stadium capacity for the team
                double teamReputation = _random.Next(50, 100); // Reputation score
                int stadiumCapacity = _random.Next(20000, 80000); // Stadium capacity

                Logger.Log($"Creating team: {_teamNames[i]} (Reputation: {teamReputation}, Stadium Capacity: {stadiumCapacity})");

                // Create a team using the TeamFactory
                Team team = _teamFactory.CreateTeam(_teamNames[i], teamReputation, stadiumCapacity);
                team.Wins = 0;
                team.Draws = 0;
                team.Losses = 0;
                team.GoalsFor = 0;
                team.GoalsAgainst = 0;

                // Add 11 players to the team using the PlayerFactory
                for (int j = 0; j < 11; j++)
                {
                    Position position = GetRandomPosition();
                    string playerName = GetUniquePlayerName();
                    Logger.Log($"Adding player: {playerName} (Position: {position})");
                    team.AddPlayer(_playerFactory.CreatePlayer(playerName, position.ToString(), team.Value, team.Reputation));
                }

                // Add the team to the league
                league.Teams.Add(team);
            }

            Logger.Log($"League {leagueName} created with {league.Teams.Count} teams.");
            return league;
        }

        private Position GetRandomPosition()
        {
            Array positions = Enum.GetValues(typeof(Position));
            object? randomPosition = positions.GetValue(_random.Next(positions.Length));
            if (randomPosition == null)
            {
                throw new InvalidOperationException("Failed to retrieve a random position from the Position enum.");
            }
            return (Position)randomPosition;
        }

        private string GetUniquePlayerName()
        {
            if (_playerNames.Count > 0)
            {
                int index = _random.Next(_playerNames.Count);
                string playerName = _playerNames[index];
                _playerNames.RemoveAt(index);
                _usedNames.Add(playerName);
                return playerName;
            }

            while (true)
            {
                string firstName = _firstNames[_random.Next(_firstNames.Count)];
                string lastName = _lastNames[_random.Next(_lastNames.Count)];
                string generatedName = $"{firstName} {lastName}";

                if (!_usedNames.Contains(generatedName))
                {
                    _usedNames.Add(generatedName);
                    return generatedName;
                }
            }
        }

        private NamePool LoadNamePool(string filePath)
        {
            try
            {
                Logger.Log($"Loading names from file: {filePath}");
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<NamePoolData>(json);

                if (data != null)
                {
                    Logger.Log("Names loaded successfully.");
                    return new NamePool
                    {
                        TeamNames = data.TeamNames ?? new List<string>(),
                        PlayerNames = data.PlayerNames ?? new List<string>(),
                        FirstNames = data.FirstNames ?? new List<string>(),
                        LastNames = data.LastNames ?? new List<string>()
                    };
                }
            }
            catch
            {
                Logger.Log("Failed to load names from file. Using default names.");
                return new NamePool
                {
                    TeamNames = new List<string>
                    {
                        "Manchester United", "Liverpool", "Chelsea", "Arsenal", "Manchester City",
                        "Tottenham Hotspur", "Leeds United", "Everton", "Newcastle United", "Aston Villa"
                    },
                    PlayerNames = new List<string>
                    {
                        "John Smith", "Michael Johnson", "David Brown", "Chris Taylor", "James Wilson",
                        "Robert Moore", "Daniel Harris", "Paul Clark", "Mark Lewis", "Andrew Walker", "Steven Hall"
                    },
                    FirstNames = new List<string>
                    {
                        "John", "Michael", "David", "Chris", "James", "Robert", "Daniel", "Paul", "Mark", "Andrew",
                        "Steven", "Kevin", "Brian", "Jason", "Matthew", "Anthony", "Joshua", "Ryan", "Nicholas"
                    },
                    LastNames = new List<string>
                    {
                        "Smith", "Johnson", "Brown", "Taylor", "Wilson", "Moore", "Harris", "Clark", "Lewis", "Walker",
                        "Hall", "Adams", "Wright", "Green", "King", "Scott", "Young", "Allen", "Hill"
                    }
                };
            }

            return new NamePool
            {
                TeamNames = new List<string>(),
                PlayerNames = new List<string>(),
                FirstNames = new List<string>(),
                LastNames = new List<string>()
            };
        }

        private class NamePoolData
        {
            public List<string>? TeamNames { get; set; }
            public List<string>? PlayerNames { get; set; }
            public List<string>? FirstNames { get; set; }
            public List<string>? LastNames { get; set; }
        }

        private class NamePool
        {
            public List<string> TeamNames { get; set; } = new List<string>();
            public List<string> PlayerNames { get; set; } = new List<string>();
            public List<string> FirstNames { get; set; } = new List<string>();
            public List<string> LastNames { get; set; } = new List<string>();
        }
    }
}