using System;
using System.Collections.Generic;
using System.Linq;
using FootballManager.Models;
using FootballManager.Data;
using FootballManager.Utilities;

namespace FootballManager.Utilities
{
    public static class DummyDbSeeder
    {
        public static void SeedTwoDummyTeams(FootballManagerDbContext dbContext)
        {

            // Remove existing dummy league and teams if they exist
            var dummyLeague = dbContext.Leagues.FirstOrDefault(l => l.Name == "Dummy League");
            if (dummyLeague != null)
            {
                var dummyTeams = dbContext.Teams.Where(t => t.LeagueId == dummyLeague.Id).ToList();
                foreach (var team in dummyTeams)
                {
                    var players = dbContext.Players.Where(p => p.TeamId == team.Id).ToList();
                    dbContext.Players.RemoveRange(players);
                    dbContext.Teams.Remove(team);
                }
                dbContext.Leagues.Remove(dummyLeague);
                dbContext.SaveChanges();
            }

            // Create Dummy League
            dummyLeague = new League
            {
                Name = "Dummy League",
                Nation = "Dummyland"
            };
            dbContext.Leagues.Add(dummyLeague);
            dbContext.SaveChanges();

            // Create Team 70s
            var team70s = new Team
            {
                Name = "Team 70s",
                Nationality = "Dummyland",
                StadiumName = "Seventy Arena",
                StadiumCapacity = 70000,
                Value = 70000000,
                Reputation = 70,
                LeagueName = dummyLeague.Name,
                LeagueId = dummyLeague.Id,
                Wins = 0, Draws = 0, Losses = 0, GoalsFor = 0, GoalsAgainst = 0
            };
            dbContext.Teams.Add(team70s);
            dbContext.SaveChanges();

            // Create Team 80s
            var team80s = new Team
            {
                Name = "Team 80s",
                Nationality = "Dummyland",
                StadiumName = "Eighty Arena",
                StadiumCapacity = 80000,
                Value = 80000000,
                Reputation = 80,
                LeagueName = dummyLeague.Name,
                LeagueId = dummyLeague.Id,
                Wins = 0, Draws = 0, Losses = 0, GoalsFor = 0, GoalsAgainst = 0
            };
            dbContext.Teams.Add(team80s);
            dbContext.SaveChanges();

            // Standard 4-3-3 positions
            var positions = new List<string> {
                "Goalkeeper",
                "Left Back",
                "Center Back 1",
                "Center Back 2",
                "Right Back",
                "Central Midfielder 1",
                "Central Midfielder 2",
                "Central Midfielder 3",
                "Left Winger",
                "Striker",
                "Right Winger"
            };

            // Add players to Team 70s (all attributes 70)
            for (int i = 0; i < positions.Count; i++)
            {
                dbContext.Players.Add(new Player
                {
                    Name = positions[i],
                    Age = 25,
                    Position = positions[i].Replace(" 1", "").Replace(" 2", ""),
                    Nationality = "Dummyland",
                    TeamId = team70s.Id,
                    Morale = 0.7,
                    MarketValue = 7000000,
                    Speed = 70, Stamina = 70, Strength = 70, Agility = 70, JumpingReach = 70,
                    DecisionMaking = 70, Leadership = 70, WorkRate = 70, Teamwork = 70, Vision = 70, Concentration = 70, Determination = 70, Composure = 70, Anticipation = 70,
                    Passing = 70, Shooting = 70, Dribbling = 70, Tackling = 70, Crossing = 70, Finishing = 70, FirstTouch = 70, Heading = 70, Marking = 70, Positioning = 70,
                    Handling = 70, Reflexes = 70, AerialAbility = 70, Communication = 70, Throwing = 70, OneOnOnes = 70,
                    Professionalism = 70, Sportsmanship = 70, Temperament = 70, Ambition = 70, Loyalty = 70,
                    GamesPlayed = 0, Goals = 0, Assists = 0, CleanSheets = 0, YellowCards = 0, RedCards = 0, Injuries = 0, MinutesPlayed = 0,
                    PerformanceRating = 7.0, TotalMatchRating = 0, MatchRatingCount = 0
                });
            }
            dbContext.SaveChanges();

            // Add players to Team 80s (all attributes 80)
            for (int i = 0; i < positions.Count; i++)
            {
                dbContext.Players.Add(new Player
                {
                    Name = positions[i],
                    Age = 25,
                    Position = positions[i].Replace(" 1", "").Replace(" 2", ""),
                    Nationality = "Dummyland",
                    TeamId = team80s.Id,
                    Morale = 0.8,
                    MarketValue = 8000000,
                    Speed = 80, Stamina = 80, Strength = 80, Agility = 80, JumpingReach = 80,
                    DecisionMaking = 80, Leadership = 80, WorkRate = 80, Teamwork = 80, Vision = 80, Concentration = 80, Determination = 80, Composure = 80, Anticipation = 80,
                    Passing = 80, Shooting = 80, Dribbling = 80, Tackling = 80, Crossing = 80, Finishing = 80, FirstTouch = 80, Heading = 80, Marking = 80, Positioning = 80,
                    Handling = 80, Reflexes = 80, AerialAbility = 80, Communication = 80, Throwing = 80, OneOnOnes = 80,
                    Professionalism = 80, Sportsmanship = 80, Temperament = 80, Ambition = 80, Loyalty = 80,
                    GamesPlayed = 0, Goals = 0, Assists = 0, CleanSheets = 0, YellowCards = 0, RedCards = 0, Injuries = 0, MinutesPlayed = 0,
                    PerformanceRating = 8.0, TotalMatchRating = 0, MatchRatingCount = 0
                });
            }
            dbContext.SaveChanges();
        }
    }
}
