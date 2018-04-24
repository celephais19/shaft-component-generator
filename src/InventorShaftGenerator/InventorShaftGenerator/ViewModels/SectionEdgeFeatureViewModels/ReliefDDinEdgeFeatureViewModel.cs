using InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefDDinEdgeFeatureViewModel : ReliefEdgeFeatureViewModel<ReliefDDinEdgeFeature>
    {
        private float width1;
        
        private float machiningAllowance;

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }

        public float Width1
        {
            get => this.width1;
            set => SetProperty(ref this.width1, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.Width1 = this.ReliefEdgeFeature.Width1;
            this.MachiningAllowance = this.ReliefEdgeFeature.MachiningAllowance;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.Width1 = this.width1;

            this.ReliefEdgeFeature.MachiningAllowance = this.machiningAllowance;
        }
    }
}