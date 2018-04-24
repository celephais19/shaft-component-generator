using System;
using Inventor;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public abstract class SectionEdgeFeatureParamsValidator<TEdgeFeature> : IDisposable
        where TEdgeFeature : ISectionEdgeFeature
    {
        protected readonly PartComponentDefinition ComponentDefinition;
        protected readonly TransientGeometry TransientGeometry;
        protected readonly TEdgeFeature EdgeFeature;
        protected readonly PartDocument PartDocument;
        protected readonly EdgeCollection EdgeCollection;

        protected SectionEdgeFeatureParamsValidator(TEdgeFeature edgeFeature)
        {
            var invApp = Shaft.Application;
            this.EdgeFeature = edgeFeature;
            this.PartDocument =
                (PartDocument) invApp.Documents.Add(DocumentTypeEnum.kPartDocumentObject,
                    CreateVisible: false);
            this.ComponentDefinition = this.PartDocument.ComponentDefinition;
            this.TransientGeometry = invApp.TransientGeometry;

            this.EdgeCollection = invApp.TransientObjects.CreateEdgeCollection();

            BuildTestCone(edgeFeature.LinkedSection);
        }

        private void BuildTestCone(ShaftSection coneSection)
        {
            var sketch = this.ComponentDefinition.Sketches.Add(this.ComponentDefinition.WorkPlanes[3]);
            var line1 = sketch.SketchLines.AddByTwoPoints(this.TransientGeometry.CreatePoint2d(),
                this.TransientGeometry.CreatePoint2d(
                    coneSection.SecondLine.StartPoint.X.InMillimeters(),
                    coneSection.SecondLine.StartPoint.Y.InMillimeters()));
            var line2 = sketch.SketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(
                    coneSection.SecondLine.StartPoint.X.InMillimeters(),
                    coneSection.SecondLine.StartPoint.Y.InMillimeters()),
                this.TransientGeometry.CreatePoint2d(
                    coneSection.SecondLine.EndPoint.X.InMillimeters(),
                    coneSection.SecondLine.EndPoint.Y.InMillimeters()));
            var line3 = sketch.SketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(
                    coneSection.SecondLine.EndPoint.X.InMillimeters(),
                    coneSection.SecondLine.EndPoint.Y.InMillimeters()),
                this.TransientGeometry.CreatePoint2d(
                    coneSection.SecondLine.EndPoint.X.InMillimeters()));
            var line4 = sketch.SketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(line3.EndSketchPoint.Geometry.X,
                    line3.EndSketchPoint.Geometry.Y),
                this.TransientGeometry.CreatePoint2d());

            line1.EndSketchPoint.Merge(line2.StartSketchPoint);
            line2.EndSketchPoint.Merge(line3.StartSketchPoint);
            line3.EndSketchPoint.Merge(line4.StartSketchPoint);
            line4.EndSketchPoint.Merge(line1.StartSketchPoint);

            var profile = sketch.Profiles.AddForSolid();
            var revolveFeature =
                this.ComponentDefinition.Features.RevolveFeatures.AddFull(profile, line4,
                    PartFeatureOperationEnum.kNewBodyOperation);
            var body = revolveFeature.SurfaceBody;
            var filletEdge = body.LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                this.TransientGeometry.CreatePoint(
                    this.EdgeFeature.EdgePoint.X.InMillimeters(), this.EdgeFeature.EdgePoint.Y.InMillimeters()),
                0.01);
            this.EdgeCollection.Add(filletEdge);
        }

        public void Dispose()
        {
            this.PartDocument.ReleaseReference();
        }
    }
}