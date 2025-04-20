using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LubeLoggerDashboard.Models.Database.Context;
using LubeLoggerDashboard.Models.Database.Entities;
using LubeLoggerDashboard.Models.Database.Enums;
using LubeLoggerDashboard.Services.Api.Interfaces;
using LubeLoggerDashboard.Services.Cache;

namespace LubeLoggerDashboard.Services.Sync
{
    /// <summary>
    /// Service responsible for synchronizing data between the local cache and the server.
    /// </summary>
    public class SyncService : ISyncService
    {
        private readonly LubeLoggerDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly ILogger<SyncService> _logger;
        private readonly IUserService _userService;
        private readonly IVehicleService _vehicleService;
        private readonly IOdometerRecordService _odometerRecordService;
        private readonly IPlanRecordService _planRecordService;
        private readonly IServiceRecordService _serviceRecordService;
        private readonly IRepairRecordService _repairRecordService;
        private readonly IUpgradeRecordService _upgradeRecordService;
        private readonly ITaxRecordService _taxRecordService;
        private readonly IGasRecordService _gasRecordService;
        private readonly IReminderService _reminderService;
        private readonly ISystemService _systemService;
        
        private CancellationTokenSource _syncCancellationTokenSource;
        private bool _isSyncing;
        private readonly object _syncLock = new object();
        private readonly Dictionary<Type, DateTime> _lastSyncTimes = new Dictionary<Type, DateTime>();

