using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Models.Database.Initialization;
using LubeLoggerDashboard.Services.DependencyInjection;
using LubeLoggerDashboard.Services.Logging;
using LubeLoggerDashboard.Services.Configuration;
using LubeLoggerDashboard.Services.Navigation;
using LubeLoggerDashboard.Views;
using Serilog;

namespace LubeLoggerDashboard
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

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Initialize the service locator
                ServiceLocator.Initialize(ServiceProvider);
                
                _logger.Information("Application starting up");
                
                // Initialize database
                await InitializeDatabaseAsync();
            }
            catch (Exception ex)
            {
                // If logging service fails, fall back to static Log
                Log.Fatal(ex, "Application failed to start");
            }
        }
        
        /// <summary>
        /// Initializes the database by ensuring it exists and applying all migrations.
        /// </summary>
        private async Task InitializeDatabaseAsync()
        {
            try
            {
                _logger.Information("Initializing database...");
                var databaseInitializer = ServiceProvider.GetRequiredService<IDatabaseInitializer>();
                await databaseInitializer.InitializeAsync();
                _logger.Information("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while initializing the database.");
                // We don't rethrow here to allow the application to continue even if database initialization fails
                // The application should handle database unavailability gracefully
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