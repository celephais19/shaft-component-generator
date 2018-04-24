using InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefBSIEdgeFeatureViewModel : ReliefEdgeFeatureViewModel<ReliefBSIEdgeFeature>
    {
        private float machiningAllowance;

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }
    }
}