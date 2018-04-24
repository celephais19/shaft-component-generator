using System.Collections.Generic;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits
{
    public class ReliefBDinEdgeFeature : ReliefDinEdgeFeature
    {
        public float Width1 { get; set; }
        public float Width2 { get; set; }
        public float Angle { get; set; }

        protected override List<ReliefDimensions> Dimensions =>
            JsonSerializer.Deserialize<List<ReliefDimensions>>(Resources.ReliefBDINDimensions);

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            base.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.Width1 = neededDimensions.Width1;
            this.Width2 = neededDimensions.Width2;
            this.Angle = 30;
        }
    }
}