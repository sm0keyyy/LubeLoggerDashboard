using System;
using System.Collections.Generic;

namespace LubeLoggerDashboard.Core.Services.Api
{
    /// <summary>
    /// Represents the health status of the API
    /// </summary>
    public class ApiHealthStatus
    {
        /// <summary>
        /// Gets or sets a value indicating whether the API is healthy
        /// </summary>
        public bool IsHealthy { get; set; }
        
        /// <summary>
        /// Gets or sets the time when the health status was last checked
        /// </summary>
        public DateTime LastChecked { get; set; }
        
        /// <summary>
        /// Gets or sets the current circuit breaker status
        /// </summary>
        public string CircuitBreakerStatus { get; set; }
        
        /// <summary>
        /// Gets or sets the current rate limit information
        /// </summary>
        public RateLimitInfo RateLimitInfo { get; set; }
        
        /// <summary>
        /// Gets or sets additional diagnostic information
        /// </summary>
        public Dictionary<string, string> Diagnostics { get; set; } = new Dictionary<string, string>();
    }
}