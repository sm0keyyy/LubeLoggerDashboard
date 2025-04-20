using System;

namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents a gas/fuel record for a vehicle.
    /// </summary>
    public class GasRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the vehicle this record belongs to.
        /// </summary>
        public int VehicleId { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the fuel was purchased.
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the odometer reading at the time of fueling.
        /// </summary>
        public int Odometer { get; set; }
        
        /// <summary>
        /// Gets or sets the amount of fuel consumed.
        /// </summary>
        public decimal FuelConsumed { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this was a full fill-up.
        /// </summary>
        public bool IsFullFill { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether a fuel-up was missed before this one.
        /// </summary>
        public bool MissedFuelUp { get; set; }
        
        /// <summary>
        /// Gets or sets the cost of the fuel.
        /// </summary>
        public decimal Cost { get; set; }
        
        /// <summary>
        /// Gets or sets additional notes about the fuel purchase.
        /// </summary>
        public string Notes { get; set; }
        
        /// <summary>
        /// Gets or sets the vehicle this record belongs to.
        /// </summary>
        public virtual Vehicle Vehicle { get; set; }
    }
}