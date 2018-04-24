using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.ValueConverters
{
    public class ShaftItemToSelectItemCmdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ICommand[] commands = (ICommand[]) parameter;
            switch (value)
            {
                case ShaftSection _:
                    return null;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
