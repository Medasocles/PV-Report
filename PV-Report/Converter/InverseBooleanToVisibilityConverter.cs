using System;
using System.Globalization;
using System.Windows;

namespace PvReport.Converter
{
    /// <summary>
    /// Represents the converter for string length to visibility
    /// </summary>
    public class InverseBooleanToVisibilityConverter : BaseValueConverter<InverseBooleanToVisibilityConverter>
    {
        /// <summary>
        /// Convert boolean to visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? Visibility.Collapsed : Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Convert visibility to boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}