using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FootballManager.Enums;
using FootballManager.Models;

namespace FormationDisplay
{
    public class FormationDisplay : Form
    {
        public FormationDisplay(List<Player> team)
        {
            this.Text = "Football Formation";
            this.Size = new Size(800, 600);
            this.BackColor = Color.Green;

            // Create labels for each player in the formation
            foreach (var player in team)
            {
                Label playerLabel = new Label
                {
                    Text = $"{player.Name}\n{player.Position}",
                    Size = new Size(100, 50),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Position players based on their roles
                switch (player.Position)
                {
                    case Position.Goalkeeper:
                        playerLabel.Location = new Point(350, 500); // Goalkeeper at the bottom center
                        break;

                    case Position.RightBack:
                        playerLabel.Location = new Point(600, 400); // Right Back
                        break;

                    case Position.LeftBack:
                        playerLabel.Location = new Point(100, 400); // Left Back
                        break;

                    case Position.CenterBack:
                        playerLabel.Location = new Point(250, 400); // Center Back 1
                        this.Controls.Add(playerLabel);
                        playerLabel = new Label
                        {
                            Text = $"{player.Name}\n{player.Position}",
                            Size = new Size(100, 50),
                            TextAlign = ContentAlignment.MiddleCenter,
                            BackColor = Color.White,
                            BorderStyle = BorderStyle.FixedSingle,
                            Location = new Point(450, 400) // Center Back 2
                        };
                        break;

                    case Position.DefensiveMidfielder:
                        playerLabel.Location = new Point(350, 300); // Defensive Midfielder
                        break;

                    case Position.CentralMidfielder:
                        playerLabel.Location = new Point(250, 200); // Central Midfielder 1
                        this.Controls.Add(playerLabel);
                        playerLabel = new Label
                        {
                            Text = $"{player.Name}\n{player.Position}",
                            Size = new Size(100, 50),
                            TextAlign = ContentAlignment.MiddleCenter,
                            BackColor = Color.White,
                            BorderStyle = BorderStyle.FixedSingle,
                            Location = new Point(450, 200) // Central Midfielder 2
                        };
                        break;

                    case Position.RightWinger:
                        playerLabel.Location = new Point(600, 100); // Right Winger
                        break;

                    case Position.LeftWinger:
                        playerLabel.Location = new Point(100, 100); // Left Winger
                        break;

                    case Position.Striker:
                        playerLabel.Location = new Point(350, 50); // Striker
                        break;
                }

                this.Controls.Add(playerLabel);
            }
        }

        [STAThread]
        static void Main()
        {
            // Create a team of 11 players
            List<Player> team = new List<Player>
            {
                new Player("John Doe", 25, Position.Goalkeeper, "English"),
                new Player("Jane Smith", 28, Position.RightBack, "Spanish"),
                new Player("Carlos Ruiz", 24, Position.CenterBack, "Mexican"),
                new Player("Lucas Martinez", 21, Position.CenterBack, "Argentinian"),
                new Player("Ella Wilson", 29, Position.LeftBack, "German"),
                new Player("Emily Johnson", 22, Position.DefensiveMidfielder, "American"),
                new Player("Liam Brown", 26, Position.CentralMidfielder, "Australian"),
                new Player("Sophia Lee", 23, Position.CentralMidfielder, "South Korean"),
                new Player("Oliver Davis", 27, Position.RightWinger, "Canadian"),
                new Player("Mia Taylor", 24, Position.LeftWinger, "French"),
                new Player("Noah White", 30, Position.Striker, "Dutch")
            };

            Application.EnableVisualStyles();
            Application.Run(new FormationDisplay(team));
        }
    }
}