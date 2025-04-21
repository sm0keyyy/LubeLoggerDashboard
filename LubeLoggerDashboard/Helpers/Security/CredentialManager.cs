using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LubeLoggerDashboard.Services.Logging; // Added for ILoggingService

namespace LubeLoggerDashboard.Helpers.Security
{
    /// <summary>
    /// Implementation of ICredentialManager that uses Windows Data Protection API (DPAPI)
    /// </summary>
    public class CredentialManager : ICredentialManager
    {
        private const string CredentialsFileName = "credentials.dat";
        private readonly string _credentialsFilePath;
        private readonly ILoggingService _logger; // Added logger field

        /// <summary>
        /// Initializes a new instance of the CredentialManager class
        /// </summary>
        /// <param name="logger">The logging service.</param> // Added logger parameter doc
        public CredentialManager(ILoggingService logger) // Added logger parameter
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Assign logger

            // Store credentials in the application directory
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LubeLoggerDashboard");
            
            // Create the directory if it doesn't exist
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            _credentialsFilePath = Path.Combine(appDataPath, CredentialsFileName);
        }

        /// <inheritdoc/>
        public void SaveCredentials(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("Username and password cannot be empty");
                }

                var credentials = new Credentials
                {
                    Username = username,
                    Password = password
                };

                // Serialize the credentials to JSON
                var json = JsonSerializer.Serialize(credentials);
                var jsonBytes = Encoding.UTF8.GetBytes(json);

                // Encrypt the JSON using DPAPI
                var encryptedBytes = ProtectedData.Protect(
                    jsonBytes,
                    null, // Optional entropy
                    DataProtectionScope.CurrentUser);

                // Write the encrypted data to the file
                File.WriteAllBytes(_credentialsFilePath, encryptedBytes);

                _logger.Information("Credentials saved successfully for user {Username}", username); // Use injected logger
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save credentials"); // Use injected logger
                throw;
            }
        }

        /// <inheritdoc/>
        public Credentials GetCredentials()
        {
            try
            {
                if (!File.Exists(_credentialsFilePath))
                {
                    _logger.Information("No saved credentials found"); // Use injected logger
                    return null;
                }

                // Read the encrypted data from the file
                var encryptedBytes = File.ReadAllBytes(_credentialsFilePath);

                // Decrypt the data using DPAPI
                var jsonBytes = ProtectedData.Unprotect(
                    encryptedBytes,
                    null, // Optional entropy
                    DataProtectionScope.CurrentUser);

                // Deserialize the JSON to a Credentials object
                var json = Encoding.UTF8.GetString(jsonBytes);
                var credentials = JsonSerializer.Deserialize<Credentials>(json);

                _logger.Information("Credentials retrieved successfully for user {Username}", credentials.Username); // Use injected logger
                return credentials;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve credentials"); // Use injected logger
                return null;
            }
        }

        /// <inheritdoc/>
        public void DeleteCredentials()
        {
            try
            {
                if (File.Exists(_credentialsFilePath))
                {
                    File.Delete(_credentialsFilePath);
                    _logger.Information("Credentials deleted successfully"); // Use injected logger
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete credentials"); // Use injected logger
                throw;
            }
        }
    }
}