using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PvReport.Library.Converter
{
    /// <summary>
    /// Represents a <see cref="IValueConverter"/> that inverses a given boolean value.
    /// </summary>
    public class BooleanToBrushConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; }
        public Brush FalseBrush { get; set; }

        /// <summary>
        /// Converts the given boolean <see cref="value"/> to its inverse boolean.
        /// If value is not of type boolean, <see cref="DependencyProperty.UnsetValue"/> is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return DependencyProperty.UnsetValue;

            return (bool)value ? TrueBrush : FalseBrush;
        }

        /// <summary>
        /// Converts <see cref="value"/> to its inverse boolean.
        /// If value is not of type boolean, <see cref="DependencyProperty.UnsetValue"/> is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Method BooleanToBrushConverter.ConvertBack() has no implementation.");
        }
    }
}
