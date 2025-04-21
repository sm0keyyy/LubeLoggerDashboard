using System.Net.Http;
using System.Threading.Tasks;
using LubeLoggerDashboard.Core.Services.Api;
using LubeLoggerDashboard.Core.Services.Api.Interfaces;
using LubeLoggerDashboard.Core.Services.Logging;

namespace LubeLoggerDashboard.Infrastructure.Services.Api
{
    /// <summary>
    /// Implementation of IVehicleService
    /// </summary>
    public class VehicleService : ApiService, IVehicleService
    {
        private readonly ILoggingService _logger;

        /// <summary>
        /// Initializes a new instance of the VehicleService class
        /// </summary>
        /// <param name="apiClient">The API client</param>
        /// <param name="logger">The logging service</param>
        public VehicleService(IApiClient apiClient, ILoggingService logger) : base(apiClient)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetVehiclesAsync()
        {
            _logger.Debug("Getting list of vehicles");
            return await ApiClient.GetAsync("/api/vehicles");
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetVehicleInfoAsync(int vehicleId)
        {
            _logger.Debug("Getting information for vehicle {VehicleId}", vehicleId);
            return await ApiClient.GetAsync("/api/vehicle/info", ("vehicleId", vehicleId.ToString()));
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetVehicleInfoAsync()
        {
            _logger.Debug("Getting information for default vehicle");
            return await ApiClient.GetAsync("/api/vehicle/info");
        }
    }
}