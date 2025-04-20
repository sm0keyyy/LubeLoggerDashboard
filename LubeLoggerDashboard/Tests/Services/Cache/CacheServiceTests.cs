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
using LubeLoggerDashboard.Services.Cache;

namespace LubeLoggerDashboard.Tests.Services.Cache
{
    public class CacheServiceTests
    {
        private readonly DbContextOptions<LubeLoggerDbContext> _dbContextOptions;
        private readonly Mock<ILogger<CacheService>> _loggerMock;

        public CacheServiceTests()
        {
            // Set up in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<LubeLoggerDbContext>()
                .UseInMemoryDatabase(databaseName: $"LubeLoggerTestDb_{Guid.NewGuid()}")
                .Options;

            _loggerMock = new Mock<ILogger<CacheService>>();

            // Initialize the database with test data
            using (var context = new LubeLoggerDbContext(_dbContextOptions))
            {
                // Add cache configurations
                context.CacheConfigurations.Add(new CacheConfiguration
                {
                    Id = 1,
                    EntityName = nameof(Vehicle),
                    ExpirationMinutes = 60 * 24,
                    IsCritical = true,
                    SyncPriority = 1
                });

                context.CacheConfigurations.Add(new CacheConfiguration
                {
                    Id = 2,
                    EntityName = nameof(OdometerRecord),
                    ExpirationMinutes = 60 * 12,
                    IsCritical = false,
                    SyncPriority = 2
                });

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task IsExpiredAsync_EntityDoesNotExist_ReturnsTrue()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var result = await cacheService.IsExpiredAsync<Vehicle>(999); // Non-existent ID

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsExpiredAsync_EntityIsExpired_ReturnsTrue()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            // Add an expired vehicle
            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "Test Vehicle",
                ExpirationTimestamp = DateTime.UtcNow.AddHours(-1), // Expired 1 hour ago
                SyncStatus = SyncStatus.Synced,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-2)
            };
            
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var result = await cacheService.IsExpiredAsync<Vehicle>(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsExpiredAsync_EntityIsNotExpired_ReturnsFalse()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            // Add a non-expired vehicle
            var vehicle = new Vehicle
            {
                Id = 2,
                Name = "Test Vehicle 2",
                ExpirationTimestamp = DateTime.UtcNow.AddHours(1), // Expires in 1 hour
                SyncStatus = SyncStatus.Synced,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-1)
            };
            
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var result = await cacheService.IsExpiredAsync<Vehicle>(2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NeedsRefreshAsync_SomeEntitiesExpired_ReturnsTrue()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            // Add a mix of expired and non-expired vehicles
            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    Id = 3,
                    Name = "Test Vehicle 3",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(-1), // Expired
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-2)
                },
                new Vehicle
                {
                    Id = 4,
                    Name = "Test Vehicle 4",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(1), // Not expired
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1)
                }
            };
            
            context.Vehicles.AddRange(vehicles);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var result = await cacheService.NeedsRefreshAsync<Vehicle>();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task NeedsRefreshAsync_NoEntitiesExpired_ReturnsFalse()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            // Clear existing vehicles and add only non-expired ones
            context.Vehicles.RemoveRange(context.Vehicles);
            await context.SaveChangesAsync();
            
            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    Id = 5,
                    Name = "Test Vehicle 5",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(1), // Not expired
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1)
                },
                new Vehicle
                {
                    Id = 6,
                    Name = "Test Vehicle 6",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(2), // Not expired
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1)
                }
            };
            
            context.Vehicles.AddRange(vehicles);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var result = await cacheService.NeedsRefreshAsync<Vehicle>();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateExpirationAsync_ValidEntity_UpdatesExpiration()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            var vehicle = new Vehicle
            {
                Id = 7,
                Name = "Test Vehicle 7",
                ExpirationTimestamp = DateTime.UtcNow.AddHours(-1), // Expired
                SyncStatus = SyncStatus.Synced,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-2)
            };
            
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);
            var oldExpiration = vehicle.ExpirationTimestamp;

            // Act
            await cacheService.UpdateExpirationAsync(vehicle);

            // Assert
            // Refresh entity from database
            await context.Entry(vehicle).ReloadAsync();
            
            Assert.True(vehicle.ExpirationTimestamp > oldExpiration);
            // Should be set to current time + expiration minutes (24 hours for Vehicle)
            Assert.True(vehicle.ExpirationTimestamp > DateTime.UtcNow.AddHours(23));
            Assert.True(vehicle.ExpirationTimestamp < DateTime.UtcNow.AddHours(25));
        }

        [Fact]
        public async Task GetExpirationMinutesAsync_EntityHasConfig_ReturnsCorrectValue()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var vehicleExpiration = await cacheService.GetExpirationMinutesAsync<Vehicle>();
            var odometerExpiration = await cacheService.GetExpirationMinutesAsync<OdometerRecord>();

            // Assert
            Assert.Equal(60 * 24, vehicleExpiration); // 24 hours
            Assert.Equal(60 * 12, odometerExpiration); // 12 hours
        }

        [Fact]
        public async Task GetExpirationMinutesAsync_EntityHasNoConfig_ReturnsDefaultValue()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            // ServiceRecord has no explicit configuration in our test setup
            var result = await cacheService.GetExpirationMinutesAsync<ServiceRecord>();

            // Assert
            Assert.Equal(60, result); // Default 1 hour
        }

        [Fact]
        public async Task IsCriticalEntityAsync_CriticalEntity_ReturnsTrue()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var result = await cacheService.IsCriticalEntityAsync<Vehicle>();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsCriticalEntityAsync_NonCriticalEntity_ReturnsFalse()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var result = await cacheService.IsCriticalEntityAsync<OdometerRecord>();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetPendingSyncItemsAsync_HasPendingItems_ReturnsItems()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            // Add vehicles with different sync statuses
            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    Id = 8,
                    Name = "Test Vehicle 8",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(1),
                    SyncStatus = SyncStatus.PendingUpload,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1)
                },
                new Vehicle
                {
                    Id = 9,
                    Name = "Test Vehicle 9",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(1),
                    SyncStatus = SyncStatus.PendingUpdate,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1)
                },
                new Vehicle
                {
                    Id = 10,
                    Name = "Test Vehicle 10",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(1),
                    SyncStatus = SyncStatus.Synced, // Not pending
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1)
                }
            };
            
            context.Vehicles.AddRange(vehicles);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            var result = await cacheService.GetPendingSyncItemsAsync<Vehicle>();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, v => v.Id == 8);
            Assert.Contains(result, v => v.Id == 9);
            Assert.DoesNotContain(result, v => v.Id == 10);
        }

        [Fact]
        public async Task MarkAsSyncedAsync_ValidEntity_UpdatesSyncStatus()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            var vehicle = new Vehicle
            {
                Id = 11,
                Name = "Test Vehicle 11",
                ExpirationTimestamp = DateTime.UtcNow.AddHours(1),
                SyncStatus = SyncStatus.PendingUpdate,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-2),
                IsDirty = true
            };
            
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);
            var oldSyncTimestamp = vehicle.LastSyncTimestamp;

            // Act
            await cacheService.MarkAsSyncedAsync(vehicle);

            // Assert
            // Refresh entity from database
            await context.Entry(vehicle).ReloadAsync();
            
            Assert.Equal(SyncStatus.Synced, vehicle.SyncStatus);
            Assert.True(vehicle.LastSyncTimestamp > oldSyncTimestamp);
            Assert.False(vehicle.IsDirty);
        }

        [Fact]
        public async Task MarkForSyncAsync_ValidEntity_UpdatesSyncStatus()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            var vehicle = new Vehicle
            {
                Id = 12,
                Name = "Test Vehicle 12",
                ExpirationTimestamp = DateTime.UtcNow.AddHours(1),
                SyncStatus = SyncStatus.Synced,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-1),
                IsDirty = false
            };
            
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act
            await cacheService.MarkForSyncAsync(vehicle, SyncStatus.PendingUpdate);

            // Assert
            // Refresh entity from database
            await context.Entry(vehicle).ReloadAsync();
            
            Assert.Equal(SyncStatus.PendingUpdate, vehicle.SyncStatus);
            Assert.True(vehicle.IsDirty);
        }

        [Fact]
        public async Task MarkForSyncAsync_SyncedStatus_ThrowsArgumentException()
        {
            // Arrange
            using var context = new LubeLoggerDbContext(_dbContextOptions);
            
            var vehicle = new Vehicle
            {
                Id = 13,
                Name = "Test Vehicle 13",
                ExpirationTimestamp = DateTime.UtcNow.AddHours(1),
                SyncStatus = SyncStatus.PendingUpdate,
                LastSyncTimestamp = DateTime.UtcNow.AddHours(-1)
            };
            
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
            
            var cacheService = new CacheService(context, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                cacheService.MarkForSyncAsync(vehicle, SyncStatus.Synced));
        }
    }
}