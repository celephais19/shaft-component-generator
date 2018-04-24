using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefASIEdgeFeatureViewModel : ReliefEdgeFeatureViewModel<ReliefASIEdgeFeature>
    {
        private float width2;
        private float machiningAllowance;

        public float Width2
        {
            get => this.width2;
            set => SetProperty(ref this.width2, value);
        }

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.Width2 = this.ReliefEdgeFeature.Width1;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.Width1 = this.width2;
        }

        protected override void UpdateReliefParameters(ReliefDimensions dimensions)
        {
            base.UpdateReliefParameters(dimensions);

            this.Width2 = dimensions.Width2;
        }
    }
}