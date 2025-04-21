using System.Net.Http;
using System.Threading.Tasks;
using LubeLoggerDashboard.Core.Services.Api;
using LubeLoggerDashboard.Core.Services.Api.Interfaces;
using LubeLoggerDashboard.Core.Services.Logging;

namespace LubeLoggerDashboard.Infrastructure.Services.Api
{
    /// <summary>
    /// Implementation of IUserService
    /// </summary>
    public class UserService : ApiService, IUserService
    {
        private readonly ILoggingService _logger;

        /// <summary>
        /// Initializes a new instance of the UserService class
        /// </summary>
        /// <param name="apiClient">The API client</param>
        /// <param name="logger">The logging service</param>
        public UserService(IApiClient apiClient, ILoggingService logger) : base(apiClient)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetCurrentUserAsync()
        {
            _logger.Debug("Getting current user information");
            return await ApiClient.GetAsync("/api/whoami");
        }
    }
}