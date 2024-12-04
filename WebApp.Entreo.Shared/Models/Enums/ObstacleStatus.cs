namespace WebApp.Entreo.Models.Enums
{
    public enum ObstacleStatus
    {
        Identified = 0,        // Just discovered/logged
        Analyzing = 1,         // Investigating the root cause
        Planning = 2,          // Developing solution strategy
        Implementing = 3,      // Actively working on solution
        Monitoring = 4,        // Testing if solution works
        Resolved = 5,          // Successfully overcome
        Recurring = 6,         // Problem has returned
        Blocked = 7,           // Cannot proceed currently
        Abandoned = 8,         // Decided not to address
        Delegated = 9         // Assigned to someone else
    }
}