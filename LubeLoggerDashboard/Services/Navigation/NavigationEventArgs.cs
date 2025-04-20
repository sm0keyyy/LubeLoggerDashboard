using System;

namespace LubeLoggerDashboard.Services.Navigation
{
    /// <summary>
    /// Event arguments for navigation events
    /// </summary>
    public class NavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the view that was navigated to
        /// </summary>
        public string ViewName { get; }
        
        /// <summary>
        /// Gets the parameter that was passed to the view
        /// </summary>
        public object Parameter { get; }
        
        /// <summary>
        /// Gets whether this was a back navigation
        /// </summary>
        public bool IsBackNavigation { get; }
        
        /// <summary>
        /// Gets whether this was a deep link navigation
        /// </summary>
        public bool IsDeepLink { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationEventArgs"/> class
        /// </summary>
        /// <param name="viewName">The name of the view that was navigated to</param>
        /// <param name="parameter">The parameter that was passed to the view</param>
        /// <param name="isBackNavigation">Whether this was a back navigation</param>
        /// <param name="isDeepLink">Whether this was a deep link navigation</param>
        public NavigationEventArgs(string viewName, object parameter, bool isBackNavigation, bool isDeepLink)
        {
            ViewName = viewName;
            Parameter = parameter;
            IsBackNavigation = isBackNavigation;
            IsDeepLink = isDeepLink;
        }
    }
}