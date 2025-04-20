namespace LubeLoggerDashboard.Models.Database.Enums
{
    /// <summary>
    /// Represents the frequency unit for recurring reminders.
    /// </summary>
    public enum ReminderFrequency
    {
        /// <summary>
        /// The reminder recurs based on a number of days.
        /// </summary>
        Days,
        
        /// <summary>
        /// The reminder recurs based on a number of weeks.
        /// </summary>
        Weeks,
        
        /// <summary>
        /// The reminder recurs based on a number of months.
        /// </summary>
        Months,
        
        /// <summary>
        /// The reminder recurs based on a number of years.
        /// </summary>
        Years,
        
        /// <summary>
        /// The reminder recurs based on a number of miles.
        /// </summary>
        Miles,
        
        /// <summary>
        /// The reminder recurs based on a number of kilometers.
        /// </summary>
        Kilometers
    }
}