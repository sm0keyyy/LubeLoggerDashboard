using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Events;
using LubeLoggerDashboard.Services.Logging;

namespace LubeLoggerDashboard.Tests.Services.Logging
{
    [TestClass]
    public class LoggerFactoryTests
    {
        private string _testLogDirectory;
        private string _testLogFilePath;

        [TestInitialize]
        public void Initialize()
        {
            // Create a temporary directory for test logs
            _testLogDirectory = Path.Combine(Path.GetTempPath(), "LubeLoggerTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testLogDirectory);
            _testLogFilePath = Path.Combine(_testLogDirectory, "test.log");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test logs
            if (Directory.Exists(_testLogDirectory))
            {
                try
                {
                    Directory.Delete(_testLogDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        [TestMethod]
        public void CreateLogger_WithDefaultParameters_ShouldCreateLogger()
        {
            // Act
            var logger = LoggerFactory.CreateLogger();

            // Assert
            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void CreateLogger_WithCustomParameters_ShouldCreateLogger()
        {
            // Act
            var logger = LoggerFactory.CreateLogger(
                logLevel: LogEventLevel.Information,
                enableConsole: false,
                logFilePath: _testLogFilePath,
                rollingInterval: RollingInterval.Hour,
                retainedFileCountLimit: 10);

            // Assert
            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void CreateLogger_ShouldCreateLogDirectory()
        {
            // Arrange
            var nonExistentDirectory = Path.Combine(_testLogDirectory, "subdir");
            var logFilePath = Path.Combine(nonExistentDirectory, "test.log");

            // Act
            var logger = LoggerFactory.CreateLogger(logFilePath: logFilePath);

            // Assert
            Assert.IsTrue(Directory.Exists(nonExistentDirectory));
        }

        [TestMethod]
        public void CreateLogger_ShouldWriteToLogFile()
        {
            // Arrange
            var logger = LoggerFactory.CreateLogger(
                logLevel: LogEventLevel.Debug,
                enableConsole: false,
                logFilePath: _testLogFilePath);

            // Act
            logger.Information("Test log message");
            Log.CloseAndFlush(); // Ensure logs are written

            // Assert
            Assert.IsTrue(File.Exists(_testLogFilePath));
            string logContent = File.ReadAllText(_testLogFilePath);
            Assert.IsTrue(logContent.Contains("Test log message"));
        }

        [TestMethod]
        public void CreateLogger_WithJsonOutput_ShouldCreateJsonLogFile()
        {
            // Arrange
            var logger = LoggerFactory.CreateLogger(
                logLevel: LogEventLevel.Debug,
                enableConsole: false,
                logFilePath: _testLogFilePath);

            // Act
            logger.Information("Test log message");
            Log.CloseAndFlush(); // Ensure logs are written

            // Assert
            string jsonLogFilePath = Path.ChangeExtension(_testLogFilePath, ".json");
            Assert.IsTrue(File.Exists(jsonLogFilePath));
            string logContent = File.ReadAllText(jsonLogFilePath);
            Assert.IsTrue(logContent.Contains("Test log message"));
        }
    }
}