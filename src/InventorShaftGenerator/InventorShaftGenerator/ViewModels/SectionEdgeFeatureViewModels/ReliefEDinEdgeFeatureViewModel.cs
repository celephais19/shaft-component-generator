using InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefEDinEdgeFeatureViewModel : ReliefEdgeFeatureViewModel<ReliefEDinEdgeFeature>
    {
        private float machiningAllowance;

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.MachiningAllowance = this.ReliefEdgeFeature.MachiningAllowance;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.MachiningAllowance = this.machiningAllowance;
        }
    }
}