using System.Collections.Generic;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits
{
    public class ReliefASIEdgeFeature : ReliefSIEdgeFeature
    {
        public float Width1 { get; set; }

        protected override List<ReliefDimensions> Dimensions { get; } =
            JsonSerializer.Deserialize<List<ReliefDimensions>>(Resources.ReliefASIDimensions);

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            base.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.Width1 = neededDimensions.Width1;
        }
    }
}