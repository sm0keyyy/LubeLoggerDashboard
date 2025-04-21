namespace LubeLoggerDashboard.Core.Helpers.Security
{
    /// <summary>
    /// Interface for managing user credentials
    /// </summary>
    public interface ICredentialManager
    {
        /// <summary>
        /// Saves user credentials securely
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        void SaveCredentials(string username, string password);
        
        /// <summary>
        /// Gets the saved credentials
        /// </summary>
        /// <returns>The saved credentials, or null if no credentials are saved</returns>
        Credentials GetCredentials();
        
        /// <summary>
        /// Deletes the saved credentials
        /// </summary>
        void DeleteCredentials();
    }
    
    /// <summary>
    /// Represents user credentials
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// The username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; }
    }
}