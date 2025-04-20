using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Services.Navigation;

namespace LubeLoggerDashboard.ViewModels
{
    /// <summary>
    /// ViewModel for the main shell window that contains navigation and authentication functionality
    /// </summary>
    public class ShellViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICredentialManager _credentialManager;
        private readonly INavigationService _navigationService;
        
        private bool _isLoggedIn;
        private string _username;
        private string _password;
        private string _loginStatusMessage;
        private NavigationItem _selectedNavigationItem;
        private ObservableCollection<NavigationItem> _navigationItems;
        
        /// <summary>
        /// Gets or sets whether the user is currently logged in
        /// </summary>
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (SetProperty(ref _isLoggedIn, value))
                {
                    // Update navigation items enabled state
                    foreach (var item in NavigationItems)
                    {
                        item.IsEnabled = value;
                    }
                    
                    // Raise can execute changed for commands
                    LogoutCommand.RaiseCanExecuteChanged();
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the username for login
        /// </summary>
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
        
        /// <summary>
        /// Gets or sets the password for login
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        /// <summary>
        /// Gets or sets the login status message
        /// </summary>
        public string LoginStatusMessage
        {
            get => _loginStatusMessage;
            set => SetProperty(ref _loginStatusMessage, value);
        }
        
        /// <summary>
        /// Gets or sets the currently selected navigation item
        /// </summary>
        public NavigationItem SelectedNavigationItem
        {
            get => _selectedNavigationItem;
            set
            {
                if (SetProperty(ref _selectedNavigationItem, value) && value != null)
                {
                    // Execute the navigation command
                    value.Command?.Execute(null);
                }
            }
        }
        
        /// <summary>
        /// Gets the collection of navigation items
        /// </summary>
        public ObservableCollection<NavigationItem> NavigationItems
        {
            get => _navigationItems;
            private set => SetProperty(ref _navigationItems, value);
        }
        
        /// <summary>
        /// Gets the command to execute when logging in
        /// </summary>
        public RelayCommand LoginCommand { get; }
        
        /// <summary>
        /// Gets the command to execute when logging out
        /// </summary>
        public RelayCommand LogoutCommand { get; }
        
        /// <summary>
        /// Gets the command to execute when navigating to a view
        /// </summary>
        public RelayCommand NavigateCommand { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class
        /// </summary>
        /// <param name="authenticationService">The authentication service</param>
        /// <param name="credentialManager">The credential manager</param>
        /// <param name="navigationService">The navigation service</param>
        public ShellViewModel(
            IAuthenticationService authenticationService,
            ICredentialManager credentialManager,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // Initialize commands
            LoginCommand = new RelayCommand(async param => await LoginAsync(), param => !IsLoggedIn);
            LogoutCommand = new RelayCommand(param => Logout(), param => IsLoggedIn);
            NavigateCommand = new RelayCommand(param => Navigate(param as string));
            
            // Initialize navigation items
            InitializeNavigationItems();
            
            // Subscribe to navigation events
            _navigationService.Navigated += OnNavigated;
            
            // Check if already authenticated
            IsLoggedIn = _authenticationService.IsAuthenticated();
            if (IsLoggedIn)
            {
                // Load saved credentials to get the username
                var credentials = _credentialManager.GetCredentials();
                if (credentials != null)
                {
                    Username = credentials.Username;
                    LoginStatusMessage = "Logged in";
                }
            }
        }
        
        /// <summary>
        /// Initializes the navigation items
        /// </summary>
        private void InitializeNavigationItems()
        {
            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem
                {
                    Name = "Dashboard",
                    Icon = "Home",
                    ViewName = "DashboardView",
                    Command = NavigateCommand,
                    IsEnabled = IsLoggedIn
                },
                new NavigationItem
                {
                    Name = "Vehicles",
                    Icon = "DirectionsCar",
                    ViewName = "VehiclesView",
                    Command = NavigateCommand,
                    IsEnabled = IsLoggedIn
                },
                new NavigationItem
                {
                    Name = "Maintenance",
                    Icon = "Build",
                    ViewName = "MaintenanceView",
                    Command = NavigateCommand,
                    IsEnabled = IsLoggedIn
                },
                new NavigationItem
                {
                    Name = "Reports",
                    Icon = "Assessment",
                    ViewName = "ReportsView",
                    Command = NavigateCommand,
                    IsEnabled = IsLoggedIn
                },
                new NavigationItem
                {
                    Name = "Settings",
                    Icon = "Settings",
                    ViewName = "SettingsView",
                    Command = NavigateCommand,
                    IsEnabled = IsLoggedIn
                }
            };
        }
        
        /// <summary>
        /// Handles the Navigated event
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Update the selected navigation item
            var item = NavigationItems.FirstOrDefault(i => i.ViewName == e.ViewName);
            if (item != null)
            {
                foreach (var navItem in NavigationItems)
                {
                    navItem.IsSelected = navItem == item;
                }
                
                // Set the selected item without triggering navigation again
                _selectedNavigationItem = item;
                OnPropertyChanged(nameof(SelectedNavigationItem));
            }
        }
        
        /// <summary>
        /// Navigates to the specified view
        /// </summary>
        /// <param name="viewName">The name of the view to navigate to</param>
        private void Navigate(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return;
            }
            
            _navigationService.NavigateTo(viewName);
        }
        
        /// <summary>
        /// Logs in with the provided credentials
        /// </summary>
        private async Task LoginAsync()
        {
            try
            {
                LoginStatusMessage = "Logging in...";
                
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    LoginStatusMessage = "Username and password are required.";
                    return;
                }
                
                var result = await _authenticationService.AuthenticateAsync(Username, Password);
                
                if (result.Success)
                {
                    // Save credentials securely
                    _credentialManager.SaveCredentials(Username, Password);
                    
                    IsLoggedIn = true;
                    LoginStatusMessage = "Login successful!";
                    
                    // Navigate to the dashboard
                    _navigationService.NavigateTo("DashboardView");
                }
                else
                {
                    LoginStatusMessage = $"Login failed: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                LoginStatusMessage = $"An error occurred: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Logs out the current user
        /// </summary>
        private void Logout()
        {
            try
            {
                _authenticationService.Logout();
                IsLoggedIn = false;
                Password = string.Empty;
                LoginStatusMessage = "Logged out";
                
                // Navigate to the welcome view
                _navigationService.NavigateTo("WelcomeView");
            }
            catch (Exception ex)
            {
                LoginStatusMessage = $"Logout error: {ex.Message}";
            }
        }
    }
}