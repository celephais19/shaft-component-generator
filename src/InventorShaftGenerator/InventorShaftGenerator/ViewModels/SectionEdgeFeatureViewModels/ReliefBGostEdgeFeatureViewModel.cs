using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefGOSTUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefBGostEdgeFeatureViewModel : ReliefGostEdgeFeatureViewModel<ReliefBGostEdgeFeature>
    {
        private float reliefDepth1;
        private float radius1;

        public float ReliefDepth1
        {
            get => this.reliefDepth1;
            set => SetProperty(ref this.reliefDepth1, value);
        }

        public float Radius1
        {
            get => this.radius1;
            set => SetProperty(ref this.radius1, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.ReliefDepth1 = this.ReliefEdgeFeature.ReliefDepth1;
            this.Radius1 = this.ReliefEdgeFeature.Radius1;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.ReliefDepth1 = this.reliefDepth1;
            this.ReliefEdgeFeature.Radius1 = this.radius1;
        }

        protected override void UpdateReliefParameters(ReliefDimensions dimensions)
        {
            base.UpdateReliefParameters(dimensions);

            this.Radius1 = dimensions.Radius1;
            this.ReliefDepth1 = dimensions.ReliefDepth1;
        }
    }
}