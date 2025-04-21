using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LubeLoggerDashboard.UI.Converters
{
    /// <summary>
    /// Converts a boolean value to a foreground brush
    /// </summary>
    public class BooleanToForegroundConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a foreground brush
        /// </summary>
        /// <param name="value">The boolean value to convert</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>Black if true, Gray if false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
            }
            
            return new SolidColorBrush(Colors.Black);
        }

        /// <summary>
        /// Converts a foreground brush back to a boolean value
        /// </summary>
        /// <param name="value">The brush to convert</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>True if the brush is black, false otherwise</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color == Colors.Black;
            }
            
            return true;
        }
    }
}