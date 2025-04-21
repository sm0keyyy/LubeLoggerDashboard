using System.Threading.Tasks;
using LubeLoggerDashboard.Core.Services.Api;
using LubeLoggerDashboard.Core.Services.Api.Interfaces;

namespace LubeLoggerDashboard.Infrastructure.Services.Api
{
    /// <summary>
    /// Base implementation of IApiService
    /// </summary>
    public abstract class ApiService : IApiService
    {
        /// <summary>
        /// Gets the API client used by this service
        /// </summary>
        public IApiClient ApiClient { get; }

        /// <summary>
        /// Initializes a new instance of the ApiService class
        /// </summary>
        /// <param name="apiClient">The API client</param>
        protected ApiService(IApiClient apiClient)
        {
            ApiClient = apiClient;
        }

        /// <summary>
        /// Checks if the service is available
        /// </summary>
        /// <returns>True if the service is available, false otherwise</returns>
        public virtual async Task<bool> IsServiceAvailableAsync()
        {
            return await ApiClient.IsApiAvailableAsync();
        }
    }
}