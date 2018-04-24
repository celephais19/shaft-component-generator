using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace InventorShaftGenerator.ValueConverters
{
    public class ParentCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int parentCount = 1;
            DependencyObject o = VisualTreeHelper.GetParent(value as DependencyObject);
            while (o != null && o.GetType().FullName != typeof(ScrollContentPresenter).FullName)
            {
                if (o.GetType().FullName == typeof(TreeViewItem).FullName ||
                    o.GetType().FullName == typeof(ListBoxItem).FullName)
                    parentCount += 1;
                o = VisualTreeHelper.GetParent(o);
            }

            return parentCount;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}