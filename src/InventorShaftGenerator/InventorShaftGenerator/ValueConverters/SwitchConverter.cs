using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Markup;

namespace InventorShaftGenerator.ValueConverters
{
    [ContentProperty(nameof(SwitchConverter.Cases))]
    public class SwitchConverter : IValueConverter
    {
        public List<SwitchConverterCase> Cases { get; set; } = new List<SwitchConverterCase>();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object results = null;

            if (this.Cases == null || this.Cases.Count == 0)
            {
                return null;
            }

            foreach (var targetCase in this.Cases)
            {
                if (object.Equals(value, targetCase.When) ||
                    string.Equals(value?.ToString(), targetCase.When?.ToString(), StringComparison.Ordinal))
                {
                    results = targetCase.Then;

                    break;
                }
            }

            return results;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ContentProperty(nameof(SwitchConverterCase.Then))]
    public class SwitchConverterCase
    {
        public SwitchConverterCase()
        {
        }

        public SwitchConverterCase(string when, object then)
        {
            this.Then = then;
            this.When = when;
        }

        public object When { get; set; }
        public object Then { get; set; }

        public override string ToString()
        {
            return $"When={this.When}; Then={this.Then}";
        }
    }
}