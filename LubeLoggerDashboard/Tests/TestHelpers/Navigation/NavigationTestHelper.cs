using System;
using System.Collections.Generic;
using System.Windows.Controls;
using LubeLoggerDashboard.Services.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace LubeLoggerDashboard.Tests.TestHelpers.Navigation
{
    /// <summary>
    /// Helper class for navigation-related testing
    /// </summary>
    public static class NavigationTestHelper
    {
        /// <summary>
        /// Creates a service provider with navigation services configured
        /// </summary>
        /// <param name="registeredViews">Dictionary of view names and their types to register</param>
        /// <returns>A configured service provider</returns>
        public static IServiceProvider CreateNavigationServiceProvider(Dictionary<string, Type> registeredViews = null)
        {
            var services = new ServiceCollection();
            
            // Set up navigation frame
            var frame = new Frame();
            services.AddSingleton(frame);
            
            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            
            // Add view factory
            services.AddSingleton<IViewFactory, ViewFactory>();
            
            // Add navigation service
            services.AddSingleton<INavigationService, NavigationService>();
            
            var serviceProvider = services.BuildServiceProvider();
            
            // Register views if provided
            if (registeredViews != null)
            {
                var viewFactory = serviceProvider.GetRequiredService<IViewFactory>() as ViewFactory;
                
                foreach (var view in registeredViews)
                {
                    viewFactory.RegisterView(view.Key, view.Value);
                }
            }
            
            return serviceProvider;
        }
        
        /// <summary>
        /// Creates a mock navigation service
        /// </summary>
        /// <param name="navigateToResult">The result to return from NavigateTo</param>
        /// <param name="navigateBackResult">The result to return from NavigateBack</param>
        /// <param name="navigateForwardResult">The result to return from NavigateForward</param>
        /// <param name="handleDeepLinkResult">The result to return from HandleDeepLink</param>
        /// <returns>A mock navigation service</returns>
        public static Mock<INavigationService> CreateMockNavigationService(
            bool navigateToResult = true,
            bool navigateBackResult = true,
            bool navigateForwardResult = true,
            bool handleDeepLinkResult = true)
        {
            var mockNavigationService = new Mock<INavigationService>();
            
            mockNavigationService.Setup(ns => ns.NavigateTo(It.IsAny<string>())).Returns(navigateToResult);
            mockNavigationService.Setup(ns => ns.NavigateBack()).Returns(navigateBackResult);
            mockNavigationService.Setup(ns => ns.NavigateForward()).Returns(navigateForwardResult);
            mockNavigationService.Setup(ns => ns.HandleDeepLink(It.IsAny<Uri>())).Returns(handleDeepLinkResult);
            
            return mockNavigationService;
        }
        
        /// <summary>
        /// Creates a mock view factory
        /// </summary>
        /// <param name="registeredViews">Dictionary of view names and whether they are registered</param>
        /// <param name="viewInstances">Dictionary of view names and their instances to return from CreateView</param>
        /// <returns>A mock view factory</returns>
        public static Mock<IViewFactory> CreateMockViewFactory(
            Dictionary<string, bool> registeredViews = null,
            Dictionary<string, UserControl> viewInstances = null)
        {
            var mockViewFactory = new Mock<IViewFactory>();
            
            if (registeredViews != null)
            {
                foreach (var view in registeredViews)
                {
                    mockViewFactory.Setup(vf => vf.IsViewRegistered(view.Key)).Returns(view.Value);
                }
            }
            
            if (viewInstances != null)
            {
                foreach (var view in viewInstances)
                {
                    mockViewFactory.Setup(vf => vf.CreateView(view.Key)).Returns(view.Value);
                }
            }
            
            return mockViewFactory;
        }
        
        /// <summary>
        /// Creates a test view
        /// </summary>
        /// <param name="name">The name of the view</param>
        /// <returns>A test view</returns>
        public static UserControl CreateTestView(string name)
        {
            var view = new UserControl();
            view.Name = name;
            return view;
        }
        
        /// <summary>
        /// Creates a deep link URI
        /// </summary>
        /// <param name="viewName">The name of the view</param>
        /// <param name="parameters">Optional parameters to include in the URI</param>
        /// <returns>A deep link URI</returns>
        public static Uri CreateDeepLinkUri(string viewName, Dictionary<string, string> parameters = null)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = "lubelogger",
                Host = viewName
            };
            
            if (parameters != null && parameters.Count > 0)
            {
                var query = new System.Text.StringBuilder();
                
                foreach (var param in parameters)
                {
                    if (query.Length > 0)
                    {
                        query.Append('&');
                    }
                    
                    query.Append(Uri.EscapeDataString(param.Key));
                    query.Append('=');
                    query.Append(Uri.EscapeDataString(param.Value));
                }
                
                uriBuilder.Query = query.ToString();
            }
            
            return uriBuilder.Uri;
        }
    }
}