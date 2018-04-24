using System.ComponentModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels
{
    public class GrooveASubFeatureViewModel : SectionFeatureViewModel, ISubFeatureViewModel, IDataErrorInfo
    {
        private float mainDiameter;
        private float sectionLength;
        private float distance;
        private DistanceFrom measureDistanceFrom;
        private float depth;
        private float radius;
        private GrooveASubFeature grooveASubFeature;
        private CylinderSection cylinderSection;
        private bool isSaveEnabled;

        public ISectionSubFeature SubFeature
        {
            get => this.grooveASubFeature;
            set => SetProperty(ref this.grooveASubFeature, (GrooveASubFeature) value);
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
            this.Distance = this.grooveASubFeature.Distance;
            this.MeasureDistanceFrom = this.grooveASubFeature.DistanceFrom;
            this.Depth = this.grooveASubFeature.Depth;
            this.Radius = this.grooveASubFeature.Radius;
        }

        protected override void SaveFeatureParameters()
        {
            this.cylinderSection.Diameter = this.mainDiameter;
            this.cylinderSection.Length = this.sectionLength;
            this.grooveASubFeature.Distance = this.distance;
            this.grooveASubFeature.DistanceFrom = this.measureDistanceFrom;
            this.grooveASubFeature.Depth = this.depth;
            this.grooveASubFeature.Radius = this.radius;
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
                }

                return error;
            }
        }

        public string Error => null;
    }
}