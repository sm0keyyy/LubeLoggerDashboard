using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LubeLoggerDashboard.Services.Navigation
{
    /// <summary>
    /// Implementation of the view factory that creates view instances
    /// </summary>
    public class ViewFactory : IViewFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ViewFactory> _logger;
        private readonly Dictionary<string, Type> _viewTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewFactory"/> class
        /// </summary>
        /// <param name="serviceProvider">The service provider to use for resolving views</param>
        /// <param name="logger">The logger to use for logging</param>
        public ViewFactory(IServiceProvider serviceProvider, ILogger<ViewFactory> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _viewTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            
            RegisterViewsByConvention();
        }

        /// <summary>
        /// Creates a view by name
        /// </summary>
        /// <param name="viewName">The name of the view to create</param>
        /// <returns>The created view instance</returns>
        public object CreateView(string viewName)
        {
            try
            {
                _logger.LogInformation("Creating view: {ViewName}", viewName);
                
                // Check if the view is registered
                if (!IsViewRegistered(viewName))
                {
                    _logger.LogWarning("View {ViewName} is not registered", viewName);
                    return null;
                }
                
                Type viewType = _viewTypes[viewName];
                
                // Try to resolve from DI container first
                try
                {
                    var view = _serviceProvider.GetService(viewType);
                    if (view != null)
                    {
                        return view;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to resolve view {ViewName} from DI container, falling back to Activator.CreateInstance", viewName);
                }
                
                // Fall back to Activator.CreateInstance
                return Activator.CreateInstance(viewType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating view {ViewName}", viewName);
                return null;
            }
        }

        /// <summary>
        /// Registers a view with a name
        /// </summary>
        /// <param name="viewName">The name to register the view with</param>
        /// <param name="viewType">The type of the view</param>
        public void RegisterView(string viewName, Type viewType)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentException("View name cannot be null or whitespace", nameof(viewName));
            }
            
            if (viewType == null)
            {
                throw new ArgumentNullException(nameof(viewType));
            }
            
            _logger.LogInformation("Registering view {ViewName} with type {ViewType}", viewName, viewType.FullName);
            
            _viewTypes[viewName] = viewType;
        }

        /// <summary>
        /// Checks if a view is registered
        /// </summary>
        /// <param name="viewName">The name of the view to check</param>
        /// <returns>True if the view is registered, false otherwise</returns>
        public bool IsViewRegistered(string viewName)
        {
            return _viewTypes.ContainsKey(viewName);
        }

        /// <summary>
        /// Gets all registered view names
        /// </summary>
        /// <returns>A collection of registered view names</returns>
        public IEnumerable<string> GetRegisteredViewNames()
        {
            return _viewTypes.Keys;
        }

        /// <summary>
        /// Registers views by convention
        /// </summary>
        private void RegisterViewsByConvention()
        {
            try
            {
                _logger.LogInformation("Registering views by convention");
                
                // Get all types in the assembly
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type[] types = assembly.GetTypes();
                
                // Filter for view types
                // Convention: Classes in the Views namespace that end with "View"
                var viewTypes = types.Where(t => 
                    t.Namespace != null && 
                    t.Namespace.Contains("LubeLoggerDashboard.Views") && 
                    t.Name.EndsWith("View") &&
                    !t.IsAbstract &&
                    t.IsClass);
                
                foreach (Type viewType in viewTypes)
                {
                    string viewName = viewType.Name;
                    RegisterView(viewName, viewType);
                }
                
                _logger.LogInformation("Registered {Count} views by convention", _viewTypes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering views by convention");
            }
        }
    }
}