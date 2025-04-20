using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Gas Record API service
    /// </summary>
    public interface IGasRecordService : IApiService
    {
        /// <summary>
        /// Gets gas records for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing gas records</returns>
        Task<HttpResponseMessage> GetGasRecordsAsync(int vehicleId);
        
        /// <summary>
        /// Adds a gas record for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="odometer">The odometer reading</param>
        /// <param name="fuelConsumed">The amount of fuel consumed</param>
        /// <param name="isFullFill">Whether this was a fill-to-full</param>
        /// <param name="missedFuelUp">Whether a fuel-up was missed</param>
        /// <param name="cost">The cost of the gas</param>
        /// <param name="notes">Optional notes</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> AddGasRecordAsync(
            int vehicleId, 
            string date, 
            int odometer, 
            decimal fuelConsumed, 
            bool isFullFill, 
            bool missedFuelUp, 
            decimal cost, 
            string notes = null);
        
        /// <summary>
        /// Updates a gas record
        /// </summary>
        /// <param name="recordId">The ID of the record to update</param>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="odometer">The odometer reading</param>
        /// <param name="fuelConsumed">The amount of fuel consumed</param>
        /// <param name="isFullFill">Whether this was a fill-to-full</param>
        /// <param name="missedFuelUp">Whether a fuel-up was missed</param>
        /// <param name="cost">The cost of the gas</param>
        /// <param name="notes">Optional notes</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> UpdateGasRecordAsync(
            int recordId,
            int vehicleId, 
            string date, 
            int odometer, 
            decimal fuelConsumed, 
            bool isFullFill, 
            bool missedFuelUp, 
            decimal cost, 
            string notes = null);
        
        /// <summary>
        /// Deletes a gas record
        /// </summary>
        /// <param name="recordId">The ID of the record to delete</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> DeleteGasRecordAsync(int recordId);
    }
}