using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Vehicle API service
    /// </summary>
    public interface IVehicleService : IApiService
    {
        /// <summary>
        /// Gets a list of all vehicles
        /// </summary>
        /// <returns>The HTTP response containing the list of vehicles</returns>
        Task<HttpResponseMessage> GetVehiclesAsync();
        
        /// <summary>
        /// Gets information about a specific vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing vehicle information</returns>
        Task<HttpResponseMessage> GetVehicleInfoAsync(int vehicleId);
        
        /// <summary>
        /// Gets information about a specific vehicle
        /// </summary>
        /// <returns>The HTTP response containing vehicle information for the default vehicle</returns>
        Task<HttpResponseMessage> GetVehicleInfoAsync();
    }
}