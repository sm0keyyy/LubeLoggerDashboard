using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the User API service
    /// </summary>
    public interface IUserService : IApiService
    {
        /// <summary>
        /// Gets information about the current user
        /// </summary>
        /// <returns>The HTTP response containing user information</returns>
        Task<HttpResponseMessage> GetCurrentUserAsync();
    }
}