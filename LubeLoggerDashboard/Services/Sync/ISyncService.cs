using System;
using System.Threading.Tasks;
using LubeLoggerDashboard.Models.Database.Entities;

namespace LubeLoggerDashboard.Services.Sync
{
    /// <summary>
    /// Interface for the synchronization service that handles data synchronization between the local cache and the server.
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Event that is raised when the sync status changes.
        /// </summary>
        event EventHandler<SyncStatusChangedEventArgs> SyncStatusChanged;

        /// <summary>
        /// Synchronizes all entity types between the local cache and the server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<SyncResult> SyncAllAsync();

        /// <summary>
        /// Synchronizes a specific entity type between the local cache and the server.
        /// </summary>
        /// <typeparam name="T">The entity type to synchronize.</typeparam>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<SyncResult> SyncEntityAsync<T>() where T : BaseEntity;

        /// <summary>
        /// Uploads pending changes to the server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with the sync result.</returns>
        Task<SyncResult> UploadPendingChangesAsync();

        /// <summary>
        /// Refreshes expired cache data from the server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with the sync result.</returns>
        Task<SyncResult> RefreshExpiredCacheAsync();

        /// <summary>
        /// Checks if the application is online and can connect to the server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with a boolean indicating if the application is online.</returns>
        Task<bool> IsOnlineAsync();

        /// <summary>
        /// Gets the last synchronization time for a specific entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>A task representing the asynchronous operation with the last sync time.</returns>
        Task<DateTime?> GetLastSyncTimeAsync<T>() where T : BaseEntity;

        /// <summary>
        /// Forces a full synchronization of a specific entity, regardless of its current sync status.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entityId">The ID of the entity to synchronize.</param>
        /// <returns>A task representing the asynchronous operation with the sync result.</returns>
        Task<SyncResult> ForceSyncEntityAsync<T>(int entityId) where T : BaseEntity;

        /// <summary>
        /// Cancels any ongoing synchronization operations.
        /// </summary>
        void CancelSync();
    }
}