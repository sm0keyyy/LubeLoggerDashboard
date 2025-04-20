using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LubeLoggerDashboard.Converters
{
    /// <summary>
    /// Converts a boolean value to a background brush
    /// </summary>
    public class BooleanToBackgroundConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a background brush
        /// </summary>
        /// <param name="value">The boolean value to convert</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The color to use when true (as a hex string)</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>The specified color brush if true, transparent if false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                if (parameter is string colorString)
                {
                    try
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString));
                    }
                    catch
                    {
                        // If color conversion fails, use a default color
                        return new SolidColorBrush(Colors.LightBlue);
                    }
                }
                
                // Default selected color
                return new SolidColorBrush(Colors.LightBlue);
            }
            
            // Not selected
            return new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Converts a background brush back to a boolean value
        /// </summary>
        /// <param name="value">The brush to convert</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>True if the brush is not transparent, false otherwise</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color != Colors.Transparent;
            }
            
            return false;
        }
    }
}