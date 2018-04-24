using System.Collections.Generic;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits
{
    public class ReliefFSIEdgeFeature : ReliefSIEdgeFeature
    {
        public float IncidenceAngle { get; set; }
        public float Width1 { get; set; }
        public float ReliefDepth2 { get; set; }

        protected override List<ReliefDimensions> Dimensions { get; } =
            JsonSerializer.Deserialize<List<ReliefDimensions>>(Resources.ReliefFSIDimensions);

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            base.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.IncidenceAngle = 30;
            this.Width1 = neededDimensions.Width;
            this.ReliefDepth2 = neededDimensions.ReliefDepth2;
            this.Radius = neededDimensions.Radius;
        }
    }
}