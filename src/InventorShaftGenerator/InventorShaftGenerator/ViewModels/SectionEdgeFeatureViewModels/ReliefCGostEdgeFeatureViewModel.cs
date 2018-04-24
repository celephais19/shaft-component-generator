using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefGOSTUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefCGostEdgeFeatureViewModel : ReliefGostEdgeFeatureViewModel<ReliefCGostEdgeFeature>
    {
        private float radius1;

        public float Radius1
        {
            get => this.radius1;
            set => SetProperty(ref this.radius1, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.Radius1 = this.ReliefEdgeFeature.Radius1;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.Radius1 = this.radius1;
        }

        protected override void UpdateReliefParameters(ReliefDimensions dimensions)
        {
            base.UpdateReliefParameters(dimensions);

            this.Radius1 = dimensions.Radius1;
        }
    }
}