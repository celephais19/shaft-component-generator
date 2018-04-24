using System.ComponentModel;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels
{
    public class ReliefDSISubFeatureViewModel : SectionFeatureViewModel, ISubFeatureViewModel, IDataErrorInfo
    {
        private float mainDiameter;
        private float sectionLength;
        private float distance;
        private DistanceFrom measureDistanceFrom;
        private float width;
        private float reliefDepth;
        private float radius;
        private float machiningAllowance;
        private ReliefDSISubFeature reliefDSiSubFeature;
        private CylinderSection cylinderSection;
        private bool isSaveEnabled;
        private bool customParameters;

        public ISectionSubFeature SubFeature
        {
            get => this.reliefDSiSubFeature;
            set => SetProperty(ref this.reliefDSiSubFeature, (ReliefDSISubFeature) value);
        }

        public float MainDiameter
        {
            get => this.mainDiameter;
            set
            {
                SetProperty(ref this.mainDiameter, value);
                ReliefDimensions neededDimensions = this.reliefDSiSubFeature.CalculateDimensions(value);
                this.Width = neededDimensions.Width;
                this.ReliefDepth = neededDimensions.ReliefDepth;
                this.Radius = neededDimensions.Radius;
            }
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

        public float ReliefDepth
        {
            get => this.reliefDepth;
            set => SetProperty(ref this.reliefDepth, value);
        }

        public float Radius
        {
            get => this.radius;
            set => SetProperty(ref this.radius, value);
        }

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }

        public bool IsSaveEnabled
        {
            get => this.isSaveEnabled;
            set => SetProperty(ref this.isSaveEnabled, value);
        }

        public bool CustomParameters
        {
            get => this.customParameters;
            set => SetProperty(ref this.customParameters, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            this.cylinderSection = (CylinderSection) this.Section;
            this.MainDiameter = this.cylinderSection.Diameter;
            this.SectionLength = this.cylinderSection.Length;
            this.Distance = this.reliefDSiSubFeature.Distance;
            this.MeasureDistanceFrom = this.reliefDSiSubFeature.DistanceFrom;
            this.Width = this.reliefDSiSubFeature.Width;
            this.ReliefDepth = this.reliefDSiSubFeature.ReliefDepth;
            this.Radius = this.reliefDSiSubFeature.Radius;
            this.MachiningAllowance = this.reliefDSiSubFeature.MachiningAllowance;
        }

        protected override void SaveFeatureParameters()
        {
            this.cylinderSection.Diameter = this.mainDiameter;
            this.cylinderSection.Length = this.sectionLength;
            this.reliefDSiSubFeature.Distance = this.distance;
            this.reliefDSiSubFeature.DistanceFrom = this.measureDistanceFrom;
            this.reliefDSiSubFeature.Width = this.width;
            this.reliefDSiSubFeature.ReliefDepth = this.reliefDepth;
            this.reliefDSiSubFeature.Radius = this.radius;
            this.reliefDSiSubFeature.MachiningAllowance = this.machiningAllowance;
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(this.SectionLength):
                        if (this.sectionLength < 0.1)
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
                        if (this.mainDiameter < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.IsSaveEnabled = false;
                        }
                        else
                        {
                            this.IsSaveEnabled = true;
                        }

                        OnPropertyChanged(nameof(this.ReliefDepth));

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
                }

                return error;
            }
        }

        public string Error => null;
    }
}