using System;
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
        /// Gets the current API version being used
        /// </summary>
        string ApiVersion { get; }
        
        /// <summary>
        /// Gets the base URL of the API
        /// </summary>
        string BaseUrl { get; }
        
        /// <summary>
        /// Gets information about current rate limit status
        /// </summary>
        RateLimitInfo RateLimitStatus { get; }
        
        /// <summary>
        /// Sets the API version to use for requests
        /// </summary>
        /// <param name="version">The API version</param>
        void SetApiVersion(string version);
        
        /// <summary>
        /// Sets the base URL for the API
        /// </summary>
        /// <param name="baseUrl">The base URL</param>
        void SetBaseUrl(string baseUrl);
        
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
        
        /// <summary>
        /// Checks if the API is available
        /// </summary>
        /// <returns>True if the API is available, false otherwise</returns>
        Task<bool> IsApiAvailableAsync();
        
        /// <summary>
        /// Resets the circuit breaker if it's in the open state
        /// </summary>
        void ResetCircuitBreaker();
        
        /// <summary>
        /// Configures the API client with the specified options
        /// </summary>
        /// <param name="options">The API client options</param>
        void Configure(ApiClientOptions options);
    }
    
    /// <summary>
    /// Information about the current rate limit status
    /// </summary>
    public class RateLimitInfo
    {
        /// <summary>
        /// The total number of requests allowed in the current time window
        /// </summary>
        public int Limit { get; set; }
        
        /// <summary>
        /// The number of requests remaining in the current time window
        /// </summary>
        public int Remaining { get; set; }
        
        /// <summary>
        /// The time when the rate limit will reset
        /// </summary>
        public DateTime ResetTime { get; set; }
        
        /// <summary>
        /// Whether the client is currently being throttled due to rate limits
        /// </summary>
        public bool IsThrottled { get; set; }
    }
    
    /// <summary>
    /// Options for configuring the API client
    /// </summary>
    public class ApiClientOptions
    {
        /// <summary>
        /// The base URL of the API
        /// </summary>
        public string BaseUrl { get; set; }
        
        /// <summary>
        /// The API version to use
        /// </summary>
        public string ApiVersion { get; set; } = "v1";
        
        /// <summary>
        /// The timeout for API requests in seconds
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
        
        /// <summary>
        /// The maximum number of retry attempts for failed requests
        /// </summary>
        public int MaxRetries { get; set; } = 3;
        
        /// <summary>
        /// The base delay in milliseconds for retry attempts
        /// </summary>
        public int BaseRetryDelayMs { get; set; } = 1000;
        
        /// <summary>
        /// Whether to enable request throttling based on rate limits
        /// </summary>
        public bool EnableThrottling { get; set; } = true;
        
        /// <summary>
        /// Whether to enable the circuit breaker pattern
        /// </summary>
        public bool EnableCircuitBreaker { get; set; } = true;
        
        /// <summary>
        /// The number of failures required to trip the circuit breaker
        /// </summary>
        public int CircuitBreakerFailureThreshold { get; set; } = 5;
        
        /// <summary>
        /// The timeout in minutes before the circuit breaker resets
        /// </summary>
        public int CircuitBreakerResetTimeoutMinutes { get; set; } = 1;
    }
}