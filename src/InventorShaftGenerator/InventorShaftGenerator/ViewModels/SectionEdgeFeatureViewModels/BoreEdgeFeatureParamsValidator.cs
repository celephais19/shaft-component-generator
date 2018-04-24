using System;
using System.Linq;
using Inventor;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public abstract class BoreEdgeFeatureParamsValidator<TEdgeFeature> : IDisposable
        where TEdgeFeature : ISectionEdgeFeature
    {
        protected readonly PartComponentDefinition ComponentDefinition;
        protected readonly TransientGeometry TransientGeometry;
        protected readonly PartDocument PartDocument;
        protected readonly TEdgeFeature EdgeFeature;
        protected readonly BoreFromEdge BoreFromEdge;

        protected BoreEdgeFeatureParamsValidator(TEdgeFeature edgeFeature)
        {
            var invApp = Shaft.Application;
            this.EdgeFeature = edgeFeature;
            this.BoreFromEdge = edgeFeature.LinkedSection.BoreFromEdge.Value;
            this.PartDocument =
                (PartDocument) invApp.Documents.Add(DocumentTypeEnum.kPartDocumentObject,
                    CreateVisible: false);
            this.ComponentDefinition = this.PartDocument.ComponentDefinition;
            this.TransientGeometry = invApp.TransientGeometry;

            BuildTestSections(edgeFeature.LinkedSection);
        }

        protected float TestShaftLength { get; private set; }

        private void BuildTestSections(ShaftSection testBoreSection)
        {
            BuildOuterSections(testBoreSection);

            BuildTestBoreSection(testBoreSection);
        }

        private void BuildTestBoreSection(ShaftSection testBoreSection)
        {
            bool fromLeft = this.BoreFromEdge == BoreFromEdge.FromLeft;
            PlanarSketch sketch = this.ComponentDefinition.Sketches.Add(this.ComponentDefinition.WorkPlanes[3]);
            sketch.NaturalAxisDirection = true;
            var sketchLines = sketch.SketchLines;

            sketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(fromLeft
                    ? testBoreSection.SecondLine.StartPoint.X.InMillimeters()
                    : (this.TestShaftLength - testBoreSection.SecondLine.StartPoint.X).InMillimeters()),
                this.TransientGeometry.CreatePoint2d(fromLeft
                    ? testBoreSection.SecondLine.EndPoint.X.InMillimeters()
                    : (this.TestShaftLength - testBoreSection.SecondLine.EndPoint.X).InMillimeters()));

            var line1 = sketch.SketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(fromLeft
                    ? testBoreSection.SecondLine.StartPoint.X.InMillimeters()
                    : (this.TestShaftLength - testBoreSection.SecondLine.StartPoint.X)
                    .InMillimeters()),
                this.TransientGeometry.CreatePoint2d(
                    fromLeft
                        ? testBoreSection.SecondLine.StartPoint.X.InMillimeters()
                        : (this.TestShaftLength - testBoreSection.SecondLine.StartPoint.X)
                        .InMillimeters(),
                    testBoreSection.SecondLine.StartPoint.Y.InMillimeters()));
            var line2 = sketch.SketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(
                    fromLeft
                        ? testBoreSection.SecondLine.StartPoint.X.InMillimeters()
                        : (this.TestShaftLength - testBoreSection.SecondLine.StartPoint.X)
                        .InMillimeters(),
                    testBoreSection.SecondLine.StartPoint.Y.InMillimeters()),
                this.TransientGeometry.CreatePoint2d(
                    fromLeft
                        ? testBoreSection.SecondLine.EndPoint.X.InMillimeters()
                        : (this.TestShaftLength - testBoreSection.SecondLine.EndPoint.X)
                        .InMillimeters(),
                    testBoreSection.SecondLine.EndPoint.Y.InMillimeters()));
            var line3 = sketch.SketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(
                    fromLeft
                        ? testBoreSection.SecondLine.EndPoint.X.InMillimeters()
                        : (this.TestShaftLength - testBoreSection.SecondLine.EndPoint.X)
                        .InMillimeters(),
                    testBoreSection.SecondLine.EndPoint.Y.InMillimeters()),
                this.TransientGeometry.CreatePoint2d(fromLeft
                    ? testBoreSection
                      .SecondLine.EndPoint.X.InMillimeters()
                    : (this.TestShaftLength -
                       testBoreSection.SecondLine.EndPoint.X).InMillimeters()));
            var line4 = sketch.SketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(line3.EndSketchPoint.Geometry.X,
                    line3.EndSketchPoint.Geometry.Y),
                line1.StartSketchPoint.Geometry);

            line1.EndSketchPoint.Merge(line2.StartSketchPoint);
            line2.EndSketchPoint.Merge(line3.StartSketchPoint);
            line3.EndSketchPoint.Merge(line4.StartSketchPoint);
            line4.EndSketchPoint.Merge(line1.StartSketchPoint);

            var profile = sketch.Profiles.AddForSolid();
            var revolveFeature =
                this.ComponentDefinition.Features.RevolveFeatures.AddFull(profile, line4,
                    PartFeatureOperationEnum.kCutOperation);
        }

        private void BuildOuterSections(ShaftSection testBoreSection)
        {
            var outerSections = testBoreSection.GetOuterSections().ToList();
            this.TestShaftLength = outerSections.Sum(section => section.Length);
            bool leftBore = testBoreSection.BoreFromEdge == BoreFromEdge.FromLeft;
            var sketch = this.ComponentDefinition.Sketches.Add(this.ComponentDefinition.WorkPlanes[3]);
            sketch.NaturalAxisDirection = true;
            var sketchLines = sketch.SketchLines;
            sketchLines.AddByTwoPoints(
                this.TransientGeometry.CreatePoint2d(outerSections.First().SecondLine.StartPoint.X.InMillimeters()),
                this.TransientGeometry.CreatePoint2d(outerSections.Last().SecondLine.EndPoint.X.InMillimeters()));

            for (var index = 0; index < outerSections.Count; index++)
            {
                var outerSection = outerSections[index];
                if (outerSection.Equals(outerSections.First()))
                {
                    Point2d firstPoint = this.TransientGeometry.CreatePoint2d(
                        outerSection.SecondLine.StartPoint.X.InMillimeters());
                    Point2d secondPoint =
                        this.TransientGeometry.CreatePoint2d(outerSection.SecondLine.StartPoint.X.InMillimeters(),
                            outerSection.SecondLine.StartPoint.Y.InMillimeters());
                    sketchLines.AddByTwoPoints(firstPoint, secondPoint);
                }


                var secondLineParams = outerSection.SecondLine;

                //if (leftBore
                //    ? Shaft.BoreOnTheLeft.IndexOf(outerSection) != 0
                //    : Shaft.BoresOnTheRight.IndexOf(outerSection) != 0)
                //{
                //    var previousSectionSecondLine = outerSections[index - 1].SecondLine;
                //    if (previousSectionSecondLine.EndPoint != secondLineParams.StartPoint)
                //    {
                //        sketchLines.AddByTwoPoints(
                //            this.TransientGeometry.CreatePoint2d(previousSectionSecondLine.EndPoint.X.InMillimeters(),
                //                previousSectionSecondLine.EndPoint.Y.InMillimeters()),
                //            this.TransientGeometry.CreatePoint2d(secondLineParams.StartPoint.X.InMillimeters(),
                //                secondLineParams.StartPoint.Y.InMillimeters()));
                //    }
                //}

                sketchLines.AddByTwoPoints(
                    this.TransientGeometry.CreatePoint2d(secondLineParams.StartPoint.X.InMillimeters(),
                        secondLineParams.StartPoint.Y.InMillimeters()),
                    this.TransientGeometry.CreatePoint2d(secondLineParams.EndPoint.X.InMillimeters(),
                        secondLineParams.EndPoint.Y.InMillimeters()));


                if (outerSection.Equals(outerSections.Last()))
                {
                    sketchLines.AddByTwoPoints(
                        this.TransientGeometry.CreatePoint2d(outerSection.SecondLine.EndPoint.X.InMillimeters(),
                            outerSection.SecondLine.EndPoint.Y.InMillimeters()),
                        this.TransientGeometry.CreatePoint2d(outerSection.SecondLine.EndPoint.X.InMillimeters()));
                }
            }

            sketchLines[1].StartSketchPoint.Merge(sketchLines[2].StartSketchPoint);
            for (int i = 2; i < sketchLines.Count; i++)
            {
                sketchLines[i].EndSketchPoint.Merge(sketchLines[i + 1].StartSketchPoint);
            }

            sketchLines[1].EndSketchPoint.Merge(sketchLines[sketchLines.Count].EndSketchPoint);
            var profile = sketch.Profiles.AddForSolid();
            var revolveFeature = this.ComponentDefinition.Features.RevolveFeatures.AddFull(profile,
                this.ComponentDefinition.WorkAxes[1],
                PartFeatureOperationEnum.kJoinOperation);
        }

        protected EdgeCollection LocateEdgeToEdgeCollection()
        {
            var edge = this.ComponentDefinition.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                this.TransientGeometry.CreatePoint(
                    (this.BoreFromEdge == BoreFromEdge.FromLeft
                        ? this.EdgeFeature.EdgePoint.X
                        : this.TestShaftLength - this.EdgeFeature.EdgePoint.X).InMillimeters(),
                    this.EdgeFeature.EdgePoint.Y.InMillimeters())) as Edge;
            var edgeColl = Shaft.Application.TransientObjects.CreateEdgeCollection();
            edgeColl.Add(edge);
            return edgeColl;
        }

        public void Dispose()
        {
            this.PartDocument.ReleaseReference();
        }
    }
}