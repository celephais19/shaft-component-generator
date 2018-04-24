using System.ComponentModel;
using InventorShaftGenerator.Extensions;

namespace InventorShaftGenerator.Models.SubFeatures
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DistanceFrom
    {
        [Description("Measure from the first edge")]
        FromFirstEdge,

        [Description("Measure from the second edge")]
        FromSecondEdge,

        [Description("Centered")] Centered
    }
}