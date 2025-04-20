using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the System API service
    /// </summary>
    public interface ISystemService : IApiService
    {
        /// <summary>
        /// Creates a backup of the system
        /// </summary>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> CreateBackupAsync();
        
        /// <summary>
        /// Creates a backup of the system with a specific name
        /// </summary>
        /// <param name="backupName">The name of the backup</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> CreateBackupAsync(string backupName);
        
        /// <summary>
        /// Cleans up the system
        /// </summary>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> CleanupSystemAsync();
        
        /// <summary>
        /// Cleans up the system with specific options
        /// </summary>
        /// <param name="options">The cleanup options</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> CleanupSystemAsync(params string[] options);
        
        /// <summary>
        /// Gets system status information
        /// </summary>
        /// <returns>The HTTP response containing system status information</returns>
        Task<HttpResponseMessage> GetSystemStatusAsync();
    }
}