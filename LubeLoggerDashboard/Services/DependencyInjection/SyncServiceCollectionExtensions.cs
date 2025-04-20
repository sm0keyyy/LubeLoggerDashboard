using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Services.Sync;

namespace LubeLoggerDashboard.Services.DependencyInjection
{
    /// <summary>
    /// Extension methods for registering sync services with the dependency injection container.
    /// </summary>
    public static class SyncServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the sync service to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddSyncServices(this IServiceCollection services)
        {
            // Register the sync service as a singleton
            services.AddSingleton<ISyncService, SyncService>();
            
            return services;
        }
    }
}