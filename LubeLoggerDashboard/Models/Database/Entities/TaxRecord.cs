using System;

namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents a tax record for a vehicle.
    /// </summary>
    public class TaxRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the vehicle this record belongs to.
        /// </summary>
        public int VehicleId { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the tax was paid.
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the tax.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the cost of the tax.
        /// </summary>
        public decimal Cost { get; set; }
        
        /// <summary>
        /// Gets or sets the vehicle this record belongs to.
        /// </summary>
        public virtual Vehicle Vehicle { get; set; }
    }
}