using System.Collections.Generic;

namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents a vehicle in the system.
    /// </summary>
    public class Vehicle : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the vehicle.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the make (manufacturer) of the vehicle.
        /// </summary>
        public string Make { get; set; }
        
        /// <summary>
        /// Gets or sets the model of the vehicle.
        /// </summary>
        public string Model { get; set; }
        
        /// <summary>
        /// Gets or sets the year of the vehicle.
        /// </summary>
        public int Year { get; set; }
        
        /// <summary>
        /// Gets or sets the Vehicle Identification Number (VIN).
        /// </summary>
        public string VIN { get; set; }
        
        /// <summary>
        /// Gets or sets the license plate number.
        /// </summary>
        public string LicensePlate { get; set; }
        
        /// <summary>
        /// Gets or sets the current mileage of the vehicle.
        /// </summary>
        public int CurrentMileage { get; set; }
        
        /// <summary>
        /// Gets or sets additional notes about the vehicle.
        /// </summary>
        public string Notes { get; set; }
        
        // Navigation properties
        
        /// <summary>
        /// Gets or sets the collection of odometer records for this vehicle.
        /// </summary>
        public virtual ICollection<OdometerRecord> OdometerRecords { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of plan records for this vehicle.
        /// </summary>
        public virtual ICollection<PlanRecord> PlanRecords { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of service records for this vehicle.
        /// </summary>
        public virtual ICollection<ServiceRecord> ServiceRecords { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of repair records for this vehicle.
        /// </summary>
        public virtual ICollection<RepairRecord> RepairRecords { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of upgrade records for this vehicle.
        /// </summary>
        public virtual ICollection<UpgradeRecord> UpgradeRecords { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of tax records for this vehicle.
        /// </summary>
        public virtual ICollection<TaxRecord> TaxRecords { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of gas records for this vehicle.
        /// </summary>
        public virtual ICollection<GasRecord> GasRecords { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of reminders for this vehicle.
        /// </summary>
        public virtual ICollection<Reminder> Reminders { get; set; }
    }
}