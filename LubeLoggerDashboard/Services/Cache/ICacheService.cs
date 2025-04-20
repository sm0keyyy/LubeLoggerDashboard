using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LubeLoggerDashboard.Models.Database.Entities;
using LubeLoggerDashboard.Models.Database.Enums;

namespace LubeLoggerDashboard.Services.Cache
{
    /// <summary>
    /// Interface for the cache service that manages cache operations, including checking expiration,
    /// updating expiration timestamps, and tracking sync status.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Checks if an entity is expired and needs to be refreshed from the API.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The entity ID.</param>
        /// <returns>True if the entity is expired, false otherwise.</returns>
        Task<bool> IsExpiredAsync<T>(int id) where T : BaseEntity;

        /// <summary>
        /// Checks if any entities of type T need refreshing from the API.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>True if any entities need refreshing, false otherwise.</returns>
        Task<bool> NeedsRefreshAsync<T>() where T : BaseEntity;

        /// <summary>
        /// Updates the expiration timestamp for an entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateExpirationAsync<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// Gets the configured expiration minutes for an entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The expiration minutes for the entity type.</returns>
        Task<int> GetExpirationMinutesAsync<T>() where T : BaseEntity;

        /// <summary>
        /// Checks if an entity type is marked as critical.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>True if the entity type is critical, false otherwise.</returns>
        Task<bool> IsCriticalEntityAsync<T>() where T : BaseEntity;

        /// <summary>
        /// Gets all entities of type T that need synchronization with the server.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>A collection of entities that need synchronization.</returns>
        Task<IEnumerable<T>> GetPendingSyncItemsAsync<T>() where T : BaseEntity;

        /// <summary>
        /// Marks an entity as synchronized with the server.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to mark as synced.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkAsSyncedAsync<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// Marks an entity for synchronization with the server.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity to mark for synchronization.</param>
        /// <param name="status">The synchronization status to set.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkForSyncAsync<T>(T entity, SyncStatus status) where T : BaseEntity;
    }
}