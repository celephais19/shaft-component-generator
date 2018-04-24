using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace InventorShaftGenerator.ValueConverters
{
    public class ObjectIsTypeOfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type potentialDerivedType = value?.GetType();
            Type potentialBaseType = parameter as Type;
            return potentialDerivedType != null
                   && potentialBaseType != null
                   && (potentialDerivedType.IsSubclassOf(potentialBaseType) || potentialDerivedType
                                                                               .GetInterfaces().Any(iType =>
                                                                                   iType == potentialBaseType));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}