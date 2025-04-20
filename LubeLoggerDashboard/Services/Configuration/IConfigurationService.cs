using LubeLoggerDashboard.Services.Logging;

namespace LubeLoggerDashboard.Services.Configuration
{
    /// <summary>
    /// Interface for the configuration service
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets the logging configuration
        /// </summary>
        /// <returns>The logging configuration</returns>
        LoggingConfiguration GetLoggingConfiguration();

        /// <summary>
        /// Gets the API configuration
        /// </summary>
        /// <returns>The API configuration</returns>
        ApiConfiguration GetApiConfiguration();
    }
}