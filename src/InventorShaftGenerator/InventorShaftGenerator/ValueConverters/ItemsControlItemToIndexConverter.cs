using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace InventorShaftGenerator.ValueConverters
{
    public class ItemsControlItemToIndexConverter : ValueConverterWithAttachedParameter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var itemsControlItem = (DependencyObject) value;
            var itemsControl = (ItemsControl) (parameter ?? this.Parameter);
            int index = itemsControl.ItemContainerGenerator.IndexFromContainer(itemsControlItem);
            return index;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}