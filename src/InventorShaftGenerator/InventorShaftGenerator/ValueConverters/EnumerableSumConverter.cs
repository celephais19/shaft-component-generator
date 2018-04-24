using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace InventorShaftGenerator.ValueConverters
{
    public class EnumerableSumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is IEnumerable enumerable))
            {
                return DependencyProperty.UnsetValue;
            }

            IEnumerable<object> collection = enumerable.Cast<object>().ToList();

            PropertyInfo property = null;
            if (parameter is string propertyName && collection.Any())
            {
                property = collection.First().GetType().GetProperty(propertyName);
            }

            return collection.Select(x => System.Convert.ToDouble(property != null ? property.GetValue(x) : x)).Sum();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
