using LubeLoggerDashboard.Infrastructure.Services.Navigation;

namespace LubeLoggerDashboard.UI.ViewModels
{
    /// <summary>
    /// Base class for view models that need to be aware of navigation events
    /// </summary>
    public abstract class NavigationAwareViewModel : ViewModelBase, INavigationAware
    {
        /// <summary>
        /// Called when navigating to this view model
        /// </summary>
        /// <param name="parameter">The navigation parameter</param>
        public virtual void OnNavigatedTo(object parameter)
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Called when navigating away from this view model
        /// </summary>
        public virtual void OnNavigatedFrom()
        {
            // Default implementation does nothing
        }
    }
}