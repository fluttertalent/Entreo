namespace WebApp.Entreo.Models.Enums
{
    public static class EnergyLevelExtensions
    {
        public static string GetDescription(this EnergyLevel level)
        {
            return level switch
            {
                EnergyLevel.Exhausted => "Completely drained, need rest",
                EnergyLevel.VeryLow => "Very little energy, basic tasks are challenging",
                EnergyLevel.Low => "Below average energy, conserving resources",
                EnergyLevel.Moderate => "Average energy level, can handle routine tasks",
                EnergyLevel.Good => "Good energy level, feeling capable",
                EnergyLevel.High => "High energy, feeling productive",
                EnergyLevel.Optimal => "Peak energy level, maximum productivity",
                _ => "Unknown energy level"
            };
        }

        public static string GetEmoji(this EnergyLevel level)
        {
            return level switch
            {
                EnergyLevel.Exhausted => "🔋",    // Empty battery
                EnergyLevel.VeryLow => "⚡",      // Single bolt
                EnergyLevel.Low => "🔋",          // Low battery
                EnergyLevel.Moderate => "⚡⚡",    // Double bolt
                EnergyLevel.Good => "🔋",         // Full battery
                EnergyLevel.High => "⚡⚡⚡",      // Triple bolt
                EnergyLevel.Optimal => "💪",      // Flexed biceps
                _ => "❓"
            };
        }

        public static string GetColor(this EnergyLevel level)
        {
            return level switch
            {
                EnergyLevel.Exhausted => "#FF0000",  // Red
                EnergyLevel.VeryLow => "#FF6B6B",    // Light Red
                EnergyLevel.Low => "#FFA07A",        // Light Salmon
                EnergyLevel.Moderate => "#FFD700",    // Gold
                EnergyLevel.Good => "#98FB98",       // Pale Green
                EnergyLevel.High => "#32CD32",       // Lime Green
                EnergyLevel.Optimal => "#00FF00",    // Bright Green
                _ => "#AAAAAA"                       // Gray
            };
        }

        public static bool IsLow(this EnergyLevel level)
        {
            return level <= EnergyLevel.Low;
        }

        public static bool IsHigh(this EnergyLevel level)
        {
            return level >= EnergyLevel.Good;
        }

        public static string GetRecommendedActivities(this EnergyLevel level)
        {
            return level switch
            {
                EnergyLevel.Exhausted => "Rest, recovery, and essential tasks only",
                EnergyLevel.VeryLow => "Light activities, self-care, and basic tasks",
                EnergyLevel.Low => "Routine tasks, avoid demanding activities",
                EnergyLevel.Moderate => "Regular activities and moderate challenges",
                EnergyLevel.Good => "Productive work and challenging tasks",
                EnergyLevel.High => "High-intensity activities and complex tasks",
                EnergyLevel.Optimal => "Peak performance activities and important challenges",
                _ => "Assess energy level before choosing activities"
            };
        }
    }
}