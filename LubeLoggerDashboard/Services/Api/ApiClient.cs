using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Serilog;

namespace LubeLoggerDashboard.Services.Api
{
    /// <summary>
    /// Implementation of IApiClient for the LubeLogger API
    /// </summary>
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://demo.lubelogger.com"; // Default to demo instance

        /// <summary>
        /// Initializes a new instance of the ApiClient class
        /// </summary>
        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <inheritdoc/>
        public void SetAuthenticationHeader(string authHeader)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                authHeader.StartsWith("Basic ") ? "Basic" : "Bearer", 
                authHeader.StartsWith("Basic ") ? authHeader.Substring(6) : authHeader);
        }

        /// <inheritdoc/>
        public void ClearAuthenticationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            try
            {
                Log.Debug("Sending GET request to {Endpoint}", endpoint);
                return await _httpClient.GetAsync(endpoint);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending GET request to {Endpoint}", endpoint);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string endpoint, params (string key, string value)[] queryParams)
        {
            try
            {
                var queryString = BuildQueryString(queryParams);
                var url = $"{endpoint}{queryString}";
                
                Log.Debug("Sending GET request to {Url}", url);
                return await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending GET request to {Endpoint} with query parameters", endpoint);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostFormAsync(string endpoint, params (string key, string value)[] formData)
        {
            try
            {
                var content = new FormUrlEncodedContent(formData.Select(p => new KeyValuePair<string, string>(p.key, p.value)));
                
                Log.Debug("Sending POST request to {Endpoint}", endpoint);
                return await _httpClient.PostAsync(endpoint, content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending POST request to {Endpoint}", endpoint);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutFormAsync(string endpoint, params (string key, string value)[] formData)
        {
            try
            {
                var content = new FormUrlEncodedContent(formData.Select(p => new KeyValuePair<string, string>(p.key, p.value)));
                
                Log.Debug("Sending PUT request to {Endpoint}", endpoint);
                return await _httpClient.PutAsync(endpoint, content);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending PUT request to {Endpoint}", endpoint);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, params (string key, string value)[] queryParams)
        {
            try
            {
                var queryString = BuildQueryString(queryParams);
                var url = $"{endpoint}{queryString}";
                
                Log.Debug("Sending DELETE request to {Url}", url);
                return await _httpClient.DeleteAsync(url);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending DELETE request to {Endpoint} with query parameters", endpoint);
                throw;
            }
        }

        /// <summary>
        /// Builds a query string from the provided parameters
        /// </summary>
        /// <param name="queryParams">The query parameters</param>
        /// <returns>The query string</returns>
        private string BuildQueryString(params (string key, string value)[] queryParams)
        {
            if (queryParams == null || queryParams.Length == 0)
            {
                return string.Empty;
            }

            var queryString = "?" + string.Join("&", queryParams
                .Where(p => !string.IsNullOrWhiteSpace(p.key) && !string.IsNullOrWhiteSpace(p.value))
                .Select(p => $"{Uri.EscapeDataString(p.key)}={Uri.EscapeDataString(p.value)}"));

            return queryString;
        }
    }
}