using System;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Models.Database.Context;
using LubeLoggerDashboard.Models.Database.Initialization;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Services.Api.Interfaces;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Services.Cache;
using LubeLoggerDashboard.Services.Configuration;
using LubeLoggerDashboard.Services.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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

            // Register configuration services
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            // Register logging services
            services.AddLoggingServices();

            // Register database services
            services.AddDbContext<LubeLoggerDbContext>();
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            
            // Register cache services
            services.AddScoped<ICacheService, CacheService>();

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
        /// Adds logging services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Get logging configuration
            var configService = new ConfigurationService();
            var loggingConfig = configService.GetLoggingConfiguration();

            // Create Serilog logger
            var logger = LoggerFactory.CreateLogger(
                logLevel: loggingConfig.MinimumLevel,
                enableConsole: loggingConfig.EnableConsole,
                logFilePath: loggingConfig.LogFilePath,
                rollingInterval: loggingConfig.RollingInterval,
                retainedFileCountLimit: loggingConfig.RetainedFileCountLimit);

            // Register Serilog logger
            services.AddSingleton(logger);

            // Register logging service
            services.AddSingleton<ILoggingService, LoggingService>();

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