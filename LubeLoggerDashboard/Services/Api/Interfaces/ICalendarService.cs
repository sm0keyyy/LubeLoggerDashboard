using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Calendar API service
    /// </summary>
    public interface ICalendarService : IApiService
    {
        /// <summary>
        /// Gets calendar data
        /// </summary>
        /// <returns>The HTTP response containing calendar data</returns>
        Task<HttpResponseMessage> GetCalendarDataAsync();
        
        /// <summary>
        /// Gets calendar data for a specific vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing calendar data for the specified vehicle</returns>
        Task<HttpResponseMessage> GetCalendarDataAsync(int vehicleId);
        
        /// <summary>
        /// Gets calendar data for a specific date range
        /// </summary>
        /// <param name="startDate">The start date (MM/DD/YYYY)</param>
        /// <param name="endDate">The end date (MM/DD/YYYY)</param>
        /// <returns>The HTTP response containing calendar data for the specified date range</returns>
        Task<HttpResponseMessage> GetCalendarDataAsync(string startDate, string endDate);
        
        /// <summary>
        /// Gets calendar data for a specific vehicle and date range
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="startDate">The start date (MM/DD/YYYY)</param>
        /// <param name="endDate">The end date (MM/DD/YYYY)</param>
        /// <returns>The HTTP response containing calendar data for the specified vehicle and date range</returns>
        Task<HttpResponseMessage> GetCalendarDataAsync(int vehicleId, string startDate, string endDate);
    }
}