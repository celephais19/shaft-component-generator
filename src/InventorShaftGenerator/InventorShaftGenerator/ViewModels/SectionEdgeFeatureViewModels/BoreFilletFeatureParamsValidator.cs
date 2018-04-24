using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class BoreFilletFeatureParamsValidator : BoreEdgeFeatureParamsValidator<FilletEdgeFeature>,
        IFilletFeatureParamsValidator
    {
        public BoreFilletFeatureParamsValidator(FilletEdgeFeature edgeFeature) : base(edgeFeature)
        {
        }

        public bool ValidateRadius(float radiusToCheck)
        {
            try
            {
                var fillet =
                    this.ComponentDefinition.Features.FilletFeatures.AddSimple(LocateEdgeToEdgeCollection(),
                        radiusToCheck.InMillimeters());
                fillet.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}