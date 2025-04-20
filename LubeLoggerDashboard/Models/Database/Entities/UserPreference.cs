namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents a user preference setting.
    /// </summary>
    public class UserPreference : BaseEntity
    {
        /// <summary>
        /// Gets or sets the key of the preference.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Gets or sets the value of the preference.
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// Gets or sets the category of the preference.
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this is a system preference.
        /// </summary>
        public bool IsSystemPreference { get; set; }
    }
}