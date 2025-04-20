using Serilog.Events;

namespace LubeLoggerDashboard.Services.Logging
{
    /// <summary>
    /// Configuration for the logging service
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        /// Gets or sets the minimum log level
        /// </summary>
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Debug;

        /// <summary>
        /// Gets or sets whether to enable console logging
        /// </summary>
        public bool EnableConsole { get; set; } = true;

        /// <summary>
        /// Gets or sets the path to the log file
        /// </summary>
        public string LogFilePath { get; set; } = "logs/lubelogger.log";

        /// <summary>
        /// Gets or sets the interval at which to roll log files
        /// </summary>
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;

        /// <summary>
        /// Gets or sets the number of log files to retain
        /// </summary>
        public int? RetainedFileCountLimit { get; set; } = 31;

        /// <summary>
        /// Gets or sets whether to enable API request/response logging
        /// </summary>
        public bool EnableApiLogging { get; set; } = true;

        /// <summary>
        /// Gets or sets the minimum log level for API request/response logging
        /// </summary>
        public LogEventLevel ApiLogLevel { get; set; } = LogEventLevel.Debug;

        /// <summary>
        /// Gets or sets whether to log request/response bodies
        /// </summary>
        public bool LogRequestResponseBodies { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum size of request/response bodies to log (in bytes)
        /// </summary>
        public int MaxBodySizeToLog { get; set; } = 32 * 1024; // 32 KB
    }
}