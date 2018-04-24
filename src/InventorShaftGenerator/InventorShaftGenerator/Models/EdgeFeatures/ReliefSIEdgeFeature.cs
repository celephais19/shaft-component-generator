namespace InventorShaftGenerator.Models.EdgeFeatures
{
    [EdgeFeature]
    public abstract class ReliefSIEdgeFeature : ReliefEdgeFeature
    {
        private float machiningAllowance;

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set
            {
                SetProperty(ref this.machiningAllowance, value);
                NotifyForAnyErrors();
            }
        }

        public override void UpdateFeatureParameters()
        {
            this.EdgePoint = this.EdgePosition == EdgeFeaturePosition.FirstEdge
                ? this.LinkedSection.SecondLine.StartPoint
                : this.LinkedSection.SecondLine.EndPoint;
        }
    }
}