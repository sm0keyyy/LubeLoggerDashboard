using System;
using System.Collections.Generic;
using System.Linq;
using LubeLoggerDashboard.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LubeLoggerDashboard.Tests.Services.Navigation
{
    /// <summary>
    /// Tests for the ViewFactory class
    /// </summary>
    public class ViewFactoryTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<ILogger<ViewFactory>> _mockLogger;
        private readonly ViewFactory _viewFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewFactoryTests"/> class
        /// </summary>
        public ViewFactoryTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockLogger = new Mock<ILogger<ViewFactory>>();
            
            _viewFactory = new ViewFactory(
                _mockServiceProvider.Object,
                _mockLogger.Object);
        }

        /// <summary>
        /// Tests that RegisterView adds the view to the registry
        /// </summary>
        [Fact]
        public void RegisterView_ValidParameters_AddsToRegistry()
        {
            // Arrange
            string viewName = "TestView";
            Type viewType = typeof(object);
            
            // Act
            _viewFactory.RegisterView(viewName, viewType);
            
            // Assert
            Assert.True(_viewFactory.IsViewRegistered(viewName));
        }

        /// <summary>
        /// Tests that RegisterView throws an exception when the view name is null
        /// </summary>
        [Fact]
        public void RegisterView_NullViewName_ThrowsArgumentException()
        {
            // Arrange
            string viewName = null;
            Type viewType = typeof(object);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _viewFactory.RegisterView(viewName, viewType));
        }

        /// <summary>
        /// Tests that RegisterView throws an exception when the view type is null
        /// </summary>
        [Fact]
        public void RegisterView_NullViewType_ThrowsArgumentNullException()
        {
            // Arrange
            string viewName = "TestView";
            Type viewType = null;
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _viewFactory.RegisterView(viewName, viewType));
        }

        /// <summary>
        /// Tests that IsViewRegistered returns false when the view is not registered
        /// </summary>
        [Fact]
        public void IsViewRegistered_ViewNotRegistered_ReturnsFalse()
        {
            // Arrange
            string viewName = "NonExistentView";
            
            // Act
            bool result = _viewFactory.IsViewRegistered(viewName);
            
            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that IsViewRegistered returns true when the view is registered
        /// </summary>
        [Fact]
        public void IsViewRegistered_ViewRegistered_ReturnsTrue()
        {
            // Arrange
            string viewName = "TestView";
            Type viewType = typeof(object);
            _viewFactory.RegisterView(viewName, viewType);
            
            // Act
            bool result = _viewFactory.IsViewRegistered(viewName);
            
            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that GetRegisteredViewNames returns all registered view names
        /// </summary>
        [Fact]
        public void GetRegisteredViewNames_ReturnsAllRegisteredViewNames()
        {
            // Arrange
            string viewName1 = "TestView1";
            string viewName2 = "TestView2";
            Type viewType = typeof(object);
            
            _viewFactory.RegisterView(viewName1, viewType);
            _viewFactory.RegisterView(viewName2, viewType);
            
            // Act
            IEnumerable<string> result = _viewFactory.GetRegisteredViewNames();
            
            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(viewName1, result);
            Assert.Contains(viewName2, result);
        }

        /// <summary>
        /// Tests that CreateView returns null when the view is not registered
        /// </summary>
        [Fact]
        public void CreateView_ViewNotRegistered_ReturnsNull()
        {
            // Arrange
            string viewName = "NonExistentView";
            
            // Act
            object result = _viewFactory.CreateView(viewName);
            
            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tests that CreateView tries to resolve the view from the DI container
        /// </summary>
        [Fact]
        public void CreateView_ViewRegistered_TriesToResolveFromDI()
        {
            // Arrange
            string viewName = "TestView";
            Type viewType = typeof(object);
            object expectedView = new object();
            
            _viewFactory.RegisterView(viewName, viewType);
            _mockServiceProvider.Setup(sp => sp.GetService(viewType)).Returns(expectedView);
            
            // Act
            object result = _viewFactory.CreateView(viewName);
            
            // Assert
            Assert.Same(expectedView, result);
            _mockServiceProvider.Verify(sp => sp.GetService(viewType), Times.Once);
        }

        /// <summary>
        /// Tests that CreateView falls back to Activator.CreateInstance when DI resolution fails
        /// </summary>
        [Fact]
        public void CreateView_DIResolutionFails_FallsBackToActivator()
        {
            // Arrange
            string viewName = "TestView";
            Type viewType = typeof(TestView);
            
            _viewFactory.RegisterView(viewName, viewType);
            _mockServiceProvider.Setup(sp => sp.GetService(viewType)).Returns(null);
            
            // Act
            object result = _viewFactory.CreateView(viewName);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<TestView>(result);
            _mockServiceProvider.Verify(sp => sp.GetService(viewType), Times.Once);
        }

        /// <summary>
        /// A test view class for testing Activator.CreateInstance
        /// </summary>
        private class TestView
        {
            // Empty class for testing
        }
    }
}