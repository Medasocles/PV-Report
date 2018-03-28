using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PvReport.Library.Converter
{
    #region Null value converters
    /// <summary>
    /// Represents a <see cref="IValueConverter"/> that converts an object value to <see cref="Visibility"/>.
    /// Method <see cref="Convert"/> returns <see cref="Visibility.Collapsed"/> if the value of the given value argument
    /// is NULL. For any other values of the value argument the method returns <see cref="Visibility.Visible"/>.
    /// Method <see cref="ConvertBack"/> is not implemented and throws <see cref="NotImplementedException"/>.
    /// </summary>
    public class NotNullValueToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Returns <see cref="Visibility.Collapsed"/> if the value of <see cref="value"/> argument
        /// is NULL. For any other values of <see cref="value"/> the method returns <see cref="Visibility.Visible"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Not implemented and throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents a <see cref="IValueConverter"/> that converts an object value to <see cref="Visibility"/>.
    /// Method <see cref="Convert"/> returns <see cref="Visibility.Collapsed"/> if the value of the given value argument
    /// is NULL. For any other values of the value argument the method returns <see cref="Visibility.Visible"/>.
    /// Method <see cref="ConvertBack"/> is not implemented and throws <see cref="NotImplementedException"/>.
    /// </summary>
    public class MultiNullValueToVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Returns <see cref="Visibility.Collapsed"/> if the value of <see cref="value"/> argument
        /// is NULL. For any other values of <see cref="values"/> the method returns <see cref="Visibility.Visible"/>
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(value => value == null) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Not implemented and throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Boolean value converters

    /// <summary>
    /// Represents the converter for string length to visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets a value indicting if conversion shall be inverted
        /// </summary>
        public bool Invert { get; set; }

        /// <summary>
        /// Convert boolean to visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool boolVal;
                if (Invert)
                    boolVal = !((bool)value);
                else
                    boolVal = (bool)value;

                return boolVal ? Visibility.Visible : Visibility.Collapsed;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                if (Invert)
                    return (Visibility)value != Visibility.Visible;

                return (Visibility)value == Visibility.Visible ? true : false;
            }

            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// Represents a <see cref="IMultiValueConverter"/> that converts multiple <see cref="Boolean"/> values
    /// to a <see cref="Visibility"/> value.
    /// </summary>
    public class MultiBooleanAndToVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts <see cref="Boolean"/> values to <see cref="Visibility"/> value.
        /// Returns <see cref="Visibility.Visible"/> if all values of type bool are true.
        /// Returns <see cref="Visibility.Collapsed"/> if one of the values of type bool is false.
        /// </summary>
        /// <param name="values">Array of boolean values.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValues = new List<bool>();
            foreach (var value in values)
            {
                if (value is bool)
                    boolValues.Add((bool)value);
                else
                    boolValues.Add(false);
            }

            return boolValues.Any(value => value == false) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Number value converters

    /// <summary>
    /// Represents the converter for string length to visibility
    /// </summary>
    public class NumberGreaterZeroToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var ret = Visibility.Collapsed;

            if (value is int && (int)value > 0) ret = Visibility.Visible;
            if (value is double && (double)value > 0) ret = Visibility.Visible;

            if (Invert)
                ret = (ret == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ZeroToVisibilityConverter cannot convert back from Visibility to Number");
        }
    }

    /// <summary>
    /// Represents the converter for string length to visibility
    /// </summary>
    public class NumberToVisibilityConverter : IValueConverter
    {
        public double Threshold { get; set; } = 0;
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var ret = Visibility.Collapsed;

            if (value is int i && i > Threshold) ret = Visibility.Visible;
            if (value is double d && d > Threshold) ret = Visibility.Visible;

            if (Invert)
                ret = (ret == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ZeroToVisibilityConverter cannot convert back from Visibility to Number");
        }
    }

    #endregion

    #region String length converter

    /// <summary>
    /// Represents the converter for string length to visibility
    /// </summary>
    public class StringIsNullOrEmptyToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// converts string length to visibility
        /// if string is null or empty, visibility is set to collapsed, to visibible else
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// not in use
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("StringIsNullOrEmptyToVisibilityConverter supports only OneWay-binding");
        }

    }

    #endregion

    /// <summary>
    /// Represents the converter for string length to visibility
    /// </summary>
    public class InverseVisibilityConverter : IValueConverter
    {
        public Visibility InversedVisibleReturnValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                if ((Visibility)value == Visibility.Visible)
                    return InversedVisibleReturnValue;

                return Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("NumberToVisibilityConverter cannot convert back from Visibility to Number");
        }
    }


    #region DEBUG compile constant converter

    /// <summary>
    /// Represents the converter for string length to visibility
    /// </summary>
    public class DebugToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register(
            "Visibility", typeof(Visibility), typeof(DebugToVisibilityConverter), new PropertyMetadata(default(Visibility)));

        public Visibility Visibility
        {
            get => (Visibility)GetValue(VisibilityProperty);
            set => SetValue(VisibilityProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            Visibility = Visibility.Visible;
#else
            Visibility = Visibility.Collapsed;
#endif
            return Visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("DebugToVisibilityConverter cannot convert back from Visibility to Number");
        }
    }

    #endregion
}
