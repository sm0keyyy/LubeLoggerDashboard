using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LubeLoggerDashboard.UI.Converters
{
    /// <summary>
    /// Converts a boolean value to a Visibility value (inverse of BooleanToVisibilityConverter)
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a Visibility value
        /// </summary>
        /// <param name="value">The boolean value to convert</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>Visibility.Collapsed if true, Visibility.Visible if false</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            
            return Visibility.Visible;
        }

        /// <summary>
        /// Converts a Visibility value back to a boolean value
        /// </summary>
        /// <param name="value">The Visibility value to convert</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="culture">The culture to use in the converter</param>
        /// <returns>False if Visibility.Visible, true otherwise</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            
            return true;
        }
    }
}