using System;

namespace LubeLoggerDashboard.Services.Sync
{
    /// <summary>
    /// Event arguments for sync status change events.
    /// </summary>
    public class SyncStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the entity type that was synchronized.
        /// </summary>
        public string EntityType { get; }

        /// <summary>
        /// Gets the sync operation that was performed.
        /// </summary>
        public SyncOperation Operation { get; }

        /// <summary>
        /// Gets the status of the sync operation.
        /// </summary>
        public SyncStatus Status { get; }

        /// <summary>
        /// Gets the message associated with the sync status change.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncStatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="entityType">The entity type that was synchronized.</param>
        /// <param name="operation">The sync operation that was performed.</param>
        /// <param name="status">The status of the sync operation.</param>
        /// <param name="message">The message associated with the sync status change.</param>
        public SyncStatusChangedEventArgs(string entityType, SyncOperation operation, SyncStatus status, string message = null)
        {
            EntityType = entityType;
            Operation = operation;
            Status = status;
            Message = message;
        }
    }

    /// <summary>
    /// Represents the type of synchronization operation.
    /// </summary>
    public enum SyncOperation
    {
        /// <summary>
        /// Uploading data to the server.
        /// </summary>
        Upload,

        /// <summary>
        /// Downloading data from the server.
        /// </summary>
        Download,

        /// <summary>
        /// Refreshing expired cache data.
        /// </summary>
        Refresh,

        /// <summary>
        /// Resolving conflicts between local and server data.
        /// </summary>
        ConflictResolution,

        /// <summary>
        /// Checking connectivity status.
        /// </summary>
        ConnectivityCheck
    }

    /// <summary>
    /// Represents the status of a synchronization operation.
    /// </summary>
    public enum SyncStatus
    {
        /// <summary>
        /// The sync operation started.
        /// </summary>
        Started,

        /// <summary>
        /// The sync operation completed successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// The sync operation failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The sync operation was skipped.
        /// </summary>
        Skipped,

        /// <summary>
        /// The sync operation is in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// A conflict was detected during the sync operation.
        /// </summary>
        ConflictDetected
    }
}