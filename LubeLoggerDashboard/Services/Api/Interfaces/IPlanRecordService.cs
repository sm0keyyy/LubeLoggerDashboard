using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Plan Record API service
    /// </summary>
    public interface IPlanRecordService : IApiService
    {
        /// <summary>
        /// Gets plan records for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing plan records</returns>
        Task<HttpResponseMessage> GetPlanRecordsAsync(int vehicleId);
        
        /// <summary>
        /// Adds a plan record for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="description">The description of the plan</param>
        /// <param name="cost">The estimated cost</param>
        /// <param name="type">The type of the plan (ServiceRecord, RepairRecord, UpgradeRecord)</param>
        /// <param name="priority">The priority of the plan (Low, Normal, Critical)</param>
        /// <param name="progress">The progress status (Backlog, InProgress, Testing)</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> AddPlanRecordAsync(
            int vehicleId, 
            string date, 
            string description, 
            decimal cost, 
            string type, 
            string priority, 
            string progress);
        
        /// <summary>
        /// Updates a plan record
        /// </summary>
        /// <param name="recordId">The ID of the record to update</param>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="date">The date of the record (MM/DD/YYYY)</param>
        /// <param name="description">The description of the plan</param>
        /// <param name="cost">The estimated cost</param>
        /// <param name="type">The type of the plan (ServiceRecord, RepairRecord, UpgradeRecord)</param>
        /// <param name="priority">The priority of the plan (Low, Normal, Critical)</param>
        /// <param name="progress">The progress status (Backlog, InProgress, Testing)</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> UpdatePlanRecordAsync(
            int recordId,
            int vehicleId, 
            string date, 
            string description, 
            decimal cost, 
            string type, 
            string priority, 
            string progress);
        
        /// <summary>
        /// Deletes a plan record
        /// </summary>
        /// <param name="recordId">The ID of the record to delete</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> DeletePlanRecordAsync(int recordId);
    }
}