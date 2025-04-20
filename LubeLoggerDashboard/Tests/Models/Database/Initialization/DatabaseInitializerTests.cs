using System;
using System.Threading.Tasks;
using LubeLoggerDashboard.Models.Database.Context;
using LubeLoggerDashboard.Models.Database.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LubeLoggerDashboard.Tests.Models.Database.Initialization
{
    public class DatabaseInitializerTests
    {
        [Fact]
        public async Task InitializeAsync_ShouldCallMigrateAsync()
        {
            // Arrange
            var mockDbContext = new Mock<LubeLoggerDbContext>();
            var mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            var mockLogger = new Mock<ILogger<DatabaseInitializer>>();

            mockDbContext.Setup(c => c.Database).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.MigrateAsync(default)).Returns(Task.CompletedTask);

            var initializer = new DatabaseInitializer(mockDbContext.Object, mockLogger.Object);

            // Act
            await initializer.InitializeAsync();

            // Assert
            mockDatabase.Verify(d => d.MigrateAsync(default), Times.Once);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Initializing database")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("completed successfully")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InitializeAsync_ShouldLogErrorAndRethrow_WhenExceptionOccurs()
        {
            // Arrange
            var mockDbContext = new Mock<LubeLoggerDbContext>();
            var mockDatabase = new Mock<DatabaseFacade>(mockDbContext.Object);
            var mockLogger = new Mock<ILogger<DatabaseInitializer>>();
            var expectedException = new Exception("Test exception");

            mockDbContext.Setup(c => c.Database).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.MigrateAsync(default)).ThrowsAsync(expectedException);

            var initializer = new DatabaseInitializer(mockDbContext.Object, mockLogger.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => initializer.InitializeAsync());
            Assert.Same(expectedException, exception);
            
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    expectedException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}