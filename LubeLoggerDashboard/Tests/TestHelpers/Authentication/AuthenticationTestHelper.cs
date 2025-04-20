using System;
using System.Threading.Tasks;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Services.Api;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LubeLoggerDashboard.Tests.TestHelpers.Authentication
{
    /// <summary>
    /// Helper class for authentication-related testing
    /// </summary>
    public static class AuthenticationTestHelper
    {
        /// <summary>
        /// Creates a service provider with authentication services configured
        /// </summary>
        /// <param name="isAuthenticated">Whether the user should be authenticated</param>
        /// <param name="username">The username to use for authentication</param>
        /// <param name="password">The password to use for authentication</param>
        /// <param name="authenticationSucceeds">Whether authentication attempts should succeed</param>
        /// <returns>A configured service provider</returns>
        public static IServiceProvider CreateAuthenticationServiceProvider(
            bool isAuthenticated = true,
            string username = "testuser",
            string password = "testpassword",
            bool authenticationSucceeds = true)
        {
            var services = new ServiceCollection();
            
            // Set up mock credential manager
            var mockCredentialManager = new Mock<ICredentialManager>();
            
            if (isAuthenticated)
            {
                mockCredentialManager.Setup(cm => cm.GetCredentials()).Returns(new Credentials
                {
                    Username = username,
                    Password = password
                });
            }
            else
            {
                mockCredentialManager.Setup(cm => cm.GetCredentials()).Returns((Credentials)null);
            }
            
            services.AddSingleton(mockCredentialManager.Object);
            
            // Set up mock API client
            var mockApiClient = new Mock<IApiClient>();
            
            if (authenticationSucceeds)
            {
                mockApiClient.Setup(c => c.GetAsync("/api/whoami")).ReturnsAsync(new System.Net.Http.HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new System.Net.Http.StringContent($"{{\"username\":\"{username}\"}}")
                });
            }
            else
            {
                mockApiClient.Setup(c => c.GetAsync("/api/whoami")).ReturnsAsync(new System.Net.Http.HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Content = new System.Net.Http.StringContent("{\"error\":\"Unauthorized\"}")
                });
            }
            
            services.AddSingleton(mockApiClient.Object);
            
            // Add authentication service
            services.AddSingleton<IAuthenticationService, BasicAuthenticationService>();
            
            return services.BuildServiceProvider();
        }
        
        /// <summary>
        /// Creates an authenticated user session
        /// </summary>
        /// <param name="authService">The authentication service</param>
        /// <param name="username">The username to authenticate with</param>
        /// <param name="password">The password to authenticate with</param>
        /// <returns>The authentication result</returns>
        public static async Task<AuthenticationResult> AuthenticateUserAsync(
            IAuthenticationService authService,
            string username = "testuser",
            string password = "testpassword")
        {
            return await authService.AuthenticateAsync(username, password);
        }
        
        /// <summary>
        /// Creates a Basic Authentication header value
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The Basic Authentication header value</returns>
        public static string CreateBasicAuthHeader(string username, string password)
        {
            var credentials = $"{username}:{password}";
            var credentialsBytes = System.Text.Encoding.UTF8.GetBytes(credentials);
            var base64Credentials = Convert.ToBase64String(credentialsBytes);
            return $"Basic {base64Credentials}";
        }
        
        /// <summary>
        /// Validates a Basic Authentication header value
        /// </summary>
        /// <param name="authHeader">The authentication header value</param>
        /// <param name="expectedUsername">The expected username</param>
        /// <param name="expectedPassword">The expected password</param>
        /// <returns>True if the header is valid, false otherwise</returns>
        public static bool ValidateBasicAuthHeader(string authHeader, string expectedUsername, string expectedPassword)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                return false;
            }
            
            try
            {
                var base64Credentials = authHeader.Substring("Basic ".Length).Trim();
                var credentialsBytes = Convert.FromBase64String(base64Credentials);
                var credentials = System.Text.Encoding.UTF8.GetString(credentialsBytes);
                var parts = credentials.Split(':');
                
                if (parts.Length != 2)
                {
                    return false;
                }
                
                var username = parts[0];
                var password = parts[1];
                
                return username == expectedUsername && password == expectedPassword;
            }
            catch
            {
                return false;
            }
        }
    }
}