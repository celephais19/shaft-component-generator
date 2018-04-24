using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class PlainKeywayGrooveEdgeFeatureViewModel : KeywayGrooveEdgeFeatureViewModel<PlainKeywayGrooveEdgeFeature>
    {
        private float radius;
        private float chamfer;
        private CylinderSection cylinderSection;

        public float Radius
        {
            get => this.radius;
            set => SetProperty(ref this.radius, value);
        }

        public float Chamfer
        {
            get => this.chamfer;
            set => SetProperty(ref this.chamfer, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.Radius = this.KeywayGrooveFeature.Radius;
            this.Chamfer = this.KeywayGrooveFeature.Chamfer;
            this.cylinderSection = (CylinderSection) this.Section;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.KeywayGrooveFeature.Radius = this.Radius;
            this.KeywayGrooveFeature.Chamfer = this.Chamfer;
        }

        public override string this[string columnName]
        {
            get
            {
                if (this.cylinderSection == null)
                {
                    return string.Empty;
                }

                string error = base[columnName];
                if (columnName == nameof(this.Depth))
                {
                    OnPropertyChanged(nameof(this.Radius));
                }

                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }

                switch (columnName)
                {
                    case nameof(this.Width):
                        if (this.Width > this.cylinderSection.Diameter / 2)
                        {
                            error = $"Valid range is < {0:F3} mm ; {(this.cylinderSection.Diameter / 2):F3} mm >";
                        }

                        break;

                    case nameof(this.Depth):
                        if (this.Depth > this.cylinderSection.Diameter / 2)
                        {
                            error = $"Valid range is < {0:F3} mm ; {(this.cylinderSection.Diameter / 2):F3} mm >";
                        }

                        break;

                    case nameof(this.Chamfer):
                        if (this.Chamfer > this.cylinderSection.Diameter / 2)
                        {
                            error = $"Valid range is < {0:F3} mm ; {(this.cylinderSection.Diameter / 2):F3} mm >";
                        }

                        break;

                    case nameof(this.Radius):
                        if (this.Radius < this.Depth)
                        {
                            error = $"Valid range is < {this.Depth} mm ; ~ >";
                        }

                        break;
                }

                return error;
            }
        }
    }
}