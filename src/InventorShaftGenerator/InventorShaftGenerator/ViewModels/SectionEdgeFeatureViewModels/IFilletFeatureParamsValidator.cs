using System;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public interface IFilletFeatureParamsValidator : IDisposable
    {
        bool ValidateRadius(float radiusToCheck);
    }
}