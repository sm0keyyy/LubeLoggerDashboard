using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Services.DependencyInjection;
using LubeLoggerDashboard.Services.Logging;
using LubeLoggerDashboard.Services.Configuration;
using Serilog;

namespace LubeLoggerDashboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly ServiceProvider ServiceProvider;
        private ILoggingService _logger;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Get the logging service
            _logger = ServiceProvider.GetRequiredService<ILoggingService>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register all application services
            services.AddApplicationServices();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Initialize the service locator
                ServiceLocator.Initialize(ServiceProvider);
                
                _logger.Information("Application starting up");
            }
            catch (Exception ex)
            {
                // If logging service fails, fall back to static Log
                Log.Fatal(ex, "Application failed to start");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                _logger.Information("Application shutting down");
            }
            catch (Exception)
            {
                // If logging service fails, fall back to static Log
                Log.Information("Application shutting down");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
            base.OnExit(e);
        }
    }
}