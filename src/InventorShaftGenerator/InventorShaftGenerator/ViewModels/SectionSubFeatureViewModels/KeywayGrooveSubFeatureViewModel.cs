using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels
{
    public class KeywayGrooveSubFeatureViewModel : KeywayGrooveFeatureViewModel<KeywayGrooveSubFeature>,
        ISubFeatureViewModel
    {
        private float distance;
        private DistanceFrom measureDistanceFrom;
        private KeywayGrooveSubFeature keywayGrooveSubFeature;

        ISectionSubFeature ISubFeatureViewModel.SubFeature
        {
            get => this.keywayGrooveSubFeature;
            set => SetProperty(ref this.keywayGrooveSubFeature, (KeywayGrooveSubFeature) value);
        }

        public float Distance
        {
            get => this.distance;
            set => SetProperty(ref this.distance, value);
        }

        public DistanceFrom MeasureDistanceFrom
        {
            get => this.measureDistanceFrom;
            set => SetProperty(ref this.measureDistanceFrom, value);
        }


        protected override void InitializeKeywayGrooveFeature()
        {
            this.KeywayGrooveFeature = this.keywayGrooveSubFeature;
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.Distance = this.KeywayGrooveFeature.Distance;
            this.MeasureDistanceFrom = this.KeywayGrooveFeature.DistanceFrom;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.KeywayGrooveFeature.Distance = this.distance;
            this.KeywayGrooveFeature.DistanceFrom = this.measureDistanceFrom;
        }

        public override string this[string columnName]
        {
            get
            {
                if (this.Section == null)
                {
                    return null;
                }

                string error = base[columnName];
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }

                switch (columnName)
                {
                    case nameof(this.Width):
                        if (this.Width > this.MainDiameter / 2)
                        {
                            error = $"Valid range is < {0:F3} mm ; {(this.MainDiameter / 2):F3} mm >";
                        }

                        break;

                    case nameof(this.Depth):
                        if (this.Depth > this.MainDiameter / 2)
                        {
                            error = $"Valid range is < {0:F3} mm ; {(this.MainDiameter / 2):F3} mm >";
                        }

                        break;

                    case nameof(this.Distance) when this.measureDistanceFrom == DistanceFrom.FromFirstEdge ||
                                                    this.measureDistanceFrom == DistanceFrom.FromSecondEdge:
                        if (this.distance > this.SectionLength)
                        {
                            error = $"Valid range is < {0:F3} mm ; {this.SectionLength:F3} mm >";
                        }

                        break;

                    case nameof(this.Distance) when this.measureDistanceFrom == DistanceFrom.Centered:
                        if (this.distance > this.SectionLength / 2 || this.distance < this.SectionLength / -2)
                        {
                            error =
                                $"Valid range is < {this.SectionLength / -2:F3} mm ; {this.SectionLength / 2:F3} mm >";
                        }

                        break;
                }

                return error;
            }
        }
    }
}