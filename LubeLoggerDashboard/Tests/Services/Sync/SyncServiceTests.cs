using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using LubeLoggerDashboard.Models.Database.Context;
using LubeLoggerDashboard.Models.Database.Entities;
using LubeLoggerDashboard.Models.Database.Enums;
using LubeLoggerDashboard.Services.Api.Interfaces;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Services.Cache;
using LubeLoggerDashboard.Services.Sync;

namespace LubeLoggerDashboard.Tests.Services.Sync
{
    public class SyncServiceTests
    {
        private readonly Mock<LubeLoggerDbContext> _mockDbContext;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<ILogger<SyncService>> _mockLogger;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IVehicleService> _mockVehicleService;
        private readonly Mock<IOdometerRecordService> _mockOdometerRecordService;
        private readonly Mock<IPlanRecordService> _mockPlanRecordService;
        private readonly Mock<IServiceRecordService> _mockServiceRecordService;
        private readonly Mock<IRepairRecordService> _mockRepairRecordService;
        private readonly Mock<IUpgradeRecordService> _mockUpgradeRecordService;
        private readonly Mock<ITaxRecordService> _mockTaxRecordService;
        private readonly Mock<IGasRecordService> _mockGasRecordService;
        private readonly Mock<IReminderService> _mockReminderService;
        private readonly Mock<ISystemService> _mockSystemService;
        private readonly Mock<IApiClient> _mockApiClient;
        
        private readonly SyncService _syncService;
        
        public SyncServiceTests()
        {
            // Setup mocks
            _mockDbContext = new Mock<LubeLoggerDbContext>();
            _mockCacheService = new Mock<ICacheService>();
            _mockLogger = new Mock<ILogger<SyncService>>();
            _mockUserService = new Mock<IUserService>();
            _mockVehicleService = new Mock<IVehicleService>();
            _mockOdometerRecordService = new Mock<IOdometerRecordService>();
            _mockPlanRecordService = new Mock<IPlanRecordService>();
            _mockServiceRecordService = new Mock<IServiceRecordService>();
            _mockRepairRecordService = new Mock<IRepairRecordService>();
            _mockUpgradeRecordService = new Mock<IUpgradeRecordService>();
            _mockTaxRecordService = new Mock<ITaxRecordService>();
            _mockGasRecordService = new Mock<IGasRecordService>();
            _mockReminderService = new Mock<IReminderService>();
            _mockSystemService = new Mock<ISystemService>();
            _mockApiClient = new Mock<IApiClient>();
            
            // Setup API client
            _mockUserService.Setup(s => s.ApiClient).Returns(_mockApiClient.Object);
            
            // Create the service with mocked dependencies
            _syncService = new SyncService(
                _mockDbContext.Object,
                _mockCacheService.Object,
                _mockLogger.Object,
                _mockUserService.Object,
                _mockVehicleService.Object,
                _mockOdometerRecordService.Object,
                _mockPlanRecordService.Object,
                _mockServiceRecordService.Object,
                _mockRepairRecordService.Object,
                _mockUpgradeRecordService.Object,
                _mockTaxRecordService.Object,
                _mockGasRecordService.Object,
                _mockReminderService.Object,
                _mockSystemService.Object
            );
        }
        
        [Fact]
        public async Task IsOnlineAsync_NetworkAvailableAndApiReachable_ReturnsTrue()
        {
            // Arrange
            _mockApiClient.Setup(c => c.IsApiAvailableAsync()).ReturnsAsync(true);
            
            // Act
            var result = await _syncService.IsOnlineAsync();
            
            // Assert
            Assert.True(result);
            _mockApiClient.Verify(c => c.IsApiAvailableAsync(), Times.Once);
        }
        
        [Fact]
        public async Task IsOnlineAsync_ApiNotReachable_ReturnsFalse()
        {
            // Arrange
            _mockApiClient.Setup(c => c.IsApiAvailableAsync()).ReturnsAsync(false);
            
            // Act
            var result = await _syncService.IsOnlineAsync();
            
            // Assert
            Assert.False(result);
            _mockApiClient.Verify(c => c.IsApiAvailableAsync(), Times.Once);
        }
        
        [Fact]
        public async Task SyncAllAsync_WhenOffline_ReturnsEmptyResult()
        {
            // Arrange
            _mockApiClient.Setup(c => c.IsApiAvailableAsync()).ReturnsAsync(false);
            
            // Act
            var result = await _syncService.SyncAllAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.SuccessCount);
            Assert.Equal(0, result.FailureCount);
            Assert.Equal(SyncResultStatus.Failure, result.Status);
        }
        
