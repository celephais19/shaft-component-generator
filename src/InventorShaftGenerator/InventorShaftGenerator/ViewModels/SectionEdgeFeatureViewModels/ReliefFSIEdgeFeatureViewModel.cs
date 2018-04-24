using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefFSIEdgeFeatureViewModel : ReliefEdgeFeatureViewModel<ReliefFSIEdgeFeature>
    {
        private float incidenceAngle;
        private float width2;
        private float reliefDepth2;
        private float machiningAllowance;

        public float IncidenceAngle
        {
            get => this.incidenceAngle;
            set => SetProperty(ref this.incidenceAngle, value);
        }

        public float Width2
        {
            get => this.width2;
            set => SetProperty(ref this.width2, value);
        }

        public float ReliefDepth2
        {
            get => this.reliefDepth2;
            set => SetProperty(ref this.reliefDepth2, value);
        }

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.IncidenceAngle = this.ReliefEdgeFeature.IncidenceAngle;
            this.Width2 = this.ReliefEdgeFeature.Width1;
            this.ReliefDepth2 = this.ReliefEdgeFeature.ReliefDepth2;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.IncidenceAngle = this.IncidenceAngle;
            this.ReliefEdgeFeature.Width1 = this.Width2;
            this.ReliefEdgeFeature.ReliefDepth2 = this.ReliefDepth2;
        }

        protected override void UpdateReliefParameters(ReliefDimensions dimensions)
        {
            base.UpdateReliefParameters(dimensions);

            this.Width2 = dimensions.Width2;
            this.ReliefDepth2 = dimensions.ReliefDepth2;
        }

        public override string this[string columnName]
        {
            get
            {
                string error = base[columnName];
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }

                switch (columnName)
                {
                    case nameof(this.IncidenceAngle):
                        if (this.incidenceAngle < 10 || this.incidenceAngle > 85)
                        {
                            error = $"Valid range is < {10:F2} deg ; {85:F2} deg >";
                            this.ErrorFields.Add(nameof(this.IncidenceAngle));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.IncidenceAngle));
                        }

                        break;
                }

                return error;
            }
        }
    }
}