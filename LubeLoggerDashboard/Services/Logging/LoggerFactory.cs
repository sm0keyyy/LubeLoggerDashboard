using System;
using System.IO;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace LubeLoggerDashboard.Services.Logging
{
    /// <summary>
    /// Factory for creating and configuring Serilog loggers
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Creates a configured Serilog logger
        /// </summary>
        /// <param name="logLevel">The minimum log level</param>
        /// <param name="enableConsole">Whether to enable console logging</param>
        /// <param name="logFilePath">The path to the log file</param>
        /// <param name="rollingInterval">The interval at which to roll log files</param>
        /// <param name="retainedFileCountLimit">The number of log files to retain</param>
        /// <returns>A configured Serilog logger</returns>
        public static ILogger CreateLogger(
            LogEventLevel logLevel = LogEventLevel.Debug,
            bool enableConsole = true,
            string logFilePath = "logs/lubelogger.log",
            RollingInterval rollingInterval = RollingInterval.Day,
            int? retainedFileCountLimit = 31)
        {
            // Ensure logs directory exists
            var logsDirectory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(logsDirectory) && !Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }

            // Create logger configuration
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("Application", "LubeLoggerDashboard");

            // Add console sink if enabled
            if (enableConsole)
            {
                loggerConfiguration.WriteTo.Console(
                    restrictedToMinimumLevel: logLevel,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            }

            // Add file sink
            loggerConfiguration.WriteTo.File(
                path: logFilePath,
                restrictedToMinimumLevel: logLevel,
                rollingInterval: rollingInterval,
                retainedFileCountLimit: retainedFileCountLimit,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

            // Add JSON file sink for structured logging
            loggerConfiguration.WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: Path.ChangeExtension(logFilePath, ".json"),
                restrictedToMinimumLevel: logLevel,
                rollingInterval: rollingInterval,
                retainedFileCountLimit: retainedFileCountLimit);

            return loggerConfiguration.CreateLogger();
        }
    }
}