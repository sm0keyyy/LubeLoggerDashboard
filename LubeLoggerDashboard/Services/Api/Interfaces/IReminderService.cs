using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Reminder API service
    /// </summary>
    public interface IReminderService : IApiService
    {
        /// <summary>
        /// Gets reminders for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing reminders</returns>
        Task<HttpResponseMessage> GetRemindersAsync(int vehicleId);
        
        /// <summary>
        /// Gets all reminders
        /// </summary>
        /// <returns>The HTTP response containing all reminders</returns>
        Task<HttpResponseMessage> GetAllRemindersAsync();
        
        /// <summary>
        /// Gets upcoming reminders
        /// </summary>
        /// <param name="days">The number of days to look ahead</param>
        /// <returns>The HTTP response containing upcoming reminders</returns>
        Task<HttpResponseMessage> GetUpcomingRemindersAsync(int days);
        
        /// <summary>
        /// Gets upcoming reminders for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="days">The number of days to look ahead</param>
        /// <returns>The HTTP response containing upcoming reminders for the vehicle</returns>
        Task<HttpResponseMessage> GetUpcomingRemindersAsync(int vehicleId, int days);
        
        /// <summary>
        /// Sends reminder emails
        /// </summary>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> SendReminderEmailsAsync();
        
        /// <summary>
        /// Sends reminder emails for a vehicle
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> SendReminderEmailsAsync(int vehicleId);
    }
}