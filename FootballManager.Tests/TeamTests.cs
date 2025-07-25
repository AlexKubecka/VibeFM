using System;
using System.Linq;
using FootballManager.Models;
using FootballManager.Enums;
using Xunit;

namespace FootballManager.Tests
{
    public class TeamTests
    {
        [Fact]
        public void AddPlayer_ShouldAddPlayerToTeam()
        {
            // Arrange
            var team = new Team("Test Team", "International", "Test Stadium", 50000);
            var player = new Player("John Doe", 25, Position.Striker, "English");

            // Act
            team.AddPlayer(player);

            // Assert
            Assert.Contains(player, team.Players);
            Assert.Equal(team, player.Team);
        }

        [Fact]
        public void RemovePlayer_ShouldRemovePlayerFromTeam()
        {
            // Arrange
            var team = new Team("Test Team", "International", "Test Stadium", 50000);
            var player = new Player("John Doe", 25, Position.Striker, "English");
            team.AddPlayer(player);

            // Act
            team.RemovePlayer(player.Name);

            // Assert
            Assert.DoesNotContain(player, team.Players);
            Assert.Null(player.Team);
        }

        [Fact]
        public void RemovePlayer_ShouldLogMessage_WhenPlayerNotFound()
        {
            // Arrange
            var team = new Team("Test Team", "International", "Test Stadium", 50000);

            // Act
            team.RemovePlayer("Nonexistent Player");

            // Assert
            // You can verify logs if you implement a mock logger or capture log output
        }

        [Fact]
        public void LogTeamInfo_ShouldLogCorrectDetails()
        {
            // Arrange
            var team = new Team("Test Team", "International", "Test Stadium", 50000);

            // Act
            team.LogTeamInfo();

            // Assert
            // You can verify logs if you implement a mock logger or capture log output
        }
    }
}