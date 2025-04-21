using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Core.Services.Authentication;
using LubeLoggerDashboard.Core.Helpers.Security;
using LubeLoggerDashboard.Infrastructure.Services.Navigation;
using LubeLoggerDashboard.UI.ViewModels;
using Serilog;

namespace LubeLoggerDashboard.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INavigationService _navigationService;
        private readonly ShellViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // Get services from DI container
            _serviceProvider = ((App)Application.Current).ServiceProvider;
            
            // Register the content frame with the navigation service
            var viewFactory = _serviceProvider.GetRequiredService<IViewFactory>();
            _navigationService = new NavigationService(
                viewFactory,
                ContentFrame,
                _serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<NavigationService>>());
            
            // Register the navigation service with the DI container
            ((App)Application.Current).RegisterNavigationService(_navigationService);
            
            // Create the view model
            _viewModel = new ShellViewModel(
                _serviceProvider.GetRequiredService<IAuthenticationService>(),
                _serviceProvider.GetRequiredService<ICredentialManager>(),
                _navigationService);
            
            // Set the data context
            DataContext = _viewModel;
            
            // Handle password changes
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            
            // Load the welcome view
            _navigationService.NavigateTo("WelcomeView");
        }

        /// <summary>
        /// Handles the PasswordChanged event of the PasswordBox control
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                // Update the password in the view model
                _viewModel.Password = PasswordBox.Password;
            }
        }

        /// <summary>
        /// Handles the Loaded event of the Window
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if already authenticated
                if (_viewModel.IsLoggedIn)
                {
                    // Navigate to the dashboard
                    _navigationService.NavigateTo("DashboardView");
                }
                else
                {
                    // Try to load saved credentials
                    var credentials = _serviceProvider.GetRequiredService<ICredentialManager>().GetCredentials();
                    if (credentials != null)
                    {
                        _viewModel.Username = credentials.Username;
                        _viewModel.LoginStatusMessage = "Saved credentials found. Enter password to login.";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during window load");
                _viewModel.LoginStatusMessage = "Error loading application.";
            }
        }
    }
}