using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LubeLoggerDashboard.Models.Database.Context;
using LubeLoggerDashboard.Models.Database.Entities;
using LubeLoggerDashboard.Models.Database.Enums;

namespace LubeLoggerDashboard.Tests.TestHelpers.MockFactories
{
    /// <summary>
    /// Factory for creating in-memory database contexts for testing
    /// </summary>
    public static class DatabaseMockFactory
    {
        /// <summary>
        /// Creates a new in-memory database context for testing
        /// </summary>
        /// <param name="databaseName">Optional name for the database. If not provided, a unique name will be generated.</param>
        /// <returns>A configured DbContextOptions instance for LubeLoggerDbContext</returns>
        public static DbContextOptions<LubeLoggerDbContext> CreateDbContextOptions(string databaseName = null)
        {
            return new DbContextOptionsBuilder<LubeLoggerDbContext>()
                .UseInMemoryDatabase(databaseName ?? $"LubeLoggerTestDb_{Guid.NewGuid()}")
                .Options;
        }

        /// <summary>
        /// Creates a new in-memory database context with seed data for testing
        /// </summary>
        /// <param name="databaseName">Optional name for the database. If not provided, a unique name will be generated.</param>
        /// <returns>A configured LubeLoggerDbContext with seed data</returns>
        public static LubeLoggerDbContext CreateDbContextWithSeedData(string databaseName = null)
        {
            var options = CreateDbContextOptions(databaseName);
            var context = new LubeLoggerDbContext(options);

            // Clear existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Add cache configurations
            context.CacheConfigurations.AddRange(new List<CacheConfiguration>
            {
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
            });

            // Add test vehicles
            context.Vehicles.AddRange(new List<Vehicle>
            {
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
            });

            // Add test odometer records
            context.OdometerRecords.AddRange(new List<OdometerRecord>
            {
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
            });

            context.SaveChanges();
            return context;
        }
    }
}