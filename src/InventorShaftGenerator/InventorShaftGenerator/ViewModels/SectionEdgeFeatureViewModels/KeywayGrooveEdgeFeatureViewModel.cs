using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public abstract class KeywayGrooveEdgeFeatureViewModel<TKeywayGrooveEdgeFeature> :
        KeywayGrooveFeatureViewModel<TKeywayGrooveEdgeFeature>,
        IEdgeFeatureViewModel
        where TKeywayGrooveEdgeFeature : KeywayGrooveFeature
    {
        public EdgeFeaturePosition EdgeFeaturePosition { get; set; }

        protected override void InitializeKeywayGrooveFeature()
        {
            if (this.EdgeFeaturePosition == EdgeFeaturePosition.FirstEdge)
            {
                this.KeywayGrooveFeature = (TKeywayGrooveEdgeFeature) this.Section.FirstEdgeFeature;
            }
            else
            {
                this.KeywayGrooveFeature = (TKeywayGrooveEdgeFeature) this.Section.SecondEdgeFeature;
            }
        }
    }
}