        /// <summary>
        /// Event that is raised when the sync status changes.
        /// </summary>
        public event EventHandler<SyncStatusChangedEventArgs> SyncStatusChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="vehicleService">The vehicle service.</param>
        /// <param name="odometerRecordService">The odometer record service.</param>
        /// <param name="planRecordService">The plan record service.</param>
        /// <param name="serviceRecordService">The service record service.</param>
        /// <param name="repairRecordService">The repair record service.</param>
        /// <param name="upgradeRecordService">The upgrade record service.</param>
        /// <param name="taxRecordService">The tax record service.</param>
        /// <param name="gasRecordService">The gas record service.</param>
        /// <param name="reminderService">The reminder service.</param>
        /// <param name="systemService">The system service.</param>
        public SyncService(
            LubeLoggerDbContext dbContext,
            ICacheService cacheService,
            ILogger<SyncService> logger,
            IUserService userService,
            IVehicleService vehicleService,
            IOdometerRecordService odometerRecordService,
            IPlanRecordService planRecordService,
            IServiceRecordService serviceRecordService,
            IRepairRecordService repairRecordService,
            IUpgradeRecordService upgradeRecordService,
            ITaxRecordService taxRecordService,
            IGasRecordService gasRecordService,
            IReminderService reminderService,
            ISystemService systemService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
            _odometerRecordService = odometerRecordService ?? throw new ArgumentNullException(nameof(odometerRecordService));
            _planRecordService = planRecordService ?? throw new ArgumentNullException(nameof(planRecordService));
            _serviceRecordService = serviceRecordService ?? throw new ArgumentNullException(nameof(serviceRecordService));
            _repairRecordService = repairRecordService ?? throw new ArgumentNullException(nameof(repairRecordService));
            _upgradeRecordService = upgradeRecordService ?? throw new ArgumentNullException(nameof(upgradeRecordService));
            _taxRecordService = taxRecordService ?? throw new ArgumentNullException(nameof(taxRecordService));
            _gasRecordService = gasRecordService ?? throw new ArgumentNullException(nameof(gasRecordService));
            _reminderService = reminderService ?? throw new ArgumentNullException(nameof(reminderService));
            _systemService = systemService ?? throw new ArgumentNullException(nameof(systemService));
            
            _syncCancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Synchronizes all entity types between the local cache and the server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with the sync result.</returns>
        public async Task<SyncResult> SyncAllAsync()
        {
            if (!await IsOnlineAsync())
            {
                _logger.LogWarning("Cannot sync all entities: Device is offline");
                OnSyncStatusChanged("All", SyncOperation.Upload, SyncStatus.Skipped, "Device is offline");
                return new SyncResult();
            }

            lock (_syncLock)
            {
                if (_isSyncing)
                {
                    _logger.LogWarning("Sync operation already in progress");
                    return new SyncResult();
                }
                _isSyncing = true;
            }

            try
            {
                _syncCancellationTokenSource = new CancellationTokenSource();
                var token = _syncCancellationTokenSource.Token;
                
                OnSyncStatusChanged("All", SyncOperation.Upload, SyncStatus.Started);
                
                var result = new SyncResult();
                
                // First upload pending changes
                var uploadResult = await UploadPendingChangesAsync();
                result.Merge(uploadResult);
                
                if (token.IsCancellationRequested)
                {
                    _logger.LogInformation("Sync operation was cancelled");
                    OnSyncStatusChanged("All", SyncOperation.Upload, SyncStatus.Skipped, "Operation cancelled");
                    return result;
                }
                
                // Then refresh expired cache
                var refreshResult = await RefreshExpiredCacheAsync();
                result.Merge(refreshResult);
                
                OnSyncStatusChanged("All", SyncOperation.Upload,
                    result.Status == SyncResultStatus.Success ? SyncStatus.Completed : SyncStatus.Failed);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sync all operation");
                OnSyncStatusChanged("All", SyncOperation.Upload, SyncStatus.Failed, ex.Message);
                return new SyncResult();
            }
            finally
            {
                lock (_syncLock)
                {
                    _isSyncing = false;
                }
            }
        }

        /// <summary>
        /// Synchronizes a specific entity type between the local cache and the server.
        /// </summary>
        /// <typeparam name="T">The entity type to synchronize.</typeparam>
        /// <returns>A task representing the asynchronous operation with the sync result.</returns>
        public async Task<SyncResult> SyncEntityAsync<T>() where T : BaseEntity
        {
            if (!await IsOnlineAsync())
            {
                _logger.LogWarning($"Cannot sync entity {typeof(T).Name}: Device is offline");
                OnSyncStatusChanged(typeof(T).Name, SyncOperation.Upload, SyncStatus.Skipped, "Device is offline");
                return new SyncResult();
            }

            try
            {
                _syncCancellationTokenSource = new CancellationTokenSource();
                var token = _syncCancellationTokenSource.Token;
                
                var entityName = typeof(T).Name;
                OnSyncStatusChanged(entityName, SyncOperation.Upload, SyncStatus.Started);
                
                var result = new SyncResult();
                
                // First upload pending changes for this entity type
                var uploadResult = await UploadPendingChangesForEntityAsync<T>();
                result.Merge(uploadResult);
                
                if (token.IsCancellationRequested)
                {
                    _logger.LogInformation($"Sync operation for {entityName} was cancelled");
                    OnSyncStatusChanged(entityName, SyncOperation.Upload, SyncStatus.Skipped, "Operation cancelled");
                    return result;
                }
                
                // Then refresh this entity type from the server
                var refreshResult = await RefreshEntityFromServerAsync<T>();
                result.Merge(refreshResult);
                
                // Update last sync time
                _lastSyncTimes[typeof(T)] = DateTime.Now;
                
                OnSyncStatusChanged(entityName, SyncOperation.Upload,
                    result.Status == SyncResultStatus.Success ? SyncStatus.Completed : SyncStatus.Failed);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during sync operation for entity {typeof(T).Name}");
                OnSyncStatusChanged(typeof(T).Name, SyncOperation.Upload, SyncStatus.Failed, ex.Message);
                return new SyncResult();
            }
        }

        /// <summary>
        /// Refreshes expired cache data from the server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with the sync result.</returns>
        public async Task<SyncResult> RefreshExpiredCacheAsync()
        {
            if (!await IsOnlineAsync())
            {
                _logger.LogWarning("Cannot refresh expired cache: Device is offline");
                OnSyncStatusChanged("All", SyncOperation.Refresh, SyncStatus.Skipped, "Device is offline");
                return new SyncResult();
            }

            try
            {
                var result = new SyncResult();
                
                OnSyncStatusChanged("All", SyncOperation.Refresh, SyncStatus.Started);
                
                // Check and refresh each entity type
                if (await _cacheService.NeedsRefreshAsync<Vehicle>())
                {
                    var vehicleResult = await RefreshEntityFromServerAsync<Vehicle>();
                    result.Merge(vehicleResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<OdometerRecord>())
                {
                    var odometerResult = await RefreshEntityFromServerAsync<OdometerRecord>();
                    result.Merge(odometerResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<Reminder>())
                {
                    var reminderResult = await RefreshEntityFromServerAsync<Reminder>();
                    result.Merge(reminderResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<PlanRecord>())
                {
                    var planResult = await RefreshEntityFromServerAsync<PlanRecord>();
                    result.Merge(planResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<ServiceRecord>())
                {
                    var serviceResult = await RefreshEntityFromServerAsync<ServiceRecord>();
                    result.Merge(serviceResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<RepairRecord>())
                {
                    var repairResult = await RefreshEntityFromServerAsync<RepairRecord>();
                    result.Merge(repairResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<UpgradeRecord>())
                {
                    var upgradeResult = await RefreshEntityFromServerAsync<UpgradeRecord>();
                    result.Merge(upgradeResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<GasRecord>())
                {
                    var gasResult = await RefreshEntityFromServerAsync<GasRecord>();
                    result.Merge(gasResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<TaxRecord>())
                {
                    var taxResult = await RefreshEntityFromServerAsync<TaxRecord>();
                    result.Merge(taxResult);
                }
                
                if (await _cacheService.NeedsRefreshAsync<UserPreference>())
                {
                    var prefResult = await RefreshEntityFromServerAsync<UserPreference>();
                    result.Merge(prefResult);
                }
                
                OnSyncStatusChanged("All", SyncOperation.Refresh,
                    result.Status == SyncResultStatus.Success ? SyncStatus.Completed : SyncStatus.Failed);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during refresh of expired cache");
                OnSyncStatusChanged("All", SyncOperation.Refresh, SyncStatus.Failed, ex.Message);
                return new SyncResult();
            }
        }

        /// <summary>
        /// Checks if the application is online and can connect to the server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with a boolean indicating if the application is online.</returns>
        public async Task<bool> IsOnlineAsync()
        {
            try
            {
                OnSyncStatusChanged("System", SyncOperation.ConnectivityCheck, SyncStatus.Started);
                
                // First check if network is available
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    _logger.LogInformation("Network is not available");
                    OnSyncStatusChanged("System", SyncOperation.ConnectivityCheck, SyncStatus.Failed, "Network is not available");
                    return false;
                }
                
                // Then check if API is reachable
                bool apiAvailable = await _userService.ApiClient.IsApiAvailableAsync();
                
                OnSyncStatusChanged("System", SyncOperation.ConnectivityCheck,
                    apiAvailable ? SyncStatus.Completed : SyncStatus.Failed);
                
                return apiAvailable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking online status");
                OnSyncStatusChanged("System", SyncOperation.ConnectivityCheck, SyncStatus.Failed, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the last synchronization time for a specific entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>A task representing the asynchronous operation with the last sync time.</returns>
        public async Task<DateTime?> GetLastSyncTimeAsync<T>() where T : BaseEntity
        {
            try
            {
                if (_lastSyncTimes.TryGetValue(typeof(T), out DateTime lastSync))
                {
                    return lastSync;
                }
                
                // If not in memory, try to get from database
                var latestEntity = await _dbContext.Set<T>()
                    .OrderByDescending(e => e.LastSyncTimestamp)
                    .FirstOrDefaultAsync();
                
                return latestEntity?.LastSyncTimestamp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting last sync time for {typeof(T).Name}");
                return null;
            }
        }

        /// <summary>
        /// Forces a full synchronization of a specific entity, regardless of its current sync status.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entityId">The ID of the entity to synchronize.</param>
        /// <returns>A task representing the asynchronous operation with the sync result.</returns>
        public async Task<SyncResult> ForceSyncEntityAsync<T>(int entityId) where T : BaseEntity
        {
            if (!await IsOnlineAsync())
            {
                _logger.LogWarning($"Cannot force sync entity {typeof(T).Name} with ID {entityId}: Device is offline");
                OnSyncStatusChanged(typeof(T).Name, SyncOperation.Upload, SyncStatus.Skipped, "Device is offline");
                return new SyncResult();
            }

            try
            {
                var result = new SyncResult();
                var entityName = typeof(T).Name;
                
                OnSyncStatusChanged(entityName, SyncOperation.Upload, SyncStatus.Started);
                
                // Get the entity from the database
                var entity = await _dbContext.Set<T>().FindAsync(entityId);
                if (entity == null)
                {
                    _logger.LogWarning($"Entity {entityName} with ID {entityId} not found in local database");
                    OnSyncStatusChanged(entityName, SyncOperation.Upload, SyncStatus.Failed, "Entity not found");
                    return result;
                }
                
                // Upload the entity to the server
                var uploadResult = await UploadEntityToServerAsync(entity);
                result.Merge(uploadResult);
                
                // Refresh the entity from the server
                var refreshResult = await RefreshEntityFromServerAsync<T>(entityId);
                result.Merge(refreshResult);
                
                OnSyncStatusChanged(entityName, SyncOperation.Upload,
                    result.Status == SyncResultStatus.Success ? SyncStatus.Completed : SyncStatus.Failed);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during force sync of entity {typeof(T).Name} with ID {entityId}");
                OnSyncStatusChanged(typeof(T).Name, SyncOperation.Upload, SyncStatus.Failed, ex.Message);
                return new SyncResult();
            }
        }

        /// <summary>
        /// Cancels any ongoing synchronization operations.
        /// </summary>
        public void CancelSync()
        {
            try
            {
                _syncCancellationTokenSource.Cancel();
                _logger.LogInformation("Sync operation cancelled");
                OnSyncStatusChanged("All", SyncOperation.Upload, SyncStatus.Skipped, "Operation cancelled by user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling sync operation");
            }
        }

        /// <summary>
        /// Uploads pending changes to the server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with the sync result.</returns>
        public async Task<SyncResult> UploadPendingChangesAsync()
        {
            if (!await IsOnlineAsync())
            {
                _logger.LogWarning("Cannot upload pending changes: Device is offline");
                OnSyncStatusChanged("All", SyncOperation.Upload, SyncStatus.Skipped, "Device is offline");
                return new SyncResult();
            }
    
        #region Private Helper Methods

        private async Task<SyncResult> UploadPendingChangesForEntityAsync<T>() where T : BaseEntity
        {
            var result = new SyncResult();
            var entityName = typeof(T).Name;
            
            try
            {
                OnSyncStatusChanged(entityName, SyncOperation.Upload, SyncStatus.Started);
                
                // Get all entities that need to be synchronized
                var pendingEntities = await _cacheService.GetPendingSyncItemsAsync<T>();
                
                if (!pendingEntities.Any())
                {
                    _logger.LogInformation($"No pending changes for {entityName}");
                    OnSyncStatusChanged(entityName, SyncOperation.Upload, SyncStatus.Skipped, "No pending changes");
                    return result;
                }
                
                _logger.LogInformation($"Uploading {pendingEntities.Count()} pending changes for {entityName}");
                
                foreach (var entity in pendingEntities)
                {
                    if (_syncCancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _logger.LogInformation($"Upload cancelled for {entityName}");
                        break;
                    }
                    
                    try
                    {
                        await UploadEntityToServerAsync(entity);
                        result.AddSuccess(entityName, entity.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error uploading {entityName} with ID {entity.Id}");
                        result.AddFailure(entityName, entity.Id, ex);
                    }
                }
                
                OnSyncStatusChanged(entityName, SyncOperation.Upload,
                    result.Status == SyncResultStatus.Success ? SyncStatus.Completed : SyncStatus.Failed);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during upload of pending changes for {entityName}");
                OnSyncStatusChanged(entityName, SyncOperation.Upload, SyncStatus.Failed, ex.Message);
                return result;
            }
        }

        private async Task<SyncResult> UploadEntityToServerAsync<T>(T entity) where T : BaseEntity
        {
            var result = new SyncResult();
            var entityName = typeof(T).Name;
            
            try
            {
                switch (entity.SyncStatus)
                {
                    case Models.Database.Enums.SyncStatus.PendingUpload:
                        await CreateEntityOnServerAsync(entity);
                        break;
                    
                    case Models.Database.Enums.SyncStatus.PendingUpdate:
                        await UpdateEntityOnServerAsync(entity);
                        break;
                    
                    case Models.Database.Enums.SyncStatus.PendingDeletion:
                        await DeleteEntityOnServerAsync(entity);
                        break;
                    
                    case Models.Database.Enums.SyncStatus.SyncFailed:
                        // Retry the operation based on IsDirty flag
                        if (entity.IsDirty)
                        {
                            if (entity.Id <= 0)
                            {
                                await CreateEntityOnServerAsync(entity);
                            }
                            else
                            {
                                await UpdateEntityOnServerAsync(entity);
                            }
                        }
                        break;
                }
                
                // Mark as synced
                await _cacheService.MarkAsSyncedAsync(entity);
                
                result.AddSuccess(entityName, entity.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading {entityName} with ID {entity.Id}");
                
                // Mark as failed
                entity.SyncStatus = Models.Database.Enums.SyncStatus.SyncFailed;
                await _dbContext.SaveChangesAsync();
                
                result.AddFailure(entityName, entity.Id, ex);
                return result;
            }
        }

        private async Task CreateEntityOnServerAsync<T>(T entity) where T : BaseEntity
        {
            if (typeof(T) == typeof(Vehicle))
            {
                await _vehicleService.CreateVehicleAsync(entity as Vehicle);
            }
            else if (typeof(T) == typeof(OdometerRecord))
            {
                await _odometerRecordService.CreateOdometerRecordAsync(entity as OdometerRecord);
            }
            else if (typeof(T) == typeof(PlanRecord))
            {
                await _planRecordService.CreatePlanRecordAsync(entity as PlanRecord);
            }
            else if (typeof(T) == typeof(ServiceRecord))
            {
                await _serviceRecordService.CreateServiceRecordAsync(entity as ServiceRecord);
            }
            else if (typeof(T) == typeof(RepairRecord))
            {
                await _repairRecordService.CreateRepairRecordAsync(entity as RepairRecord);
            }
            else if (typeof(T) == typeof(UpgradeRecord))
            {
                await _upgradeRecordService.CreateUpgradeRecordAsync(entity as UpgradeRecord);
            }
            else if (typeof(T) == typeof(TaxRecord))
            {
                await _taxRecordService.CreateTaxRecordAsync(entity as TaxRecord);
            }
            else if (typeof(T) == typeof(GasRecord))
            {
                await _gasRecordService.CreateGasRecordAsync(entity as GasRecord);
            }
            else if (typeof(T) == typeof(Reminder))
            {
                await _reminderService.CreateReminderAsync(entity as Reminder);
            }
            else
            {
                throw new NotSupportedException($"Entity type {typeof(T).Name} is not supported for server creation");
            }
        }

        private async Task UpdateEntityOnServerAsync<T>(T entity) where T : BaseEntity
        {
            if (typeof(T) == typeof(Vehicle))
            {
                await _vehicleService.UpdateVehicleAsync(entity as Vehicle);
            }
            else if (typeof(T) == typeof(OdometerRecord))
            {
                await _odometerRecordService.UpdateOdometerRecordAsync(entity as OdometerRecord);
            }
            else if (typeof(T) == typeof(PlanRecord))
            {
                await _planRecordService.UpdatePlanRecordAsync(entity as PlanRecord);
            }
            else if (typeof(T) == typeof(ServiceRecord))
            {
                await _serviceRecordService.UpdateServiceRecordAsync(entity as ServiceRecord);
            }
            else if (typeof(T) == typeof(RepairRecord))
            {
                await _repairRecordService.UpdateRepairRecordAsync(entity as RepairRecord);
            }
            else if (typeof(T) == typeof(UpgradeRecord))
            {
                await _upgradeRecordService.UpdateUpgradeRecordAsync(entity as UpgradeRecord);
            }
            else if (typeof(T) == typeof(TaxRecord))
            {
                await _taxRecordService.UpdateTaxRecordAsync(entity as TaxRecord);
            }
            else if (typeof(T) == typeof(GasRecord))
            {
                await _gasRecordService.UpdateGasRecordAsync(entity as GasRecord);
            }
            else if (typeof(T) == typeof(Reminder))
            {
                await _reminderService.UpdateReminderAsync(entity as Reminder);
            }
            else
            {
                throw new NotSupportedException($"Entity type {typeof(T).Name} is not supported for server update");
            }
        }

        private async Task DeleteEntityOnServerAsync<T>(T entity) where T : BaseEntity
        {
            if (typeof(T) == typeof(Vehicle))
            {
                await _vehicleService.DeleteVehicleAsync(entity.Id);
            }
            else if (typeof(T) == typeof(OdometerRecord))
            {
                await _odometerRecordService.DeleteOdometerRecordAsync(entity.Id);
            }
            else if (typeof(T) == typeof(PlanRecord))
            {
                await _planRecordService.DeletePlanRecordAsync(entity.Id);
            }
            else if (typeof(T) == typeof(ServiceRecord))
            {
                await _serviceRecordService.DeleteServiceRecordAsync(entity.Id);
            }
            else if (typeof(T) == typeof(RepairRecord))
            {
                await _repairRecordService.DeleteRepairRecordAsync(entity.Id);
            }
            else if (typeof(T) == typeof(UpgradeRecord))
            {
                await _upgradeRecordService.DeleteUpgradeRecordAsync(entity.Id);
            }
            else if (typeof(T) == typeof(TaxRecord))
            {
                await _taxRecordService.DeleteTaxRecordAsync(entity.Id);
            }
            else if (typeof(T) == typeof(GasRecord))
            {
                await _gasRecordService.DeleteGasRecordAsync(entity.Id);
            }
            else if (typeof(T) == typeof(Reminder))
            {
                await _reminderService.DeleteReminderAsync(entity.Id);
            }
            else
            {
                throw new NotSupportedException($"Entity type {typeof(T).Name} is not supported for server deletion");
            }
            
            // Remove from local database after successful server deletion
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<SyncResult> RefreshEntityFromServerAsync<T>(int? specificEntityId = null) where T : BaseEntity
        {
            var result = new SyncResult();
            var entityName = typeof(T).Name;
            
            try
            {
                OnSyncStatusChanged(entityName, SyncOperation.Download, SyncStatus.Started);
                
                if (typeof(T) == typeof(Vehicle))
                {
                    if (specificEntityId.HasValue)
                    {
                        var vehicle = await _vehicleService.GetVehicleAsync(specificEntityId.Value);
                        await UpdateLocalEntityAsync(vehicle, result);
                    }
                    else
                    {
                        var vehicles = await _vehicleService.GetAllVehiclesAsync();
                        foreach (var vehicle in vehicles)
                        {
                            if (_syncCancellationTokenSource.Token.IsCancellationRequested)
                                break;
                                
                            await UpdateLocalEntityAsync(vehicle, result);
                        }
                    }
                }
                else if (typeof(T) == typeof(OdometerRecord))
                {
                    if (specificEntityId.HasValue)
                    {
                        var record = await _odometerRecordService.GetOdometerRecordAsync(specificEntityId.Value);
                        await UpdateLocalEntityAsync(record, result);
                    }
                    else
                    {
                        var records = await _odometerRecordService.GetAllOdometerRecordsAsync();
                        foreach (var record in records)
                        {
                            if (_syncCancellationTokenSource.Token.IsCancellationRequested)
                                break;
                                
                            await UpdateLocalEntityAsync(record, result);
                        }
                    }
                }
                // Similar implementation for other entity types
                // ...
                
                OnSyncStatusChanged(entityName, SyncOperation.Download,
                    result.Status == SyncResultStatus.Success ? SyncStatus.Completed : SyncStatus.Failed);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error refreshing {entityName} from server");
                OnSyncStatusChanged(entityName, SyncOperation.Download, SyncStatus.Failed, ex.Message);
                return result;
            }
        }

        private async Task UpdateLocalEntityAsync<T>(T serverEntity, SyncResult result) where T : BaseEntity
        {
            if (serverEntity == null)
                return;
                
            var entityName = typeof(T).Name;
            
            try
            {
                // Find the entity in the local database
                var localEntity = await _dbContext.Set<T>().FindAsync(serverEntity.Id);
                
                if (localEntity == null)
                {
                    // Entity doesn't exist locally, add it
                    _dbContext.Set<T>().Add(serverEntity);
                    serverEntity.SyncStatus = Models.Database.Enums.SyncStatus.Synced;
                    serverEntity.LastSyncTimestamp = DateTime.Now;
                    await _cacheService.UpdateExpirationAsync(serverEntity);
                    result.AddSuccess(entityName, serverEntity.Id);
                }
                else
                {
                    // Entity exists locally, check if it has pending changes
                    if (localEntity.SyncStatus == Models.Database.Enums.SyncStatus.Synced ||
                        localEntity.SyncStatus == Models.Database.Enums.SyncStatus.SyncFailed)
                    {
                        // Update the local entity with server data
                        _dbContext.Entry(localEntity).CurrentValues.SetValues(serverEntity);
                        localEntity.SyncStatus = Models.Database.Enums.SyncStatus.Synced;
                        localEntity.LastSyncTimestamp = DateTime.Now;
                        await _cacheService.UpdateExpirationAsync(localEntity);
                        result.AddSuccess(entityName, localEntity.Id);
                    }
                    else
                    {
                        // Local entity has pending changes, handle conflict
                        // Default to "server wins" strategy
                        _dbContext.Entry(localEntity).CurrentValues.SetValues(serverEntity);
                        localEntity.SyncStatus = Models.Database.Enums.SyncStatus.Synced;
                        localEntity.LastSyncTimestamp = DateTime.Now;
                        localEntity.IsDirty = false;
                        await _cacheService.UpdateExpirationAsync(localEntity);
                        result.AddConflict(entityName, localEntity.Id, "Server data used (server wins)");
                        
                        OnSyncStatusChanged(entityName, SyncOperation.ConflictResolution,
                            SyncStatus.ConflictDetected, "Conflict resolved using server data");
                    }
                }
                
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating local {entityName} with ID {serverEntity.Id}");
                result.AddFailure(entityName, serverEntity.Id, ex);
            }
        }

        private void OnSyncStatusChanged(string entityType, SyncOperation operation, SyncStatus status, string message = null)
        {
            SyncStatusChanged?.Invoke(this, new SyncStatusChangedEventArgs(entityType, operation, status, message));
        }

        #endregion

            try
            {
                var result = new SyncResult();
                
                OnSyncStatusChanged("All", SyncOperation.Upload, SyncStatus.Started);
                
                // Upload each entity type in priority order
                var vehicleResult = await UploadPendingChangesForEntityAsync<Vehicle>();
                result.Merge(vehicleResult);
                
                var odometerResult = await UploadPendingChangesForEntityAsync<OdometerRecord>();
                result.Merge(odometerResult);
                
                var reminderResult = await UploadPendingChangesForEntityAsync<Reminder>();
                result.Merge(reminderResult);
                
                var planResult = await UploadPendingChangesForEntityAsync<PlanRecord>();
                result.Merge(planResult);
                
                var serviceResult = await UploadPendingChangesForEntityAsync<ServiceRecord>();
                result.Merge(serviceResult);
                
                var repairResult = await UploadPendingChangesForEntityAsync<RepairRecord>();
                result.Merge(repairResult);
                
                var upgradeResult = await UploadPendingChangesForEntityAsync<UpgradeRecord>();
                result.Merge(upgradeResult);
                
                var gasResult = await UploadPendingChangesForEntityAsync<GasRecord>();
                result.Merge(gasResult);
                
                var taxResult = await UploadPendingChangesForEntityAsync<TaxRecord>();
                result.Merge(taxResult);
                
                var prefResult = await UploadPendingChangesForEntityAsync<UserPreference>();
                result.Merge(prefResult);
                
                OnSyncStatusChanged("All", SyncOperation.Upload,
                    result.Status == SyncResultStatus.Success ? SyncStatus.Completed : SyncStatus.Failed);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during upload of pending changes");
                OnSyncStatusChanged("All", SyncOperation.Upload, SyncStatus.Failed, ex.Message);
                return new SyncResult();
            }
        }
