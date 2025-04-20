using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using Serilog.Events;
using LubeLoggerDashboard.Services.Logging;

namespace LubeLoggerDashboard.Tests.Services.Logging
{
    [TestClass]
    public class LoggingServiceTests
    {
        private Mock<ILogger> _mockLogger;
        private LoggingService _loggingService;

        [TestInitialize]
        public void Initialize()
        {
            _mockLogger = new Mock<ILogger>();
            
            // Setup the mock logger to return itself for the various logging methods
            _mockLogger.Setup(l => l.Verbose(It.IsAny<string>(), It.IsAny<object[]>())).Returns(_mockLogger.Object);
            _mockLogger.Setup(l => l.Debug(It.IsAny<string>(), It.IsAny<object[]>())).Returns(_mockLogger.Object);
            _mockLogger.Setup(l => l.Information(It.IsAny<string>(), It.IsAny<object[]>())).Returns(_mockLogger.Object);
            _mockLogger.Setup(l => l.Warning(It.IsAny<string>(), It.IsAny<object[]>())).Returns(_mockLogger.Object);
            _mockLogger.Setup(l => l.Error(It.IsAny<string>(), It.IsAny<object[]>())).Returns(_mockLogger.Object);
            _mockLogger.Setup(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>())).Returns(_mockLogger.Object);
            _mockLogger.Setup(l => l.Fatal(It.IsAny<string>(), It.IsAny<object[]>())).Returns(_mockLogger.Object);
            _mockLogger.Setup(l => l.Fatal(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>())).Returns(_mockLogger.Object);
            
            _loggingService = new LoggingService(_mockLogger.Object);
        }

        [TestMethod]
        public void Verbose_ShouldCallSerilogVerbose()
        {
            // Arrange
            string message = "Test verbose message";
            object[] args = new object[] { "arg1", 123 };

            // Act
            _loggingService.Verbose(message, args);

            // Assert
            _mockLogger.Verify(l => l.Verbose(message, args), Times.Once);
        }

        [TestMethod]
        public void Debug_ShouldCallSerilogDebug()
        {
            // Arrange
            string message = "Test debug message";
            object[] args = new object[] { "arg1", 123 };

            // Act
            _loggingService.Debug(message, args);

            // Assert
            _mockLogger.Verify(l => l.Debug(message, args), Times.Once);
        }

        [TestMethod]
        public void Information_ShouldCallSerilogInformation()
        {
            // Arrange
            string message = "Test information message";
            object[] args = new object[] { "arg1", 123 };

            // Act
            _loggingService.Information(message, args);

            // Assert
            _mockLogger.Verify(l => l.Information(message, args), Times.Once);
        }

        [TestMethod]
        public void Warning_ShouldCallSerilogWarning()
        {
            // Arrange
            string message = "Test warning message";
            object[] args = new object[] { "arg1", 123 };

            // Act
            _loggingService.Warning(message, args);

            // Assert
            _mockLogger.Verify(l => l.Warning(message, args), Times.Once);
        }

        [TestMethod]
        public void Error_WithoutException_ShouldCallSerilogError()
        {
            // Arrange
            string message = "Test error message";
            object[] args = new object[] { "arg1", 123 };

            // Act
            _loggingService.Error(message, args);

            // Assert
            _mockLogger.Verify(l => l.Error(message, args), Times.Once);
        }

        [TestMethod]
        public void Error_WithException_ShouldCallSerilogError()
        {
            // Arrange
            Exception exception = new Exception("Test exception");
            string message = "Test error message";
            object[] args = new object[] { "arg1", 123 };

            // Act
            _loggingService.Error(exception, message, args);

            // Assert
            _mockLogger.Verify(l => l.Error(exception, message, args), Times.Once);
        }

        [TestMethod]
        public void Fatal_WithoutException_ShouldCallSerilogFatal()
        {
            // Arrange
            string message = "Test fatal message";
            object[] args = new object[] { "arg1", 123 };

            // Act
            _loggingService.Fatal(message, args);

            // Assert
            _mockLogger.Verify(l => l.Fatal(message, args), Times.Once);
        }

        [TestMethod]
        public void Fatal_WithException_ShouldCallSerilogFatal()
        {
            // Arrange
            Exception exception = new Exception("Test exception");
            string message = "Test fatal message";
            object[] args = new object[] { "arg1", 123 };

            // Act
            _loggingService.Fatal(exception, message, args);

            // Assert
            _mockLogger.Verify(l => l.Fatal(exception, message, args), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act
            new LoggingService(null);
        }
    }
}