using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.Infrastructure
{
    public struct FeatureConstructionError
    {
        public ISectionFeature Feature { get; }

        public FeatureConstructionError(ISectionFeature feature)
        {
            this.Feature = feature;
        }
    }
}
