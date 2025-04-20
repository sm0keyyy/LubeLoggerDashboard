using System.Net.Http;
using System.Threading.Tasks;
using LubeLoggerDashboard.Services.Api.Interfaces;
using Serilog;

namespace LubeLoggerDashboard.Services.Api
{
    /// <summary>
    /// Implementation of IVehicleService
    /// </summary>
    public class VehicleService : ApiService, IVehicleService
    {
        /// <summary>
        /// Initializes a new instance of the VehicleService class
        /// </summary>
        /// <param name="apiClient">The API client</param>
        public VehicleService(IApiClient apiClient) : base(apiClient)
        {
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetVehiclesAsync()
        {
            Log.Debug("Getting list of vehicles");
            return await ApiClient.GetAsync("/api/vehicles");
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetVehicleInfoAsync(int vehicleId)
        {
            Log.Debug("Getting information for vehicle {VehicleId}", vehicleId);
            return await ApiClient.GetAsync("/api/vehicle/info", ("vehicleId", vehicleId.ToString()));
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetVehicleInfoAsync()
        {
            Log.Debug("Getting information for default vehicle");
            return await ApiClient.GetAsync("/api/vehicle/info");
        }
    }
}