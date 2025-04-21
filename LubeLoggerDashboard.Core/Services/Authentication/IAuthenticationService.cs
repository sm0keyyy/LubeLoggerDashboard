using System.Threading.Tasks;

namespace LubeLoggerDashboard.Core.Services.Authentication
{
    /// <summary>
    /// Interface for authentication services
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user with the provided credentials
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <returns>Authentication result containing success status and error message if applicable</returns>
        Task<AuthenticationResult> AuthenticateAsync(string username, string password);
        
        /// <summary>
        /// Gets the current authentication token or credentials
        /// </summary>
        /// <returns>The authentication token or credentials</returns>
        string GetAuthenticationHeader();
        
        /// <summary>
        /// Checks if the user is currently authenticated
        /// </summary>
        /// <returns>True if authenticated, false otherwise</returns>
        bool IsAuthenticated();
        
        /// <summary>
        /// Logs the user out
        /// </summary>
        void Logout();
    }
    
    /// <summary>
    /// Result of an authentication attempt
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Whether the authentication was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Error message if authentication failed
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// Creates a successful authentication result
        /// </summary>
        /// <returns>A successful authentication result</returns>
        public static AuthenticationResult Successful() => new AuthenticationResult { Success = true };
        
        /// <summary>
        /// Creates a failed authentication result with the specified error message
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        /// <returns>A failed authentication result</returns>
        public static AuthenticationResult Failed(string errorMessage) => new AuthenticationResult 
        { 
            Success = false, 
            ErrorMessage = errorMessage 
        };
    }
}