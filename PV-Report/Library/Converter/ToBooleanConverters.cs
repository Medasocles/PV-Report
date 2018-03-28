using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PvReport.Library.Converter
{
    #region NULL value converters

    /// <summary>
    /// Represents a <see cref="IValueConverter"/> that inverses a given boolean value.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
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

            return !(bool)value;

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
            return Convert(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// Represents a <see cref="IValueConverter"/> that converts a value to <see cref="bool"/>.
    /// Method <see cref="Convert"/> returns true if the value of the argument value is not null.
    /// Returns false if value is null.
    /// Method <see cref="ConvertBack"/> is not implemented and throws <see cref="NotImplementedException"/>.
    /// </summary>
    public class NotNullValueToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="value"/> to <see cref="bool"/>.
        /// Returns true if <see cref="value"/> is not null (or empty for string value). 
        /// Returns false else.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        /// <summary>
        /// Not implemented.
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
    /// Represents a <see cref="IValueConverter"/> that converts a value to <see cref="bool"/>.
    /// Method <see cref="Convert"/> returns true if the value of the argument value is not null.
    /// Returns false if value is null.
    /// Method <see cref="ConvertBack"/> is not implemented and throws <see cref="NotImplementedException"/>.
    /// </summary>
    public class StringIsNullorEmptyToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="value"/> to <see cref="bool"/>.
        /// Returns true if <see cref="value"/> is not null (or empty for string value). 
        /// Returns false else.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string);
        }

        /// <summary>
        /// Not implemented.
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
    /// Represents a <see cref="IValueConverter"/> that converts a value to <see cref="bool"/>.
    /// Method <see cref="Convert"/> returns true if the value of the argument value is not null.
    /// Returns false if value is null.
    /// Method <see cref="ConvertBack"/> is not implemented and throws <see cref="NotImplementedException"/>.
    /// </summary>
    public class StringIsNotNullorEmptyToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="value"/> to <see cref="bool"/>.
        /// Returns true if <see cref="value"/> is not null (or empty for string value). 
        /// Returns false else.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        /// <summary>
        /// Not implemented.
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
    /// Represents a <see cref="IValueConverter"/> that converts a list of values to <see cref="bool"/>.
    /// Method <see cref="Convert"/> returns false if all values of the given values argument
    /// are NULL. For any other values of the values argument the method returns true.
    /// 
    /// Method <see cref="ConvertBack"/> is not implemented and throws <see cref="NotImplementedException"/>.
    /// </summary>
    public class MultiNotNullValueToBooleanConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts <see cref="values"/> to <see cref="bool"/>.
        /// Returns true if all <see cref="values"/> are not null (or empty for string values). 
        /// Returns false else.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string)
                return values.All(value => !string.IsNullOrEmpty((string)value));

            return values.All(value => value != null);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility && (Visibility)value == Visibility.Visible)
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents a <see cref="IMultiValueConverter"/> that converts multiple <see cref="Boolean"/> values
    /// to a single boolean value using logical AND operation. 
    /// All values true => return true; Any value false => return false.
    /// </summary>
    public class MultiBooleanAndConverter : IMultiValueConverter
    {
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
            return boolValues.All(value => value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents a <see cref="IMultiValueConverter"/> that converts multiple <see cref="Boolean"/> values
    /// to a single boolean value using logical AND operation. 
    /// All values true => return true; Any value false => return false.
    /// </summary>
    public class MultiBooleanOrConverter : IMultiValueConverter
    {
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
            var ret = boolValues.Any(value => value == true);
            return ret;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { 
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents the converter for string length to visibility
    /// </summary>
    public class NumberToBooleanConverter : IValueConverter
    {
        public double TrueThreshold { get; set; } = 0;
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            var ret = value is int i && i > TrueThreshold || value is double && (double)value > TrueThreshold;

            return Invert ? !ret : ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("NumberToVisibilityConverter cannot convert back from Visibility to Number");
        }
    }

    #endregion
}
