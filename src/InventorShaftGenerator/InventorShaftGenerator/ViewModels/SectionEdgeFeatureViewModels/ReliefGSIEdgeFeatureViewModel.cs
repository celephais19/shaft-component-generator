using InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefGSIEdgeFeatureViewModel : ReliefEdgeFeatureViewModel<ReliefGSIEdgeFeature>
    {
        private float incidenceAngle;
        private float machiningAllowance;

        public float IncidenceAngle
        {
            get => this.incidenceAngle;
            set => SetProperty(ref this.incidenceAngle, value);
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
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.IncidenceAngle = this.incidenceAngle;
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
                        if (this.incidenceAngle < 15 || this.incidenceAngle > 85)
                        {
                            error = $"Valid range is < {15:F2} deg ; {85:F2} deg >";
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