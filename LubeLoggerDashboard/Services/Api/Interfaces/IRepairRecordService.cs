using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Repair Record API service
    /// </summary>
    public interface IRepairRecordService : IMaintenanceRecordService
    {
        // Inherits all methods from IMaintenanceRecordService
        // This interface can be extended with repair-specific methods if needed
    }
}