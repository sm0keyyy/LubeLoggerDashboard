using System;
using Microsoft.Extensions.DependencyInjection;

namespace LubeLoggerDashboard.Services.DependencyInjection
{
    /// <summary>
    /// Service locator for accessing services from the dependency injection container
    /// </summary>
    public static class ServiceLocator
    {
        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes the service locator with the specified service provider
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Gets a service of the specified type
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>The service instance</returns>
        public static T GetService<T>() where T : class
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("Service locator has not been initialized");
            }

            return _serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Tries to get a service of the specified type
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <param name="service">The service instance, or null if not found</param>
        /// <returns>True if the service was found, false otherwise</returns>
        public static bool TryGetService<T>(out T service) where T : class
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("Service locator has not been initialized");
            }

            service = _serviceProvider.GetService<T>();
            return service != null;
        }
    }
}