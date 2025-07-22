using System.ComponentModel;

namespace FootballManager.Enums
{
    public enum Position
    {
        [Description("Goalkeeper")]
        Goalkeeper,

        [Description("Right Back")]
        RightBack,

        [Description("Center Back")]
        CenterBack,

        [Description("Left Back")]
        LeftBack,

        [Description("Defensive Midfielder")]
        DefensiveMidfielder,

        [Description("Central Midfielder")]
        CentralMidfielder,

        [Description("Right Winger")]
        RightWinger,

        [Description("Left Winger")]
        LeftWinger,

        [Description("Striker")]
        Striker
    }
}