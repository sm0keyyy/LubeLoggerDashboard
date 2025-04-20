using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace LubeLoggerDashboard.Tests.TestHelpers.Mocking
{
    /// <summary>
    /// Utility class for mocking API responses in tests
    /// </summary>
    public static class ApiResponseMocker
    {
        /// <summary>
        /// Creates a mock HttpMessageHandler that returns the specified JSON response
        /// </summary>
        /// <typeparam name="T">The type of object to serialize as the response</typeparam>
        /// <param name="responseObject">The object to serialize as the response</param>
        /// <param name="statusCode">The HTTP status code to return</param>
        /// <param name="headers">Optional headers to include in the response</param>
        /// <returns>A mock HttpMessageHandler</returns>
        public static Mock<HttpMessageHandler> CreateJsonResponseHandler<T>(T responseObject, HttpStatusCode statusCode = HttpStatusCode.OK, Dictionary<string, string> headers = null)
        {
            var json = JsonSerializer.Serialize(responseObject, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            return CreateStringResponseHandler(json, "application/json", statusCode, headers);
        }
        
        /// <summary>
        /// Creates a mock HttpMessageHandler that returns the specified string response
        /// </summary>
        /// <param name="responseContent">The string content to return</param>
        /// <param name="contentType">The content type of the response</param>
        /// <param name="statusCode">The HTTP status code to return</param>
        /// <param name="headers">Optional headers to include in the response</param>
        /// <returns>A mock HttpMessageHandler</returns>
        public static Mock<HttpMessageHandler> CreateStringResponseHandler(string responseContent, string contentType = "text/plain", HttpStatusCode statusCode = HttpStatusCode.OK, Dictionary<string, string> headers = null)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseContent, Encoding.UTF8, contentType)
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
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            
            return mockHandler;
        }
        
        /// <summary>
        /// Creates a mock HttpMessageHandler that returns different responses based on the request URI
        /// </summary>
        /// <param name="responseMap">A dictionary mapping URI patterns to response functions</param>
        /// <returns>A mock HttpMessageHandler</returns>
        public static Mock<HttpMessageHandler> CreateMappedResponseHandler(Dictionary<string, Func<HttpRequestMessage, HttpResponseMessage>> responseMap)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns<HttpRequestMessage, CancellationToken>((request, cancellationToken) =>
                {
                    foreach (var entry in responseMap)
                    {
                        if (request.RequestUri.ToString().Contains(entry.Key))
                        {
                            return Task.FromResult(entry.Value(request));
                        }
                    }
                    
                    // Default response if no match is found
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("No mock response configured for this URI")
                    });
                });
            
            return mockHandler;
        }
        
        /// <summary>
        /// Creates a mock HttpMessageHandler that returns a sequence of responses
        /// </summary>
        /// <param name="responses">The sequence of responses to return</param>
        /// <returns>A mock HttpMessageHandler</returns>
        public static Mock<HttpMessageHandler> CreateSequenceResponseHandler(params HttpResponseMessage[] responses)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            
            var sequence = mockHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
            
            foreach (var response in responses)
            {
                sequence = sequence.ReturnsAsync(response);
            }
            
            return mockHandler;
        }
        
        /// <summary>
        /// Creates a standard rate limit headers dictionary
        /// </summary>
        /// <param name="limit">The rate limit</param>
        /// <param name="remaining">The remaining requests</param>
        /// <param name="resetSeconds">The seconds until the rate limit resets</param>
        /// <returns>A dictionary of rate limit headers</returns>
        public static Dictionary<string, string> CreateRateLimitHeaders(int limit = 100, int remaining = 99, int resetSeconds = 60)
        {
            return new Dictionary<string, string>
            {
                { "X-RateLimit-Limit", limit.ToString() },
                { "X-RateLimit-Remaining", remaining.ToString() },
                { "X-RateLimit-Reset", resetSeconds.ToString() }
            };
        }
    }
}