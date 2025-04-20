using System;
using System.Collections.Generic;
using System.Windows.Controls;
using LubeLoggerDashboard.Services.Navigation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LubeLoggerDashboard.Tests.Services.Navigation
{
    /// <summary>
    /// Tests for the NavigationService class
    /// </summary>
    public class NavigationServiceTests
    {
        private readonly Mock<IViewFactory> _mockViewFactory;
        private readonly Mock<Frame> _mockFrame;
        private readonly Mock<ILogger<NavigationService>> _mockLogger;
        private readonly NavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationServiceTests"/> class
        /// </summary>
        public NavigationServiceTests()
        {
            _mockViewFactory = new Mock<IViewFactory>();
            _mockFrame = new Mock<Frame>();
            _mockLogger = new Mock<ILogger<NavigationService>>();
            
            _navigationService = new NavigationService(
                _mockViewFactory.Object,
                _mockFrame.Object,
                _mockLogger.Object);
        }

        /// <summary>
        /// Tests that NavigateTo returns false when the view is not registered
        /// </summary>
        [Fact]
        public void NavigateTo_ViewNotRegistered_ReturnsFalse()
        {
            // Arrange
            string viewName = "NonExistentView";
            _mockViewFactory.Setup(f => f.IsViewRegistered(viewName)).Returns(false);
            
            // Act
            bool result = _navigationService.NavigateTo(viewName);
            
            // Assert
            Assert.False(result);
            _mockViewFactory.Verify(f => f.IsViewRegistered(viewName), Times.Once);
            _mockViewFactory.Verify(f => f.CreateView(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Tests that NavigateTo returns false when the view factory returns null
        /// </summary>
        [Fact]
        public void NavigateTo_ViewFactoryReturnsNull_ReturnsFalse()
        {
            // Arrange
            string viewName = "TestView";
            _mockViewFactory.Setup(f => f.IsViewRegistered(viewName)).Returns(true);
            _mockViewFactory.Setup(f => f.CreateView(viewName)).Returns(null);
            
            // Act
            bool result = _navigationService.NavigateTo(viewName);
            
            // Assert
            Assert.False(result);
            _mockViewFactory.Verify(f => f.IsViewRegistered(viewName), Times.Once);
            _mockViewFactory.Verify(f => f.CreateView(viewName), Times.Once);
        }

        /// <summary>
        /// Tests that NavigateTo returns true when navigation is successful
        /// </summary>
        [Fact]
        public void NavigateTo_ValidView_ReturnsTrue()
        {
            // Arrange
            string viewName = "TestView";
            var mockView = new Mock<UserControl>();
            
            _mockViewFactory.Setup(f => f.IsViewRegistered(viewName)).Returns(true);
            _mockViewFactory.Setup(f => f.CreateView(viewName)).Returns(mockView.Object);
            
            // Act
            bool result = _navigationService.NavigateTo(viewName);
            
            // Assert
            Assert.True(result);
            _mockViewFactory.Verify(f => f.IsViewRegistered(viewName), Times.Once);
            _mockViewFactory.Verify(f => f.CreateView(viewName), Times.Once);
            _mockFrame.VerifySet(f => f.Content = mockView.Object, Times.Once);
        }

        /// <summary>
        /// Tests that NavigateBack returns false when there is no back history
        /// </summary>
        [Fact]
        public void NavigateBack_NoBackHistory_ReturnsFalse()
        {
            // Act
            bool result = _navigationService.NavigateBack();
            
            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that NavigateForward returns false when there is no forward history
        /// </summary>
        [Fact]
        public void NavigateForward_NoForwardHistory_ReturnsFalse()
        {
            // Act
            bool result = _navigationService.NavigateForward();
            
            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that HandleDeepLink returns false when the URI is null
        /// </summary>
        [Fact]
        public void HandleDeepLink_NullUri_ReturnsFalse()
        {
            // Act
            bool result = _navigationService.HandleDeepLink(null);
            
            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that HandleDeepLink returns false when the URI scheme is invalid
        /// </summary>
        [Fact]
        public void HandleDeepLink_InvalidScheme_ReturnsFalse()
        {
            // Arrange
            var uri = new Uri("http://testview");
            
            // Act
            bool result = _navigationService.HandleDeepLink(uri);
            
            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that HandleDeepLink returns false when the view name is empty
        /// </summary>
        [Fact]
        public void HandleDeepLink_EmptyViewName_ReturnsFalse()
        {
            // Arrange
            var uri = new Uri("lubelogger://");
            
            // Act
            bool result = _navigationService.HandleDeepLink(uri);
            
            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that HandleDeepLink returns true when navigation is successful
        /// </summary>
        [Fact]
        public void HandleDeepLink_ValidUri_ReturnsTrue()
        {
            // Arrange
            string viewName = "testview";
            var uri = new Uri($"lubelogger://{viewName}?param1=value1&param2=value2");
            var mockView = new Mock<UserControl>();
            
            _mockViewFactory.Setup(f => f.IsViewRegistered(viewName)).Returns(true);
            _mockViewFactory.Setup(f => f.CreateView(viewName)).Returns(mockView.Object);
            
            // Act
            bool result = _navigationService.HandleDeepLink(uri);
            
            // Assert
            Assert.True(result);
            _mockViewFactory.Verify(f => f.IsViewRegistered(viewName), Times.Once);
            _mockViewFactory.Verify(f => f.CreateView(viewName), Times.Once);
            _mockFrame.VerifySet(f => f.Content = mockView.Object, Times.Once);
        }
    }
}