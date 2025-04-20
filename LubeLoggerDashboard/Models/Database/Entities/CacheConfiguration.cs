namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents configuration settings for entity caching.
    /// </summary>
    public class CacheConfiguration
    {
        /// <summary>
        /// Gets or sets the unique identifier for the configuration.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the entity type this configuration applies to.
        /// </summary>
        public string EntityName { get; set; }
        
        /// <summary>
        /// Gets or sets the expiration time in minutes for cached entities of this type.
        /// </summary>
        public int ExpirationMinutes { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this entity type is critical for application functionality.
        /// </summary>
        public bool IsCritical { get; set; }
        
        /// <summary>
        /// Gets or sets the synchronization priority for this entity type (lower values = higher priority).
        /// </summary>
        public int SyncPriority { get; set; }
    }
}