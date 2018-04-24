using System;
using System.Globalization;
using System.Windows.Data;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.ValueConverters
{
    public class SubFeatureEnumMemberToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SubFeature cylinderSubfeatureEnumMember)
            {
                switch (cylinderSubfeatureEnumMember)
                {
                    case SubFeature.ThroughHole:
                        return "Through Hole";
                    case SubFeature.RetainingRingGroove:
                        return "Retaining Ring Groove";
                    case SubFeature.RetainingRingBore:
                        return "Retaining Ring";
                    case SubFeature.KeywayGroove:
                        return "Keyway Groove";
                    case SubFeature.Wrench:
                        return "Wrench";
                    case SubFeature.ReliefDSI:
                        return "Relief-D (SI Units)";
                    case SubFeature.GrooveA:
                        return "Groove - A";
                    case SubFeature.GrooveB:
                        return "Groove - B";
                    default:
                        return string.Empty;
                }
            }

            // Else, a type of the section is polygon
            return "Through Hole";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
