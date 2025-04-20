using LubeLoggerDashboard.Services.Navigation;

namespace LubeLoggerDashboard.ViewModels
{
    /// <summary>
    /// Base class for view models that need to be notified of navigation events
    /// </summary>
    public abstract class NavigationAwareViewModel : ViewModelBase, INavigationAware
    {
        /// <summary>
        /// Called when navigating to this view
        /// </summary>
        /// <param name="parameter">The parameter passed to the view</param>
        public virtual void OnNavigatedTo(object parameter)
        {
            // Default implementation does nothing
            // Derived classes should override this method to handle navigation
        }

        /// <summary>
        /// Called when navigating away from this view
        /// </summary>
        /// <returns>True to allow navigation, false to cancel</returns>
        public virtual bool OnNavigatingFrom()
        {
            // Default implementation allows navigation
            // Derived classes can override this method to prevent navigation if needed
            return true;
        }
    }
}