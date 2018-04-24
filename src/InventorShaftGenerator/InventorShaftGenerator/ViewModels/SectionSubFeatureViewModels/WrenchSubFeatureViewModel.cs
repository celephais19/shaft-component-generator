using System.ComponentModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels
{
    public class WrenchSubFeatureViewModel : SectionFeatureViewModel, ISubFeatureViewModel, IDataErrorInfo
    {
        private float mainDiameter;
        private float sectionLength;
        private float wrenchLength;
        private float widthAcrossFlats;
        private float angle;
        private float distance;
        private DistanceFrom measureDistanceFrom;
        private WrenchSubFeature wrenchSubFeature;
        private bool isSaveEnabled;
        private CylinderSection cylinderSection;

        public ISectionSubFeature SubFeature
        {
            get => this.wrenchSubFeature;
            set => SetProperty(ref this.wrenchSubFeature, (WrenchSubFeature) value);
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

        public float WrenchLength
        {
            get => this.wrenchLength;
            set => SetProperty(ref this.wrenchLength, value);
        }

        public float WidthAcrossFlats
        {
            get => this.widthAcrossFlats;
            set => SetProperty(ref this.widthAcrossFlats, value);
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
            this.Distance = this.wrenchSubFeature.Distance;
            this.MeasureDistanceFrom = this.wrenchSubFeature.DistanceFrom;
            this.WrenchLength = this.wrenchSubFeature.WrenchLength;
            this.WidthAcrossFlats = this.wrenchSubFeature.WidthAcrossFlats;
            this.Angle = this.wrenchSubFeature.Angle;
        }

        protected override void SaveFeatureParameters()
        {
            this.cylinderSection.Diameter = this.MainDiameter;
            this.cylinderSection.Length = this.SectionLength;
            this.wrenchSubFeature.Distance = this.distance;
            this.wrenchSubFeature.DistanceFrom = this.measureDistanceFrom;
            this.wrenchSubFeature.WrenchLength = this.wrenchLength;
            this.wrenchSubFeature.WidthAcrossFlats = this.widthAcrossFlats;
            this.wrenchSubFeature.Angle = this.angle;
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

                        OnPropertyChanged(nameof(this.WidthAcrossFlats));

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

                    case nameof(this.WrenchLength):
                        if (this.wrenchLength > this.SectionLength || this.wrenchLength < 0.01f)
                        {
                            error = $"Valid range is < {0.01:F3} mm ; {this.SectionLength:F3} mm >";
                        }

                        break;

                    case nameof(this.WidthAcrossFlats):
                        if (this.widthAcrossFlats < 0.01f || this.widthAcrossFlats > this.MainDiameter)
                        {
                            error = $"Valid range is < {0.01:F3} mm ; {this.MainDiameter:F3} mm >";
                        }

                        break;

                    case nameof(this.Angle):
                        if (this.angle < -360 || this.angle > 360)
                        {
                            error = $"Valid range is < {-360:F2} deg ; {360:F2} deg >";
                        }

                        break;

                    case nameof(this.MeasureDistanceFrom):
                        if (this.measureDistanceFrom == DistanceFrom.Centered)
                        {
                            this.Distance = 0;
                        }

                        OnPropertyChanged(nameof(this.Distance));
                        break;
                }

                return error;
            }
        }

        public string Error => null;
    }
}