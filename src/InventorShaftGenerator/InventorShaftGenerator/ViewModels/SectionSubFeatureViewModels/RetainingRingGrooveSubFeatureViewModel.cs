using System.ComponentModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels
{
    public class RetainingRingGrooveSubFeatureViewModel : SectionFeatureViewModel,
        ISubFeatureViewModel, IDataErrorInfo
    {
        private float mainDiameter;
        private float sectionLength;
        private float width;
        private float diameter1;
        private float activeLength;
        private float distance;
        private DistanceFrom measureDistanceFrom;
        private RetainingRingGrooveSubFeature retainingRingGrooveSubFeature;
        private CylinderSection cylinderSection;
        private bool isSaveEnabled;

        public ISectionSubFeature SubFeature
        {
            get => this.retainingRingGrooveSubFeature;
            set => SetProperty(ref this.retainingRingGrooveSubFeature, (RetainingRingGrooveSubFeature) value);
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

        public float Width
        {
            get => this.width;
            set => SetProperty(ref this.width, value);
        }

        public float Diameter1
        {
            get => this.diameter1;
            set => SetProperty(ref this.diameter1, value);
        }

        public float ActiveLength
        {
            get => this.activeLength;
            set => SetProperty(ref this.activeLength, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            this.cylinderSection = (CylinderSection) this.Section;
            this.MainDiameter = this.cylinderSection.Diameter;
            this.SectionLength = this.cylinderSection.Length;
            this.Distance = this.retainingRingGrooveSubFeature.Distance;
            this.MeasureDistanceFrom = this.retainingRingGrooveSubFeature.DistanceFrom;
            this.Width = this.retainingRingGrooveSubFeature.Width;
            this.ActiveLength = this.retainingRingGrooveSubFeature.ActiveLength;
            this.Diameter1 = this.retainingRingGrooveSubFeature.Diameter1;
        }

        protected override void SaveFeatureParameters()
        {
            this.cylinderSection.Diameter = this.mainDiameter;
            this.cylinderSection.Length = this.sectionLength;
            this.retainingRingGrooveSubFeature.Distance = this.distance;
            this.retainingRingGrooveSubFeature.DistanceFrom = this.measureDistanceFrom;
            this.retainingRingGrooveSubFeature.Width = this.width;
            this.retainingRingGrooveSubFeature.ActiveLength = this.activeLength;
            this.retainingRingGrooveSubFeature.Diameter1 = this.diameter1;
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

                        OnPropertyChanged(nameof(this.Diameter1));

                        break;

                    case nameof(this.Distance) when this.measureDistanceFrom == DistanceFrom.FromFirstEdge ||
                                                    this.measureDistanceFrom == DistanceFrom.FromSecondEdge:
                        if (this.distance > this.SectionLength)
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

                    case nameof(this.Width):
                        if (this.width < 0.01f || this.width > this.SectionLength)
                        {
                            error = $"Valid range is < {0.01:F3} mm ; {this.sectionLength:F3} mm >";
                        }

                        break;

                    case nameof(this.Diameter1) when this.Section.IsBore:
                        if (this.diameter1 < this.mainDiameter)
                        {
                            error = $"Valid range is < {this.mainDiameter:F3} mm ; ~ >";
                        }

                        break;
                    
                    case nameof(this.Diameter1):
                        if (this.diameter1 < 0.01f || this.diameter1 > this.MainDiameter)
                        {
                            error = $"Valid range is < {0.01:F3} mm ; {this.mainDiameter:F3} mm >";
                        }

                        break;
                }

                return error;
            }
        }

        public bool IsSaveEnabled
        {
            get => this.isSaveEnabled;
            set => SetProperty(ref this.isSaveEnabled, value);
        }

        public string Error => null;
    }
}