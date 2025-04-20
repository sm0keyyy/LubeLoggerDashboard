using System;
using System.Collections.Generic;

namespace LubeLoggerDashboard.Services.Navigation
{
    /// <summary>
    /// Interface for the view factory that creates view instances
    /// </summary>
    public interface IViewFactory
    {
        /// <summary>
        /// Creates a view by name
        /// </summary>
        /// <param name="viewName">The name of the view to create</param>
        /// <returns>The created view instance</returns>
        object CreateView(string viewName);
        
        /// <summary>
        /// Registers a view with a name
        /// </summary>
        /// <param name="viewName">The name to register the view with</param>
        /// <param name="viewType">The type of the view</param>
        void RegisterView(string viewName, Type viewType);
        
        /// <summary>
        /// Checks if a view is registered
        /// </summary>
        /// <param name="viewName">The name of the view to check</param>
        /// <returns>True if the view is registered, false otherwise</returns>
        bool IsViewRegistered(string viewName);
        
        /// <summary>
        /// Gets all registered view names
        /// </summary>
        /// <returns>A collection of registered view names</returns>
        IEnumerable<string> GetRegisteredViewNames();
    }
}