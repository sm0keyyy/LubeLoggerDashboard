using System;

namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Base abstract class for all record types with common properties.
    /// </summary>
    public abstract class BaseRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the vehicle this record belongs to.
        /// </summary>
        public int VehicleId { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the record was created.
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the odometer reading at the time of the record.
        /// </summary>
        public int? Odometer { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the record.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the cost associated with the record.
        /// </summary>
        public decimal Cost { get; set; }
        
        /// <summary>
        /// Gets or sets the vehicle this record belongs to.
        /// </summary>
        public virtual Vehicle Vehicle { get; set; }
    }
}