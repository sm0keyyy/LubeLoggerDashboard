using System;

namespace LubeLoggerDashboard.Services.Navigation
{
    /// <summary>
    /// Represents an entry in the navigation history
    /// </summary>
    public class NavigationHistoryEntry
    {
        /// <summary>
        /// Gets or sets the name of the view
        /// </summary>
        public string ViewName { get; set; }
        
        /// <summary>
        /// Gets or sets the parameter passed to the view (must be serializable)
        /// </summary>
        public object Parameter { get; set; }
        
        /// <summary>
        /// Gets or sets the timestamp of the navigation
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationHistoryEntry"/> class
        /// </summary>
        public NavigationHistoryEntry()
        {
            Timestamp = DateTime.Now;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationHistoryEntry"/> class
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        /// <param name="parameter">The parameter passed to the view</param>
        public NavigationHistoryEntry(string viewName, object parameter)
        {
            ViewName = viewName;
            Parameter = parameter;
            Timestamp = DateTime.Now;
        }
    }
}