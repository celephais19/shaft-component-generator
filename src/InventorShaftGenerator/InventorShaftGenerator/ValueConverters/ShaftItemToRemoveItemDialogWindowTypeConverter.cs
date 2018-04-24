using System;
using System.Globalization;
using System.Windows.Data;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ValueConverters
{
    public class ShaftItemToRemoveItemDialogWindowTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case ISectionFeature _:
                    return typeof(RemoveSubFeatureDialogWindow);

                case ShaftSection _:
                    return typeof(RemoveSectionDialogWindow);

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}