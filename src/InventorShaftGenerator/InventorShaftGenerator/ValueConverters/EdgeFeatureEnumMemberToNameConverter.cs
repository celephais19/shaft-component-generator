using System;
using System.Globalization;
using System.Windows.Data;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ValueConverters
{
    public class EdgeFeatureEnumMemberToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var edgeFeature = (EdgeFeature) value;
            switch (edgeFeature)
            {
                case EdgeFeature.None:
                    return "No feature";
                case EdgeFeature.Chamfer:
                    return "Chamfer";
                case EdgeFeature.Fillet:
                    return "Fillet";
                case EdgeFeature.Thread:
                    return "Thread";
                case EdgeFeature.LockNutGroove:
                    return "Lock Nut Groove";
                case EdgeFeature.PlainKeywayGroove:
                    return "Plain Keyway Groove";
                case EdgeFeature.KeywayGrooveRoundedEnd:
                    return "Keyway Groove with one rounded end";
                case EdgeFeature.ReliefSI:
                    return "Reliefs (SI Units)";
                case EdgeFeature.ReliefDIN:
                    return "Reliefs (DIN)";
                case EdgeFeature.ReliefGOST:
                    return "Reliefs (GOST)";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
