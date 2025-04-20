using System;
using System.Threading.Tasks;
using LubeLoggerDashboard.Models.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LubeLoggerDashboard.Models.Database.Initialization
{
    /// <summary>
    /// Implementation of the database initializer that ensures the database exists and all migrations are applied.
    /// </summary>
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly LubeLoggerDbContext _dbContext;
        private readonly ILogger<DatabaseInitializer> _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseInitializer"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger.</param>
        public DatabaseInitializer(LubeLoggerDbContext dbContext, ILogger<DatabaseInitializer> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Initializes the database by ensuring it exists and applying all migrations.
        /// </summary>
        /// <returns>A task that completes when initialization is done.</returns>
        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing database...");
                
                // Ensure database is created and all migrations are applied
                await _dbContext.Database.MigrateAsync();
                
                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
}