using System;
using System.IO;
using System.Text.Json;
using LubeLoggerDashboard.Services.Logging;

namespace LubeLoggerDashboard.Services.Configuration
{
    /// <summary>
    /// Service for loading application configuration
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private const string DefaultConfigPath = "appsettings.json";
        private readonly string _configPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService"/> class.
        /// </summary>
        /// <param name="configPath">The path to the configuration file</param>
        public ConfigurationService(string configPath = DefaultConfigPath)
        {
            _configPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
        }

        /// <inheritdoc />
        public LoggingConfiguration GetLoggingConfiguration()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    return new LoggingConfiguration();
                }

                var json = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return config?.Logging ?? new LoggingConfiguration();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading logging configuration: {ex.Message}");
                return new LoggingConfiguration();
            }
        }

        /// <summary>
        /// Gets the API configuration
        /// </summary>
        /// <returns>The API configuration</returns>
        public ApiConfiguration GetApiConfiguration()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    return new ApiConfiguration();
                }

                var json = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return config?.Api ?? new ApiConfiguration();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading API configuration: {ex.Message}");
                return new ApiConfiguration();
            }
        }

        /// <summary>
        /// Application settings class
        /// </summary>
        private class AppSettings
        {
            /// <summary>
            /// Gets or sets the logging configuration
            /// </summary>
            public LoggingConfiguration Logging { get; set; }

            /// <summary>
            /// Gets or sets the API configuration
            /// </summary>
            public ApiConfiguration Api { get; set; }
        }
    }

    /// <summary>
    /// API configuration class
    /// </summary>
    public class ApiConfiguration
    {
        /// <summary>
        /// Gets or sets the base URL for the API
        /// </summary>
        public string BaseUrl { get; set; } = "https://demo.lubelogger.com/api";

        /// <summary>
        /// Gets or sets the timeout in seconds
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// Gets or sets the number of retry attempts
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the delay between retry attempts in milliseconds
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 1000;
    }
}