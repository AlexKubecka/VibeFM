using System.ComponentModel;

namespace FootballManager.Enums
{
    public enum Job
    {
        [Description("Manager")]
        Manager,

        [Description("Assistant Manager")]
        AssistantManager,

        [Description("Coach")]
        Coach,

        [Description("Fitness Coach")]
        FitnessCoach,

        [Description("Goalkeeping Coach")]
        GoalkeepingCoach,

        [Description("Physio")]
        Physio,

        [Description("Scout")]
        Scout,

        [Description("Analyst")]
        Analyst
    }
}