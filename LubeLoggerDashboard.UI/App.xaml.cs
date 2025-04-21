using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Core.Services.Authentication;
using LubeLoggerDashboard.Core.Services.Logging;
using LubeLoggerDashboard.Core.Helpers.Security;
using LubeLoggerDashboard.Infrastructure.Services.Api;
using LubeLoggerDashboard.Infrastructure.Services.Authentication;
using LubeLoggerDashboard.Infrastructure.Helpers.Security;
using LubeLoggerDashboard.Infrastructure.Services.Logging;
using LubeLoggerDashboard.Infrastructure.Services.Navigation;
using LubeLoggerDashboard.Infrastructure.Services.Configuration;
using LubeLoggerDashboard.Infrastructure.Services.DependencyInjection;
using LubeLoggerDashboard.UI.Views;
using Serilog;

namespace LubeLoggerDashboard.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ServiceProvider ServiceProvider { get; private set; }
        private ILoggingService _logger;
        private INavigationService _navigationService;

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
            
            // Register navigation services
            services.AddSingleton<IViewFactory, ViewFactory>();
            
            // Register views
            services.AddTransient<WelcomeView>();
            services.AddTransient<DashboardView>();
            services.AddTransient<VehiclesView>();
            services.AddTransient<MaintenanceView>();
            services.AddTransient<ReportsView>();
            services.AddTransient<SettingsView>();
        }

        /// <summary>
        /// Registers the navigation service with the DI container
        /// </summary>
        /// <param name="navigationService">The navigation service to register</param>
        public void RegisterNavigationService(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
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
                // Save navigation history
                _navigationService?.SaveNavigationHistory();
                
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
