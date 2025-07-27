using System;
using FootballManager.Enums;
using FootballManager.Models;

namespace FootballManager.Utilities
{
    public class PlayerFactory
    {
        private readonly Random _random = new Random();

        public Player CreatePlayer(string name, string position, double teamValue, double teamReputation)
        {
            // Adjust ranges based on team value and reputation
            int minAttribute = (int)Math.Max(50, teamReputation); // Minimum attribute value
            int maxAttribute = (int)Math.Min(100, teamValue / 10 + 50); // Maximum attribute value

            Player player = new Player(name, _random.Next(18, 35), position, "International");

            string pos = position.ToLowerInvariant();
            switch (pos)
            {
                case "goalkeeper":
                    player.Handling = _random.Next(minAttribute, maxAttribute);
                    player.Reflexes = _random.Next(minAttribute, maxAttribute);
                    player.Communication = _random.Next(minAttribute, maxAttribute);
                    player.AerialAbility = _random.Next(minAttribute, maxAttribute);
                    player.JumpingReach = _random.Next(minAttribute, maxAttribute);
                    break;
                case "right back":
                case "left back":
                    player.Marking = _random.Next(minAttribute, maxAttribute);
                    player.Tackling = _random.Next(minAttribute, maxAttribute);
                    player.Positioning = _random.Next(minAttribute, maxAttribute);
                    player.Speed = _random.Next(minAttribute, maxAttribute);
                    player.Crossing = _random.Next(minAttribute, maxAttribute);
                    break;
                case "center back":
                    player.Marking = _random.Next(minAttribute, maxAttribute);
                    player.Tackling = _random.Next(minAttribute, maxAttribute);
                    player.Heading = _random.Next(minAttribute, maxAttribute);
                    player.Positioning = _random.Next(minAttribute, maxAttribute);
                    player.Strength = _random.Next(minAttribute, maxAttribute);
                    break;
                case "defensive midfielder":
                    player.Passing = _random.Next(minAttribute, maxAttribute);
                    player.Vision = _random.Next(minAttribute, maxAttribute);
                    player.Teamwork = _random.Next(minAttribute, maxAttribute);
                    player.WorkRate = _random.Next(minAttribute, maxAttribute);
                    player.Tackling = _random.Next(minAttribute, maxAttribute);
                    break;
                case "central midfielder":
                    player.Passing = _random.Next(minAttribute, maxAttribute);
                    player.Vision = _random.Next(minAttribute, maxAttribute);
                    player.Teamwork = _random.Next(minAttribute, maxAttribute);
                    player.WorkRate = _random.Next(minAttribute, maxAttribute);
                    player.Dribbling = _random.Next(minAttribute, maxAttribute);
                    break;
                case "right winger":
                case "left winger":
                    player.Dribbling = _random.Next(minAttribute, maxAttribute);
                    player.Crossing = _random.Next(minAttribute, maxAttribute);
                    player.Speed = _random.Next(minAttribute, maxAttribute);
                    player.Finishing = _random.Next(minAttribute, maxAttribute);
                    player.Composure = _random.Next(minAttribute, maxAttribute);
                    break;
                case "striker":
                    player.Finishing = _random.Next(minAttribute, maxAttribute);
                    player.Dribbling = _random.Next(minAttribute, maxAttribute);
                    player.Shooting = _random.Next(minAttribute, maxAttribute);
                    player.Heading = _random.Next(minAttribute, maxAttribute);
                    player.Composure = _random.Next(minAttribute, maxAttribute);
                    break;
            }

            return player;
        }
    }
}