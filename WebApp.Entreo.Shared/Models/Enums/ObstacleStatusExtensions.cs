namespace WebApp.Entreo.Models.Enums
{
    public static class ObstacleStatusExtensions
    {
        public static bool IsActive(this ObstacleStatus status)
        {
            return status switch
            {
                ObstacleStatus.Identified => true,
                ObstacleStatus.Analyzing => true,
                ObstacleStatus.Planning => true,
                ObstacleStatus.Implementing => true,
                ObstacleStatus.Monitoring => true,
                ObstacleStatus.Recurring => true,
                ObstacleStatus.Blocked => true,
                _ => false
            };
        }

        public static string GetDescription(this ObstacleStatus status)
        {
            return status switch
            {
                ObstacleStatus.Identified => "Obstacle has been identified and logged",
                ObstacleStatus.Analyzing => "Currently analyzing the root cause",
                ObstacleStatus.Planning => "Developing a strategy to overcome",
                ObstacleStatus.Implementing => "Actively implementing solution",
                ObstacleStatus.Monitoring => "Monitoring effectiveness of solution",
                ObstacleStatus.Resolved => "Obstacle has been successfully overcome",
                ObstacleStatus.Recurring => "Problem has resurfaced and needs attention",
                ObstacleStatus.Blocked => "Progress is blocked by external factors",
                ObstacleStatus.Abandoned => "Decided not to pursue resolution",
                ObstacleStatus.Delegated => "Responsibility assigned to another party",
                _ => "Unknown status"
            };
        }

        public static string GetColor(this ObstacleStatus status)
        {
            return status switch
            {
                ObstacleStatus.Identified => "#FFA500",  // Orange
                ObstacleStatus.Analyzing => "#FFD700",   // Gold
                ObstacleStatus.Planning => "#87CEEB",    // Sky Blue
                ObstacleStatus.Implementing => "#4169E1", // Royal Blue
                ObstacleStatus.Monitoring => "#9370DB",  // Medium Purple
                ObstacleStatus.Resolved => "#32CD32",    // Lime Green
                ObstacleStatus.Recurring => "#FF6347",   // Tomato
                ObstacleStatus.Blocked => "#DC143C",     // Crimson
                ObstacleStatus.Abandoned => "#808080",   // Gray
                ObstacleStatus.Delegated => "#20B2AA",   // Light Sea Green
                _ => "#000000"                           // Black
            };
        }
    }
}