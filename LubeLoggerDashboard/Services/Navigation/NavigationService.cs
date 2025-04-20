using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Text.Json;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;

namespace LubeLoggerDashboard.Services.Navigation
{
    /// <summary>
    /// Implementation of the navigation service that manages navigation between views
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IViewFactory _viewFactory;
        private readonly Frame _frame;
        private readonly ILogger<NavigationService> _logger;
        private readonly Stack<NavigationHistoryEntry> _backStack;
        private readonly Stack<NavigationHistoryEntry> _forwardStack;
        private readonly string _historyFilePath;
        private bool _isNavigatingBack;
        private bool _isNavigatingForward;

        /// <summary>
        /// Event raised when navigation occurs
        /// </summary>
        public event EventHandler<NavigationEventArgs> Navigated;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class
        /// </summary>
        /// <param name="viewFactory">The view factory to use for creating views</param>
        /// <param name="frame">The frame to use for content hosting</param>
        /// <param name="logger">The logger to use for logging</param>
        public NavigationService(IViewFactory viewFactory, Frame frame, ILogger<NavigationService> logger)
        {
            _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _backStack = new Stack<NavigationHistoryEntry>();
            _forwardStack = new Stack<NavigationHistoryEntry>();
            
            // Set up the history file path in the local app data folder
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LubeLoggerDashboard");
            
            // Create the directory if it doesn't exist
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            _historyFilePath = Path.Combine(appDataPath, "navigation_history.json");
            
            _logger.LogInformation("NavigationService initialized with history path: {HistoryFilePath}", _historyFilePath);
        }

        /// <summary>
        /// Navigate to a view by name
        /// </summary>
        /// <param name="viewName">The name of the view to navigate to</param>
        /// <returns>True if navigation was successful, false otherwise</returns>
        public bool NavigateTo(string viewName)
        {
            return NavigateTo(viewName, null);
        }

        /// <summary>
        /// Navigate to a view with parameters
        /// </summary>
        /// <param name="viewName">The name of the view to navigate to</param>
        /// <param name="parameter">The parameter to pass to the view</param>
        /// <returns>True if navigation was successful, false otherwise</returns>
        public bool NavigateTo(string viewName, object parameter)
        {
            try
            {
                _logger.LogInformation("Navigating to {ViewName} with parameter: {Parameter}", viewName, parameter);
                
                // Check if the view exists
                if (!_viewFactory.IsViewRegistered(viewName))
                {
                    _logger.LogWarning("View {ViewName} is not registered", viewName);
                    return false;
                }
                
                // Check if the current view allows navigation away
                if (_frame.Content is FrameworkElement currentView && 
                    currentView.DataContext is INavigationAware navigationAware)
                {
                    if (!navigationAware.OnNavigatingFrom())
                    {
                        _logger.LogInformation("Navigation from {CurrentView} to {ViewName} was cancelled by the view model", 
                            _frame.Content.GetType().Name, viewName);
                        return false;
                    }
                }
                
                // Create the new view
                object view = _viewFactory.CreateView(viewName);
                if (view == null)
                {
                    _logger.LogWarning("Failed to create view {ViewName}", viewName);
                    return false;
                }
                
                // Update navigation stacks if not navigating back or forward
                if (!_isNavigatingBack && !_isNavigatingForward)
                {
                    // If we're not navigating back or forward, add the current view to the back stack
                    // and clear the forward stack
                    if (_frame.Content != null)
                    {
                        _backStack.Push(new NavigationHistoryEntry(CurrentView, null));
                    }
                    
                    _forwardStack.Clear();
                }
                else if (_isNavigatingBack)
                {
                    // If we're navigating back, add the current view to the forward stack
                    if (_frame.Content != null)
                    {
                        _forwardStack.Push(new NavigationHistoryEntry(CurrentView, null));
                    }
                    
                    _isNavigatingBack = false;
                }
                else if (_isNavigatingForward)
                {
                    // If we're navigating forward, add the current view to the back stack
                    if (_frame.Content != null)
                    {
                        _backStack.Push(new NavigationHistoryEntry(CurrentView, null));
                    }
                    
                    _isNavigatingForward = false;
                }
                
                // Set the frame content
                _frame.Content = view;
                
                // Notify the view model
                if (view is FrameworkElement frameworkElement && 
                    frameworkElement.DataContext is INavigationAware navigationAwareViewModel)
                {
                    navigationAwareViewModel.OnNavigatedTo(parameter);
                }
                
                // Save history
                SaveNavigationHistory();
                
                // Raise the event
                bool isBackNavigation = _isNavigatingBack;
                bool isDeepLink = false;
                
                Navigated?.Invoke(this, new NavigationEventArgs(viewName, parameter, isBackNavigation, isDeepLink));
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to {ViewName}", viewName);
                return false;
            }
        }

