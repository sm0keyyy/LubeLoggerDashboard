using System.Net.Http;
using System.Threading.Tasks;

namespace LubeLoggerDashboard.Services.Api
{
    /// <summary>
    /// Interface for the LubeLogger API client
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Sets the authentication header for API requests
        /// </summary>
        /// <param name="authHeader">The authentication header value</param>
        void SetAuthenticationHeader(string authHeader);
        
        /// <summary>
        /// Clears the authentication header
        /// </summary>
        void ClearAuthenticationHeader();
        
        /// <summary>
        /// Sends a GET request to the specified endpoint
        /// </summary>
        /// <param name="endpoint">The API endpoint</param>
        /// <returns>The HTTP response message</returns>
        Task<HttpResponseMessage> GetAsync(string endpoint);
        
        /// <summary>
        /// Sends a GET request to the specified endpoint with query parameters
        /// </summary>
        /// <param name="endpoint">The API endpoint</param>
        /// <param name="queryParams">The query parameters</param>
        /// <returns>The HTTP response message</returns>
        Task<HttpResponseMessage> GetAsync(string endpoint, params (string key, string value)[] queryParams);
        
        /// <summary>
        /// Sends a POST request to the specified endpoint with form data
        /// </summary>
        /// <param name="endpoint">The API endpoint</param>
        /// <param name="formData">The form data</param>
        /// <returns>The HTTP response message</returns>
        Task<HttpResponseMessage> PostFormAsync(string endpoint, params (string key, string value)[] formData);
        
        /// <summary>
        /// Sends a PUT request to the specified endpoint with form data
        /// </summary>
        /// <param name="endpoint">The API endpoint</param>
        /// <param name="formData">The form data</param>
        /// <returns>The HTTP response message</returns>
        Task<HttpResponseMessage> PutFormAsync(string endpoint, params (string key, string value)[] formData);
        
        /// <summary>
        /// Sends a DELETE request to the specified endpoint with query parameters
        /// </summary>
        /// <param name="endpoint">The API endpoint</param>
        /// <param name="queryParams">The query parameters</param>
        /// <returns>The HTTP response message</returns>
        Task<HttpResponseMessage> DeleteAsync(string endpoint, params (string key, string value)[] queryParams);
    }
}