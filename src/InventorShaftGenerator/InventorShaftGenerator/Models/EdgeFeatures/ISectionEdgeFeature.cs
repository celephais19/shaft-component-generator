using System.Drawing;

namespace InventorShaftGenerator.Models.EdgeFeatures
{
    public interface ISectionEdgeFeature : ISectionFeature
    {
        EdgeFeaturePosition EdgePosition { get; set; }
        PointF EdgePoint { get; }
    }
}