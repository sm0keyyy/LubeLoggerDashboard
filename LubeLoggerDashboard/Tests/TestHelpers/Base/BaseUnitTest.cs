using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Tests.TestHelpers.Configuration;
using LubeLoggerDashboard.Tests.TestHelpers.MockFactories;

namespace LubeLoggerDashboard.Tests.TestHelpers.Base
{
    /// <summary>
    /// Base class for unit tests providing common setup and utilities
    /// </summary>
    public abstract class BaseUnitTest : IDisposable
    {
        /// <summary>
        /// The service provider for dependency injection
        /// </summary>
        protected IServiceProvider ServiceProvider { get; private set; }
        
        /// <summary>
        /// The service collection for registering services
        /// </summary>
        protected IServiceCollection Services { get; private set; }
        
        /// <summary>
        /// The configuration for the test
        /// </summary>
        protected IConfiguration Configuration { get; private set; }
        
        /// <summary>
        /// Mock for the API client
        /// </summary>
        protected Mock<IApiClient> MockApiClient { get; private set; }
        
        /// <summary>
        /// Mock for the authentication service
        /// </summary>
        protected Mock<IAuthenticationService> MockAuthService { get; private set; }
        
        /// <summary>
        /// Mock for the credential manager
        /// </summary>
        protected Mock<ICredentialManager> MockCredentialManager { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseUnitTest"/> class
        /// </summary>
        protected BaseUnitTest()
        {
            // Set up configuration
            Configuration = TestConfigurationHelper.CreateStandardTestConfiguration();
            
            // Set up service collection
            Services = new ServiceCollection();
            
            // Add configuration
            Services.AddSingleton(Configuration);
            
            // Add logging
            Services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            
            // Set up mocks
            MockApiClient = ApiClientMockFactory.CreateMockApiClient();
            MockAuthService = AuthenticationServiceMockFactory.CreateMockAuthenticationService();
            MockCredentialManager = new Mock<ICredentialManager>();
            
            // Register mocks with the service collection
            Services.AddSingleton(MockApiClient.Object);
            Services.AddSingleton(MockAuthService.Object);
            Services.AddSingleton(MockCredentialManager.Object);
            
            // Build the service provider
            ServiceProvider = Services.BuildServiceProvider();
        }
        
        /// <summary>
        /// Gets a service from the service provider
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>The service instance</returns>
        protected T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }
        
        /// <summary>
        /// Gets a required service from the service provider
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>The service instance</returns>
        /// <exception cref="InvalidOperationException">Thrown if the service is not registered</exception>
        protected T GetRequiredService<T>() where T : class
        {
            return ServiceProvider.GetRequiredService<T>();
        }
        
        /// <summary>
        /// Creates a mock logger for the specified type
        /// </summary>
        /// <typeparam name="T">The type to create a logger for</typeparam>
        /// <returns>A mock logger</returns>
        protected Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }
        
        /// <summary>
        /// Disposes of resources used by the test
        /// </summary>
        public virtual void Dispose()
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}