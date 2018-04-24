using System;
using System.Windows.Data;

namespace InventorShaftGenerator.ValueConverters
{
    public class SumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double totalWidth = System.Convert.ToDouble(values[0]);
            double parentCount = System.Convert.ToDouble(values[1]);
            return totalWidth - parentCount * 20.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
