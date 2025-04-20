using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LubeLoggerDashboard.Models.Database.Context;
using LubeLoggerDashboard.Models.Database.Entities;
using LubeLoggerDashboard.Models.Database.Enums;

namespace LubeLoggerDashboard.Services.Cache
{
    /// <summary>
    /// Service that manages cache operations, including checking expiration,
    /// updating expiration timestamps, and tracking sync status.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly LubeLoggerDbContext _dbContext;
        private readonly ILogger<CacheService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public CacheService(LubeLoggerDbContext dbContext, ILogger<CacheService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<bool> IsExpiredAsync<T>(int id) where T : BaseEntity
        {
            try
            {
                var entityName = typeof(T).Name;
                var entity = await GetEntityByIdAsync<T>(id);

                if (entity == null)
                {
                    _logger.LogWarning("Entity {EntityType} with ID {EntityId} not found in cache", entityName, id);
                    return true; // If entity doesn't exist, consider it expired
                }

                var isExpired = entity.ExpirationTimestamp < DateTime.UtcNow;
                _logger.LogDebug("Entity {EntityType} with ID {EntityId} expiration check: {IsExpired}", 
                    entityName, id, isExpired);
                
                return isExpired;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if entity {EntityType} with ID {EntityId} is expired", 
                    typeof(T).Name, id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> NeedsRefreshAsync<T>() where T : BaseEntity
        {
            try
            {
                var entityName = typeof(T).Name;
                
                // Check if any entity of type T is expired
                var anyExpired = await _dbContext.Set<T>()
                    .AnyAsync(e => e.ExpirationTimestamp < DateTime.UtcNow);

                _logger.LogDebug("Entities of type {EntityType} need refresh: {NeedsRefresh}", 
                    entityName, anyExpired);
                
                return anyExpired;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if entities of type {EntityType} need refresh", 
                    typeof(T).Name);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateExpirationAsync<T>(T entity) where T : BaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                var entityName = typeof(T).Name;
                var expirationMinutes = await GetExpirationMinutesAsync<T>();
                
                // Update expiration timestamp
                entity.ExpirationTimestamp = DateTime.UtcNow.AddMinutes(expirationMinutes);
                
                // If entity is already tracked by the context, just update it
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbContext.Attach(entity);
                }
                
                _dbContext.Entry(entity).Property(e => e.ExpirationTimestamp).IsModified = true;
                await _dbContext.SaveChangesAsync();
                
                _logger.LogDebug("Updated expiration for {EntityType} with ID {EntityId} to {ExpirationTime}", 
                    entityName, entity.Id, entity.ExpirationTimestamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expiration for {EntityType} with ID {EntityId}", 
                    typeof(T).Name, entity.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetExpirationMinutesAsync<T>() where T : BaseEntity
        {
            try
            {
                var entityName = typeof(T).Name;
                
                var cacheConfig = await _dbContext.CacheConfigurations
                    .FirstOrDefaultAsync(c => c.EntityName == entityName);

                if (cacheConfig == null)
                {
                    _logger.LogWarning("No cache configuration found for entity type {EntityType}. Using default expiration.", 
                        entityName);
                    return 60; // Default to 1 hour if no configuration is found
                }

                _logger.LogDebug("Expiration minutes for {EntityType}: {ExpirationMinutes}", 
                    entityName, cacheConfig.ExpirationMinutes);
                
                return cacheConfig.ExpirationMinutes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiration minutes for entity type {EntityType}", 
                    typeof(T).Name);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsCriticalEntityAsync<T>() where T : BaseEntity
        {
            try
            {
                var entityName = typeof(T).Name;
                
                var cacheConfig = await _dbContext.CacheConfigurations
                    .FirstOrDefaultAsync(c => c.EntityName == entityName);

                if (cacheConfig == null)
                {
                    _logger.LogWarning("No cache configuration found for entity type {EntityType}. Assuming not critical.", 
                        entityName);
                    return false; // Default to not critical if no configuration is found
                }

                _logger.LogDebug("Entity type {EntityType} is critical: {IsCritical}", 
                    entityName, cacheConfig.IsCritical);
                
                return cacheConfig.IsCritical;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if entity type {EntityType} is critical", 
                    typeof(T).Name);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetPendingSyncItemsAsync<T>() where T : BaseEntity
        {
            try
            {
                var entityName = typeof(T).Name;
                
                var pendingItems = await _dbContext.Set<T>()
                    .Where(e => e.SyncStatus == SyncStatus.PendingUpload || 
                                e.SyncStatus == SyncStatus.PendingUpdate || 
                                e.SyncStatus == SyncStatus.PendingDeletion ||
                                e.SyncStatus == SyncStatus.SyncFailed)
                    .ToListAsync();

                _logger.LogDebug("Found {Count} pending sync items for entity type {EntityType}", 
                    pendingItems.Count, entityName);
                
                return pendingItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending sync items for entity type {EntityType}", 
                    typeof(T).Name);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task MarkAsSyncedAsync<T>(T entity) where T : BaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                var entityName = typeof(T).Name;
                
                // Update sync status and timestamp
                entity.SyncStatus = SyncStatus.Synced;
                entity.LastSyncTimestamp = DateTime.UtcNow;
                entity.IsDirty = false;
                
                // If entity is already tracked by the context, just update it
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbContext.Attach(entity);
                }
                
                _dbContext.Entry(entity).Property(e => e.SyncStatus).IsModified = true;
                _dbContext.Entry(entity).Property(e => e.LastSyncTimestamp).IsModified = true;
                _dbContext.Entry(entity).Property(e => e.IsDirty).IsModified = true;
                
                await _dbContext.SaveChangesAsync();
                
                _logger.LogDebug("Marked {EntityType} with ID {EntityId} as synced", 
                    entityName, entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking {EntityType} with ID {EntityId} as synced", 
                    typeof(T).Name, entity.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task MarkForSyncAsync<T>(T entity, SyncStatus status) where T : BaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (status == SyncStatus.Synced)
            {
                throw new ArgumentException("Cannot mark for sync with Synced status. Use MarkAsSyncedAsync instead.", 
                    nameof(status));
            }

            try
            {
                var entityName = typeof(T).Name;
                
                // Update sync status and dirty flag
                entity.SyncStatus = status;
                entity.IsDirty = true;
                
                // If entity is already tracked by the context, just update it
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbContext.Attach(entity);
                }
                
                _dbContext.Entry(entity).Property(e => e.SyncStatus).IsModified = true;
                _dbContext.Entry(entity).Property(e => e.IsDirty).IsModified = true;
                
                await _dbContext.SaveChangesAsync();
                
                _logger.LogDebug("Marked {EntityType} with ID {EntityId} for sync with status {SyncStatus}", 
                    entityName, entity.Id, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking {EntityType} with ID {EntityId} for sync", 
                    typeof(T).Name, entity.Id);
                throw;
            }
        }

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The entity ID.</param>
        /// <returns>The entity, or null if not found.</returns>
        private async Task<T> GetEntityByIdAsync<T>(int id) where T : BaseEntity
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
    }
}