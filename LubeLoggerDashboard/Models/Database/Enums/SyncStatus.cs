namespace LubeLoggerDashboard.Models.Database.Enums
{
    /// <summary>
    /// Represents the synchronization status of an entity in the local cache.
    /// </summary>
    public enum SyncStatus
    {
        /// <summary>
        /// The entity is synchronized with the server.
        /// </summary>
        Synced,
        
        /// <summary>
        /// The entity is pending upload to the server (new entity).
        /// </summary>
        PendingUpload,
        
        /// <summary>
        /// The entity is pending update on the server (modified entity).
        /// </summary>
        PendingUpdate,
        
        /// <summary>
        /// The entity is pending deletion on the server.
        /// </summary>
        PendingDeletion,
        
        /// <summary>
        /// The synchronization of the entity with the server failed.
        /// </summary>
        SyncFailed
    }
}