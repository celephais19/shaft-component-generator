using System;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.Models
{
    public static class SectionFeatureInterfaceExtensions
    {
        public static bool IsEdgeFeature(this ISectionFeature sectionFeature)
        {
            bool hasEdgeFeatureAttribute =
                Attribute.GetCustomAttribute(sectionFeature.GetType(), typeof(EdgeFeatureAttribute)) != null;
            return hasEdgeFeatureAttribute;
        }
    }
}