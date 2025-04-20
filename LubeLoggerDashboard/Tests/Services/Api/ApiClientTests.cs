using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LubeLoggerDashboard.Services.Api;
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
                (HttpStatusCode.TooManyRequests, "Rate limit exceeded", new[] { new KeyValuePair<string, string>("Retry-After", "1") }),
                (HttpStatusCode.OK, "Success")
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

        private ApiClient CreateApiClient()
        {
            return new ApiClient(_options)
            {
                // Use reflection or a test constructor to inject the mock HttpClient
                // For simplicity, we'll use a test subclass
                TestHttpClient = _httpClient
            };
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
        
        public TestApiClient(HttpClient httpClient, ApiClientOptions options) : base(options)
        {
            TestHttpClient = httpClient;
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