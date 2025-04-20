using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using LubeLoggerDashboard.Services.Logging;

namespace LubeLoggerDashboard.Services.Api
{
    /// <summary>
    /// Implementation of IApiClient for the LubeLogger API
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILoggingService _logger;
        private string _baseUrl = "https://demo.lubelogger.com"; // Default to demo instance
        private string _apiVersion = "v1"; // Default API version
        private readonly SemaphoreSlim _throttleSemaphore = new SemaphoreSlim(1, 1);
        private ApiClientOptions _options;
        private CircuitBreakerState _circuitBreaker = new CircuitBreakerState();
        private readonly Dictionary<string, bool> _detectedFeatures = new Dictionary<string, bool>();
        private DateTime _lastHealthCheck = DateTime.MinValue;
        private bool _isHealthy = true;
        
        // Rate limit tracking
        private int _rateLimitLimit = int.MaxValue;
        private int _rateLimitRemaining = int.MaxValue;
        private DateTime _rateLimitReset = DateTime.MaxValue;
        private bool _isThrottled = false;

        /// <inheritdoc/>
        public string ApiVersion => _apiVersion;

        /// <inheritdoc/>
        public string BaseUrl => _baseUrl;

        /// <inheritdoc/>
        public RateLimitInfo RateLimitStatus => new RateLimitInfo
        {
            Limit = _rateLimitLimit,
            Remaining = _rateLimitRemaining,
            ResetTime = _rateLimitReset,
            IsThrottled = _isThrottled
        };

        /// <inheritdoc/>
        public bool IsHealthy => _isHealthy;

        /// <inheritdoc/>
        public DateTime LastHealthCheckTime => _lastHealthCheck;

        /// <summary>
        /// Initializes a new instance of the ApiClient class
        /// </summary>
        /// <param name="logger">The logging service</param>
        public ApiClient(ILoggingService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            // Set default options
            _options = new ApiClientOptions();
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            
            _logger.Debug("ApiClient initialized with default settings");
        }
        
        /// <summary>
        /// Initializes a new instance of the ApiClient class with the specified options
        /// </summary>
        /// <param name="logger">The logging service</param>
        /// <param name="options">The API client options</param>
        public ApiClient(ILoggingService logger, ApiClientOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            // Set default options
            _options = new ApiClientOptions();
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            
            Configure(options);
        }

        /// <inheritdoc/>
        public void Configure(ApiClientOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            _options = options;
            
            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                SetBaseUrl(options.BaseUrl);
            }
            
            if (!string.IsNullOrWhiteSpace(options.ApiVersion))
            {
                SetApiVersion(options.ApiVersion);
            }
            
            _httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            
            // Configure circuit breaker
            _circuitBreaker = new CircuitBreakerState(
                options.CircuitBreakerFailureThreshold,
                TimeSpan.FromMinutes(options.CircuitBreakerResetTimeoutMinutes)
            );
            
