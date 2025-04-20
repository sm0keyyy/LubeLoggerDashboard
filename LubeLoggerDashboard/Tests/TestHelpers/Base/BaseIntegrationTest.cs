using System;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LubeLoggerDashboard.Models.Database.Context;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Services.Cache;
using LubeLoggerDashboard.Services.Sync;
using LubeLoggerDashboard.Tests.TestHelpers.Configuration;
using LubeLoggerDashboard.Tests.TestHelpers.MockFactories;

namespace LubeLoggerDashboard.Tests.TestHelpers.Base
{
    /// <summary>
    /// Base class for integration tests providing common setup and utilities
    /// </summary>
    public abstract class BaseIntegrationTest : IDisposable
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
        /// The database context for the test
        /// </summary>
        protected LubeLoggerDbContext DbContext { get; private set; }
        
        /// <summary>
        /// The HTTP client for API requests
        /// </summary>
        protected HttpClient HttpClient { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseIntegrationTest"/> class
        /// </summary>
        protected BaseIntegrationTest()
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
            
            // Set up in-memory database
            Services.AddDbContext<LubeLoggerDbContext>(options =>
            {
                options.UseInMemoryDatabase($"LubeLoggerTestDb_{Guid.NewGuid()}");
            });
            
            // Set up HTTP client
            var mockHandler = ApiClientMockFactory.CreateMockHttpMessageHandler(
                System.Net.HttpStatusCode.OK, "Test response");
            HttpClient = new HttpClient(mockHandler.Object);
            Services.AddSingleton(HttpClient);
            
            // Add API client
            Services.AddSingleton<IApiClient>(provider =>
            {
                var options = new ApiClientOptions
                {
                    BaseUrl = Configuration["ApiClient:BaseUrl"],
                    ApiVersion = Configuration["ApiClient:ApiVersion"],
                    TimeoutSeconds = int.Parse(Configuration["ApiClient:TimeoutSeconds"]),
                    MaxRetries = int.Parse(Configuration["ApiClient:MaxRetries"]),
                    BaseRetryDelayMs = int.Parse(Configuration["ApiClient:BaseRetryDelayMs"]),
                    EnableThrottling = bool.Parse(Configuration["ApiClient:EnableThrottling"]),
                    EnableCircuitBreaker = bool.Parse(Configuration["ApiClient:EnableCircuitBreaker"]),
                    CircuitBreakerFailureThreshold = int.Parse(Configuration["ApiClient:CircuitBreakerFailureThreshold"]),
                    CircuitBreakerResetTimeoutMinutes = int.Parse(Configuration["ApiClient:CircuitBreakerResetTimeoutMinutes"])
                };
                
                var apiClient = new ApiClient(options);
                return apiClient;
            });
            
            // Add authentication service
            Services.AddSingleton<IAuthenticationService, BasicAuthenticationService>();
            
            // Add cache service
            Services.AddSingleton<ICacheService, CacheService>();
            
            // Add sync service
            Services.AddSingleton<ISyncService, SyncService>();
            
            // Build the service provider
            ServiceProvider = Services.BuildServiceProvider();
            
            // Initialize the database context
            DbContext = ServiceProvider.GetRequiredService<LubeLoggerDbContext>();
            DbContext.Database.EnsureCreated();
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
        /// Disposes of resources used by the test
        /// </summary>
        public virtual void Dispose()
        {
            DbContext?.Dispose();
            HttpClient?.Dispose();
            
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
