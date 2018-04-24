using System;
using System.Globalization;
using System.Windows.Data;

namespace InventorShaftGenerator.ValueConverters
{
    public class WidthRelativeToParentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parentWidth = (double) value;
            return parentWidth * 0.9;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
