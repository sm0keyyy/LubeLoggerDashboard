using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Services.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace LubeLoggerDashboard.Tests.Services.Api
{
    [TestClass]
    public class ApiClientTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private ApiClientOptions _options;

        [TestInitialize]
        public void Initialize()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            
            _options = new ApiClientOptions
            {
                BaseUrl = "https://test.lubelogger.com",
                ApiVersion = "v1",
                TimeoutSeconds = 5,
                MaxRetries = 2,
                BaseRetryDelayMs = 100,
                EnableThrottling = true,
                EnableCircuitBreaker = true,
                CircuitBreakerFailureThreshold = 3,
                CircuitBreakerResetTimeoutMinutes = 1
            };
        }

        [TestMethod]
        public async Task GetAsync_SuccessfulResponse_ReturnsResponse()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.OK, "Success");
            var apiClient = CreateApiClient();

            // Act
            var response = await apiClient.GetAsync("/api/test");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("Success", await response.Content.ReadAsStringAsync());
            
            // Verify the request was made with the correct URL
            VerifyRequest(HttpMethod.Get, "https://test.lubelogger.com/api/test", Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_RateLimitExceeded_RetriesAfterDelay()
        {
            // Arrange
            SetupMockResponseSequence(
                (HttpStatusCode.TooManyRequests, "Rate limit exceeded", new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("Retry-After", "1") }),
                (HttpStatusCode.OK, "Success", null)
            );
            
            var apiClient = CreateApiClient();

            // Act
            var response = await apiClient.GetAsync("/api/test");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("Success", await response.Content.ReadAsStringAsync());
            
            // Verify the request was made twice (initial + retry)
            VerifyRequest(HttpMethod.Get, "https://test.lubelogger.com/api/test", Times.Exactly(2));
        }

        [TestMethod]
        public async Task GetAsync_ServerError_TripsCircuitBreaker()
        {
            // Arrange
            SetupMockResponseSequence(
                (HttpStatusCode.InternalServerError, "Server Error"),
                (HttpStatusCode.InternalServerError, "Server Error"),
                (HttpStatusCode.InternalServerError, "Server Error")
            );
            
            var apiClient = CreateApiClient();

            // Act & Assert
            // First request - should retry and eventually fail
            await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => 
                await apiClient.GetAsync("/api/test"));
            
            // Second request - should throw CircuitBreakerOpenException
            await Assert.ThrowsExceptionAsync<CircuitBreakerOpenException>(async () => 
                await apiClient.GetAsync("/api/test"));
            
            // Verify the request was made 3 times (initial + 2 retries)
            VerifyRequest(HttpMethod.Get, "https://test.lubelogger.com/api/test", Times.Exactly(3));
        }

        [TestMethod]
        public async Task GetAsync_WithRateLimitHeaders_UpdatesRateLimitInfo()
        {
            // Arrange
            var headers = new[]
            {
                new KeyValuePair<string, string>("X-RateLimit-Limit", "100"),
                new KeyValuePair<string, string>("X-RateLimit-Remaining", "99"),
                new KeyValuePair<string, string>("X-RateLimit-Reset", "60")
            };
            
            SetupMockResponse(HttpStatusCode.OK, "Success", headers);
            var apiClient = CreateApiClient();

            // Act
            await apiClient.GetAsync("/api/test");

            // Assert
            var rateLimitInfo = apiClient.RateLimitStatus;
            Assert.AreEqual(100, rateLimitInfo.Limit);
            Assert.AreEqual(99, rateLimitInfo.Remaining);
            Assert.IsTrue((rateLimitInfo.ResetTime - DateTime.UtcNow).TotalSeconds <= 60);
            Assert.IsFalse(rateLimitInfo.IsThrottled);
        }

        [TestMethod]
        public async Task GetAsync_WithVersioning_AppendsVersionToUrl()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.OK, "Success");
            
            // Create a custom ApiClient that uses versioning in the URL
            var apiClient = new TestApiClient(_httpClient, _options);

            // Act
            await apiClient.GetAsync("/test");

            // Assert
            // Verify the request was made with the versioned URL
            VerifyRequest(HttpMethod.Get, "https://test.lubelogger.com/api/v1/test", Times.Once());
        }
        
        [TestMethod]
        public async Task DetectFeatureAsync_FeatureExists_ReturnsTrue()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.OK, "Feature exists");
            var apiClient = CreateApiClient();
            
            // Act
            var result = await apiClient.DetectFeatureAsync("testFeature", "/api/test-feature");
            
            // Assert
            Assert.IsTrue(result);
            VerifyRequest(HttpMethod.Get, "https://test.lubelogger.com/api/test-feature", Times.Once());
        }
        
        [TestMethod]
        public async Task DetectFeatureAsync_FeatureDoesNotExist_ReturnsFalse()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.NotImplemented, "Feature not implemented");
            var apiClient = CreateApiClient();
            
            // Act
            var result = await apiClient.DetectFeatureAsync("testFeature", "/api/test-feature");
            
            // Assert
            Assert.IsFalse(result);
            VerifyRequest(HttpMethod.Get, "https://test.lubelogger.com/api/test-feature", Times.Once());
        }
        
        [TestMethod]
        public async Task DetectFeatureAsync_CacheWorks_DoesNotMakeSecondRequest()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.OK, "Feature exists");
            var apiClient = CreateApiClient();
            
            // Act
            var result1 = await apiClient.DetectFeatureAsync("testFeature", "/api/test-feature");
            var result2 = await apiClient.DetectFeatureAsync("testFeature", "/api/test-feature");
            
            // Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            VerifyRequest(HttpMethod.Get, "https://test.lubelogger.com/api/test-feature", Times.Once());
        }
        
        [TestMethod]
        public async Task GetDetailedHealthStatusAsync_ReturnsHealthStatus()
        {
            // Arrange
            SetupMockResponse(HttpStatusCode.OK, "API is healthy");
            var apiClient = CreateApiClient();
            
            // Act
            var healthStatus = await apiClient.GetDetailedHealthStatusAsync();
            
            // Assert
            Assert.IsTrue(healthStatus.IsHealthy);
            Assert.AreEqual("Closed", healthStatus.CircuitBreakerStatus);
            Assert.IsNotNull(healthStatus.RateLimitInfo);
            Assert.IsTrue((DateTime.UtcNow - healthStatus.LastChecked).TotalSeconds < 5);
        }
        
        [TestMethod]
        public void ValidatePayloadSize_SizeWithinLimit_ReturnsTrue()
        {
            // Arrange
            var content = new StringContent("Test content");
            content.Headers.ContentLength = 100;
            var apiClient = CreateApiClient();
            
            // Act
            var result = apiClient.ValidatePayloadSize(content, 1000);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void ValidatePayloadSize_SizeExceedsLimit_ReturnsFalse()
        {
            // Arrange
            var content = new StringContent("Test content");
            content.Headers.ContentLength = 2000;
            var apiClient = CreateApiClient();
            
            // Act
            var result = apiClient.ValidatePayloadSize(content, 1000);
            
            // Assert
            Assert.IsFalse(result);
        }

        private ApiClient CreateApiClient()
        {
            var mockLogger = new Mock<ILoggingService>();
            return new TestApiClient(_httpClient, _options);
        }

        private void SetupMockResponse(HttpStatusCode statusCode, string content, KeyValuePair<string, string>[] headers = null)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            };
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    response.Headers.Add(header.Key, header.Value);
                }
            }
            
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        private void SetupMockResponseSequence(params (HttpStatusCode statusCode, string content, KeyValuePair<string, string>[] headers)[] responses)
        {
            var sequence = _mockHttpMessageHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
            
            foreach (var (statusCode, content, headers) in responses)
            {
                var response = new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(content)
                };
                
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        response.Headers.Add(header.Key, header.Value);
                    }
                }
                
                sequence = sequence.ReturnsAsync(response);
            }
        }

        private void SetupMockResponseSequence(params (HttpStatusCode statusCode, string content)[] responses)
        {
            var sequence = _mockHttpMessageHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
            
            foreach (var (statusCode, content) in responses)
            {
                var response = new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(content)
                };
                
                sequence = sequence.ReturnsAsync(response);
            }
        }

        private void VerifyRequest(HttpMethod method, string url, Times times)
        {
            _mockHttpMessageHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    times,
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>());
        }
    }

    // Test subclass to allow injecting the HttpClient for testing
    public class TestApiClient : ApiClient
    {
        public HttpClient TestHttpClient { get; set; }
        
        public TestApiClient(HttpClient httpClient, ApiClientOptions options) : base(CreateMockLoggingService(), options)
        {
            TestHttpClient = httpClient;
        }
        
        private static ILoggingService CreateMockLoggingService()
        {
            var mockLogger = new Mock<ILoggingService>();
            return mockLogger.Object;
        }
        
        // Override the GetVersionedEndpoint method to test versioning
        protected override string GetVersionedEndpoint(string endpoint)
        {
            if (!endpoint.StartsWith("/"))
            {
                endpoint = "/" + endpoint;
            }
            
            return $"/api/{ApiVersion}{endpoint}";
        }
        
        // Override the SendRequestWithRateLimitHandlingAsync method to use the test HttpClient
        protected override async Task<HttpResponseMessage> SendRequestWithRateLimitHandlingAsync(Func<Task<HttpResponseMessage>> requestFunc)
        {
            return await base.SendRequestWithRateLimitHandlingAsync(requestFunc);
        }
    }
}