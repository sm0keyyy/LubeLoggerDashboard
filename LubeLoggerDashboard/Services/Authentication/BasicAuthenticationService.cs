using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LubeLoggerDashboard.Helpers.Security;
using LubeLoggerDashboard.Services.Api;
using Serilog;

namespace LubeLoggerDashboard.Services.Authentication
{
    /// <summary>
    /// Implementation of IAuthenticationService that uses Basic Authentication
    /// </summary>
    public class BasicAuthenticationService : IAuthenticationService
    {
        private readonly IApiClient _apiClient;
        private readonly ICredentialManager _credentialManager;
        private string _username;
        private string _password;
        private bool _isAuthenticated;

        /// <summary>
        /// Initializes a new instance of the BasicAuthenticationService class
        /// </summary>
        /// <param name="apiClient">The API client</param>
        /// <param name="credentialManager">The credential manager</param>
        public BasicAuthenticationService(IApiClient apiClient, ICredentialManager credentialManager)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return AuthenticationResult.Failed("Username and password are required");
                }

                // Set the credentials for the API client
                _apiClient.SetAuthenticationHeader(GetBasicAuthHeader(username, password));

                // Test the credentials by making a request to the whoami endpoint
                var response = await _apiClient.GetAsync("/api/whoami");
                
                if (response.IsSuccessStatusCode)
                {
                    // Authentication successful
                    _username = username;
                    _password = password;
                    _isAuthenticated = true;
                    
                    Log.Information("User {Username} authenticated successfully", username);
                    return AuthenticationResult.Successful();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Log.Warning("Authentication failed for user {Username}: Invalid credentials", username);
                    return AuthenticationResult.Failed("Invalid username or password");
                }
                else
                {
                    Log.Warning("Authentication failed for user {Username}: {StatusCode} - {ReasonPhrase}", 
                        username, response.StatusCode, response.ReasonPhrase);
                    return AuthenticationResult.Failed($"Server returned {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "Authentication failed due to HTTP request error");
                return AuthenticationResult.Failed($"Connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Authentication failed due to unexpected error");
                return AuthenticationResult.Failed($"Unexpected error: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public string GetAuthenticationHeader()
        {
            if (!_isAuthenticated)
            {
                return string.Empty;
            }

            return GetBasicAuthHeader(_username, _password);
        }

        /// <inheritdoc/>
        public bool IsAuthenticated()
        {
            return _isAuthenticated;
        }

        /// <inheritdoc/>
        public void Logout()
        {
            _username = null;
            _password = null;
            _isAuthenticated = false;
            _apiClient.ClearAuthenticationHeader();
            
            Log.Information("User logged out");
        }

        /// <summary>
        /// Creates a Basic Authentication header value
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>The Basic Authentication header value</returns>
        private string GetBasicAuthHeader(string username, string password)
        {
            var authString = $"{username}:{password}";
            var authBytes = Encoding.UTF8.GetBytes(authString);
            var base64Auth = Convert.ToBase64String(authBytes);
            return $"Basic {base64Auth}";
        }
    }
}