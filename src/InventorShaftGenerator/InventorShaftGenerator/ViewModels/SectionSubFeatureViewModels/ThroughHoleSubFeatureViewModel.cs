using System.ComponentModel;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels
{
    public class ThroughHoleSubFeatureViewModel : SectionFeatureViewModel, ISubFeatureViewModel, IDataErrorInfo
    {
        private float mainDiameter;
        private float sectionLength;
        private float holeDiameter;
        private float chamfering;
        private float angle;
        private float distance;
        private DistanceFrom measureDistanceFrom;
        private ThroughHoleSubFeature throughHoleSubFeature;

        public ISectionSubFeature SubFeature
        {
            get => this.throughHoleSubFeature;
            set => SetProperty(ref this.throughHoleSubFeature, (ThroughHoleSubFeature) value);
        }

        public float MainDiameter
        {
            get => this.mainDiameter;
            set => SetProperty(ref this.mainDiameter, value);
        }

        public float SectionLength
        {
            get => this.sectionLength;
            set => SetProperty(ref this.sectionLength, value);
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

        public float HoleDiameter
        {
            get => this.holeDiameter;
            set => SetProperty(ref this.holeDiameter, value);
        }

        public float Chamfering
        {
            get => this.chamfering;
            set => SetProperty(ref this.chamfering, value);
        }

        public float Angle
        {
            get => this.angle;
            set => SetProperty(ref this.angle, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            switch (this.Section)
            {
                case CylinderSection cylinderSection:
                    this.MainDiameter = cylinderSection.Diameter;
                    break;
                case PolygonSection polygonSection:
                    this.MainDiameter = polygonSection.CircumscribedCircleDiameter;
                    break;
            }

            this.SectionLength = this.Section.Length;
            this.Distance = this.throughHoleSubFeature.Distance;
            this.MeasureDistanceFrom = this.throughHoleSubFeature.DistanceFrom;
            this.HoleDiameter = this.throughHoleSubFeature.HoleDiameter;
            this.Chamfering = this.throughHoleSubFeature.Chamfering;
            this.Angle = this.throughHoleSubFeature.Angle;
        }

        protected override void SaveFeatureParameters()
        {
            switch (this.Section)
            {
                case CylinderSection cylinderSection:
                    cylinderSection.Diameter = this.mainDiameter;
                    break;
                case PolygonSection polygonSection:
                    polygonSection.CircumscribedCircleDiameter = this.mainDiameter;
                    break;
            }

            this.Section.Length = this.sectionLength;
            this.throughHoleSubFeature.Distance = this.distance;
            this.throughHoleSubFeature.DistanceFrom = this.measureDistanceFrom;
            this.throughHoleSubFeature.HoleDiameter = this.holeDiameter;
            this.throughHoleSubFeature.Chamfering = this.chamfering;
            this.throughHoleSubFeature.Angle = this.angle;
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(this.MainDiameter):
                        OnPropertyChanged(nameof(this.HoleDiameter));
                        OnPropertyChanged(nameof(this.Distance));
                        break;

                    case nameof(this.SectionLength):
                        OnPropertyChanged(nameof(this.HoleDiameter));
                        OnPropertyChanged(nameof(this.Distance));
                        break;

                    case nameof(this.HoleDiameter) when this.Section is CylinderSection cylinderSection:
                        if (this.holeDiameter > cylinderSection.Diameter)
                        {
                            error = $"Valid range is < {0:F3} mm ; {cylinderSection.Diameter:F3} mm >";
                        }

                        OnPropertyChanged(nameof(this.Chamfering));

                        break;

                    case nameof(this.HoleDiameter) when this.Section is PolygonSection polygonSection:
                        if (this.holeDiameter > polygonSection.CircumscribedCircleDiameter)
                        {
                            error =
                                $"Valid range is < {0:F3} mm ; {polygonSection.CircumscribedCircleDiameter:F3} mm >";
                        }

                        OnPropertyChanged(nameof(this.Chamfering));

                        break;

                    case nameof(this.Chamfering) when this.Section is CylinderSection cylinderSection:
                        if (this.chamfering > (cylinderSection.Diameter - this.holeDiameter) / 2)
                        {
                            error =
                                $"Valid range is < {0:F3} mm ; {(cylinderSection.Diameter - this.holeDiameter) / 2:F3} mm >";
                        }

                        break;

                    case nameof(this.Chamfering) when this.Section is PolygonSection polygonSection:
                        if (this.chamfering >
                            (polygonSection.CircumscribedCircleDiameter - this.holeDiameter) / 2 - 0.1)
                        {
                            error =
                                $"Valid range is < {0:F3} mm ;" +
                                $" {(polygonSection.CircumscribedCircleDiameter - this.holeDiameter) / 2 - 0.1:F3} mm >";
                        }

                        break;

                    case nameof(this.Distance) when this.measureDistanceFrom == DistanceFrom.FromFirstEdge ||
                                                    this.measureDistanceFrom == DistanceFrom.FromSecondEdge:
                        if (this.distance > this.sectionLength)
                        {
                            error = $"Valid range is < {0:F3} mm ; {this.sectionLength:F3} mm >";
                        }

                        break;

                    case nameof(this.Distance) when this.measureDistanceFrom == DistanceFrom.Centered:
                        if (this.distance > this.sectionLength / 2 || this.distance < this.sectionLength / -2)
                        {
                            error =
                                $"Valid range is < {this.sectionLength / -2:F3} mm ; {this.sectionLength / 2:F3} mm >";
                        }

                        break;

                    case nameof(this.Angle):
                        if (this.angle < -360 || this.angle > 360)
                        {
                            error = $"Valid range is < {-360:F2} deg ; {360:F2} deg >";
                        }

                        break;

                    case nameof(this.MeasureDistanceFrom):
                        OnPropertyChanged(nameof(this.Distance));
                        break;
                }

                return error;
            }
        }

        public string Error => null;
    }
}