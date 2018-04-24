using System.ComponentModel;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public abstract class ReliefEdgeFeatureViewModel<TReliefEdgeFeature> : SectionFeatureViewModel,
        IEdgeFeatureViewModel, IDataErrorInfo
        where TReliefEdgeFeature : ReliefEdgeFeature
    {
        private float width;
        private float reliefDepth;
        private float radius;
        private float mainDiameter;
        private float sectionLength;
        private bool customParameters;

        protected TReliefEdgeFeature ReliefEdgeFeature { get; set; }

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

        public float MainDiameter
        {
            get => this.mainDiameter;
            set
            {
                SetProperty(ref this.mainDiameter, value);
                var neededDimensions = this.ReliefEdgeFeature.CalculateReliefDimensions(value);
                UpdateReliefParameters(neededDimensions);
            }
        }

        public float SectionLength
        {
            get => this.sectionLength;
            set => SetProperty(ref this.sectionLength, value);
        }

        public bool CustomParameters
        {
            get => this.customParameters;
            set => SetProperty(ref this.customParameters, value);
        }

        public EdgeFeaturePosition EdgeFeaturePosition { get; set; }

        protected override void InitilizeFeatureParameters()
        {
            if (this.EdgeFeaturePosition == EdgeFeaturePosition.FirstEdge)
            {
                this.ReliefEdgeFeature = (TReliefEdgeFeature) this.Section.FirstEdgeFeature;
            }
            else
            {
                this.ReliefEdgeFeature = (TReliefEdgeFeature) this.Section.SecondEdgeFeature;
            }

            this.Width = this.ReliefEdgeFeature.Width;
            this.ReliefDepth = this.ReliefEdgeFeature.ReliefDepth;
            this.Radius = this.ReliefEdgeFeature.Radius;
            var cylinderSection = (CylinderSection) this.Section;
            this.MainDiameter = cylinderSection.Diameter;
            this.SectionLength = cylinderSection.Length;
        }

        protected override void SaveFeatureParameters()
        {
            this.ReliefEdgeFeature.Width = this.width;
            this.ReliefEdgeFeature.ReliefDepth = this.reliefDepth;
            this.ReliefEdgeFeature.Radius = this.radius;
        }

        protected virtual void UpdateReliefParameters(ReliefDimensions dimensions)
        {
            this.Width = dimensions.Width;
            this.ReliefDepth = dimensions.ReliefDepth;
            this.Radius = dimensions.Radius;
        }

        public virtual string this[string columnName]
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
                            this.ErrorFields.Add(nameof(this.SectionLength));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.SectionLength));
                        }

                        break;

                    case nameof(this.MainDiameter):
                        if (this.mainDiameter < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.MainDiameter));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.MainDiameter));
                        }

                        break;
                }

                return error;
            }
        }

        public string Error => null;
    }
}