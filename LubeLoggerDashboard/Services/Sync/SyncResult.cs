using System;
using System.Collections.Generic;

namespace LubeLoggerDashboard.Services.Sync
{
    /// <summary>
    /// Represents the result of a synchronization operation.
    /// </summary>
    public class SyncResult
    {
        /// <summary>
        /// Gets the number of successful operations.
        /// </summary>
        public int SuccessCount { get; private set; }

        /// <summary>
        /// Gets the number of failed operations.
        /// </summary>
        public int FailureCount { get; private set; }

        /// <summary>
        /// Gets the number of skipped operations.
        /// </summary>
        public int SkippedCount { get; private set; }

        /// <summary>
        /// Gets the number of conflict operations.
        /// </summary>
        public int ConflictCount { get; private set; }

        /// <summary>
        /// Gets the collection of errors encountered during synchronization.
        /// </summary>
        public List<SyncError> Errors { get; } = new List<SyncError>();

        /// <summary>
        /// Gets the overall status of the synchronization operation.
        /// </summary>
        public SyncResultStatus Status
        {
            get
            {
                if (FailureCount == 0 && SuccessCount > 0)
                {
                    return SyncResultStatus.Success;
                }
                else if (FailureCount > 0 && SuccessCount > 0)
                {
                    return SyncResultStatus.PartialSuccess;
                }
                else
                {
                    return SyncResultStatus.Failure;
                }
            }
        }

        /// <summary>
        /// Gets the timestamp when the synchronization operation started.
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// Gets the timestamp when the synchronization operation completed.
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Gets the duration of the synchronization operation.
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncResult"/> class.
        /// </summary>
        public SyncResult()
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
        }

        /// <summary>
        /// Adds a successful operation to the result.
        /// </summary>
        /// <param name="entityType">The type of entity that was successfully synchronized.</param>
        /// <param name="entityId">The ID of the entity that was successfully synchronized.</param>
        public void AddSuccess(string entityType, int entityId)
        {
            SuccessCount++;
            UpdateEndTime();
        }

        /// <summary>
        /// Adds a failed operation to the result.
        /// </summary>
        /// <param name="entityType">The type of entity that failed to synchronize.</param>
        /// <param name="entityId">The ID of the entity that failed to synchronize.</param>
        /// <param name="error">The error that occurred during synchronization.</param>
        public void AddFailure(string entityType, int entityId, Exception error)
        {
            FailureCount++;
            Errors.Add(new SyncError(entityType, entityId, error));
            UpdateEndTime();
        }

        /// <summary>
        /// Adds a skipped operation to the result.
        /// </summary>
        /// <param name="entityType">The type of entity that was skipped.</param>
        /// <param name="entityId">The ID of the entity that was skipped.</param>
        /// <param name="reason">The reason the entity was skipped.</param>
        public void AddSkipped(string entityType, int entityId, string reason)
        {
            SkippedCount++;
            UpdateEndTime();
        }

        /// <summary>
        /// Adds a conflict operation to the result.
        /// </summary>
        /// <param name="entityType">The type of entity that had a conflict.</param>
        /// <param name="entityId">The ID of the entity that had a conflict.</param>
        /// <param name="resolution">The resolution strategy applied to the conflict.</param>
        public void AddConflict(string entityType, int entityId, string resolution)
        {
            ConflictCount++;
            UpdateEndTime();
        }

        /// <summary>
        /// Merges another sync result into this one.
        /// </summary>
        /// <param name="other">The other sync result to merge.</param>
        public void Merge(SyncResult other)
        {
            if (other == null)
            {
                return;
            }

            SuccessCount += other.SuccessCount;
            FailureCount += other.FailureCount;
            SkippedCount += other.SkippedCount;
            ConflictCount += other.ConflictCount;
            Errors.AddRange(other.Errors);
            
            if (other.EndTime > EndTime)
            {
                EndTime = other.EndTime;
            }
        }

        private void UpdateEndTime()
        {
            EndTime = DateTime.Now;
        }
    }

    /// <summary>
    /// Represents the overall status of a synchronization operation.
    /// </summary>
    public enum SyncResultStatus
    {
        /// <summary>
        /// All operations were successful.
        /// </summary>
        Success,

        /// <summary>
        /// Some operations were successful, but others failed.
        /// </summary>
        PartialSuccess,

        /// <summary>
        /// All operations failed.
        /// </summary>
        Failure
    }

    /// <summary>
    /// Represents an error that occurred during synchronization.
    /// </summary>
    public class SyncError
    {
        /// <summary>
        /// Gets the type of entity that failed to synchronize.
        /// </summary>
        public string EntityType { get; }

        /// <summary>
        /// Gets the ID of the entity that failed to synchronize.
        /// </summary>
        public int EntityId { get; }

        /// <summary>
        /// Gets the error that occurred during synchronization.
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// Gets the timestamp when the error occurred.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncError"/> class.
        /// </summary>
        /// <param name="entityType">The type of entity that failed to synchronize.</param>
        /// <param name="entityId">The ID of the entity that failed to synchronize.</param>
        /// <param name="error">The error that occurred during synchronization.</param>
        public SyncError(string entityType, int entityId, Exception error)
        {
            EntityType = entityType;
            EntityId = entityId;
            Error = error;
            Timestamp = DateTime.Now;
        }
    }
}