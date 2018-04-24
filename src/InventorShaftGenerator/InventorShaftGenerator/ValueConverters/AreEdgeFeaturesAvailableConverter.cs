using System;
using System.Globalization;
using System.Windows.Data;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ValueConverters
{
    public class AreEdgeFeaturesAvailableConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] is ISectionFeature && (EdgeFeature) values[1] != EdgeFeature.NotAvailable;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}