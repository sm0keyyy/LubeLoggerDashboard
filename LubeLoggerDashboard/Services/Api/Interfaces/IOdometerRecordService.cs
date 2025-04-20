using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Odometer Record API service
    /// </summary>
    public interface IOdometerRecordService : IApiService
    {
        /// <summary>
        /// Gets odometer records for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing odometer records</returns>
        Task<HttpResponseMessage> GetOdometerRecordsAsync(int vehicleId);
        
        /// <summary>
        /// Gets the latest odometer record for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing the latest odometer record</returns>
        Task<HttpResponseMessage> GetLatestOdometerRecordAsync(int vehicleId);
        
        /// <summary>
        /// Gets the adjusted odometer for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing the adjusted odometer</returns>
        Task<HttpResponseMessage> GetAdjustedOdometerAsync(int vehicleId);
        
        /// <summary>
        /// Adds an odometer record for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="odometer">The odometer reading</param>
        /// <param name="notes">Optional notes</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> AddOdometerRecordAsync(int vehicleId, string date, int odometer, string notes = null);
        
        /// <summary>
        /// Updates an odometer record
        /// </summary>
        /// <param name="recordId">The ID of the record to update</param>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="odometer">The odometer reading</param>
        /// <param name="notes">Optional notes</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> UpdateOdometerRecordAsync(int recordId, int vehicleId, string date, int odometer, string notes = null);
        
        /// <summary>
        /// Deletes an odometer record
        /// </summary>
        /// <param name="recordId">The ID of the record to delete</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> DeleteOdometerRecordAsync(int recordId);
    }
}