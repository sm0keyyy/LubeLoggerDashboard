using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Models.Database.Context;
using LubeLoggerDashboard.Models.Database.Entities;
using LubeLoggerDashboard.Models.Database.Enums;

namespace LubeLoggerDashboard.Tests.TestHelpers.Database
{
    /// <summary>
    /// Helper class for setting up and tearing down database test contexts
    /// </summary>
    public class DatabaseTestContext : IDisposable
    {
        /// <summary>
        /// The database context for the test
        /// </summary>
        public LubeLoggerDbContext DbContext { get; private set; }
        
        /// <summary>
        /// The service provider for dependency injection
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTestContext"/> class
        /// </summary>
        /// <param name="databaseName">Optional name for the database. If not provided, a unique name will be generated.</param>
        public DatabaseTestContext(string databaseName = null)
        {
            var services = new ServiceCollection();
            
            // Set up in-memory database
            services.AddDbContext<LubeLoggerDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName ?? $"LubeLoggerTestDb_{Guid.NewGuid()}");
            });
            
            ServiceProvider = services.BuildServiceProvider();
            DbContext = ServiceProvider.GetRequiredService<LubeLoggerDbContext>();
            
            // Ensure database is created
            DbContext.Database.EnsureCreated();
        }
        
        /// <summary>
        /// Seeds the database with test data
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task SeedDatabaseAsync()
        {
            // Add cache configurations
            DbContext.CacheConfigurations.AddRange(
                new CacheConfiguration
                {
                    Id = 1,
                    EntityName = nameof(Vehicle),
                    ExpirationMinutes = 60 * 24,
                    IsCritical = true,
                    SyncPriority = 1
                },
                new CacheConfiguration
                {
                    Id = 2,
                    EntityName = nameof(OdometerRecord),
                    ExpirationMinutes = 60 * 12,
                    IsCritical = false,
                    SyncPriority = 2
                },
                new CacheConfiguration
                {
                    Id = 3,
                    EntityName = nameof(ServiceRecord),
                    ExpirationMinutes = 60 * 12,
                    IsCritical = false,
                    SyncPriority = 3
                }
            );
            
            // Add test vehicles
            DbContext.Vehicles.AddRange(
                new Vehicle
                {
                    Id = 1,
                    Name = "Test Vehicle 1",
                    Make = "Test Make",
                    Model = "Test Model",
                    Year = 2020,
                    VIN = "TEST12345678901234",
                    LicensePlate = "TEST123",
                    Notes = "Test vehicle for unit tests",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(24),
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1),
                    IsDirty = false
                },
                new Vehicle
                {
                    Id = 2,
                    Name = "Test Vehicle 2",
                    Make = "Test Make 2",
                    Model = "Test Model 2",
                    Year = 2021,
                    VIN = "TEST23456789012345",
                    LicensePlate = "TEST456",
                    Notes = "Another test vehicle for unit tests",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(24),
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-2),
                    IsDirty = false
                }
            );
            
            // Add test odometer records
            DbContext.OdometerRecords.AddRange(
                new OdometerRecord
                {
                    Id = 1,
                    VehicleId = 1,
                    Date = DateTime.UtcNow.AddDays(-30),
                    Odometer = 10000,
                    Notes = "Initial odometer reading",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(12),
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-3),
                    IsDirty = false
                },
                new OdometerRecord
                {
                    Id = 2,
                    VehicleId = 1,
                    Date = DateTime.UtcNow.AddDays(-15),
                    Odometer = 10500,
                    Notes = "Mid-month reading",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(12),
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-2),
                    IsDirty = false
                },
                new OdometerRecord
                {
                    Id = 3,
                    VehicleId = 1,
                    Date = DateTime.UtcNow.AddDays(-1),
                    Odometer = 11000,
                    Notes = "Latest reading",
                    ExpirationTimestamp = DateTime.UtcNow.AddHours(12),
                    SyncStatus = SyncStatus.Synced,
                    LastSyncTimestamp = DateTime.UtcNow.AddHours(-1),
                    IsDirty = false
                }
            );
            
            await DbContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// Clears all data from the database
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task ClearDatabaseAsync()
        {
            DbContext.OdometerRecords.RemoveRange(DbContext.OdometerRecords);
            DbContext.Vehicles.RemoveRange(DbContext.Vehicles);
            DbContext.CacheConfigurations.RemoveRange(DbContext.CacheConfigurations);
            
            await DbContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// Resets the database by clearing all data and then seeding it with test data
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task ResetDatabaseAsync()
        {
            await ClearDatabaseAsync();
            await SeedDatabaseAsync();
        }
        
        /// <summary>
        /// Disposes of resources used by the test context
        /// </summary>
        public void Dispose()
        {
            DbContext?.Dispose();
            
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}