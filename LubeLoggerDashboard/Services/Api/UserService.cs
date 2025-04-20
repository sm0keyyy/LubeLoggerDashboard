using System.Net.Http;
using System.Threading.Tasks;
using LubeLoggerDashboard.Services.Api.Interfaces;
using Serilog;

namespace LubeLoggerDashboard.Services.Api
{
    /// <summary>
    /// Implementation of IUserService
    /// </summary>
    public class UserService : ApiService, IUserService
    {
        /// <summary>
        /// Initializes a new instance of the UserService class
        /// </summary>
        /// <param name="apiClient">The API client</param>
        public UserService(IApiClient apiClient) : base(apiClient)
        {
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetCurrentUserAsync()
        {
            Log.Debug("Getting current user information");
            return await ApiClient.GetAsync("/api/whoami");
        }
    }
}