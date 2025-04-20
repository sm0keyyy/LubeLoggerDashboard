using System;
using LubeLoggerDashboard.Models.Database.Enums;

namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents a reminder for a vehicle.
    /// </summary>
    public class Reminder : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the vehicle this reminder belongs to.
        /// </summary>
        public int VehicleId { get; set; }
        
        /// <summary>
        /// Gets or sets the title of the reminder.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the reminder.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets the due date of the reminder.
        /// </summary>
        public DateTime DueDate { get; set; }
        
        /// <summary>
        /// Gets or sets the due odometer reading for the reminder.
        /// </summary>
        public int? DueOdometer { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the reminder is active.
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the reminder has been dismissed.
        /// </summary>
        public bool IsDismissed { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the reminder was dismissed.
        /// </summary>
        public DateTime? DismissedDate { get; set; }
        
        /// <summary>
        /// Gets or sets the type of the reminder (Date, Odometer, Both).
        /// </summary>
        public ReminderType Type { get; set; }
        
        /// <summary>
        /// Gets or sets the frequency of the recurring reminder.
        /// </summary>
        public ReminderFrequency? Frequency { get; set; }
        
        /// <summary>
        /// Gets or sets the value for the frequency (e.g., number of days, miles, etc.).
        /// </summary>
        public int? FrequencyValue { get; set; }
        
        /// <summary>
        /// Gets or sets the vehicle this reminder belongs to.
        /// </summary>
        public virtual Vehicle Vehicle { get; set; }
    }
}