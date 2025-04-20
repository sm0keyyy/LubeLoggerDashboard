using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Service Record API service
    /// </summary>
    public interface IServiceRecordService : IMaintenanceRecordService
    {
        // Inherits all methods from IMaintenanceRecordService
        // This interface can be extended with service-specific methods if needed
    }
}