        /// <summary>
        /// Navigate back to the previous view
        /// </summary>
        /// <returns>True if navigation was successful, false otherwise</returns>
        public bool NavigateBack()
        {
            if (!CanNavigateBack)
            {
                _logger.LogInformation("Cannot navigate back - no back history");
                return false;
            }
            
            try
            {
                _isNavigatingBack = true;
                
                NavigationHistoryEntry entry = _backStack.Pop();
                return NavigateTo(entry.ViewName, entry.Parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating back");
                _isNavigatingBack = false;
                return false;
            }
        }

        /// <summary>
        /// Navigate forward to the next view (if available)
        /// </summary>
        /// <returns>True if navigation was successful, false otherwise</returns>
        public bool NavigateForward()
        {
            if (!CanNavigateForward)
            {
                _logger.LogInformation("Cannot navigate forward - no forward history");
                return false;
            }
            
            try
            {
                _isNavigatingForward = true;
                
                NavigationHistoryEntry entry = _forwardStack.Pop();
                return NavigateTo(entry.ViewName, entry.Parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating forward");
                _isNavigatingForward = false;
                return false;
            }
        }

        /// <summary>
        /// Gets the current view name
        /// </summary>
        public string CurrentView
        {
            get
            {
                if (_frame.Content == null)
                {
                    return null;
                }
                
                return _frame.Content.GetType().Name;
            }
        }

        /// <summary>
        /// Checks if navigation back is possible
        /// </summary>
        public bool CanNavigateBack => _backStack.Count > 0;

        /// <summary>
        /// Checks if navigation forward is possible
        /// </summary>
        public bool CanNavigateForward => _forwardStack.Count > 0;

        /// <summary>
        /// Gets the navigation history
        /// </summary>
        public IReadOnlyList<NavigationHistoryEntry> NavigationHistory => _backStack.ToList().AsReadOnly();

        /// <summary>
        /// Clears the navigation history
        /// </summary>
        public void ClearHistory()
        {
            _logger.LogInformation("Clearing navigation history");
            _backStack.Clear();
            _forwardStack.Clear();
            SaveNavigationHistory();
        }

        /// <summary>
        /// Saves the navigation history
        /// </summary>
        public void SaveNavigationHistory()
        {
            try
            {
                _logger.LogInformation("Saving navigation history to {HistoryFilePath}", _historyFilePath);
                
                var historyData = new
                {
                    BackStack = _backStack.ToArray(),
                    ForwardStack = _forwardStack.ToArray()
                };
                
                string json = JsonSerializer.Serialize(historyData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                File.WriteAllText(_historyFilePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving navigation history");
            }
        }

        /// <summary>
        /// Loads the navigation history
        /// </summary>
        public void LoadNavigationHistory()
        {
            try
            {
                if (!File.Exists(_historyFilePath))
                {
                    _logger.LogInformation("No navigation history file found at {HistoryFilePath}", _historyFilePath);
                    return;
                }
                
                _logger.LogInformation("Loading navigation history from {HistoryFilePath}", _historyFilePath);
                
                string json = File.ReadAllText(_historyFilePath);
                
                var historyData = JsonSerializer.Deserialize<HistoryData>(json);
                
                _backStack.Clear();
                _forwardStack.Clear();
                
                if (historyData.BackStack != null)
                {
                    foreach (var entry in historyData.BackStack.Reverse())
                    {
                        _backStack.Push(entry);
                    }
                }
                
                if (historyData.ForwardStack != null)
                {
                    foreach (var entry in historyData.ForwardStack.Reverse())
                    {
                        _forwardStack.Push(entry);
                    }
                }
                
                // Navigate to the current view if there is one
                if (_backStack.Count > 0)
                {
                    var currentEntry = _backStack.Pop();
                    NavigateTo(currentEntry.ViewName, currentEntry.Parameter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading navigation history");
            }
        }

        /// <summary>
        /// Parses a deep link URI and navigates accordingly
        /// </summary>
        /// <param name="uri">The URI to parse</param>
        /// <returns>True if navigation was successful, false otherwise</returns>
        public bool HandleDeepLink(Uri uri)
        {
            try
            {
                _logger.LogInformation("Handling deep link: {Uri}", uri);
                
                if (uri == null)
                {
                    _logger.LogWarning("Deep link URI is null");
                    return false;
                }
                
                // Parse the URI
                // Expected format: lubelogger://viewname?param1=value1&param2=value2
                
                // Check the scheme
                if (uri.Scheme != "lubelogger")
                {
                    _logger.LogWarning("Invalid deep link scheme: {Scheme}", uri.Scheme);
                    return false;
                }
                
                // Get the view name from the host
                string viewName = uri.Host;
                
                if (string.IsNullOrEmpty(viewName))
                {
                    _logger.LogWarning("Deep link view name is empty");
                    return false;
                }
                
                // Parse the query parameters
                var parameters = new Dictionary<string, string>();
                
                if (!string.IsNullOrEmpty(uri.Query))
                {
                    string query = uri.Query.TrimStart('?');
                    string[] pairs = query.Split('&');
                    
                    foreach (string pair in pairs)
                    {
                        string[] keyValue = pair.Split('=');
                        
                        if (keyValue.Length == 2)
                        {
                            parameters[keyValue[0]] = Uri.UnescapeDataString(keyValue[1]);
                        }
                    }
                }
                
                // Navigate to the view
                return NavigateTo(viewName, parameters.Count > 0 ? parameters : null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling deep link");
                return false;
            }
        }

        /// <summary>
        /// Class for deserializing navigation history
        /// </summary>
        private class HistoryData
        {
            public NavigationHistoryEntry[] BackStack { get; set; }
            public NavigationHistoryEntry[] ForwardStack { get; set; }
        }
    }
}