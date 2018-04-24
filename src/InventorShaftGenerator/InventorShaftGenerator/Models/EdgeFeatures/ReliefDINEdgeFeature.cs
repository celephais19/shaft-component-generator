namespace InventorShaftGenerator.Models.EdgeFeatures
{
    [EdgeFeature]
    public abstract class ReliefDinEdgeFeature : ReliefEdgeFeature
    {
        public float MachiningAllowance { get; set; }

        public override void UpdateFeatureParameters()
        {
            this.EdgePoint = this.EdgePosition == EdgeFeaturePosition.FirstEdge
                ? this.LinkedSection.SecondLine.StartPoint
                : this.LinkedSection.SecondLine.EndPoint;
        }

    }
}