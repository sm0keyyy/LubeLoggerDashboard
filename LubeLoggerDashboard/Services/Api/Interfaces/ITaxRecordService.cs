using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Tax Record API service
    /// </summary>
    public interface ITaxRecordService : IApiService
    {
        /// <summary>
        /// Gets tax records for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing tax records</returns>
        Task<HttpResponseMessage> GetTaxRecordsAsync(int vehicleId);
        
        /// <summary>
        /// Adds a tax record for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="description">The description of the tax</param>
        /// <param name="cost">The cost of the tax</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> AddTaxRecordAsync(
            int vehicleId, 
            string date, 
            string description, 
            decimal cost);
        
        /// <summary>
        /// Updates a tax record
        /// </summary>
        /// <param name="recordId">The ID of the record to update</param>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="description">The description of the tax</param>
        /// <param name="cost">The cost of the tax</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> UpdateTaxRecordAsync(
            int recordId,
            int vehicleId, 
            string date, 
            string description, 
            decimal cost);
        
        /// <summary>
        /// Deletes a tax record
        /// </summary>
        /// <param name="recordId">The ID of the record to delete</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> DeleteTaxRecordAsync(int recordId);
    }
}