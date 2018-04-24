using System.Collections.Generic;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits
{
    public class ReliefCDinEdgeFeature : ReliefDinEdgeFeature
    {
        protected override List<ReliefDimensions> Dimensions =>
            JsonSerializer.Deserialize<List<ReliefDimensions>>(Resources.ReliefCDINDimensions);

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            base.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.Width = neededDimensions.Width;
            this.ReliefDepth = neededDimensions.ReliefDepth;
            this.Radius = neededDimensions.Radius;
        }
    }
}