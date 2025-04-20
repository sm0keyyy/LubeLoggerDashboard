using System;
using System.IO;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LubeLoggerDashboard.Services.Configuration;
using LubeLoggerDashboard.Services.Logging;
using Serilog.Events;

namespace LubeLoggerDashboard.Tests.Services.Configuration
{
    [TestClass]
    public class ConfigurationServiceTests
    {
        private string _testConfigDirectory;
        private string _testConfigFilePath;

        [TestInitialize]
        public void Initialize()
        {
            // Create a temporary directory for test configuration
            _testConfigDirectory = Path.Combine(Path.GetTempPath(), "LubeLoggerTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testConfigDirectory);
            _testConfigFilePath = Path.Combine(_testConfigDirectory, "appsettings.test.json");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test configuration
            if (Directory.Exists(_testConfigDirectory))
            {
                try
                {
                    Directory.Delete(_testConfigDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        [TestMethod]
        public void GetLoggingConfiguration_WithNoConfigFile_ShouldReturnDefaultConfiguration()
        {
            // Arrange
            var configService = new ConfigurationService("nonexistent.json");

            // Act
            var config = configService.GetLoggingConfiguration();

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(LogEventLevel.Debug, config.MinimumLevel);
            Assert.AreEqual(true, config.EnableConsole);
            Assert.AreEqual("logs/lubelogger.log", config.LogFilePath);
            Assert.AreEqual(RollingInterval.Day, config.RollingInterval);
            Assert.AreEqual(31, config.RetainedFileCountLimit);
        }

        [TestMethod]
        public void GetLoggingConfiguration_WithValidConfigFile_ShouldReturnConfigurationFromFile()
        {
            // Arrange
            var testConfig = new
            {
                Logging = new
                {
                    MinimumLevel = "Information",
                    EnableConsole = false,
                    LogFilePath = "logs/custom.log",
                    RollingInterval = "Hour",
                    RetainedFileCountLimit = 10,
                    EnableApiLogging = true,
                    ApiLogLevel = "Warning",
                    LogRequestResponseBodies = false,
                    MaxBodySizeToLog = 1024
                }
            };

            File.WriteAllText(_testConfigFilePath, JsonSerializer.Serialize(testConfig));
            var configService = new ConfigurationService(_testConfigFilePath);

            // Act
            var config = configService.GetLoggingConfiguration();

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(LogEventLevel.Information, config.MinimumLevel);
            Assert.AreEqual(false, config.EnableConsole);
            Assert.AreEqual("logs/custom.log", config.LogFilePath);
            Assert.AreEqual(RollingInterval.Hour, config.RollingInterval);
            Assert.AreEqual(10, config.RetainedFileCountLimit);
            Assert.AreEqual(true, config.EnableApiLogging);
            Assert.AreEqual(LogEventLevel.Warning, config.ApiLogLevel);
            Assert.AreEqual(false, config.LogRequestResponseBodies);
            Assert.AreEqual(1024, config.MaxBodySizeToLog);
        }

        [TestMethod]
        public void GetApiConfiguration_WithNoConfigFile_ShouldReturnDefaultConfiguration()
        {
            // Arrange
            var configService = new ConfigurationService("nonexistent.json");

            // Act
            var config = configService.GetApiConfiguration();

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual("https://demo.lubelogger.com/api", config.BaseUrl);
            Assert.AreEqual(30, config.Timeout);
            Assert.AreEqual(3, config.RetryCount);
            Assert.AreEqual(1000, config.RetryDelayMilliseconds);
        }

        [TestMethod]
        public void GetApiConfiguration_WithValidConfigFile_ShouldReturnConfigurationFromFile()
        {
            // Arrange
            var testConfig = new
            {
                Api = new
                {
                    BaseUrl = "https://custom.lubelogger.com/api",
                    Timeout = 60,
                    RetryCount = 5,
                    RetryDelayMilliseconds = 2000
                }
            };

            File.WriteAllText(_testConfigFilePath, JsonSerializer.Serialize(testConfig));
            var configService = new ConfigurationService(_testConfigFilePath);

            // Act
            var config = configService.GetApiConfiguration();

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual("https://custom.lubelogger.com/api", config.BaseUrl);
            Assert.AreEqual(60, config.Timeout);
            Assert.AreEqual(5, config.RetryCount);
            Assert.AreEqual(2000, config.RetryDelayMilliseconds);
        }

        [TestMethod]
        public void GetLoggingConfiguration_WithInvalidConfigFile_ShouldReturnDefaultConfiguration()
        {
            // Arrange
            File.WriteAllText(_testConfigFilePath, "invalid json");
            var configService = new ConfigurationService(_testConfigFilePath);

            // Act
            var config = configService.GetLoggingConfiguration();

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(LogEventLevel.Debug, config.MinimumLevel);
        }
    }
}