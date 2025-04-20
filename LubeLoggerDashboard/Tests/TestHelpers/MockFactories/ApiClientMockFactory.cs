using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LubeLoggerDashboard.Services.Api;
using Moq;
using Moq.Protected;

namespace LubeLoggerDashboard.Tests.TestHelpers.MockFactories
{
    /// <summary>
    /// Factory for creating mocked API client instances for testing
    /// </summary>
    public static class ApiClientMockFactory
    {
        /// <summary>
        /// Creates a mock HttpMessageHandler that returns the specified responses in sequence
        /// </summary>
        /// <param name="responses">The sequence of responses to return</param>
        /// <returns>A mock HttpMessageHandler</returns>
        public static Mock<HttpMessageHandler> CreateMockHttpMessageHandler(params (HttpStatusCode statusCode, string content, KeyValuePair<string, string>[] headers)[] responses)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            
            var sequence = mockHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    Moq.It.IsAny<HttpRequestMessage>(),
                    Moq.It.IsAny<CancellationToken>());
            
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
            
            return mockHandler;
        }
        
        /// <summary>
        /// Creates a mock HttpMessageHandler that returns the specified response for all requests
        /// </summary>
        /// <param name="statusCode">The HTTP status code to return</param>
        /// <param name="content">The content to return</param>
        /// <param name="headers">Optional headers to include in the response</param>
        /// <returns>A mock HttpMessageHandler</returns>
        public static Mock<HttpMessageHandler> CreateMockHttpMessageHandler(HttpStatusCode statusCode, string content, KeyValuePair<string, string>[] headers = null)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            
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
            
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    Moq.It.IsAny<HttpRequestMessage>(),
                    Moq.It.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            
            return mockHandler;
        }
        
        /// <summary>
        /// Creates a mock IApiClient with the specified behavior
        /// </summary>
        /// <param name="isHealthy">Whether the API is healthy</param>
        /// <param name="isAuthenticated">Whether the client is authenticated</param>
        /// <returns>A mock IApiClient</returns>
        public static Mock<IApiClient> CreateMockApiClient(bool isHealthy = true, bool isAuthenticated = true)
        {
            var mockApiClient = new Mock<IApiClient>();
            
            mockApiClient.Setup(c => c.IsHealthy).Returns(isHealthy);
            mockApiClient.Setup(c => c.ApiVersion).Returns("v1");
            mockApiClient.Setup(c => c.BaseUrl).Returns("https://test.lubelogger.com");
            
            var rateLimitInfo = new RateLimitInfo
            {
                Limit = 100,
                Remaining = 99,
                ResetTime = DateTime.UtcNow.AddMinutes(60),
                IsThrottled = false
            };
            
            mockApiClient.Setup(c => c.RateLimitStatus).Returns(rateLimitInfo);
            
            if (isAuthenticated)
            {
                mockApiClient.Setup(c => c.GetAuthenticationHeader()).Returns("Basic dGVzdDp0ZXN0");
            }
            else
            {
                mockApiClient.Setup(c => c.GetAuthenticationHeader()).Returns((string)null);
            }
            
            // Setup IsApiAvailableAsync to return isHealthy
            mockApiClient.Setup(c => c.IsApiAvailableAsync()).ReturnsAsync(isHealthy);
            
            // Setup GetDetailedHealthStatusAsync
            var healthStatus = new ApiHealthStatus
            {
                IsHealthy = isHealthy,
                LastChecked = DateTime.UtcNow,
                CircuitBreakerStatus = "Closed",
                RateLimitInfo = rateLimitInfo
            };
            
            mockApiClient.Setup(c => c.GetDetailedHealthStatusAsync()).ReturnsAsync(healthStatus);
            
            return mockApiClient;
        }
    }
}