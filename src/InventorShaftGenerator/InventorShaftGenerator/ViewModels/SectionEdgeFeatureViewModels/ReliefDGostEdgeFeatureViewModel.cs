using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefGOSTUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefDGostEdgeFeatureViewModel : ReliefGostEdgeFeatureViewModel<ReliefDGostEdgeFeature>
    {
        private float reliefDepth1;

        public float ReliefDepth1
        {
            get => this.reliefDepth1;
            set => SetProperty(ref this.reliefDepth1, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.ReliefDepth1 = this.ReliefEdgeFeature.ReliefDepth1;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.ReliefDepth1 = this.reliefDepth1;
        }

        protected override void UpdateReliefParameters(ReliefDimensions dimensions)
        {
            base.UpdateReliefParameters(dimensions);

            this.ReliefDepth1 = dimensions.ReliefDepth1;
        }
    }
}