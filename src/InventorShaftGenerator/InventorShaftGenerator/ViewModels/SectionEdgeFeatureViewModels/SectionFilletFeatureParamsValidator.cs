using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class SectionFilletFeatureParamsValidator : SectionEdgeFeatureParamsValidator<FilletEdgeFeature>,
        IFilletFeatureParamsValidator
    {
        public SectionFilletFeatureParamsValidator(FilletEdgeFeature filletEdgeFeature) : base(filletEdgeFeature)
        {
        }

        public bool ValidateRadius(float radiusToCheck)
        {
            try
            {
                var fillet =
                    this.ComponentDefinition.Features.FilletFeatures.AddSimple(this.EdgeCollection,
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