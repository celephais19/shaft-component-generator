using System.Collections.Generic;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits
{
    public class ReliefGSIEdgeFeature : ReliefSIEdgeFeature
    {
        public float IncidenceAngle { get; set; }

        protected override List<ReliefDimensions> Dimensions { get; } =
            JsonSerializer.Deserialize<List<ReliefDimensions>>(Resources.ReliefGSIDimensions);

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            base.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.Radius = neededDimensions.Radius;
            this.IncidenceAngle = 30;
        }
    }
}