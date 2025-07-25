using System;
using System.Collections.Generic;
using System.Linq;
using FootballManager.Enums;
using FootballManager.Utilities;

namespace FootballManager.Models
{
    public class Staff
    {
        public List<StaffMember> Members { get; set; } = new List<StaffMember>(); // List of all staff members

        // Method to Add a Staff Member
        public void AddStaffMember(StaffMember staffMember)
        {
            Members.Add(staffMember);
            Logger.Log($"Staff Member {staffMember.Name} added as {EnumHelper.GetDescription(staffMember.Job)}.");
        }

        // Method to Remove a Staff Member
        public void RemoveStaffMember(string staffName)
        {
            var staffMember = Members.FirstOrDefault(s => s.Name == staffName);
            if (staffMember != null)
            {
                Members.Remove(staffMember);
                Logger.Log($"Staff Member {staffMember.Name} removed.");
            }
            else
            {
                Logger.Log($"Staff Member {staffName} not found.");
            }
        }

        // Method to Display Staff Info
        public void DisplayStaff()
        {
            Console.WriteLine("Staff Members:");
            foreach (var member in Members)
            {
                Console.WriteLine($"- {member.Name}, Job: {EnumHelper.GetDescription(member.Job)}, Overall Rating: {member.CalculateOverallRating()}");
            }
        }
    }

    // StaffMember Class
    public class StaffMember
    {
        public int Id { get; set; } // Primary key
        public int? TeamId { get; set; } // Foreign key
        public Team? Team { get; set; } // Navigation property
        public string Name { get; set; }
        public Job Job { get; set; }
        public double TacticalKnowledge { get; set; }
        public double Leadership { get; set; }
        public double DecisionMaking { get; set; }
        public double Communication { get; set; }
        public double TrainingKnowledge { get; set; }
        public double FitnessKnowledge { get; set; } // Specific to Fitness Coach
        public double GoalkeepingKnowledge { get; set; } // Specific to Goalkeeping Coach
        public double MedicalKnowledge { get; set; } // Specific to Physio
        public double ScoutingAbility { get; set; } // Specific to Scout
        public double AnalyticalAbility { get; set; } // Specific to Analyst

        public StaffMember(string name, Job job)
        {
            Name = name;
            Job = job;
        }

        // Method to Calculate Overall Rating Based on Job
        public double CalculateOverallRating()
        {
            double overallRating = 0;

            switch (Job)
            {
                case Job.Manager:
                    // Manager-specific attributes
                    overallRating = (TacticalKnowledge + Leadership + DecisionMaking) / 3.0;
                    break;

                case Job.AssistantManager:
                    // Assistant Manager-specific attributes
                    overallRating = (Leadership + Communication + TrainingKnowledge) / 3.0;
                    break;

                case Job.Coach:
                    // Coach-specific attributes
                    overallRating = (TrainingKnowledge + Communication + TacticalKnowledge) / 3.0;
                    break;

                case Job.FitnessCoach:
                    // Fitness Coach-specific attributes
                    overallRating = FitnessKnowledge;
                    break;

                case Job.GoalkeepingCoach:
                    // Goalkeeping Coach-specific attributes
                    overallRating = GoalkeepingKnowledge;
                    break;

                case Job.Physio:
                    // Physio-specific attributes
                    overallRating = MedicalKnowledge;
                    break;

                case Job.Scout:
                    // Scout-specific attributes
                    overallRating = ScoutingAbility;
                    break;

                case Job.Analyst:
                    // Analyst-specific attributes
                    overallRating = AnalyticalAbility;
                    break;

                default:
                    throw new ArgumentException("Unknown job type");
            }

            return Math.Round(overallRating, 2); // Round to 2 decimal places
        }
    }
}