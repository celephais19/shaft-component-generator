using System.Linq;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public abstract class ReliefGostEdgeFeatureViewModel<TReliefGostEdgeFeauture> :
        ReliefEdgeFeatureViewModel<TReliefGostEdgeFeauture>
        where TReliefGostEdgeFeauture : ReliefGostEdgeFeature
    {
        private float[] widthes;
        private float machiningAllowance;

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }

        public float[] Widthes
        {
            get => this.widthes;
            set => SetProperty(ref this.widthes, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.Widthes = this.ReliefEdgeFeature.Widthes;
            this.MachiningAllowance = this.ReliefEdgeFeature.MachiningAllowance;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.MachiningAllowance = this.machiningAllowance;
            this.ReliefEdgeFeature.Width = this.Width;
        }

        protected override void UpdateReliefParameters(ReliefDimensions dimensions)
        {
            base.UpdateReliefParameters(dimensions);

            this.Widthes = dimensions.Widthes;
            this.Width = this.widthes.FirstOrDefault();
        }
    }
}