using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventorShaftGenerator.ValueConverters
{
    public abstract class ValueConverterWithAttachedParameter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ParameterProperty = DependencyProperty.RegisterAttached(
            "Parameter", typeof(object), typeof(ValueConverterWithAttachedParameter));

        public object Parameter
        {
            get => GetValue(ParameterProperty);
            set => SetValue(ParameterProperty, value);
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}