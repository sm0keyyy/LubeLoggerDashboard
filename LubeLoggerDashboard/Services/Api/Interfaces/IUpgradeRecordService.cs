using System.Threading.Tasks;
using System.Net.Http;

namespace LubeLoggerDashboard.Services.Api.Interfaces
{
    /// <summary>
    /// Interface for the Upgrade Record API service
    /// </summary>
    public interface IUpgradeRecordService : IMaintenanceRecordService
    {
        // Inherits all methods from IMaintenanceRecordService
        // This interface can be extended with upgrade-specific methods if needed
    }
}