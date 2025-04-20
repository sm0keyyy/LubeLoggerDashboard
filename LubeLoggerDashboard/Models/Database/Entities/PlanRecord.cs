namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents a planned maintenance, repair, or upgrade record for a vehicle.
    /// </summary>
    public class PlanRecord : BaseRecord
    {
        /// <summary>
        /// Gets or sets the type of the planned record (ServiceRecord, RepairRecord, UpgradeRecord).
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Gets or sets the priority of the planned record (Low, Normal, Critical).
        /// </summary>
        public string Priority { get; set; }
        
        /// <summary>
        /// Gets or sets the progress status of the planned record (Backlog, InProgress, Testing).
        /// </summary>
        public string Progress { get; set; }
    }
}