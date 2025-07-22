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
            Console.WriteLine($"Staff Member {staffMember.Name} added as {EnumHelper.GetDescription(staffMember.Job)}.");
        }

        // Method to Remove a Staff Member
        public void RemoveStaffMember(string staffName)
        {
            var staffMember = Members.FirstOrDefault(s => s.Name == staffName);
            if (staffMember != null)
            {
                Members.Remove(staffMember);
                Console.WriteLine($"Staff Member {staffMember.Name} removed.");
            }
            else
            {
                Console.WriteLine($"Staff Member {staffName} not found.");
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
        public string Name { get; set; }
        public Job Job { get; set; } // Role of the staff member (e.g., Manager, Assistant Manager, Coach, etc.)
        public int TacticalKnowledge { get; set; } = 50; // Default 0-100 scale
        public int Leadership { get; set; } = 50;
        public int Communication { get; set; } = 50;
        public int DecisionMaking { get; set; } = 50;
        public int TrainingKnowledge { get; set; } = 50;
        public int FitnessKnowledge { get; set; } = 50;
        public int MedicalExpertise { get; set; } = 50;
        public int TalentIdentification { get; set; } = 50;
        public int DataAnalysis { get; set; } = 50;

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
                    overallRating = (FitnessKnowledge + Communication + TrainingKnowledge) / 3.0;
                    break;

                case Job.GoalkeepingCoach:
                    // Goalkeeping Coach-specific attributes
                    overallRating = (TrainingKnowledge + Communication + TacticalKnowledge) / 3.0;
                    break;

                case Job.Physio:
                    // Physio-specific attributes
                    overallRating = (MedicalExpertise + Communication + DecisionMaking) / 3.0;
                    break;

                case Job.Scout:
                    // Scout-specific attributes
                    overallRating = (TalentIdentification + Communication + DecisionMaking) / 3.0;
                    break;

                case Job.Analyst:
                    // Analyst-specific attributes
                    overallRating = (DataAnalysis + TacticalKnowledge + Communication) / 3.0;
                    break;

                default:
                    throw new ArgumentException("Unknown job type");
            }

            return Math.Round(overallRating, 2); // Round to 2 decimal places
        }
    }
}