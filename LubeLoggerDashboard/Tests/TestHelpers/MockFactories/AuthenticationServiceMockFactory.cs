using System;
using LubeLoggerDashboard.Services.Authentication;
using Moq;

namespace LubeLoggerDashboard.Tests.TestHelpers.MockFactories
{
    /// <summary>
    /// Factory for creating mocked authentication service instances for testing
    /// </summary>
    public static class AuthenticationServiceMockFactory
    {
        /// <summary>
        /// Creates a mock IAuthenticationService with the specified behavior
        /// </summary>
        /// <param name="isAuthenticated">Whether the user is authenticated</param>
        /// <param name="authenticationSucceeds">Whether authentication attempts succeed</param>
        /// <param name="authHeader">The authentication header to return</param>
        /// <returns>A mock IAuthenticationService</returns>
        public static Mock<IAuthenticationService> CreateMockAuthenticationService(
            bool isAuthenticated = true,
            bool authenticationSucceeds = true,
            string authHeader = "Basic dGVzdDp0ZXN0")
        {
            var mockAuthService = new Mock<IAuthenticationService>();
            
            // Setup IsAuthenticated
            mockAuthService.Setup(a => a.IsAuthenticated()).Returns(isAuthenticated);
            
            // Setup GetAuthenticationHeader
            mockAuthService.Setup(a => a.GetAuthenticationHeader()).Returns(isAuthenticated ? authHeader : null);
            
            // Setup AuthenticateAsync
            if (authenticationSucceeds)
            {
                mockAuthService.Setup(a => a.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(AuthenticationResult.Successful());
            }
            else
            {
                mockAuthService.Setup(a => a.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(AuthenticationResult.Failed("Invalid credentials"));
            }
            
            // Setup Logout
            mockAuthService.Setup(a => a.Logout())
                .Callback(() => mockAuthService.Setup(a => a.IsAuthenticated()).Returns(false));
            
            return mockAuthService;
        }
    }
}
