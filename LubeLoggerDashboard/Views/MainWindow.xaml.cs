using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Helpers.Security;
using Serilog;

namespace LubeLoggerDashboard.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAuthenticationService _authService;
        private readonly ICredentialManager _credentialManager;

        public MainWindow()
        {
            InitializeComponent();

            // Get services from DI container
            var serviceProvider = ((App)Application.Current).ServiceProvider;
            _authService = serviceProvider.GetRequiredService<IAuthenticationService>();
            _credentialManager = serviceProvider.GetRequiredService<ICredentialManager>();

            // Try to load saved credentials
            TryLoadSavedCredentials();
        }

        private void TryLoadSavedCredentials()
        {
            try
            {
                var credentials = _credentialManager.GetCredentials();
                if (credentials != null)
                {
                    UsernameTextBox.Text = credentials.Username;
                    // We don't set the password in the UI for security reasons
                    // but we could indicate that a password is saved
                    LoginStatusText.Text = "Saved credentials found. Enter password to login.";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load saved credentials");
                LoginStatusText.Text = "Could not load saved credentials.";
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginButton.IsEnabled = false;
                LoginStatusText.Text = "Logging in...";

                string username = UsernameTextBox.Text;
                string password = PasswordBox.Password;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    LoginStatusText.Text = "Username and password are required.";
                    return;
                }

                var result = await _authService.AuthenticateAsync(username, password);

                if (result.Success)
                {
                    // Save credentials securely
                    _credentialManager.SaveCredentials(username, password);
                    
                    LoginStatusText.Text = "Login successful!";
                    
                    // In a real application, we would navigate to the main dashboard
                    // or enable the navigation menu items
                    MessageBox.Show("Login successful! The full application would now load the dashboard.", 
                        "Authentication Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    LoginStatusText.Text = $"Login failed: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Login attempt failed");
                LoginStatusText.Text = $"An error occurred: {ex.Message}";
            }
            finally
            {
                LoginButton.IsEnabled = true;
            }
        }
    }
}