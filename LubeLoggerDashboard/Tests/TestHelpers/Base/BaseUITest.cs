using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LubeLoggerDashboard.Services.Navigation;
using LubeLoggerDashboard.Tests.TestHelpers.Configuration;
using LubeLoggerDashboard.Tests.TestHelpers.MockFactories;
using LubeLoggerDashboard.Tests.TestHelpers.UI;

namespace LubeLoggerDashboard.Tests.TestHelpers.Base
{
    /// <summary>
    /// Base class for UI tests providing common setup and utilities
    /// </summary>
    public abstract class BaseUITest : IDisposable
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
        /// The navigation frame for the test
        /// </summary>
        protected Frame NavigationFrame { get; private set; }
        
        /// <summary>
        /// The navigation service for the test
        /// </summary>
        protected INavigationService NavigationService { get; private set; }
        
        /// <summary>
        /// The view factory for the test
        /// </summary>
        protected IViewFactory ViewFactory { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseUITest"/> class
        /// </summary>
        protected BaseUITest()
        {
            // Ensure we're running on the UI thread
            EnsureUIThread();
            
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
            
            // Set up navigation frame
            NavigationFrame = WpfTestHelper.RunOnUIThread(() => new Frame());
            Services.AddSingleton(NavigationFrame);
            
            // Add view factory
            Services.AddSingleton<IViewFactory, ViewFactory>();
            
            // Add navigation service
            Services.AddSingleton<INavigationService, NavigationService>();
            
            // Add API client and other services from mock factories
            Services.AddSingleton(ApiClientMockFactory.CreateMockApiClient().Object);
            Services.AddSingleton(AuthenticationServiceMockFactory.CreateMockAuthenticationService().Object);
            
            // Build the service provider
            ServiceProvider = Services.BuildServiceProvider();
            
            // Initialize navigation services
            ViewFactory = ServiceProvider.GetRequiredService<IViewFactory>();
            NavigationService = ServiceProvider.GetRequiredService<INavigationService>();
        }
        
        /// <summary>
        /// Ensures that the test is running on a UI thread
        /// </summary>
        private void EnsureUIThread()
        {
            if (Application.Current == null)
            {
                // Create a new application if one doesn't exist
                new Application();
            }
        }
        
        /// <summary>
        /// Registers a view with the view factory
        /// </summary>
        /// <typeparam name="TView">The type of view to register</typeparam>
        /// <param name="viewName">The name of the view</param>
        protected void RegisterView<TView>(string viewName) where TView : UserControl
        {
            WpfTestHelper.RunOnUIThread(() => 
            {
                ((ViewFactory)ViewFactory).RegisterView(viewName, typeof(TView));
            });
        }
        
        /// <summary>
        /// Navigates to a view and waits for the navigation to complete
        /// </summary>
        /// <param name="viewName">The name of the view to navigate to</param>
        /// <returns>True if navigation was successful, false otherwise</returns>
        protected bool NavigateToView(string viewName)
        {
            return WpfTestHelper.RunOnUIThread(() => NavigationService.NavigateTo(viewName));
        }
        
        /// <summary>
        /// Waits for a condition to be true with a timeout
        /// </summary>
        /// <param name="condition">The condition to wait for</param>
        /// <param name="timeout">The maximum time to wait</param>
        /// <param name="interval">The interval between condition checks</param>
        /// <returns>True if the condition was met within the timeout, false otherwise</returns>
        protected bool WaitForCondition(Func<bool> condition, TimeSpan timeout, TimeSpan? interval = null)
        {
            return WpfTestHelper.WaitForCondition(condition, timeout, interval);
        }
        
        /// <summary>
        /// Waits for a condition to be true with a timeout (async version)
        /// </summary>
        /// <param name="condition">The condition to wait for</param>
        /// <param name="timeout">The maximum time to wait</param>
        /// <param name="interval">The interval between condition checks</param>
        /// <returns>True if the condition was met within the timeout, false otherwise</returns>
        protected Task<bool> WaitForConditionAsync(Func<bool> condition, TimeSpan timeout, TimeSpan? interval = null)
        {
            return WpfTestHelper.WaitForConditionAsync(condition, timeout, interval);
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
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}