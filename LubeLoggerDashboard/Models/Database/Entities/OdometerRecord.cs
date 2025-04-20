using System;
using System.Collections.Generic;
using System.Text.Json;

namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents an odometer reading record for a vehicle.
    /// </summary>
    public class OdometerRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the vehicle this record belongs to.
        /// </summary>
        public int VehicleId { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the odometer reading was recorded.
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the current odometer reading.
        /// </summary>
        public int Odometer { get; set; }
        
        /// <summary>
        /// Gets or sets the initial odometer reading.
        /// </summary>
        public int InitialOdometer { get; set; }
        
        /// <summary>
        /// Gets or sets additional notes about the odometer reading.
        /// </summary>
        public string Notes { get; set; }
        
        /// <summary>
        /// Gets or sets the vehicle this record belongs to.
        /// </summary>
        public virtual Vehicle Vehicle { get; set; }
        
        /// <summary>
        /// Gets or sets the JSON string representation of extra fields.
        /// </summary>
        public string ExtraFieldsJson { get; set; }
        
        /// <summary>
        /// Gets the extra fields as a list of ExtraField objects.
        /// </summary>
        /// <returns>A list of ExtraField objects.</returns>
        public List<ExtraField> GetExtraFields()
        {
            if (string.IsNullOrEmpty(ExtraFieldsJson))
                return new List<ExtraField>();
                
            return JsonSerializer.Deserialize<List<ExtraField>>(ExtraFieldsJson);
        }
        
        /// <summary>
        /// Sets the extra fields from a list of ExtraField objects.
        /// </summary>
        /// <param name="extraFields">The list of ExtraField objects to set.</param>
        public void SetExtraFields(List<ExtraField> extraFields)
        {
            ExtraFieldsJson = JsonSerializer.Serialize(extraFields);
        }
    }
}