        [Fact]
        public async Task UploadPendingChangesAsync_WithPendingVehicles_UploadsToServer()
        {
            // Arrange
            _mockApiClient.Setup(c => c.IsApiAvailableAsync()).ReturnsAsync(true);
            
            var pendingVehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Name = "Test Vehicle", SyncStatus = Models.Database.Enums.SyncStatus.PendingUpload }
            };
            
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<Vehicle>())
                .ReturnsAsync(pendingVehicles);
                
            _mockVehicleService.Setup(s => s.CreateVehicleAsync(It.IsAny<Vehicle>()))
                .Returns(Task.CompletedTask);
                
            _mockCacheService.Setup(c => c.MarkAsSyncedAsync(It.IsAny<Vehicle>()))
                .Returns(Task.CompletedTask);
                
            // Setup empty lists for other entity types
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<OdometerRecord>())
                .ReturnsAsync(new List<OdometerRecord>());
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<PlanRecord>())
                .ReturnsAsync(new List<PlanRecord>());
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<ServiceRecord>())
                .ReturnsAsync(new List<ServiceRecord>());
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<RepairRecord>())
                .ReturnsAsync(new List<RepairRecord>());
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<UpgradeRecord>())
                .ReturnsAsync(new List<UpgradeRecord>());
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<TaxRecord>())
                .ReturnsAsync(new List<TaxRecord>());
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<GasRecord>())
                .ReturnsAsync(new List<GasRecord>());
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<Reminder>())
                .ReturnsAsync(new List<Reminder>());
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<UserPreference>())
                .ReturnsAsync(new List<UserPreference>());
            
            // Act
            var result = await _syncService.UploadPendingChangesAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.SuccessCount);
            Assert.Equal(0, result.FailureCount);
            Assert.Equal(SyncResultStatus.Success, result.Status);
            
            _mockVehicleService.Verify(s => s.CreateVehicleAsync(It.IsAny<Vehicle>()), Times.Once);
            _mockCacheService.Verify(c => c.MarkAsSyncedAsync(It.IsAny<Vehicle>()), Times.Once);
        }
        
        [Fact]
        public async Task RefreshExpiredCacheAsync_WithExpiredVehicles_RefreshesFromServer()
        {
            // Arrange
            _mockApiClient.Setup(c => c.IsApiAvailableAsync()).ReturnsAsync(true);
            
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<Vehicle>()).ReturnsAsync(true);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<OdometerRecord>()).ReturnsAsync(false);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<PlanRecord>()).ReturnsAsync(false);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<ServiceRecord>()).ReturnsAsync(false);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<RepairRecord>()).ReturnsAsync(false);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<UpgradeRecord>()).ReturnsAsync(false);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<TaxRecord>()).ReturnsAsync(false);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<GasRecord>()).ReturnsAsync(false);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<Reminder>()).ReturnsAsync(false);
            _mockCacheService.Setup(c => c.NeedsRefreshAsync<UserPreference>()).ReturnsAsync(false);
            
            var serverVehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Name = "Test Vehicle" }
            };
            
            _mockVehicleService.Setup(s => s.GetAllVehiclesAsync())
                .ReturnsAsync(serverVehicles);
                
            var mockDbSet = new Mock<DbSet<Vehicle>>();
            _mockDbContext.Setup(c => c.Set<Vehicle>()).Returns(mockDbSet.Object);
            
            // Act
            var result = await _syncService.RefreshExpiredCacheAsync();
            
            // Assert
            Assert.NotNull(result);
            _mockVehicleService.Verify(s => s.GetAllVehiclesAsync(), Times.Once);
        }
        
        [Fact]
        public async Task SyncEntityAsync_ForVehicles_SynchronizesVehicles()
        {
            // Arrange
            _mockApiClient.Setup(c => c.IsApiAvailableAsync()).ReturnsAsync(true);
            
            var pendingVehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Name = "Test Vehicle", SyncStatus = Models.Database.Enums.SyncStatus.PendingUpload }
            };
            
            _mockCacheService.Setup(c => c.GetPendingSyncItemsAsync<Vehicle>())
                .ReturnsAsync(pendingVehicles);
                
            _mockVehicleService.Setup(s => s.CreateVehicleAsync(It.IsAny<Vehicle>()))
                .Returns(Task.CompletedTask);
                
            _mockCacheService.Setup(c => c.MarkAsSyncedAsync(It.IsAny<Vehicle>()))
                .Returns(Task.CompletedTask);
                
            var serverVehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Name = "Test Vehicle" }
            };
            
            _mockVehicleService.Setup(s => s.GetAllVehiclesAsync())
                .ReturnsAsync(serverVehicles);
                
            var mockDbSet = new Mock<DbSet<Vehicle>>();
            _mockDbContext.Setup(c => c.Set<Vehicle>()).Returns(mockDbSet.Object);
            
            // Act
            var result = await _syncService.SyncEntityAsync<Vehicle>();
            
            // Assert
            Assert.NotNull(result);
            _mockVehicleService.Verify(s => s.CreateVehicleAsync(It.IsAny<Vehicle>()), Times.Once);
            _mockVehicleService.Verify(s => s.GetAllVehiclesAsync(), Times.Once);
        }
        
        [Fact]
        public void CancelSync_CancelsOngoingSyncOperation()
        {
            // Act
            _syncService.CancelSync();
            
            // Assert - No exception should be thrown
            // This is a simple test to ensure the method doesn't throw exceptions
        }
    }
}