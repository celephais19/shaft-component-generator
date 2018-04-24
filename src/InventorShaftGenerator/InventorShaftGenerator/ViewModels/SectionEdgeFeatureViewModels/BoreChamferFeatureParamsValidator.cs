using Inventor;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class BoreChamferFeatureParamsValidator : BoreEdgeFeatureParamsValidator<ChamferEdgeFeature>,
        IChamferFeatureParamsValidator
    {
        public BoreChamferFeatureParamsValidator(ChamferEdgeFeature edgeFeature) : base(edgeFeature)
        {
        }

        public bool ValidateDistance(float distance)
        {
            try
            {
                var chamfer =
                    this.ComponentDefinition.Features.ChamferFeatures.AddUsingDistance(LocateEdgeToEdgeCollection(),
                        distance.InMillimeters());
                chamfer.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateDistanceAndAngle(float distance, float angle)
        {
            try
            {
                var chamfer =
                    this.ComponentDefinition.Features.ChamferFeatures.AddUsingDistanceAndAngle(
                        LocateEdgeToEdgeCollection(),
                        LocateFace(), distance.InMillimeters(), angle + "deg");
                chamfer.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateTwoDistances(float distance1, float distance2)
        {
            try
            {
                var chamfer =
                    this.ComponentDefinition.Features.ChamferFeatures.AddUsingTwoDistances(LocateEdgeToEdgeCollection(),
                        LocateFace(),
                        distance1.InMillimeters(), distance2.InMillimeters());
                chamfer.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Face LocateFace()
        {
            var face = this.ComponentDefinition.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kFaceObject,
                this.TransientGeometry.CreatePoint(
                    (this.BoreFromEdge == BoreFromEdge.FromLeft
                        ? this.EdgeFeature.EdgePoint.X - 0.1f
                        : this.TestShaftLength - this.EdgeFeature.EdgePoint.X - 0.1f)
                    .InMillimeters(),
                    this.EdgeFeature.EdgePoint.Y.InMillimeters()), 0.01);
            return face;
        }
    }
}