using System;
using System.Globalization;
using System.Windows.Data;

namespace InventorShaftGenerator.ValueConverters
{
    public class BaseToDerivedTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var potentialDerivedType = (Type) parameter;
            return System.Convert.ChangeType(value, potentialDerivedType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
