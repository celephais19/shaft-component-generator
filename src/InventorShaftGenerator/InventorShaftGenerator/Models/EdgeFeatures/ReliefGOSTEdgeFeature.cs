using System.Linq;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Models.EdgeFeatures
{
    [EdgeFeature]
    public abstract class ReliefGostEdgeFeature : ReliefEdgeFeature
    {
        private float[] widthes;
        private float machiningAllowance;

        public float[] Widthes
        {
            get => widthes;
            set => SetProperty(ref this.widthes, value);
        }

        public float MachiningAllowance
        {
            get => machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }

        public override void UpdateFeatureParameters()
        {
            this.EdgePoint = this.EdgePosition == EdgeFeaturePosition.FirstEdge
                ? this.LinkedSection.SecondLine.StartPoint
                : this.LinkedSection.SecondLine.EndPoint;
        }

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            base.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.Widthes = neededDimensions.Widthes;
            this.Width = this.widthes.FirstOrDefault();
            
            
        }
    }
}