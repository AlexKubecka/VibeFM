using FootballManager.Models;
using FootballManager.Enums;
using Xunit;

namespace FootballManager.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void CalculateOverallRating_ShouldReturnCorrectRating_ForGoalkeeper()
        {
            // Arrange
            var player = new Player("John Doe", 25, Position.Goalkeeper, "English")
            {
                Handling = 80,
                Reflexes = 85,
                AerialAbility = 75,
                Communication = 70,
                Throwing = 65,
                OneOnOnes = 90,
                JumpingReach = 80
            };

            // Act
            var rating = player.CalculateOverallRating();

            // Assert
            Assert.Equal(77.86, rating);
        }

        [Fact]
        public void ToString_ShouldReturnFormattedPlayerInfo()
        {
            // Arrange
            var player = new Player("John Doe", 25, Position.Striker, "English")
            {
                Finishing = 90,
                Dribbling = 85,
                Shooting = 80,
                Heading = 75,
                Composure = 70,
                Anticipation = 65,
                Positioning = 60
            };

            // Act
            var playerInfo = player.ToString();
            // Assert
            Assert.Contains("John Doe", playerInfo);
            Assert.Contains("Striker", playerInfo);
            Assert.Equal(75.0, player.CalculateOverallRating());
        }
    }
}