using System.Threading.Tasks;

namespace LubeLoggerDashboard.Core.Services.Api.Interfaces
{
    /// <summary>
    /// Base interface for all API services
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Gets the API client used by this service
        /// </summary>
        IApiClient ApiClient { get; }
        
        /// <summary>
        /// Checks if the service is available
        /// </summary>
        /// <returns>True if the service is available, false otherwise</returns>
        Task<bool> IsServiceAvailableAsync();
    }
}