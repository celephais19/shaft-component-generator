using System.ComponentModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels
{
    public class GrooveBSubFeatureViewModel : SectionFeatureViewModel, ISubFeatureViewModel, IDataErrorInfo
    {
        private float mainDiameter;
        private float sectionLength;
        private float distance;
        private DistanceFrom measureDistanceFrom;
        private float depth;
        private float radius;
        private float angle;
        private GrooveBSubFeature grooveBSubFeature;
        private CylinderSection cylinderSection;
        private bool isSaveEnabled;

        ISectionSubFeature ISubFeatureViewModel.SubFeature
        {
            get => this.grooveBSubFeature;
            set => SetProperty(ref this.grooveBSubFeature, (GrooveBSubFeature) value);
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

        public float Depth
        {
            get => this.depth;
            set => SetProperty(ref this.depth, value);
        }

        public float Radius
        {
            get => this.radius;
            set => SetProperty(ref this.radius, value);
        }

        public float Angle
        {
            get => this.angle;
            set => SetProperty(ref this.angle, value);
        }

        public bool IsSaveEnabled
        {
            get => this.isSaveEnabled;
            set => SetProperty(ref this.isSaveEnabled, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            this.cylinderSection = (CylinderSection) this.Section;
            this.MainDiameter = this.cylinderSection.Diameter;
            this.SectionLength = this.cylinderSection.Length;
            this.Distance = this.grooveBSubFeature.Distance;
            this.MeasureDistanceFrom = this.grooveBSubFeature.DistanceFrom;
            this.Depth = this.grooveBSubFeature.Depth;
            this.Radius = this.grooveBSubFeature.Radius;
            this.Angle = this.grooveBSubFeature.Angle;
        }

        protected override void SaveFeatureParameters()
        {
            this.cylinderSection.Diameter = this.mainDiameter;
            this.cylinderSection.Length = this.sectionLength;
            this.grooveBSubFeature.Distance = this.distance;
            this.grooveBSubFeature.DistanceFrom = this.measureDistanceFrom;
            this.grooveBSubFeature.Depth = this.depth;
            this.grooveBSubFeature.Radius = this.radius;
            this.grooveBSubFeature.Angle = this.angle;
        }

        public string this[string columnName]
        {
            get
            {
                if (this.Section == null)
                {
                    return null;
                }

                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(this.SectionLength):
                        if (!this.sectionLength.IsGreaterThanZero())
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.IsSaveEnabled = false;
                        }
                        else
                        {
                            this.IsSaveEnabled = true;
                        }

                        OnPropertyChanged(nameof(this.Distance));
                        OnPropertyChanged(nameof(this.Radius));

                        break;

                    case nameof(this.MainDiameter):
                        if (!this.mainDiameter.IsGreaterThanZero())
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.IsSaveEnabled = false;
                        }
                        else
                        {
                            this.IsSaveEnabled = true;
                        }

                        OnPropertyChanged(nameof(this.Depth));

                        break;

                    case nameof(this.Distance) when this.measureDistanceFrom == DistanceFrom.FromFirstEdge ||
                                                    this.measureDistanceFrom == DistanceFrom.FromSecondEdge:
                        if (this.distance > this.SectionLength)
                        {
                            error = $"Valid range is < {0:F3} mm ; {this.SectionLength:F3} mm >";
                        }

                        break;

                    case nameof(this.Distance) when this.measureDistanceFrom == DistanceFrom.Centered:
                        if (this.distance > this.sectionLength / 2 || this.distance < this.sectionLength / -2)
                        {
                            error =
                                $"Valid range is < {this.sectionLength / -2:F3} mm ; {this.sectionLength / 2:F3} mm >";
                        }

                        break;

                    case nameof(this.Depth):
                        if (this.depth > this.mainDiameter / 2 || this.depth.NearlyEqual(0))
                        {
                            error = $"Valid range is < {0.01:F3} mm ; {this.mainDiameter / 2:F3} mm >";
                        }

                        break;

                    case nameof(this.Radius):
                        if (this.radius > this.sectionLength / 2 || this.depth.NearlyEqual(0))
                        {
                            error = $"Valid range is < {0.01:F3} mm ; {this.sectionLength / 2:F3} mm >";
                        }

                        break;

                    case nameof(this.Angle):
                        if (this.angle < -360 || this.angle > 360)
                        {
                            error = $"Valid range is < {-360:F2} deg ; {360:F2} deg >";
                        }

                        break;
                }

                return error;
            }
        }

        public string Error => null;
    }
}