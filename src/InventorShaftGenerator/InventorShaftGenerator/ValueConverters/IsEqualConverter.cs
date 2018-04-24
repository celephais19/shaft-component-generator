using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace InventorShaftGenerator.ValueConverters
{
    public sealed class IsEqualConverter : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return DependencyProperty.UnsetValue;
            return ConverterHelper.ResultWithParameterValue(
                p => String.Equals(value.ToString(), p, StringComparison.CurrentCultureIgnoreCase), targetType,
                parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterString = parameter.ToString();
            bool isTrue = false;
            if (parameterString.Contains("?"))
            {
                var compareValue = parameterString.Substring(0, parameterString.IndexOf("?", StringComparison.Ordinal));
                isTrue = values.All(i => i?.ToString() == compareValue);
            }
            else
            {
                isTrue = values.Skip(1).All(i => i?.ToString() == values[0]?.ToString());
            }

            return ConverterHelper.Result<bool?>(isTrue, i => i, targetType, parameter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}