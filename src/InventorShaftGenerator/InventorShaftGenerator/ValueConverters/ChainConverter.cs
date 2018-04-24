using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace InventorShaftGenerator.ValueConverters
{
    public class ChainConverter : List<ValueConverterWithAttachedParameter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return this.Aggregate(value,
                (current, converter) => converter.Convert(current, targetType,
                        converter.Parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}