namespace LubeLoggerDashboard.Models.Database.Enums
{
    /// <summary>
    /// Represents the type of reminder trigger.
    /// </summary>
    public enum ReminderType
    {
        /// <summary>
        /// The reminder is triggered based on a specific date.
        /// </summary>
        Date,
        
        /// <summary>
        /// The reminder is triggered based on a specific odometer reading.
        /// </summary>
        Odometer,
        
        /// <summary>
        /// The reminder is triggered based on both date and odometer reading.
        /// </summary>
        Both
    }
}