using System.Threading.Tasks;

namespace LubeLoggerDashboard.Models.Database.Initialization
{
    /// <summary>
    /// Interface for database initialization operations.
    /// </summary>
    public interface IDatabaseInitializer
    {
        /// <summary>
        /// Initializes the database by ensuring it exists and applying all migrations.
        /// </summary>
        /// <returns>A task that completes when initialization is done.</returns>
        Task InitializeAsync();
    }
}