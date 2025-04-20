namespace LubeLoggerDashboard.Services.Navigation
{
    /// <summary>
    /// Interface for view models that need to be notified of navigation events
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called when navigating to this view
        /// </summary>
        /// <param name="parameter">The parameter passed to the view</param>
        void OnNavigatedTo(object parameter);
        
        /// <summary>
        /// Called when navigating away from this view
        /// </summary>
        /// <returns>True to allow navigation, false to cancel</returns>
        bool OnNavigatingFrom();
    }
}