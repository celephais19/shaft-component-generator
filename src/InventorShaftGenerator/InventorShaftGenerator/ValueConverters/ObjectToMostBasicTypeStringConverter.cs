using System;
using System.Globalization;
using System.Windows.Data;

namespace InventorShaftGenerator.ValueConverters
{
    public class ObjectToMostBasicTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type baseType;
            do
            {
                baseType = value.GetType().BaseType;
            } while (baseType.BaseType != typeof(object));

            return baseType.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
