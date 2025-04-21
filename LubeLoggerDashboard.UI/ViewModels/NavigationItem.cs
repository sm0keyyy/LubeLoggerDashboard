using System;
using System.Windows.Input;

namespace LubeLoggerDashboard.UI.ViewModels
{
    /// <summary>
    /// Represents an item in the navigation menu
    /// </summary>
    public class NavigationItem : ViewModelBase
    {
        private string _name;
        private string _icon;
        private string _viewName;
        private bool _isSelected;
        private bool _isEnabled;
        private ICommand _command;

        /// <summary>
        /// Gets or sets the display name of the navigation item
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Gets or sets the Material Design icon name for the navigation item
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        /// <summary>
        /// Gets or sets the name of the view to navigate to
        /// </summary>
        public string ViewName
        {
            get => _viewName;
            set => SetProperty(ref _viewName, value);
        }

        /// <summary>
        /// Gets or sets whether this navigation item is currently selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        /// <summary>
        /// Gets or sets whether this navigation item is enabled
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        /// <summary>
        /// Gets or sets the command to execute when this navigation item is selected
        /// </summary>
        public ICommand Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class
        /// </summary>
        public NavigationItem()
        {
            IsEnabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class with the specified properties
        /// </summary>
        /// <param name="name">The display name</param>
        /// <param name="icon">The Material Design icon name</param>
        /// <param name="viewName">The name of the view to navigate to</param>
        /// <param name="command">The command to execute when selected</param>
        public NavigationItem(string name, string icon, string viewName, ICommand command)
        {
            Name = name;
            Icon = icon;
            ViewName = viewName;
            Command = command;
            IsEnabled = true;
        }
    }
}