namespace LubeLoggerDashboard.Models.Database.Entities
{
    /// <summary>
    /// Represents a dynamic extra field for entities that support custom fields.
    /// </summary>
    public class ExtraField
    {
        /// <summary>
        /// Gets or sets the name of the extra field.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the value of the extra field.
        /// </summary>
        public string Value { get; set; }
    }
}