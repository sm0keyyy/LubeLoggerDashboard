using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Base interface for maintenance record API services (service, repair, upgrade)
    /// </summary>
    public interface IMaintenanceRecordService : IApiService
    {
        /// <summary>
        /// Gets the type of maintenance record
        /// </summary>
        string RecordType { get; }
        
        /// <summary>
        /// Gets maintenance records for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing maintenance records</returns>
        Task<HttpResponseMessage> GetRecordsAsync(int vehicleId);
        
        /// <summary>
        /// Adds a maintenance record for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="odometer">The odometer reading</param>
        /// <param name="description">The description of the maintenance</param>
        /// <param name="cost">The cost of the maintenance</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> AddRecordAsync(
            int vehicleId, 
            string date, 
            int odometer, 
            string description, 
            decimal cost);
        
        /// <summary>
        /// Updates a maintenance record
        /// </summary>
        /// <param name="recordId">The ID of the record to update</param>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="odometer">The odometer reading</param>
        /// <param name="description">The description of the maintenance</param>
        /// <param name="cost">The cost of the maintenance</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> UpdateRecordAsync(
            int recordId,
            int vehicleId, 
            string date, 
            int odometer, 
            string description, 
            decimal cost);
        
        /// <summary>
        /// Deletes a maintenance record
        /// </summary>
        /// <param name="recordId">The ID of the record to delete</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> DeleteRecordAsync(int recordId);
    }
}