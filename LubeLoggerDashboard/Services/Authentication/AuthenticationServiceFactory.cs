using System;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Services.Api;
using Microsoft.Extensions.DependencyInjection;

namespace LubeLoggerDashboard.Services.Authentication
{
    /// <summary>
    /// Factory for creating authentication services
    /// </summary>
    public class AuthenticationServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the AuthenticationServiceFactory class
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        public AuthenticationServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Creates an authentication service
        /// </summary>
        /// <returns>The authentication service</returns>
        public IAuthenticationService CreateAuthenticationService()
        {
            var apiClient = _serviceProvider.GetRequiredService<IApiClient>();
            var credentialManager = _serviceProvider.GetRequiredService<ICredentialManager>();
            
            return new BasicAuthenticationService(apiClient, credentialManager);
        }
    }
}