            _logger.Information("API client configured with BaseUrl: {BaseUrl}, ApiVersion: {ApiVersion}, Timeout: {Timeout}s",
                _baseUrl, _apiVersion, options.TimeoutSeconds);
        }

        /// <inheritdoc/>
        public void SetApiVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("API version cannot be null or empty", nameof(version));
            }
            
            _apiVersion = version;
            _logger.Information("API version set to {ApiVersion}", version);
        }

        /// <inheritdoc/>
        public void SetBaseUrl(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));
            }
            
            _baseUrl = baseUrl;
            _httpClient.BaseAddress = new Uri(baseUrl);
            _logger.Information("Base URL set to {BaseUrl}", baseUrl);
        }

        /// <inheritdoc/>
        public void SetAuthenticationHeader(string authHeader)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                authHeader.StartsWith("Basic ") ? "Basic" : "Bearer",
                authHeader.StartsWith("Basic ") ? authHeader.Substring(6) : authHeader);
            
            _logger.Debug("Authentication header set");
        }

        /// <inheritdoc/>
        public void ClearAuthenticationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _logger.Debug("Authentication header cleared");
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            return await SendRequestWithRateLimitHandlingAsync(() =>
            {
                var versionedEndpoint = GetVersionedEndpoint(endpoint);
                _logger.Debug("Sending GET request to {Endpoint}", versionedEndpoint);
                return _httpClient.GetAsync(versionedEndpoint);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string endpoint, params (string key, string value)[] queryParams)
        {
            return await SendRequestWithRateLimitHandlingAsync(() =>
            {
                var queryString = BuildQueryString(queryParams);
                var versionedEndpoint = GetVersionedEndpoint(endpoint);
                var url = $"{versionedEndpoint}{queryString}";
                
                _logger.Debug("Sending GET request to {Url}", url);
                return _httpClient.GetAsync(url);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostFormAsync(string endpoint, params (string key, string value)[] formData)
        {
            return await SendRequestWithRateLimitHandlingAsync(() =>
            {
                var content = new FormUrlEncodedContent(formData.Select(p => new KeyValuePair<string, string>(p.key, p.value)));
                var versionedEndpoint = GetVersionedEndpoint(endpoint);
                
                _logger.Debug("Sending POST request to {Endpoint}", versionedEndpoint);
                return _httpClient.PostAsync(versionedEndpoint, content);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutFormAsync(string endpoint, params (string key, string value)[] formData)
        {
            return await SendRequestWithRateLimitHandlingAsync(() =>
            {
                var content = new FormUrlEncodedContent(formData.Select(p => new KeyValuePair<string, string>(p.key, p.value)));
                var versionedEndpoint = GetVersionedEndpoint(endpoint);
                
                _logger.Debug("Sending PUT request to {Endpoint}", versionedEndpoint);
                return _httpClient.PutAsync(versionedEndpoint, content);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, params (string key, string value)[] queryParams)
        {
            return await SendRequestWithRateLimitHandlingAsync(() =>
            {
                var queryString = BuildQueryString(queryParams);
                var versionedEndpoint = GetVersionedEndpoint(endpoint);
                var url = $"{versionedEndpoint}{queryString}";
                
                _logger.Debug("Sending DELETE request to {Url}", url);
                return _httpClient.DeleteAsync(url);
            });
        }

        /// <inheritdoc/>
        public async Task<bool> IsApiAvailableAsync()
        {
            try
            {
                // If circuit breaker is open, API is considered unavailable
                if (_circuitBreaker.IsOpen && _options.EnableCircuitBreaker)
                {
                    _logger.Warning("API availability check failed: Circuit breaker is open");
                    return false;
                }
                
                // Try to make a simple request to check API availability
                var response = await _httpClient.GetAsync("/api/whoami");
                
                // Update health status
                _isHealthy = response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
                _lastHealthCheck = DateTime.UtcNow;
                
                // Log health check result
                if (_isHealthy)
                {
                    _logger.Information("API health check successful. Status code: {StatusCode}", response.StatusCode);
                }
                else
                {
                    _logger.Warning("API health check failed. Status code: {StatusCode}", response.StatusCode);
                }
                
                // 401 is expected if not authenticated, but means the API is available
                return _isHealthy;
            }
            catch (Exception ex)
            {
                _isHealthy = false;
                _lastHealthCheck = DateTime.UtcNow;
                _logger.Error(ex, "API availability check failed");
                return false;
            }
        }

        /// <inheritdoc/>
        public void ResetCircuitBreaker()
        {
            _circuitBreaker.Reset();
            _logger.Information("Circuit breaker reset");
        }
        
        /// <inheritdoc/>
        public async Task<bool> DetectFeatureAsync(string featureName, string testEndpoint)
        {
            if (_detectedFeatures.TryGetValue(featureName, out bool isSupported))
            {
                return isSupported;
            }
            
            try
            {
                _logger.Debug("Detecting feature: {FeatureName} using endpoint: {Endpoint}", featureName, testEndpoint);
                var response = await GetAsync(testEndpoint);
                
                // Consider the feature supported if the response is successful or returns a 404
                // (404 means the endpoint exists but the resource wasn't found)
                isSupported = response.IsSuccessStatusCode ||
                              response.StatusCode == System.Net.HttpStatusCode.NotFound;
                
                _detectedFeatures[featureName] = isSupported;
                _logger.Information("Feature detection result: {FeatureName} is {SupportStatus}",
                    featureName, isSupported ? "supported" : "not supported");
                
                return isSupported;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Feature detection failed for {FeatureName}", featureName);
                _detectedFeatures[featureName] = false;
                return false;
            }
        }
        
        /// <inheritdoc/>
        public void ClearFeatureCache()
        {
            _detectedFeatures.Clear();
            _logger.Information("Feature detection cache cleared");
        }
        
        /// <inheritdoc/>
        public async Task<ApiHealthStatus> GetDetailedHealthStatusAsync()
        {
            var status = new ApiHealthStatus
            {
                IsHealthy = await IsApiAvailableAsync(),
                LastChecked = DateTime.UtcNow,
                CircuitBreakerStatus = _circuitBreaker.IsOpen ? "Open" : (_circuitBreaker._state == CircuitState.HalfOpen ? "Half-Open" : "Closed"),
                RateLimitInfo = RateLimitStatus
            };
            
            _logger.Information("Detailed health status: IsHealthy={IsHealthy}, CircuitBreaker={CircuitBreaker}, RateLimit={RateLimit}/{TotalLimit}",
                status.IsHealthy, status.CircuitBreakerStatus, status.RateLimitInfo.Remaining, status.RateLimitInfo.Limit);
            
            return status;
        }
        
        /// <inheritdoc/>
        public bool ValidatePayloadSize(HttpContent content, long maxSizeBytes)
        {
            if (content == null)
            {
                return true;
            }
            
            try
            {
                var contentLength = content.Headers.ContentLength ?? 0;
                var isValid = contentLength <= maxSizeBytes;
                
                if (!isValid)
                {
                    _logger.Warning("Payload size validation failed. Size: {Size} bytes, Max allowed: {MaxSize} bytes",
                        contentLength, maxSizeBytes);
                }
                
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating payload size");
                return false;
            }
        }

        /// <summary>
        /// Builds a query string from the provided parameters
        /// </summary>
        /// <param name="queryParams">The query parameters</param>
        /// <returns>The query string</returns>
        private string BuildQueryString(params (string key, string value)[] queryParams)
        {
            if (queryParams == null || queryParams.Length == 0)
            {
                return string.Empty;
            }

            var queryString = "?" + string.Join("&", queryParams
                .Where(p => !string.IsNullOrWhiteSpace(p.key) && !string.IsNullOrWhiteSpace(p.value))
                .Select(p => $"{Uri.EscapeDataString(p.key)}={Uri.EscapeDataString(p.value)}"));

            return queryString;
        }
        
        /// <summary>
        /// Gets a versioned endpoint path
        /// </summary>
        /// <param name="endpoint">The original endpoint</param>
        /// <returns>The versioned endpoint</returns>
        protected virtual string GetVersionedEndpoint(string endpoint)
        {
            // Currently, the LubeLogger API doesn't use versioning in the URL
            // This method is prepared for future versioning
            
            // If the API starts using versioning, uncomment the following:
            // if (!endpoint.StartsWith("/"))
            // {
            //     endpoint = "/" + endpoint;
            // }
            // return $"/api/{_apiVersion}{endpoint}";
            
            return endpoint;
        }
        
        /// <summary>
        /// Sends a request with rate limit handling
        /// </summary>
        /// <param name="requestFunc">The request function</param>
        /// <returns>The HTTP response message</returns>
        protected virtual async Task<HttpResponseMessage> SendRequestWithRateLimitHandlingAsync(Func<Task<HttpResponseMessage>> requestFunc)
        {
            int retryCount = 0;
            
            while (true)
            {
                // Check if we're rate limited
                if (_options.EnableThrottling)
                {
                    await _throttleSemaphore.WaitAsync();
                    try
                    {
                        if (_rateLimitRemaining <= 0 && DateTime.UtcNow < _rateLimitReset)
                        {
                            // We're rate limited, calculate delay
                            var delay = _rateLimitReset - DateTime.UtcNow;
                            _isThrottled = true;
                            _logger.Warning("Rate limit reached. Waiting for {Delay} before retrying", delay);
                            
                            // Release semaphore during delay
                            _throttleSemaphore.Release();
                            await Task.Delay(delay);
                            continue;
                        }
                        
                        _isThrottled = false;
                    }
                    finally
                    {
                        if (_throttleSemaphore.CurrentCount == 0)
                        {
                            _throttleSemaphore.Release();
                        }
                    }
                }
                
                // Check circuit breaker
                if (_options.EnableCircuitBreaker && _circuitBreaker.IsOpen)
                {
                    if (DateTime.UtcNow < _circuitBreaker.ResetTime)
                    {
                        _logger.Warning("Circuit breaker is open. API is currently unavailable.");
                        throw new CircuitBreakerOpenException("Circuit breaker is open. API is currently unavailable.");
                    }
                    
                    // Try to reset the circuit breaker
                    _circuitBreaker.HalfOpen();
                    _logger.Information("Circuit breaker is half-open. Testing API availability.");
                }
                
                try
                {
                    var response = await SendWithRetryAsync(requestFunc, retryCount);
                    
                    // Check for rate limit headers
                    UpdateRateLimitInfo(response);
                    
                    // Update circuit breaker state
                    if (_options.EnableCircuitBreaker)
                    {
                        if (response.IsSuccessStatusCode || (int)response.StatusCode < 500)
                        {
                            _circuitBreaker.Success();
                        }
                        else
                        {
                            _circuitBreaker.Failure();
                            if (_circuitBreaker.IsOpen)
                            {
                                _logger.Warning("Circuit breaker opened due to server errors");
                            }
                        }
                    }
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (retryCount >= _options.MaxRetries)
                        {
                            _logger.Error("Rate limit exceeded and retry count reached");
                            return response; // Return the 429 response after max retries
                        }
                        
                        retryCount++;
                        var retryAfter = GetRetryAfterDelay(response, retryCount);
                        _logger.Warning("Rate limit exceeded. Retrying after {RetryAfter}ms (Attempt {RetryCount}/{MaxRetries})",
                            retryAfter, retryCount, _options.MaxRetries);
                        
                        await Task.Delay(retryAfter);
                        continue;
                    }
                    
                    return response;
                }
                catch (CircuitBreakerOpenException)
                {
                    throw; // Re-throw circuit breaker exceptions
                }
                catch (Exception ex)
                {
                    if (_options.EnableCircuitBreaker)
                    {
                        _circuitBreaker.Failure();
                        if (_circuitBreaker.IsOpen)
                        {
                            _logger.Warning("Circuit breaker opened due to exceptions");
                        }
                    }
                    
                    if (retryCount >= _options.MaxRetries)
                    {
                        Log.Error(ex, "Error sending request after {RetryCount} retries", retryCount);
                        throw;
                    }
                    
                    retryCount++;
                    var delay = CalculateExponentialBackoff(retryCount);
                    Log.Warning(ex, "Error sending request. Retrying after {Delay}ms (Attempt {RetryCount}/{MaxRetries})",
                        delay, retryCount, _options.MaxRetries);
                    
                    await Task.Delay(delay);
                }
            }
        }
        
        /// <summary>
        /// Sends a request with retry logic
        /// </summary>
        /// <param name="requestFunc">The request function</param>
        /// <param name="retryCount">The current retry count</param>
        /// <returns>The HTTP response message</returns>
        protected virtual async Task<HttpResponseMessage> SendWithRetryAsync(Func<Task<HttpResponseMessage>> requestFunc, int retryCount)
        {
            try
            {
                return await requestFunc();
            }
            catch (HttpRequestException ex) when (retryCount < _options.MaxRetries)
            {
                // Only retry transient errors
                throw; // Let the outer handler deal with retries
            }
        }
        
        /// <summary>
        /// Updates rate limit information from response headers
        /// </summary>
        /// <param name="response">The HTTP response message</param>
        private void UpdateRateLimitInfo(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("X-RateLimit-Limit", out var limitValues) &&
                int.TryParse(limitValues.FirstOrDefault(), out var limit))
            {
                _rateLimitLimit = limit;
            }
            
            if (response.Headers.TryGetValues("X-RateLimit-Remaining", out var remainingValues) &&
                int.TryParse(remainingValues.FirstOrDefault(), out var remaining))
            {
                _rateLimitRemaining = remaining;
            }
            
            if (response.Headers.TryGetValues("X-RateLimit-Reset", out var resetValues))
            {
                if (int.TryParse(resetValues.FirstOrDefault(), out var resetSeconds))
                {
                    _rateLimitReset = DateTime.UtcNow.AddSeconds(resetSeconds);
                }
                else if (DateTime.TryParse(resetValues.FirstOrDefault(), out var resetTime))
                {
                    _rateLimitReset = resetTime;
                }
            }
            
            Log.Debug("Rate limit info updated: Limit={Limit}, Remaining={Remaining}, Reset={Reset}",
                _rateLimitLimit, _rateLimitRemaining, _rateLimitReset);
        }
        
        /// <summary>
        /// Gets the retry delay from the Retry-After header or calculates exponential backoff
        /// </summary>
        /// <param name="response">The HTTP response message</param>
        /// <param name="retryCount">The current retry count</param>
        /// <returns>The retry delay in milliseconds</returns>
        private int GetRetryAfterDelay(HttpResponseMessage response, int retryCount)
        {
            if (response.Headers.TryGetValues("Retry-After", out var values))
            {
                if (int.TryParse(values.FirstOrDefault(), out var seconds))
                {
                    return seconds * 1000; // Convert to milliseconds
                }
                
                if (DateTime.TryParse(values.FirstOrDefault(), out var retryAfterDate))
                {
                    var delay = retryAfterDate - DateTime.UtcNow;
                    return (int)delay.TotalMilliseconds;
                }
            }
            
            return CalculateExponentialBackoff(retryCount);
        }
        
        /// <summary>
        /// Calculates exponential backoff with jitter
        /// </summary>
        /// <param name="retryCount">The current retry count</param>
        /// <returns>The backoff delay in milliseconds</returns>
        private int CalculateExponentialBackoff(int retryCount)
        {
            // Calculate exponential backoff: baseDelay * 2^retryCount
            var backoff = _options.BaseRetryDelayMs * Math.Pow(2, Math.Min(retryCount, 6)); // Cap at 2^6 to avoid overflow
            
            // Add jitter (0-50% random variation) to avoid thundering herd
            var jitter = new Random().NextDouble() * 0.5 + 0.5; // 0.5-1.0 multiplier
            
            return (int)(backoff * jitter);
        }
    }
    
    /// <summary>
    /// Exception thrown when the circuit breaker is open
    /// </summary>
    public class CircuitBreakerOpenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CircuitBreakerOpenException class
        /// </summary>
        /// <param name="message">The exception message</param>
        public CircuitBreakerOpenException(string message) : base(message) { }
    }
    
    /// <summary>
    /// Represents the state of the circuit breaker
    /// </summary>
    internal class CircuitBreakerState
    {
        private int _failureCount;
        private readonly int _failureThreshold;
        private readonly TimeSpan _resetTimeout;
        internal CircuitState _state = CircuitState.Closed;
        
        /// <summary>
        /// Gets the time when the circuit breaker will reset
        /// </summary>
        public DateTime ResetTime { get; private set; }
        
        /// <summary>
        /// Gets a value indicating whether the circuit breaker is open
        /// </summary>
        public bool IsOpen => _state == CircuitState.Open;
        
        /// <summary>
        /// Initializes a new instance of the CircuitBreakerState class
        /// </summary>
        public CircuitBreakerState() : this(5, TimeSpan.FromMinutes(1)) { }
        
        /// <summary>
        /// Initializes a new instance of the CircuitBreakerState class
        /// </summary>
        /// <param name="failureThreshold">The number of failures required to trip the circuit breaker</param>
        /// <param name="resetTimeout">The timeout before the circuit breaker resets</param>
        public CircuitBreakerState(int failureThreshold, TimeSpan resetTimeout)
        {
            _failureThreshold = failureThreshold;
            _resetTimeout = resetTimeout;
        }
        
        /// <summary>
        /// Records a successful request
        /// </summary>
        public void Success()
        {
            _failureCount = 0;
            _state = CircuitState.Closed;
        }
        
        /// <summary>
        /// Records a failed request
        /// </summary>
        public void Failure()
        {
            _failureCount++;
            if (_failureCount >= _failureThreshold || _state == CircuitState.HalfOpen)
            {
                _state = CircuitState.Open;
                ResetTime = DateTime.UtcNow.Add(_resetTimeout);
            }
        }
        
        /// <summary>
        /// Sets the circuit breaker to half-open state
        /// </summary>
        public void HalfOpen()
        {
            _state = CircuitState.HalfOpen;
        }
        
        /// <summary>
        /// Resets the circuit breaker to closed state
        /// </summary>
        public void Reset()
        {
            _failureCount = 0;
            _state = CircuitState.Closed;
        }
        
        /// <summary>
        /// Represents the state of the circuit
        /// </summary>
        internal enum CircuitState
        {
            /// <summary>
            /// Circuit is closed and requests are allowed
            /// </summary>
            Closed,
            
            /// <summary>
            /// Circuit is open and requests are blocked
            /// </summary>
            Open,
            
            /// <summary>
            /// Circuit is half-open and a single request is allowed to test the API
            /// </summary>
            HalfOpen
        }
    }
    
    /// <summary>
    /// Detailed health status of the API
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