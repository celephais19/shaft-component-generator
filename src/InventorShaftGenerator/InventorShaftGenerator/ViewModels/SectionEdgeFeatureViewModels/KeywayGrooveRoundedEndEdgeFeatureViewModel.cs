using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class
        KeywayGrooveRoundedEndEdgeFeatureViewModel : KeywayGrooveEdgeFeatureViewModel<KeywayGrooveRoundedEndEdgeFeature>
    {
        private float chamfer;
        private CylinderSection cylinderSection;

        public float Chamfer
        {
            get => this.chamfer;
            set => SetProperty(ref this.chamfer, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.Chamfer = this.KeywayGrooveFeature.Chamfer;
            this.cylinderSection = (CylinderSection) this.Section;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.KeywayGrooveFeature.Chamfer = this.chamfer;
        }

        public override string this[string columnName]
        {
            get
            {
                if (this.cylinderSection == null)
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

                    case nameof(this.Chamfer):
                        if (this.Chamfer > this.MainDiameter / 2)
                        {
                            error = $"Valid range is < {0:F3} mm ; {(this.MainDiameter / 2):F3} mm >";
                        }

                        break;
                }

                return error;
            }
        }
    }
}