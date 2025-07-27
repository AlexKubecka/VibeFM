using System;
using FootballManager.Enums;
using FootballManager.Models;

namespace FootballManager.Utilities
{
    public class TeamFactory
    {
        private readonly Random _random = new Random();

        public Team CreateTeam(string name, double reputation, int stadiumCapacity)
        {
            double value = CalculateTeamValue(reputation, stadiumCapacity);

            Team team = new Team(name, "International", $"Stadium {name}", stadiumCapacity)
            {
                Value = value,
                Reputation = reputation,
                Wins = 0,
                Draws = 0,
                Losses = 0,
                GoalsFor = 0,
                GoalsAgainst = 0
            };

            // Add staff members directly to the StaffMembers collection
            team.StaffMembers.Add(new StaffMember($"Manager {name}", Job.Manager)
            {
                TacticalKnowledge = _random.Next(70, 100),
                Leadership = _random.Next(70, 100),
                DecisionMaking = _random.Next(70, 100)
            });

            team.StaffMembers.Add(new StaffMember($"Assistant Manager {name}", Job.AssistantManager)
            {
                Leadership = _random.Next(60, 90),
                Communication = _random.Next(60, 90),
                TrainingKnowledge = _random.Next(60, 90)
            });

            team.StaffMembers.Add(new StaffMember($"Coach {name}", Job.Coach)
            {
                TacticalKnowledge = _random.Next(60, 90),
                Communication = _random.Next(60, 90),
                TrainingKnowledge = _random.Next(60, 90)
            });

            team.StaffMembers.Add(new StaffMember($"Fitness Coach {name}", Job.FitnessCoach)
            {
                FitnessKnowledge = _random.Next(60, 90)
            });

            team.StaffMembers.Add(new StaffMember($"Goalkeeping Coach {name}", Job.GoalkeepingCoach)
            {
                GoalkeepingKnowledge = _random.Next(60, 90)
            });

            team.StaffMembers.Add(new StaffMember($"Physio {name}", Job.Physio)
            {
                MedicalKnowledge = _random.Next(60, 90)
            });

            team.StaffMembers.Add(new StaffMember($"Scout {name}", Job.Scout)
            {
                ScoutingAbility = _random.Next(60, 90)
            });

            team.StaffMembers.Add(new StaffMember($"Analyst {name}", Job.Analyst)
            {
                AnalyticalAbility = _random.Next(60, 90)
            });

            return team;
        }

        private double CalculateTeamValue(double reputation, int stadiumCapacity)
        {
            // Formula: Value is influenced by reputation and stadium capacity
            return Math.Round((reputation * 10) + (stadiumCapacity / 1000), 2);
        }
    }
}