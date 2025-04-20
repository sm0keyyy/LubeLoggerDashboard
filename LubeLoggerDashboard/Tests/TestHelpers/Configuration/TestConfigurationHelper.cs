using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace LubeLoggerDashboard.Tests.TestHelpers.Configuration
{
    /// <summary>
    /// Helper class for managing test configurations
    /// </summary>
    public static class TestConfigurationHelper
    {
        /// <summary>
        /// Creates a configuration for testing with the specified settings
        /// </summary>
        /// <param name="configValues">Dictionary of configuration values</param>
        /// <returns>An IConfiguration instance</returns>
        public static IConfiguration CreateConfiguration(params (string Key, string Value)[] configValues)
        {
            var configBuilder = new ConfigurationBuilder();
            var memoryConfig = new Microsoft.Extensions.Configuration.Memory.MemoryConfigurationSource();
            var initialData = new System.Collections.Generic.Dictionary<string, string>();
            
            foreach (var (key, value) in configValues)
            {
                initialData[key] = value;
            }
            
            memoryConfig.InitialData = initialData;
            configBuilder.Add(memoryConfig);
            
            return configBuilder.Build();
        }
        
        /// <summary>
        /// Creates a test appsettings.json file with the specified content
        /// </summary>
        /// <param name="filePath">The path where the file should be created</param>
        /// <param name="settings">The settings object to serialize to JSON</param>
        public static void CreateTestAppSettingsFile(string filePath, object settings)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, json);
        }
        
        /// <summary>
        /// Creates a configuration from a JSON string
        /// </summary>
        /// <param name="json">The JSON configuration string</param>
        /// <returns>An IConfiguration instance</returns>
        public static IConfiguration CreateConfigurationFromJson(string json)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.json");
            File.WriteAllText(tempFile, json);
            
            try
            {
                var configBuilder = new ConfigurationBuilder();
                configBuilder.AddJsonFile(tempFile, optional: false);
                return configBuilder.Build();
            }
            finally
            {
                // Clean up the temporary file
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
        
        /// <summary>
        /// Creates a standard test configuration with common test settings
        /// </summary>
        /// <returns>An IConfiguration instance with standard test settings</returns>
        public static IConfiguration CreateStandardTestConfiguration()
        {
            return CreateConfiguration(
                ("ApiClient:BaseUrl", "https://test.lubelogger.com"),
                ("ApiClient:ApiVersion", "v1"),
                ("ApiClient:TimeoutSeconds", "5"),
                ("ApiClient:MaxRetries", "2"),
                ("ApiClient:BaseRetryDelayMs", "100"),
                ("ApiClient:EnableThrottling", "true"),
                ("ApiClient:EnableCircuitBreaker", "true"),
                ("ApiClient:CircuitBreakerFailureThreshold", "3"),
                ("ApiClient:CircuitBreakerResetTimeoutMinutes", "1"),
                ("Database:ConnectionString", "Data Source=:memory:"),
                ("Logging:LogLevel:Default", "Information"),
                ("Logging:LogLevel:Microsoft", "Warning"),
                ("Logging:LogLevel:System", "Warning")
            );
        }
        
        /// <summary>
        /// Gets the standard test settings as a JSON string
        /// </summary>
        /// <returns>A JSON string with standard test settings</returns>
        public static string GetStandardTestSettingsJson()
        {
            return @"{
                ""ApiClient"": {
                    ""BaseUrl"": ""https://test.lubelogger.com"",
                    ""ApiVersion"": ""v1"",
                    ""TimeoutSeconds"": 5,
                    ""MaxRetries"": 2,
                    ""BaseRetryDelayMs"": 100,
                    ""EnableThrottling"": true,
                    ""EnableCircuitBreaker"": true,
                    ""CircuitBreakerFailureThreshold"": 3,
                    ""CircuitBreakerResetTimeoutMinutes"": 1
                },
                ""Database"": {
                    ""ConnectionString"": ""Data Source=:memory:""
                },
                ""Logging"": {
                    ""LogLevel"": {
                        ""Default"": ""Information"",
                        ""Microsoft"": ""Warning"",
                        ""System"": ""Warning""
                    }
                }
            }";
        }
    }
}
