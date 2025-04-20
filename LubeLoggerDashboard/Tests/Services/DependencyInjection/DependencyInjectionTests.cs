using System;
using Microsoft.Extensions.DependencyInjection;
using LubeLoggerDashboard.Services.Api;
using LubeLoggerDashboard.Services.Api.Interfaces;
using LubeLoggerDashboard.Services.Authentication;
using LubeLoggerDashboard.Services.DependencyInjection;
using LubeLoggerDashboard.Helpers.Security;
using Xunit;

namespace LubeLoggerDashboard.Tests.Services.DependencyInjection
{
    /// <summary>
    /// Tests for the dependency injection system
    /// </summary>
    public class DependencyInjectionTests
    {
        /// <summary>
        /// Tests that all services can be resolved from the container
        /// </summary>
        [Fact]
        public void CanResolveAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act & Assert
            Assert.NotNull(serviceProvider.GetService<ICredentialManager>());
            Assert.NotNull(serviceProvider.GetService<IApiClient>());
            Assert.NotNull(serviceProvider.GetService<IAuthenticationService>());
            Assert.NotNull(serviceProvider.GetService<IUserService>());
            Assert.NotNull(serviceProvider.GetService<IVehicleService>());
        }

        /// <summary>
        /// Tests that the authentication service factory works correctly
        /// </summary>
        [Fact]
        public void AuthenticationServiceFactoryCreatesService()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<ICredentialManager, CredentialManager>();
            services.AddSingleton<IApiClient, ApiClient>();
            services.AddSingleton<AuthenticationServiceFactory>();
            services.AddSingleton<IAuthenticationService>(provider => 
                provider.GetRequiredService<AuthenticationServiceFactory>().CreateAuthenticationService());
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var authService = serviceProvider.GetService<IAuthenticationService>();

            // Assert
            Assert.NotNull(authService);
            Assert.IsType<BasicAuthenticationService>(authService);
        }

        /// <summary>
        /// Tests that the service locator works correctly
        /// </summary>
        [Fact]
        public void ServiceLocatorReturnsCorrectServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();
            ServiceLocator.Initialize(serviceProvider);

            // Act & Assert
            Assert.NotNull(ServiceLocator.GetService<ICredentialManager>());
            Assert.NotNull(ServiceLocator.GetService<IApiClient>());
            Assert.NotNull(ServiceLocator.GetService<IAuthenticationService>());
            Assert.NotNull(ServiceLocator.GetService<IUserService>());
            Assert.NotNull(ServiceLocator.GetService<IVehicleService>());
        }

        /// <summary>
        /// Tests that the TryGetService method works correctly
        /// </summary>
        [Fact]
        public void TryGetServiceReturnsCorrectResult()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddApplicationServices();
            var serviceProvider = services.BuildServiceProvider();
            ServiceLocator.Initialize(serviceProvider);

            // Act & Assert
            Assert.True(ServiceLocator.TryGetService<ICredentialManager>(out var credentialManager));
            Assert.NotNull(credentialManager);

            Assert.False(ServiceLocator.TryGetService<INonExistentService>(out var nonExistentService));
            Assert.Null(nonExistentService);
        }

        /// <summary>
        /// Dummy interface for testing non-existent services
        /// </summary>
        private interface INonExistentService { }
    }
}