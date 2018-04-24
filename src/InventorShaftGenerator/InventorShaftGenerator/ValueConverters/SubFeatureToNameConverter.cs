using System;
using System.Globalization;
using System.Windows.Data;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ValueConverters
{
    public class SubFeatureToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var subFeature = value as ISectionFeature;
            switch (subFeature)
            {
                case ThroughHoleSubFeature _:
                    return "Through Hole";
                case RetainingRingGrooveSubFeature feature when feature.LinkedSection.IsBore:
                    return "Retaining Ring";
                case RetainingRingGrooveSubFeature _:
                    return "Retaining Ring Groove";
                case KeywayGrooveSubFeature _:
                    return "Keyway Groove";
                case WrenchSubFeature _:
                    return "Wrench";
                case ReliefDSISubFeature _:
                    return "Relief-D (SI Units)";
                case GrooveASubFeature _:
                    return "Groove - A";
                case GrooveBSubFeature _:
                    return "Groove - B";

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