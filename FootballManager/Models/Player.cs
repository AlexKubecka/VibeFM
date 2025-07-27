using FootballManager.Enums;
using FootballManager.Utilities;

namespace FootballManager.Models
{
    public class Player
    {
        // Db Attributes
        public int Id { get; set; } // Primary key
        public int? TeamId { get; set; } // Foreign key

        // Basic Attributes
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public Team? Team { get; set; } // Reference to the team the player belongs to
        // Market value in millions (from Transfermarkt)
        public double MarketValue { get; set; }

        // Physical Attributes
        public double Height { get; set; } = 1.8; // Default height in meters
        public double Weight { get; set; } = 75; // Default weight in kilograms
        public int Speed { get; set; } = 50; // Default 0-100 scale
        public int Stamina { get; set; } = 50;
        public int Strength { get; set; } = 50;
        public int Agility { get; set; } = 50;
        public int JumpingReach { get; set; } = 50;

        // Mental Attributes
        public int DecisionMaking { get; set; } = 50;
        public int Leadership { get; set; } = 50;
        public int WorkRate { get; set; } = 50;
        public int Teamwork { get; set; } = 50;
        public int Vision { get; set; } = 50;
        public int Concentration { get; set; } = 50;
        public int Determination { get; set; } = 50;
        public int Composure { get; set; } = 50;
        public int Anticipation { get; set; } = 50;

        // Technical Attributes
        public int Passing { get; set; } = 50;
        public int Shooting { get; set; } = 50;
        public int Dribbling { get; set; } = 50;
        public int Tackling { get; set; } = 50;
        public int Crossing { get; set; } = 50;
        public int Finishing { get; set; } = 50;
        public int FirstTouch { get; set; } = 50;
        public int Heading { get; set; } = 50;
        public int Marking { get; set; } = 50;
        public int Positioning { get; set; } = 50;

        // Goalkeeper Attributes
        public int Handling { get; set; } = 50;
        public int Reflexes { get; set; } = 50;
        public int AerialAbility { get; set; } = 50;
        public int Communication { get; set; } = 50;
        public int Throwing { get; set; } = 50;
        public int OneOnOnes { get; set; } = 50;

        // Personality Attributes
        public int Professionalism { get; set; } = 50;
        public int Sportsmanship { get; set; } = 50;
        public int Temperament { get; set; } = 50;
        public int Ambition { get; set; } = 50;
        public int Loyalty { get; set; } = 50;

        // Parameterless constructor for EF Core
        public Player() { }

        // Constructor with only basic attributes required
        public Player(string name, int age, string position, string nationality, Team? team = null)
        {
            Name = name;
            Age = age;
            Position = position;
            Nationality = nationality;
            Team = team; // Assign the team (optional)
        }

        // Method to Get Formatted Info
        public string GetFormattedInfo()
        {
            return $"{Name}, Position: {Position}, Overall Rating: {CalculateOverallRating()}";
        }

        // Method to calculate Overall Rating based on Position
        public double CalculateOverallRating()
        {
            double overallRating = 0;
            string pos = Position.ToLowerInvariant();
            if (pos == "goalkeeper")
                overallRating = (Handling + Reflexes + AerialAbility + Communication + Throwing + OneOnOnes + JumpingReach) / 7.0;
            else if (pos == "right back" || pos == "left back")
                overallRating = (Marking + Tackling + Positioning + Strength + Speed + Crossing + Stamina) / 7.0;
            else if (pos == "center back")
                overallRating = (Marking + Tackling + Heading + Positioning + Strength + Concentration + Anticipation) / 7.0;
            else if (pos == "defensive midfielder")
                overallRating = (Passing + Vision + Teamwork + WorkRate + Tackling + Strength + DecisionMaking) / 7.0;
            else if (pos == "central midfielder")
                overallRating = (Passing + Vision + Teamwork + WorkRate + Stamina + DecisionMaking + Dribbling) / 7.0;
            else if (pos == "right winger" || pos == "left winger")
                overallRating = (Dribbling + Crossing + Speed + Finishing + Composure + Vision + Positioning) / 7.0;
            else if (pos == "striker")
                overallRating = (Finishing + Dribbling + Shooting + Heading + Composure + Anticipation + Positioning) / 7.0;
            else
                overallRating = 50; // Default fallback
            return Math.Round(overallRating, 2); // Round to 2 decimal places
        }

        // Log Player Info (for debugging purposes)
        public void LogPlayerInfo()
        {
            Logger.Log($"Player: {Name} ({Nationality})");
            Logger.Log($"Age: {Age}, Position: {Position}");
            Logger.Log($"Overall Rating: {CalculateOverallRating()}");
        }

        // Print Player Info (dedicated printing function)
        public void PrintPlayerInfo()
        {
            Console.WriteLine($"Player: {Name} ({Nationality})");
            Console.WriteLine($"Age: {Age}, Position: {Position}");
            Console.WriteLine($"Overall Rating: {CalculateOverallRating()}");
        }

        // Method to Display Player Info as a String
        public override string ToString()
        {
            return $@"
        Player Information:
        -------------------
        Name:           {Name}
        Age:            {Age}
        Position:       {Position}
        Nationality:    {Nationality}
        Team:           {(Team != null ? Team.Name : "No Team")}
        Height:         {Height} m
        Weight:         {Weight} kg
        Market Value:  €{MarketValue} m

        Physical Attributes:
        --------------------
        Speed:          {Speed}/100
        Stamina:        {Stamina}/100
        Strength:       {Strength}/100
        Agility:        {Agility}/100

        Skill Attributes:
        -----------------
        Handling:       {Handling}/100
        Reflexes:       {Reflexes}/100
        Communication:  {Communication}/100
        Aerial Ability: {AerialAbility}/100
        Jumping Reach:  {JumpingReach}/100
        Passing:        {Passing}/100
        Shooting:       {Shooting}/100
        Dribbling:      {Dribbling}/100
        Tackling:       {Tackling}/100
        Crossing:       {Crossing}/100
        Finishing:      {Finishing}/100
        Positioning:    {Positioning}/100
        Heading:        {Heading}/100
        Composure:      {Composure}/100
        Anticipation:   {Anticipation}/100

        Overall Rating: {CalculateOverallRating()}
        ";
        }
    }
}