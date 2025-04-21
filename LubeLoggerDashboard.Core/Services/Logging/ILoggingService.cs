using System;

namespace LubeLoggerDashboard.Core.Services.Logging
{
    /// <summary>
    /// Interface for the application logging service
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Logs a verbose message
        /// </summary>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Optional property values</param>
        void Verbose(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Optional property values</param>
        void Debug(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Logs an information message
        /// </summary>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Optional property values</param>
        void Information(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Optional property values</param>
        void Warning(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Optional property values</param>
        void Error(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Logs an error message with an exception
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Optional property values</param>
        void Error(Exception exception, string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Logs a fatal message
        /// </summary>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Optional property values</param>
        void Fatal(string messageTemplate, params object[] propertyValues);

        /// <summary>
        /// Logs a fatal message with an exception
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Optional property values</param>
        void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);
    }
}