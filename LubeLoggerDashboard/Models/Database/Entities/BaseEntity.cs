using System;
using LubeLoggerDashboard.Models.Database.Enums;

namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Base abstract class for all database entities with common properties for cache management.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the timestamp of the last synchronization with the server.
        /// </summary>
        public DateTime LastSyncTimestamp { get; set; }
        
        /// <summary>
        /// Gets or sets the timestamp when the cached entity expires and should be refreshed.
        /// </summary>
        public DateTime ExpirationTimestamp { get; set; }
        
        /// <summary>
        /// Gets or sets the synchronization status of the entity.
        /// </summary>
        public SyncStatus SyncStatus { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the entity has been modified locally.
        /// </summary>
        public bool IsDirty { get; set; }
    }
}