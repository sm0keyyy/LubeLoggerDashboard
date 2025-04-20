using System;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Services.Api.Interfaces;
using LubeLoggerDashboard.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace LubeLoggerDashboard.Services.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring services in the dependency injection container
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all application services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Register security services
            services.AddSingleton<ICredentialManager, CredentialManager>();

            // Register API client
            services.AddSingleton<IApiClient, ApiClient>();

            // Register API services
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IVehicleService, VehicleService>();

            // Register authentication services
            services.AddSingleton<AuthenticationServiceFactory>();
            services.AddSingleton<IAuthenticationService>(provider => 
                provider.GetRequiredService<AuthenticationServiceFactory>().CreateAuthenticationService());

            return services;
        }

        /// <summary>
        /// Adds all API services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Register API services
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IVehicleService, VehicleService>();
            
            // TODO: Register other API services as they are implemented
            // services.AddSingleton<IOdometerRecordService, OdometerRecordService>();
            // services.AddSingleton<IPlanRecordService, PlanRecordService>();
            // services.AddSingleton<IMaintenanceRecordService, MaintenanceRecordService>();
            // services.AddSingleton<IServiceRecordService, ServiceRecordService>();
            // services.AddSingleton<IRepairRecordService, RepairRecordService>();
            // services.AddSingleton<IUpgradeRecordService, UpgradeRecordService>();

            return services;
        }
    }
}