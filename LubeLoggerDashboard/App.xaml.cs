using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Helpers.Security;
using Serilog;

namespace LubeLoggerDashboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly ServiceProvider ServiceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Configure logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/lubelogger.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register services
            services.AddSingleton<ICredentialManager, CredentialManager>();
            services.AddSingleton<IAuthenticationService, BasicAuthenticationService>();
            services.AddSingleton<IApiClient, ApiClient>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Create logs directory if it doesn't exist
                System.IO.Directory.CreateDirectory("logs");
                
                Log.Information("Application starting up");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Application shutting down");
            Log.CloseAndFlush();
            
            base.OnExit(e);
        }
    }
}