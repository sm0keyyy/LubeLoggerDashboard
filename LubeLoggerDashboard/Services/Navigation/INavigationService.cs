using System;
using System.Collections.Generic;

namespace LubeLoggerDashboard.Services.Navigation
{
    /// <summary>
    /// Interface for the navigation service that manages navigation between views
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigate to a view by name
        /// </summary>
        /// <param name="viewName">The name of the view to navigate to</param>
        /// <returns>True if navigation was successful, false otherwise</returns>
        bool NavigateTo(string viewName);
        
        /// <summary>
        /// Navigate to a view with parameters
        /// </summary>
        /// <param name="viewName">The name of the view to navigate to</param>
        /// <param name="parameter">The parameter to pass to the view</param>
        /// <returns>True if navigation was successful, false otherwise</returns>
        bool NavigateTo(string viewName, object parameter);
        
        /// <summary>
        /// Navigate back to the previous view
        /// </summary>
        /// <returns>True if navigation was successful, false otherwise</returns>
        bool NavigateBack();
        
        /// <summary>
        /// Navigate forward to the next view (if available)
        /// </summary>
        /// <returns>True if navigation was successful, false otherwise</returns>
        bool NavigateForward();
        
        /// <summary>
        /// Gets the current view name
        /// </summary>
        string CurrentView { get; }
        
        /// <summary>
        /// Checks if navigation back is possible
        /// </summary>
        bool CanNavigateBack { get; }
        
        /// <summary>
        /// Checks if navigation forward is possible
        /// </summary>
        bool CanNavigateForward { get; }
        
        /// <summary>
        /// Gets the navigation history
        /// </summary>
        IReadOnlyList<NavigationHistoryEntry> NavigationHistory { get; }
        
        /// <summary>
        /// Clears the navigation history
        /// </summary>
        void ClearHistory();
        
        /// <summary>
        /// Saves the navigation history
        /// </summary>
        void SaveNavigationHistory();
        
        /// <summary>
        /// Loads the navigation history
        /// </summary>
        void LoadNavigationHistory();
        
        /// <summary>
        /// Parses a deep link URI and navigates accordingly
        /// </summary>
        /// <param name="uri">The URI to parse</param>
        /// <returns>True if navigation was successful, false otherwise</returns>
        bool HandleDeepLink(Uri uri);
        
        /// <summary>
        /// Event raised when navigation occurs
        /// </summary>
        event EventHandler<NavigationEventArgs> Navigated;
    }
}