using System.ComponentModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class FilletEdgeFeatureViewModel : SectionFeatureViewModel, IEdgeFeatureViewModel, IDataErrorInfo
    {
        private float radius;
        private FilletEdgeFeature filletEdgeFeature;
        private IFilletFeatureParamsValidator filletParamsValidator;
        private bool boreSection;

        public EdgeFeaturePosition EdgeFeaturePosition { get; set; }

        public float Radius
        {
            get => this.radius;
            set => SetProperty(ref this.radius, value);
        }

        protected override void InitilizeFeatureParameters()
        {
            if (this.EdgeFeaturePosition == EdgeFeaturePosition.FirstEdge)
            {
                this.filletEdgeFeature = (FilletEdgeFeature) this.Section.FirstEdgeFeature;
            }
            else
            {
                this.filletEdgeFeature = (FilletEdgeFeature) this.Section.SecondEdgeFeature;
            }

            if (this.Section.IsBore)
            {
                this.boreSection = true;
            }

            if (this.Section is ConeSection && !this.Section.IsBore)
            {
                this.filletParamsValidator = new SectionFilletFeatureParamsValidator(this.filletEdgeFeature);
            }
            else if (this.Section.IsBore)
            {
                this.filletParamsValidator = new BoreFilletFeatureParamsValidator(this.filletEdgeFeature);
            }

            this.Radius = this.filletEdgeFeature.Radius;
        }

        protected override void SaveFeatureParameters()
        {
            this.filletEdgeFeature.Radius = this.Radius;
        }

        protected override void SaveChanges(IDialogView dialogView)
        {
            this.filletParamsValidator?.Dispose();

            base.SaveChanges(dialogView);
        }

        protected override void CancelChanges(IDialogView dialogView)
        {
            this.filletParamsValidator?.Dispose();

            base.CancelChanges(dialogView);
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                if (this.radius <= 0)
                {
                    error = "The radius of fillet must be greater than zero";
                    return error;
                }

                switch (this.Section)
                {
                    case CylinderSection cylinderSection:
                        if ((this.boreSection
                                ? !this.filletParamsValidator.ValidateRadius(this.radius)
                                : this.radius > cylinderSection.Diameter / 2) ||
                            this.radius.NearlyEqual(0))
                        {
                            error = "An incorrect radius of the fillet";
                        }

                        break;

                    case ConeSection _:
                        if (!this.filletParamsValidator.ValidateRadius(this.radius) ||
                            this.radius.NearlyEqual(0))
                        {
                            error = "An incorrect radius of the fillet";
                        }

                        break;
                }

                return error;
            }
        }

        public string Error => null;
    }
}