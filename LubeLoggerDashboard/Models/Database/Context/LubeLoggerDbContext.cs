using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LubeLoggerDashboard.Models.Database.Entities;

namespace LubeLoggerDashboard.Models.Database.Context
{
    /// <summary>
    /// Database context for the LubeLogger local cache.
    /// </summary>
    public class LubeLoggerDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Gets or sets the vehicles in the database.
        /// </summary>
        public DbSet<Vehicle> Vehicles { get; set; }

        /// <summary>
        /// Gets or sets the odometer records in the database.
        /// </summary>
        public DbSet<OdometerRecord> OdometerRecords { get; set; }

        /// <summary>
        /// Gets or sets the plan records in the database.
        /// </summary>
        public DbSet<PlanRecord> PlanRecords { get; set; }

        /// <summary>
        /// Gets or sets the service records in the database.
        /// </summary>
        public DbSet<ServiceRecord> ServiceRecords { get; set; }

        /// <summary>
        /// Gets or sets the repair records in the database.
        /// </summary>
        public DbSet<RepairRecord> RepairRecords { get; set; }

        /// <summary>
        /// Gets or sets the upgrade records in the database.
        /// </summary>
        public DbSet<UpgradeRecord> UpgradeRecords { get; set; }

        /// <summary>
        /// Gets or sets the tax records in the database.
        /// </summary>
        public DbSet<TaxRecord> TaxRecords { get; set; }

        /// <summary>
        /// Gets or sets the gas records in the database.
        /// </summary>
        public DbSet<GasRecord> GasRecords { get; set; }

        /// <summary>
        /// Gets or sets the reminders in the database.
        /// </summary>
        public DbSet<Reminder> Reminders { get; set; }

        /// <summary>
        /// Gets or sets the user preferences in the database.
        /// </summary>
        public DbSet<UserPreference> UserPreferences { get; set; }

        /// <summary>
        /// Gets or sets the cache configurations in the database.
        /// </summary>
        public DbSet<CacheConfiguration> CacheConfigurations { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LubeLoggerDbContext"/> class.
        /// </summary>
        public LubeLoggerDbContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LubeLoggerDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the context.</param>
        public LubeLoggerDbContext(DbContextOptions<LubeLoggerDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LubeLoggerDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the context.</param>
        /// <param name="configuration">The configuration.</param>
        public LubeLoggerDbContext(DbContextOptions<LubeLoggerDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configures the database to be used for this context.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "LubeLoggerDashboard",
                    "LubeLoggerCache.db");

                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Vehicle entity
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(e => e.OdometerRecords)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.PlanRecords)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.ServiceRecords)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.RepairRecords)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.UpgradeRecords)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.TaxRecords)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.GasRecords)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Reminders)
                      .WithOne(e => e.Vehicle)
                      .HasForeignKey(e => e.VehicleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OdometerRecord entity
            modelBuilder.Entity<OdometerRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Odometer).IsRequired();
            });

            // Configure PlanRecord entity
            modelBuilder.Entity<PlanRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Description).IsRequired();
            });

            // Configure ServiceRecord entity
            modelBuilder.Entity<ServiceRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Description).IsRequired();
            });

            // Configure RepairRecord entity
            modelBuilder.Entity<RepairRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Description).IsRequired();
            });

            // Configure UpgradeRecord entity
            modelBuilder.Entity<UpgradeRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Description).IsRequired();
            });

            // Configure TaxRecord entity
            modelBuilder.Entity<TaxRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Description).IsRequired();
            });

            // Configure GasRecord entity
            modelBuilder.Entity<GasRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Odometer).IsRequired();
                entity.Property(e => e.FuelConsumed).IsRequired();
            });

            // Configure Reminder entity
            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.DueDate).IsRequired();
            });

            // Configure UserPreference entity
            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Key).IsRequired();
                entity.Property(e => e.Value).IsRequired();
                entity.HasIndex(e => e.Key).IsUnique();
            });

            // Configure CacheConfiguration entity
            modelBuilder.Entity<CacheConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntityName).IsRequired();
                entity.Property(e => e.ExpirationMinutes).IsRequired();
                entity.HasIndex(e => e.EntityName).IsUnique();
            });

            // Seed default cache configuration
            modelBuilder.Entity<CacheConfiguration>().HasData(
                new CacheConfiguration { Id = 1, EntityName = "Vehicle", ExpirationMinutes = 60 * 24, IsCritical = true, SyncPriority = 1 },
                new CacheConfiguration { Id = 2, EntityName = "OdometerRecord", ExpirationMinutes = 60 * 24, IsCritical = true, SyncPriority = 2 },
                new CacheConfiguration { Id = 3, EntityName = "PlanRecord", ExpirationMinutes = 60 * 12, IsCritical = false, SyncPriority = 3 },
                new CacheConfiguration { Id = 4, EntityName = "ServiceRecord", ExpirationMinutes = 60 * 12, IsCritical = false, SyncPriority = 3 },
                new CacheConfiguration { Id = 5, EntityName = "RepairRecord", ExpirationMinutes = 60 * 12, IsCritical = false, SyncPriority = 3 },
                new CacheConfiguration { Id = 6, EntityName = "UpgradeRecord", ExpirationMinutes = 60 * 12, IsCritical = false, SyncPriority = 3 },
                new CacheConfiguration { Id = 7, EntityName = "TaxRecord", ExpirationMinutes = 60 * 12, IsCritical = false, SyncPriority = 4 },
                new CacheConfiguration { Id = 8, EntityName = "GasRecord", ExpirationMinutes = 60 * 12, IsCritical = false, SyncPriority = 3 },
                new CacheConfiguration { Id = 9, EntityName = "Reminder", ExpirationMinutes = 60 * 24, IsCritical = true, SyncPriority = 2 },
                new CacheConfiguration { Id = 10, EntityName = "UserPreference", ExpirationMinutes = 60 * 24 * 7, IsCritical = true, SyncPriority = 1 }
            );
        }
    }
}