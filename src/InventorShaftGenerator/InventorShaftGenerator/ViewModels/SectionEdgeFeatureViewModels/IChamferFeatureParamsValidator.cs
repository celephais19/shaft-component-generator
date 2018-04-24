using System;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public interface IChamferFeatureParamsValidator : IDisposable
    {
        bool ValidateDistance(float distane);
        bool ValidateDistanceAndAngle(float distance, float angle);
        bool ValidateTwoDistances(float distance1, float distance2);
    }
}