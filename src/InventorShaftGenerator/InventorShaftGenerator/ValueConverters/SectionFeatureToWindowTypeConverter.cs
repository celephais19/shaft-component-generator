using System;
using System.Globalization;
using System.Windows.Data;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.ValueConverters
{
    public class SectionFeatureToWindowTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var sectionFeature = (ISectionFeature) value;
            bool isEdgeFeature = sectionFeature.IsEdgeFeature();

            var sectionFeatureWindowType = Type.GetType(
                isEdgeFeature
                    ? $"InventorShaftGenerator.Views.EdgeFeatureWindows.{sectionFeature.GetType().Name + "Window"}"
                    : $"InventorShaftGenerator.Views.SubFeatureWindows.{sectionFeature.GetType().Name + "Window"}");

            return sectionFeatureWindowType ?? throw new ArgumentException(
                       $"Cannot find a window`s type for the {sectionFeature.GetType().Name}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}