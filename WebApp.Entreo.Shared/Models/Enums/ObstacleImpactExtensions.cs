namespace WebApp.Entreo.Models.Enums
{
    public static class ObstacleImpactExtensions
    {
        public static string GetDescription(this ObstacleImpact impact)
        {
            return impact switch
            {
                ObstacleImpact.Negligible => "Minimal effect on habit formation, can be easily overcome",
                ObstacleImpact.Minor => "Small hindrance that slightly affects habit performance",
                ObstacleImpact.Moderate => "Notable impact that affects habit consistency",
                ObstacleImpact.Major => "Significant barrier that seriously disrupts habit formation",
                ObstacleImpact.Severe => "Critical obstacle that completely blocks habit formation",
                ObstacleImpact.Variable => "Impact level varies based on circumstances or timing",
                ObstacleImpact.Cascading => "Impact affects multiple habits or related behaviors",
                _ => "Unknown impact level"
            };
        }

        public static string GetColor(this ObstacleImpact impact)
        {
            return impact switch
            {
                ObstacleImpact.Negligible => "#98FB98",  // Pale Green
                ObstacleImpact.Minor => "#FFF68F",       // Khaki
                ObstacleImpact.Moderate => "#FFA07A",    // Light Salmon
                ObstacleImpact.Major => "#FF6B6B",       // Light Red
                ObstacleImpact.Severe => "#DC143C",      // Crimson
                ObstacleImpact.Variable => "#DDA0DD",    // Plum
                ObstacleImpact.Cascading => "#9932CC",   // Dark Orchid
                _ => "#808080"                           // Gray
            };
        }

        public static int GetPriorityScore(this ObstacleImpact impact)
        {
            return impact switch
            {
                ObstacleImpact.Negligible => 1,
                ObstacleImpact.Minor => 2,
                ObstacleImpact.Moderate => 3,
                ObstacleImpact.Major => 4,
                ObstacleImpact.Severe => 5,
                ObstacleImpact.Variable => 3,  // Default to moderate for variable impact
                ObstacleImpact.Cascading => 4, // High priority due to multiple effects
                _ => 0
            };
        }

        public static string GetRecommendedAction(this ObstacleImpact impact)
        {
            return impact switch
            {
                ObstacleImpact.Negligible => "Monitor and address during routine reviews",
                ObstacleImpact.Minor => "Address when convenient, maintain awareness",
                ObstacleImpact.Moderate => "Develop specific action plan within next few days",
                ObstacleImpact.Major => "Immediate attention required, prioritize resolution",
                ObstacleImpact.Severe => "Urgent intervention needed, may need external support",
                ObstacleImpact.Variable => "Identify trigger patterns and prepare contingency plans",
                ObstacleImpact.Cascading => "Systematic analysis needed, address root causes",
                _ => "Assessment needed"
            };
        }

        public static bool RequiresImmediate(this ObstacleImpact impact)
        {
            return impact switch
            {
                ObstacleImpact.Major => true,
                ObstacleImpact.Severe => true,
                ObstacleImpact.Cascading => true,
                _ => false
            };
        }
    }
}