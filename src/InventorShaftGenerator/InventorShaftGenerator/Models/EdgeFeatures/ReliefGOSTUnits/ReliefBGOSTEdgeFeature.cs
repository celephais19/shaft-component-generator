using System.Collections.Generic;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures.ReliefGOSTUnits
{
    public class ReliefBGostEdgeFeature : ReliefGostEdgeFeature
    {
        private float reliefDepth1;
        private float radius1;

        public float ReliefDepth1
        {
            get => reliefDepth1;
            set => SetProperty(ref this.reliefDepth1, value);
        }

        public float Radius1
        {
            get => radius1;
            set => SetProperty(ref this.radius1, value);
        }

        protected override List<ReliefDimensions> Dimensions { get; } = JsonSerializer
            .Deserialize<List<ReliefDimensions>>(Resources.ReliefBGOSTDimensions);

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            base.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.Radius1 = neededDimensions.Radius1;
            this.ReliefDepth1 = neededDimensions.ReliefDepth1;
        }
    }
}