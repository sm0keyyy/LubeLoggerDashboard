using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Document API service
    /// </summary>
    public interface IDocumentService : IApiService
    {
        /// <summary>
        /// Uploads a document
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="documentType">The type of document</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="fileContent">The content of the file</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> UploadDocumentAsync(
            int vehicleId, 
            string documentType, 
            string fileName, 
            byte[] fileContent);
        
        /// <summary>
        /// Uploads a document
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="documentType">The type of document</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="fileStream">The stream containing the file content</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> UploadDocumentAsync(
            int vehicleId, 
            string documentType, 
            string fileName, 
            Stream fileStream);
        
        /// <summary>
        /// Uploads a document with metadata
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle</param>
        /// <param name="documentType">The type of document</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="fileContent">The content of the file</param>
        /// <param name="metadata">Additional metadata for the document</param>
        /// <returns>The HTTP response containing the result of the operation</returns>
        Task<HttpResponseMessage> UploadDocumentWithMetadataAsync(
            int vehicleId, 
            string documentType, 
            string fileName, 
            byte[] fileContent,
            params (string key, string value)[] metadata);
    }
}