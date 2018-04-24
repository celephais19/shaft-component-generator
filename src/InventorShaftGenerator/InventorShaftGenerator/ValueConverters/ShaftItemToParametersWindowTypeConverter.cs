using System;
using System.Globalization;
using System.Windows.Data;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Views.SectionDimensionsWindows;

namespace InventorShaftGenerator.ValueConverters
{
    public class ShaftItemToParametersWindowTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type shaftItemParametersWindowType = null;

            switch (value)
            {
                case ISectionFeature sectionFeature:
                    shaftItemParametersWindowType = Type.GetType(sectionFeature.IsEdgeFeature()
                        ? $"InventorShaftGenerator.Views.EdgeFeatureWindows.{sectionFeature.GetType().Name + "Window"}"
                        : $"InventorShaftGenerator.Views.SubFeatureWindows.{sectionFeature.GetType().Name + "Window"}");
                    break;

                case ShaftSection shaftSection:
                    switch (shaftSection)
                    {
                        case CylinderSection _:
                            shaftItemParametersWindowType = typeof(CylinderDimensionsWindow);
                            break;
                        case ConeSection _:
                            shaftItemParametersWindowType = typeof(ConeDimensionsWindow);
                            break;
                        case PolygonSection _:
                            shaftItemParametersWindowType = typeof(PolygonDimensionsWindow);
                            break;
                    }

                    break;

                default:
                    return null;
            }

            return shaftItemParametersWindowType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}