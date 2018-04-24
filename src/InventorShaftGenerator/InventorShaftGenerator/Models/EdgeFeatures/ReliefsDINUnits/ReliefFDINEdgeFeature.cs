using System.Collections.Generic;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits
{
    public class ReliefFDinEdgeFeature : ReliefDinEdgeFeature
    {
        public float Width1 { get; set; }
        public float ReliefDepth1 { get; set; }

        protected override List<ReliefDimensions> Dimensions =>
            JsonSerializer.Deserialize<List<ReliefDimensions>>(Resources.ReliefFDINDimensions);

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            base.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.Width1 = neededDimensions.Width1;
            this.ReliefDepth1 = neededDimensions.ReliefDepth1;
        }
    }
}