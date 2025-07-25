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
                Reputation = reputation
            };

            // Add staff members
            team.TeamStaff.AddStaffMember(new StaffMember($"Manager {name}", Job.Manager)
            {
                TacticalKnowledge = _random.Next(70, 100),
                Leadership = _random.Next(70, 100),
                DecisionMaking = _random.Next(70, 100)
            });

            team.TeamStaff.AddStaffMember(new StaffMember($"Assistant Manager {name}", Job.AssistantManager)
            {
                Leadership = _random.Next(60, 90),
                Communication = _random.Next(60, 90),
                TrainingKnowledge = _random.Next(60, 90)
            });

            team.TeamStaff.AddStaffMember(new StaffMember($"Coach {name}", Job.Coach)
            {
                TacticalKnowledge = _random.Next(60, 90)
            });

            team.TeamStaff.AddStaffMember(new StaffMember($"Fitness Coach {name}", Job.FitnessCoach)
            {
                FitnessKnowledge = _random.Next(60, 90)
            });

            team.TeamStaff.AddStaffMember(new StaffMember($"Goalkeeping Coach {name}", Job.GoalkeepingCoach)
            {
                GoalkeepingKnowledge = _random.Next(60, 90)
            });

            team.TeamStaff.AddStaffMember(new StaffMember($"Physio {name}", Job.Physio)
            {
                MedicalKnowledge = _random.Next(60, 90)
            });

            team.TeamStaff.AddStaffMember(new StaffMember($"Scout {name}", Job.Scout)
            {
                ScoutingAbility = _random.Next(60, 90)
            });

            team.TeamStaff.AddStaffMember(new StaffMember($"Analyst {name}", Job.Analyst)
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