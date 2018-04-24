using Inventor;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class SectionChamferFeatureParamsValidator : SectionEdgeFeatureParamsValidator<ChamferEdgeFeature>,
        IChamferFeatureParamsValidator
    {
        public SectionChamferFeatureParamsValidator(ChamferEdgeFeature chamferEdgeFeature) : base(chamferEdgeFeature)
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

        private EdgeCollection LocateEdgeToEdgeCollection()
        {
            var edge = this.ComponentDefinition.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                this.TransientGeometry.CreatePoint(this.EdgeFeature.EdgePoint.X.InMillimeters(),
                    this.EdgeFeature.EdgePoint.Y.InMillimeters())) as Edge;
            var edgeColl = Shaft.Application.TransientObjects.CreateEdgeCollection();
            edgeColl.Add(edge);
            return edgeColl;
        }

        private Face LocateFace()
        {
            var face = this.ComponentDefinition.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kFaceObject,
                this.TransientGeometry.CreatePoint(
                    (this.EdgeFeature.EdgePoint.X - 0.1f).InMillimeters(),
                    this.EdgeFeature.EdgePoint.Y.InMillimeters()), 0.01);
            return face;
        }
    }
}