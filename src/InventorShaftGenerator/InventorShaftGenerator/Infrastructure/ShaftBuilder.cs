using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Inventor;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefGOSTUnits;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models.SubFeatures;
using Point = Inventor.Point;

namespace InventorShaftGenerator.Infrastructure
{
    public class ShaftBuilder
    {
        private static readonly AssemblyDocument AsmDoc;
        private static readonly Application InvApp;
        private static readonly TransientGeometry TransientGeometry;
        private static readonly string ColorName = "Steel";
        private static PartDocument partDocument;
        private static readonly List<FeatureConstructionError> ConstructionErrors = new List<FeatureConstructionError>();

        static ShaftBuilder()
        {
            InvApp = Shaft.Application;
            AsmDoc = (AssemblyDocument)InvApp.ActiveDocument;
            TransientGeometry = InvApp.TransientGeometry;
        }

        public static List<FeatureConstructionError> BuildShaft()
        {
            ConstructionErrors.Clear();

            if (AsmDoc.ComponentDefinition.Occurrences.Count == 1)
            {
                AsmDoc.ComponentDefinition.Occurrences[1].Delete();
            }

            var partDoc = partDocument =
                (PartDocument)InvApp.Documents.Add(DocumentTypeEnum.kPartDocumentObject, CreateVisible: false);
            partDoc.UnitsOfMeasure.LengthUnits = UnitsTypeEnum.kMillimeterLengthUnits;

            partDoc.DisplayName = "Shaft";

            PartComponentDefinition compDef = partDoc.ComponentDefinition;

            BuildMainRevolve(compDef);
            
            BuildChamfers(compDef);

            BuildFillets(compDef);

            BuildThreads(compDef);

            BuildFinalPolygons(compDef);
            
            BuildLockNutGrooves(compDef);
            
            BuildPlainKeywayGrooves(compDef);
            
            BuildKeywayGroovesRoundedEnd(compDef);

            BuildReliefsASi(compDef);

            BuildReliefsBSi(compDef);

            BuildReliefsFSi(compDef);

            BuildReliefsGSi(compDef);

            BuildRelifsADin(compDef);

            BuildRelifsBDin(compDef);

            BuildRelifsCDin(compDef);

            BuildReliefsDDin(compDef);

            BuildReliefsEDin(compDef);

            BuildRelifsFDin(compDef);

            BuildRelifsAGost(compDef);

            BuildRelifsBGost(compDef);

            BuildRelifsCGost(compDef);
            
            BuildReliefsDGost(compDef);
            
            BuildReliefsEGost(compDef);
            
            BuildThroughHoles(compDef);
            
            BuildWrenches(compDef);
            
            BuildRetainingRings(compDef);
            
            BuildKeywayGrooves(compDef);
            
            BuildGroovesA(compDef);
            
            BuildGroovesB(compDef);
            
            BuildReliefsDSI(compDef);
            
            BuildLeftBore(compDef);
            
            BuildRightBore(compDef);
            
            BuildChamfers(compDef, true, BoreFromEdge.FromLeft);
            
            BuildChamfers(compDef, true, BoreFromEdge.FromRight);
            
            BuildFillets(compDef, true, BoreFromEdge.FromLeft);
            
            BuildFillets(compDef, true, BoreFromEdge.FromRight);
            
            BuildThreads(compDef, true, BoreFromEdge.FromLeft);
            
            BuildThreads(compDef, true, BoreFromEdge.FromRight);
            
            BuildRetainingRings(compDef, true, BoreFromEdge.FromLeft);
            
            BuildRetainingRings(compDef, true, BoreFromEdge.FromRight);

            AsmDoc.ComponentDefinition.Occurrences.AddByComponentDefinition(
                (ComponentDefinition)partDoc.ComponentDefinition, TransientGeometry.CreateMatrix());
            AsmDoc.ComponentDefinition.Occurrences[1].SetRenderStyle(StyleSourceTypeEnum.kOverrideRenderStyle,
                AsmDoc.RenderStyles[ColorName]);
            AsmDoc.ComponentDefinition.Occurrences[1].Grounded = false;
            InvApp.CommandManager.ControlDefinitions["AppViewCubeHomeCmd"].Execute();
            InvApp.ActiveView.Fit();

            return ConstructionErrors;
        }

        private static void BuildMainRevolve(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;

            PlanarSketch mainSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
            mainSketch.NaturalAxisDirection = true;
            var sketchLines = mainSketch.SketchLines;
            var axisParams = Shaft.RevolveAxis;

            sketchLines.AddByTwoPoints(
                TransientGeometry.CreatePoint2d(axisParams.StartPoint.X.InMillimeters(),
                    axisParams.StartPoint.Y.InMillimeters()),
                TransientGeometry.CreatePoint2d(axisParams.EndPoint.X.InMillimeters(),
                    axisParams.EndPoint.Y.InMillimeters()));

            for (var index = 0; index < sections.Count; index++)
            {
                var section = sections[index];
                if (section.FirstLine != null)
                {
                    var firstLineParams = section.FirstLine;
                    Point2d firstPoint = TransientGeometry.CreatePoint2d(firstLineParams.StartPoint.X.InMillimeters(),
                        firstLineParams.StartPoint.Y.InMillimeters());
                    Point2d secondPoint =
                        TransientGeometry.CreatePoint2d(firstLineParams.EndPoint.X.InMillimeters(),
                            firstLineParams.EndPoint.Y.InMillimeters());
                    sketchLines.AddByTwoPoints(firstPoint, secondPoint);
                }


                var secondLineParams = section.SecondLine;

                if (Shaft.Sections.IndexOf(section) != 0)
                {
                    var previousSectionSecondLine = sections[index - 1].SecondLine;
                    if (previousSectionSecondLine.EndPoint != secondLineParams.StartPoint)
                    {
                        sketchLines.AddByTwoPoints(
                            TransientGeometry.CreatePoint2d(previousSectionSecondLine.EndPoint.X.InMillimeters(),
                                previousSectionSecondLine.EndPoint.Y.InMillimeters()),
                            TransientGeometry.CreatePoint2d(secondLineParams.StartPoint.X.InMillimeters(),
                                secondLineParams.StartPoint.Y.InMillimeters()));
                    }
                }

                sketchLines.AddByTwoPoints(
                    TransientGeometry.CreatePoint2d(secondLineParams.StartPoint.X.InMillimeters(),
                        secondLineParams.StartPoint.Y.InMillimeters()),
                    TransientGeometry.CreatePoint2d(secondLineParams.EndPoint.X.InMillimeters(),
                        secondLineParams.EndPoint.Y.InMillimeters()));


                if (section.ThirdLine != null)
                {
                    var thirdLineParams = section.ThirdLine;
                    sketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(thirdLineParams.StartPoint.X.InMillimeters(),
                            thirdLineParams.StartPoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(thirdLineParams.EndPoint.X.InMillimeters(),
                            thirdLineParams.EndPoint.Y.InMillimeters()));
                }
            }

            sketchLines[1].StartSketchPoint.Merge(sketchLines[2].StartSketchPoint);
            for (int i = 2; i < sketchLines.Count; i++)
            {
                sketchLines[i].EndSketchPoint.Merge(sketchLines[i + 1].StartSketchPoint);
            }

            sketchLines[1].EndSketchPoint.Merge(sketchLines[sketchLines.Count].EndSketchPoint);


            mainSketch.Color = InvApp.TransientObjects.CreateColor(102, 158, 97);
            var profile = mainSketch.Profiles.AddForSolid();
            var revolveFeature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                PartFeatureOperationEnum.kJoinOperation);
            revolveFeature.Name = "Shaft_Revolution";
        }

        private static void BuildFinalPolygons(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;

            int polygonNumber = 0;
            foreach (var polygon in sections.Where(section => section.GetType() == typeof(PolygonSection))
                                            .Cast<PolygonSection>())
            {
                polygonNumber++;
                var plane =
                    compDef.WorkPlanes.AddByPlaneAndOffset(compDef.WorkPlanes[1],
                        polygon.SecondLine.StartPoint.X.InMillimeters());
                var polygonSweepSketch = compDef.Sketches.Add(plane);

                var polygonVertices = polygon.CalculateVertices(
                    inscribedRadius: (polygon.InscribedCircleDiameter / 2),
                    edgesCount: polygon.NumberOfEdges,
                    startingAngle: polygon.SectionAngle,
                    centerPoint: PointF.Empty);


                // Draw polygon`s vertices points
                foreach (var vertex in polygonVertices)
                {
                    polygonSweepSketch.SketchPoints.Add(
                        TransientGeometry.CreatePoint2d(vertex.X.InMillimeters(),
                            vertex.Y.InMillimeters()));
                }

                // Draw polygon`s lines
                for (int i = 0; i < polygonVertices.Count; i++)
                {
                    var point1 = TransientGeometry.CreatePoint2d(polygonVertices[i].X.InMillimeters(),
                        polygonVertices[i].Y.InMillimeters());
                    int temp = i + 1;
                    if (temp == polygonVertices.Count)
                    {
                        i = -1;
                    }

                    var point2 = TransientGeometry.CreatePoint2d(polygonVertices[i + 1].X.InMillimeters(),
                        polygonVertices[i + 1].Y.InMillimeters());
                    polygonSweepSketch.SketchLines.AddByTwoPoints(point1, point2);
                    if (i == -1)
                    {
                        break;
                    }
                }

                // Merge polygon`s lines
                var polygonSweepSketchLines = polygonSweepSketch.SketchLines;
                polygonSweepSketchLines[1]
                    .StartSketchPoint.Merge(polygonSweepSketchLines[polygonSweepSketchLines.Count].EndSketchPoint);
                for (int i = 1; i < polygonSweepSketchLines.Count; i++)
                {
                    polygonSweepSketchLines[i].EndSketchPoint.Merge(polygonSweepSketchLines[i + 1].StartSketchPoint);
                }

                var polygonExtrudeProfile = polygonSweepSketch.Profiles.AddForSolid();

                var exturusion = compDef.Features.ExtrudeFeatures.AddByDistanceExtent(Profile: polygonExtrudeProfile,
                    Distance: polygon.Length.InMillimeters(),
                    ExtentDirection:
                    PartFeatureExtentDirectionEnum
                        .kPositiveExtentDirection,
                    Operation:
                    PartFeatureOperationEnum
                        .kJoinOperation);
                plane.Name = $"Polygon_#{polygonNumber}_Plane";
                polygonSweepSketch.Name = $"Polygon_#{polygonNumber}_Sketch";
                exturusion.Name = $"Polygon_#{polygonNumber}";
                plane.Visible = false;
                polygonSweepSketch.Visible = false;

                // Build polygon`s chamfer
                ChamferEdgeFeature feature1 = null;
                ChamferEdgeFeature feature2 = null;
                if (polygon.FirstEdgeFeature is ChamferEdgeFeature cef1)
                {
                    feature1 = cef1;
                }
                if (polygon.SecondEdgeFeature is ChamferEdgeFeature cef2)
                {
                    feature2 = cef2;
                }

                if (feature1 != null)
                {
                    BuildChamfer(feature1);
                }
                else if (feature2 != null)
                {
                    BuildChamfer(feature2);
                }

                void BuildChamfer(ChamferEdgeFeature chamferEdgeFeature)
                {
                    var hight = polygon.CircumscribedCircleDiameter * 0.07734;
                    var polygonChamferSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);

                    var line1 = polygonChamferSketch.SketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(
                            chamferEdgeFeature.EdgePoint.X.InMillimeters(),
                            chamferEdgeFeature.EdgePoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(
                            chamferEdgeFeature.EdgePoint.X.InMillimeters(),
                            chamferEdgeFeature.EdgePoint.Y.InMillimeters() +
                            hight / 10));

                    var line2 = polygonChamferSketch.SketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(
                            line1.EndSketchPoint.Geometry.X,
                            line1.EndSketchPoint.Geometry.Y),
                        TransientGeometry.CreatePoint2d(
                            chamferEdgeFeature.EdgePosition ==
                            EdgeFeaturePosition.FirstEdge
                                ? line1.EndSketchPoint.Geometry.X +
                                  chamferEdgeFeature.Distance.InMillimeters()
                                : line1.EndSketchPoint.Geometry.X -
                                  chamferEdgeFeature.Distance.InMillimeters(),
                            line1.EndSketchPoint.Geometry.Y));

                    var line3 = polygonChamferSketch.SketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(
                            line2.EndSketchPoint.Geometry.X,
                            line2.EndSketchPoint.Geometry.Y),
                        TransientGeometry.CreatePoint2d(
                            line1.StartSketchPoint.Geometry.X,
                            line1.StartSketchPoint.Geometry.Y));
                    line1.StartSketchPoint.Merge(line3.EndSketchPoint);
                    line1.EndSketchPoint.Merge(line2.StartSketchPoint);
                    line2.EndSketchPoint.Merge(line3.StartSketchPoint);
                    var p = polygonChamferSketch.Profiles.AddForSolid();
                    var revolve =
                        compDef.Features.RevolveFeatures.AddFull(p, compDef.WorkAxes[1],
                            PartFeatureOperationEnum.kCutOperation);
                    revolve.Name = $"Polygon_{polygonNumber}_Chamfer";
                }
            }
        }

        private static void BuildFillets(PartComponentDefinition compDef, bool boreSections = false,
            BoreFromEdge? boreFromEdge = null)
        {
            var sections = boreSections
                ? (boreFromEdge == BoreFromEdge.FromLeft ? Shaft.BoreOnTheLeft : Shaft.BoreOnTheRight)
                : Shaft.Sections;
            var shaftLength = Shaft.Sections.Sum(section => section.Length);

            List<FilletEdgeFeature> allFillets = new List<FilletEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is FilletEdgeFeature fillet1 && fillet1.ShouldBeBuilt)
                {
                    allFillets.Add(fillet1);
                }

                if (section.SecondEdgeFeature is FilletEdgeFeature fillet2 && fillet2.ShouldBeBuilt)
                {
                    allFillets.Add(fillet2);
                }
            }

            foreach (var fillet in allFillets)
            {
                Edge filletEdge;
                if (boreSections && boreFromEdge == BoreFromEdge.FromRight)
                {
                    filletEdge = compDef.SurfaceBodies[1]
                                        .LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                                            TransientGeometry.CreatePoint(shaftLength.InMillimeters() -
                                                                          fillet.EdgePoint.X.InMillimeters(),
                                                fillet.EdgePoint.Y.InMillimeters()), 0.01);
                }
                else
                {
                    filletEdge = compDef.SurfaceBodies[1]
                                        .LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                                            TransientGeometry.CreatePoint(
                                                fillet.EdgePoint.X.InMillimeters(),
                                                fillet.EdgePoint.Y.InMillimeters()), 0.01);
                }

                var edgeCollection = InvApp.TransientObjects.CreateEdgeCollection();

                edgeCollection.Add(filletEdge);
                var filletFeature =
                    compDef.Features.FilletFeatures.AddSimple(edgeCollection, fillet.Radius.InMillimeters());
            }
        }

        private static void BuildChamfers(PartComponentDefinition compDef, bool boreSections = false,
            BoreFromEdge? boreFromEdge = null)
        {
            var sections = boreSections
                ? (boreFromEdge == BoreFromEdge.FromLeft ? Shaft.BoreOnTheLeft : Shaft.BoreOnTheRight)
                : Shaft.Sections;
            var shaftLength = Shaft.Sections.Sum(section => section.Length);
            List<ChamferEdgeFeature> allChamfers = new List<ChamferEdgeFeature>();
            foreach (var section in sections)
            {
                if (section is PolygonSection)
                {
                    continue;
                }

                if (section.FirstEdgeFeature is ChamferEdgeFeature chamfer && chamfer.ShouldBeBuilt)
                {
                    allChamfers.Add(chamfer);
                }

                if (section.SecondEdgeFeature is ChamferEdgeFeature chamfer2 && chamfer2.ShouldBeBuilt)
                {
                    allChamfers.Add(chamfer2);
                }
            }

            foreach (var chamfer in allChamfers)
            {
                try
                {
                    Edge chamferEdge;
                    if (boreSections && boreFromEdge == BoreFromEdge.FromRight)
                    {
                        chamferEdge = compDef.SurfaceBodies[1].LocateUsingPoint(
                            ObjectTypeEnum.kEdgeObject,
                            TransientGeometry.CreatePoint(
                                shaftLength.InMillimeters() - chamfer.EdgePoint.X.InMillimeters(),
                                chamfer.EdgePoint.Y.InMillimeters()),
                            0.01);
                    }
                    else
                    {
                        chamferEdge = compDef.SurfaceBodies[1].LocateUsingPoint(
                            ObjectTypeEnum.kEdgeObject,
                            TransientGeometry.CreatePoint(chamfer.EdgePoint.X.InMillimeters(),
                                chamfer.EdgePoint.Y.InMillimeters()),
                            0.01);
                    }

                    Face sectionFace;
                    if (boreSections && boreFromEdge == BoreFromEdge.FromRight)
                    {
                        sectionFace = compDef.SurfaceBodies[1].LocateUsingPoint(
                            ObjectTypeEnum.kFaceObject,
                            TransientGeometry.CreatePoint(
                                (shaftLength - chamfer.EdgePoint.X - 0.1f).InMillimeters(),
                                chamfer.EdgePoint.Y.InMillimeters()));
                    }
                    else
                    {
                        sectionFace = compDef.SurfaceBodies[1].LocateUsingPoint(
                            ObjectTypeEnum.kFaceObject,
                            TransientGeometry.CreatePoint(
                                (chamfer.EdgePoint.X - 0.1f).InMillimeters(),
                                chamfer.EdgePoint.Y.InMillimeters()));
                    }

                    var chamferFeatures = compDef.Features.ChamferFeatures;
                    var edgeCollection = InvApp.TransientObjects.CreateEdgeCollection();
                    edgeCollection.Add(chamferEdge);
                    switch (chamfer.ChamferType)
                    {
                        case ChamferType.Distance:
                            chamferFeatures.AddUsingDistance(edgeCollection, chamfer.Distance.InMillimeters());
                            break;
                        case ChamferType.DistanceAndAngle:
                            chamferFeatures.AddUsingDistanceAndAngle(edgeCollection, sectionFace,
                                chamfer.Distance.InMillimeters(),
                                chamfer.Angle + "deg");
                            break;
                        case ChamferType.TwoDistances:
                            chamferFeatures.AddUsingTwoDistances(edgeCollection, sectionFace,
                                chamfer.Distance1.InMillimeters(),
                                chamfer.Distance2.InMillimeters());

                            break;
                    }
                }
                catch
                {
                    ConstructionErrors.Add(new FeatureConstructionError(chamfer));
                    continue;
                }
            }
        }

        private static void BuildThreads(PartComponentDefinition compDef, bool boreSections = false,
            BoreFromEdge? boreFromEdge = null)
        {
            var sections =
                boreSections
                    ? (boreFromEdge == BoreFromEdge.FromLeft ? Shaft.BoreOnTheLeft : Shaft.BoreOnTheRight)
                    : Shaft.Sections;
            List<ThreadEdgeFeature> allThreads = new List<ThreadEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ThreadEdgeFeature thread1 && thread1.ShouldBeBuilt)
                {
                    allThreads.Add(thread1);
                }

                if (section.SecondEdgeFeature is ThreadEdgeFeature thread2 && thread2.ShouldBeBuilt)
                {
                    allThreads.Add(thread2);
                }
            }

            foreach (var thread in allThreads)
            {
                BuildThread(thread, compDef);
            }
        }

        private static void BuildThread(ThreadEdgeFeature threadEdgeFeature, PartComponentDefinition compDef)
        {
            try
            {
                var shaftLength = Shaft.Sections.Sum(section => section.Length);
                Edge threadEdge;
                if (threadEdgeFeature.LinkedSection.IsBore &&
                    threadEdgeFeature.LinkedSection.BoreFromEdge == BoreFromEdge.FromRight)
                {
                    threadEdge = compDef.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                        TransientGeometry.CreatePoint(
                            shaftLength.InMillimeters() -
                            threadEdgeFeature.EdgePoint.X.InMillimeters(),
                            threadEdgeFeature.EdgePoint.Y.InMillimeters()),
                        0.01) as Edge;
                }
                else
                {
                    threadEdge = compDef.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                        TransientGeometry.CreatePoint(
                            threadEdgeFeature.EdgePoint.X.InMillimeters(),
                            threadEdgeFeature.EdgePoint.Y.InMillimeters()),
                        0.01) as Edge;
                }

                Face sectionFace;
                if (threadEdgeFeature.LinkedSection.IsBore &&
                    threadEdgeFeature.LinkedSection.BoreFromEdge == BoreFromEdge.FromRight)
                {
                    sectionFace = compDef.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kFaceObject,
                        TransientGeometry.CreatePoint(
                            threadEdgeFeature.EdgePosition == EdgeFeaturePosition.FirstEdge
                                ? shaftLength.InMillimeters() -
                                  threadEdgeFeature.EdgePoint.X.InMillimeters() -
                                  threadEdgeFeature.ThreadLength.InMillimeters()
                                : shaftLength.InMillimeters() -
                                  threadEdgeFeature.EdgePoint.X.InMillimeters() +
                                  threadEdgeFeature.ThreadLength.InMillimeters(),
                            threadEdgeFeature.EdgePoint.Y.InMillimeters()), 0.01) as Face;
                }
                else
                {
                    sectionFace = compDef.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kFaceObject,
                        TransientGeometry.CreatePoint(
                            threadEdgeFeature.EdgePosition == EdgeFeaturePosition.FirstEdge
                                ? threadEdgeFeature.EdgePoint.X.InMillimeters() +
                                  threadEdgeFeature.ThreadLength.InMillimeters()
                                : threadEdgeFeature.EdgePoint.X.InMillimeters() -
                                  threadEdgeFeature.ThreadLength.InMillimeters(),
                            threadEdgeFeature.EdgePoint.Y.InMillimeters()), 0.01) as Face;
                }

                ThreadInfo threadInfo = (ThreadInfo)compDef.Features.ThreadFeatures.CreateStandardThreadInfo(
                    Internal: false,
                    RightHanded: threadEdgeFeature.ThreadDirection ==
                                 ThreadDirection.RightHand,
                    ThreadType: threadEdgeFeature.ThreadType,
                    ThreadDesignation: threadEdgeFeature.Designation,
                    Class: threadEdgeFeature.Class);
                compDef.Features.ThreadFeatures.Add(
                    Face: sectionFace,
                    StartEdge: threadEdge,
                    ThreadInfo: threadInfo,
                    FullDepth: false,
                    ThreadDepth: threadEdgeFeature.ThreadLength.InMillimeters() -
                                 threadEdgeFeature.Chamfer.InMillimeters(),
                    ThreadOffset: threadEdgeFeature.Chamfer.InMillimeters());


                if (threadEdgeFeature.Chamfer > 0)
                {
                    BuildThreadChamfer(threadEdgeFeature, compDef);
                }
            }
            catch
            {
                ConstructionErrors.Add(new FeatureConstructionError(threadEdgeFeature));
            }
        }

        private static void BuildThreadChamfer(ThreadEdgeFeature threadEdgeFeature,
            PartComponentDefinition compDef)
        {
            Edge chamferEdge;
            if (threadEdgeFeature.LinkedSection.IsBore &&
                threadEdgeFeature.LinkedSection.BoreFromEdge == BoreFromEdge.FromRight)
            {
                var shaftLength = Shaft.Sections.Sum(section => section.Length);
                chamferEdge = compDef.SurfaceBodies[1].LocateUsingPoint(
                    ObjectTypeEnum.kEdgeObject,
                    TransientGeometry.CreatePoint(
                        shaftLength.InMillimeters() -
                        threadEdgeFeature.EdgePoint.X.InMillimeters(),
                        threadEdgeFeature.EdgePoint.Y.InMillimeters()),
                    0.01);
            }
            else
            {
                chamferEdge = compDef.SurfaceBodies[1].LocateUsingPoint(
                    ObjectTypeEnum.kEdgeObject,
                    TransientGeometry.CreatePoint(threadEdgeFeature.EdgePoint.X.InMillimeters(),
                        threadEdgeFeature.EdgePoint.Y.InMillimeters()),
                    0.01);
            }

            var edgeCollection = InvApp.TransientObjects.CreateEdgeCollection();
            edgeCollection.Add(chamferEdge);
            compDef.Features.ChamferFeatures.AddUsingDistance(edgeCollection,
                threadEdgeFeature.Chamfer.InMillimeters());
        }

        private static void BuildLockNutGrooves(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            List<LockNutGrooveEdgeFeature> allLockNutGrooves = new List<LockNutGrooveEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is LockNutGrooveEdgeFeature lockNutGrooveEdgeFeature1 &&
                    lockNutGrooveEdgeFeature1.ShouldBeBuilt)

                {
                    allLockNutGrooves.Add(lockNutGrooveEdgeFeature1);
                }

                if (section.SecondEdgeFeature is LockNutGrooveEdgeFeature lockNutGrooveEdgeFeature2 &&
                    lockNutGrooveEdgeFeature2.ShouldBeBuilt)
                {
                    allLockNutGrooves.Add(lockNutGrooveEdgeFeature2);
                }
            }

            int lockNutGrooveNumber = 0;
            foreach (var lockNutGroove in allLockNutGrooves)
            {
                try
                {
                    lockNutGrooveNumber++;
                    if (lockNutGroove.ThreadEdgeFeature.ThreadType != "No thread")

                    {
                        BuildThread(lockNutGroove.ThreadEdgeFeature, compDef);
                    }

                    else if (lockNutGroove.ThreadEdgeFeature.Chamfer.IsGreaterThanZero())
                    {
                        BuildThreadChamfer(lockNutGroove.ThreadEdgeFeature, compDef);
                    }

                    WorkPlane lockNutGroovePlane = null;
                    if (lockNutGroove.Angle > 0 || lockNutGroove.Angle < 0)
                    {
                        lockNutGroovePlane =
                            compDef.WorkPlanes.AddByLinePlaneAndAngle(compDef.WorkAxes[1], compDef.WorkPlanes[2],
                                -lockNutGroove.Angle + "deg");
                        lockNutGroovePlane.Visible = false;
                    }

                    var lockNutGrooveSketch = compDef.Sketches.Add(lockNutGroovePlane ?? compDef.WorkPlanes[2]);
                    lockNutGrooveSketch.Visible = false;
                    lockNutGrooveSketch.AxisEntity = compDef.WorkAxes[1];
                    var lines = lockNutGrooveSketch.SketchLines;

                    var line1 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(lockNutGroove.EdgePoint.X.InMillimeters(),
                            -lockNutGroove.EdgePoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(lockNutGroove.EdgePoint.X.InMillimeters(),
                            -lockNutGroove.EdgePoint.Y.InMillimeters() + lockNutGroove.Depth.InMillimeters()));
                    var line2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line1.Geometry.EndPoint.X, line1.Geometry.EndPoint.Y),
                        TransientGeometry.CreatePoint2d(
                            lockNutGroove.ThreadEdgeFeature.EdgePosition == EdgeFeaturePosition.FirstEdge
                                ? lockNutGroove.ThreadEdgeFeature.ThreadLength.InMillimeters()
                                : lockNutGroove.LinkedSection.Length.InMillimeters() -
                                  lockNutGroove.ThreadEdgeFeature.ThreadLength.InMillimeters(),
                            line1.Geometry.EndPoint.Y));

                    var circle = lockNutGrooveSketch.SketchCircles.AddByCenterRadius(
                        TransientGeometry.CreatePoint2d(line2.Geometry.EndPoint.X,
                            line2.Geometry.EndPoint.Y -
                            lockNutGroove.Radius.InMillimeters()),
                        lockNutGroove.Radius.InMillimeters());
                    var line3IntersectWithCircle = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line1.Geometry.StartPoint.X, line1.Geometry.StartPoint.Y),
                        TransientGeometry.CreatePoint2d(
                            lockNutGroove.EdgePosition == EdgeFeaturePosition.FirstEdge
                                ? lockNutGroove.LinkedSection.Length.InMillimeters()
                                : lockNutGroove.LinkedSection.Length.InMillimeters() -
                                  lockNutGroove.LinkedSection.Length.InMillimeters(),
                            line1.Geometry.StartPoint.Y));
                    var intersectPoint = line3IntersectWithCircle.Geometry.IntersectWithCurve(circle.Geometry)[1];
                    var line4 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line1.Geometry.StartPoint.X, line1.Geometry.StartPoint.Y),
                        intersectPoint) as SketchLine;
                    lockNutGrooveSketch.GeometricConstraints.AddCoincident((SketchEntity)line4.EndSketchPoint,
                        (SketchEntity)circle);
                    line3IntersectWithCircle.Delete();
                    line1.StartSketchPoint.Merge(line4.StartSketchPoint);
                    line1.EndSketchPoint.Merge(line2.StartSketchPoint);
                    lockNutGrooveSketch.GeometricConstraints.AddCoincident((SketchEntity)line2.EndSketchPoint,
                        (SketchEntity)circle);

                    var profile = lockNutGrooveSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.ExtrudeFeatures.AddByDistanceExtent(profile,
                        lockNutGroove.Width.InMillimeters(),
                        PartFeatureExtentDirectionEnum.kSymmetricExtentDirection,
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"LockNutGroove_#{lockNutGrooveNumber}";
                }
                catch
                {
                    ConstructionErrors.Add(new FeatureConstructionError(lockNutGroove));
                }
            }
        }

        private static void BuildPlainKeywayGrooves(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            List<PlainKeywayGrooveEdgeFeature> allPlainKeywayGrooves = new List<PlainKeywayGrooveEdgeFeature>();

            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is PlainKeywayGrooveEdgeFeature lockNutGrooveEdgeFeature1 &&
                    lockNutGrooveEdgeFeature1.ShouldBeBuilt)

                {
                    allPlainKeywayGrooves.Add(lockNutGrooveEdgeFeature1);
                }

                if (section.SecondEdgeFeature is PlainKeywayGrooveEdgeFeature lockNutGrooveEdgeFeature2 &&
                    lockNutGrooveEdgeFeature2.ShouldBeBuilt)
                {
                    allPlainKeywayGrooves.Add(lockNutGrooveEdgeFeature2);
                }
            }

            int plainKeywayGrooveNumber = 0;
            foreach (var plainKeywayGroove in allPlainKeywayGrooves)
            {
                try
                {
                    plainKeywayGrooveNumber++;
                    WorkPlane plainKeywayGrooveWorkPlane = null;
                    if (plainKeywayGroove.Angle > 0 || plainKeywayGroove.Angle < 0)

                    {
                        plainKeywayGrooveWorkPlane =
                            compDef.WorkPlanes.AddByLinePlaneAndAngle(compDef.WorkAxes[1], compDef.WorkPlanes[2],
                                -plainKeywayGroove.Angle + "deg");

                        plainKeywayGrooveWorkPlane.Visible = false;
                    }

                    var plainKeywayGrooveSketch =
                        compDef.Sketches.Add(plainKeywayGrooveWorkPlane ?? compDef.WorkPlanes[2]);
                    plainKeywayGrooveSketch.Visible = false;
                    plainKeywayGrooveSketch.AxisEntity = compDef.WorkAxes[1];
                    var lines = plainKeywayGrooveSketch.SketchLines;
                    var line1 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(plainKeywayGroove.EdgePoint.X.InMillimeters(),
                            -plainKeywayGroove.EdgePoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(plainKeywayGroove.EdgePoint.X.InMillimeters(),
                            -plainKeywayGroove.EdgePoint.Y.InMillimeters() + plainKeywayGroove.Depth.InMillimeters()));
                    var line2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line1.Geometry.EndPoint.X, line1.Geometry.EndPoint.Y),
                        TransientGeometry.CreatePoint2d(
                            plainKeywayGroove.EdgePosition == EdgeFeaturePosition.FirstEdge
                                ? line1.EndSketchPoint.Geometry.X + plainKeywayGroove.KeywayLength.InMillimeters()
                                : line1.EndSketchPoint.Geometry.X -
                                  plainKeywayGroove.KeywayLength.InMillimeters(),
                            line1.Geometry.EndPoint.Y));
                    var circle = plainKeywayGrooveSketch.SketchCircles.AddByCenterRadius(
                        TransientGeometry.CreatePoint2d(line2.Geometry.EndPoint.X,
                            line2.Geometry.EndPoint.Y -
                            plainKeywayGroove.Radius.InMillimeters()),
                        plainKeywayGroove.Radius.InMillimeters());
                    var line3IntersectWithCircle = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line1.Geometry.StartPoint.X, line1.Geometry.StartPoint.Y),
                        TransientGeometry.CreatePoint2d(
                            plainKeywayGroove.EdgePosition == EdgeFeaturePosition.FirstEdge
                                ? plainKeywayGroove.LinkedSection.Length.InMillimeters()
                                : plainKeywayGroove.LinkedSection.Length.InMillimeters() -
                                  plainKeywayGroove.LinkedSection.Length.InMillimeters(),
                            line1.Geometry.StartPoint.Y));
                    var intersectPoint = line3IntersectWithCircle.Geometry.IntersectWithCurve(circle.Geometry)[1];
                    var line4 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line1.Geometry.StartPoint.X, line1.Geometry.StartPoint.Y),
                        intersectPoint) as SketchLine;
                    plainKeywayGrooveSketch.GeometricConstraints.AddCoincident((SketchEntity)line4.EndSketchPoint,
                        (SketchEntity)circle);
                    line3IntersectWithCircle.Delete();
                    line1.StartSketchPoint.Merge(line4.StartSketchPoint);

                    line1.EndSketchPoint.Merge(line2.StartSketchPoint);
                    plainKeywayGrooveSketch.GeometricConstraints.AddCoincident((SketchEntity)line2.EndSketchPoint,
                        (SketchEntity)circle);

                    if (plainKeywayGroove.Chamfer.IsGreaterThanZero())
                    {
                        var chamferEdge = compDef.SurfaceBodies[1].LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                            TransientGeometry.CreatePoint(
                                plainKeywayGroove.EdgePoint.X.InMillimeters(),
                                -plainKeywayGroove.EdgePoint.Y.InMillimeters()), 0.01);
                        var edgeColl = InvApp.TransientObjects.CreateEdgeCollection();
                        edgeColl.Add(chamferEdge);
                        compDef.Features.ChamferFeatures.AddUsingDistance(edgeColl,
                            plainKeywayGroove.Chamfer.InMillimeters());
                    }

                    var profile = plainKeywayGrooveSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.ExtrudeFeatures.AddByDistanceExtent(profile,
                        plainKeywayGroove.Width.InMillimeters(),
                        PartFeatureExtentDirectionEnum.kSymmetricExtentDirection,
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"PlainKeywayGroove_#{plainKeywayGrooveNumber}";
                    if (plainKeywayGroove.NumberOfKeys == 1)
                    {
                        continue;
                    }

                    var featuresColl = InvApp.TransientObjects.CreateObjectCollection();
                    featuresColl.Add(feature);
                    var definition =
                        compDef.Features.CircularPatternFeatures.CreateDefinition(
                            ParentFeatures: featuresColl,
                            AxisEntity: compDef.WorkAxes[1],
                            NaturalAxisDirection: true,
                            Count: plainKeywayGroove.NumberOfKeys,
                            Angle: 0);
                    var circularPatternFeature =
                        compDef.Features.CircularPatternFeatures.AddByDefinition(definition);
                    circularPatternFeature.Angle._Value = plainKeywayGroove.NumberOfKeys == 2
                        ? MathExtensions.DegreesToRadians(plainKeywayGroove.AngleBetweenKeys)
                        : MathExtensions.DegreesToRadians(360);
                }
                catch
                {
                    ConstructionErrors.Add(new FeatureConstructionError(plainKeywayGroove));
                }
            }
        }

        private static void BuildKeywayGroovesRoundedEnd(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;

            var allKeywayGroovesRoundedEnd = new List<KeywayGrooveRoundedEndEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is KeywayGrooveRoundedEndEdgeFeature keywayGrooveRoundedEndEdgeFeature1 &&
                    keywayGrooveRoundedEndEdgeFeature1.ShouldBeBuilt)
                {
                    allKeywayGroovesRoundedEnd.Add(keywayGrooveRoundedEndEdgeFeature1);
                }


                if (section.SecondEdgeFeature is KeywayGrooveRoundedEndEdgeFeature keywayGrooveRoundedEndEdgeFeature2 &&
                    keywayGrooveRoundedEndEdgeFeature2.ShouldBeBuilt)
                {
                    allKeywayGroovesRoundedEnd.Add(keywayGrooveRoundedEndEdgeFeature2);
                }
            }

            int keywayGrooveNumber = 0;
            foreach (var keywayGrooveRoundedEnd in allKeywayGroovesRoundedEnd)
            {
                try
                {
                    keywayGrooveNumber++;
                    var cylinderSection = (CylinderSection)keywayGrooveRoundedEnd.LinkedSection;
                    WorkPlane keywayGrooveWorkPlane = null;
                    if (keywayGrooveRoundedEnd.Angle > 0 || keywayGrooveRoundedEnd.Angle < 0)
                    {
                        var helperWorkPlane =
                            compDef.WorkPlanes.AddByLinePlaneAndAngle(compDef.WorkAxes[1], compDef.WorkPlanes[3],
                                -keywayGrooveRoundedEnd.Angle + "deg");

                        helperWorkPlane.Visible = false;
                        keywayGrooveWorkPlane = compDef.WorkPlanes.AddByPlaneAndOffset(helperWorkPlane,
                            (cylinderSection.Diameter / 2).InMillimeters());
                    }
                    else
                    {
                        keywayGrooveWorkPlane = compDef.WorkPlanes.AddByPlaneAndOffset(compDef.WorkPlanes[3],
                            (cylinderSection.Diameter / 2).InMillimeters());
                    }

                    keywayGrooveWorkPlane.Visible = false;
                    var keywayGrooveSketch = compDef.Sketches.Add(keywayGrooveWorkPlane);
                    keywayGrooveSketch.Visible = false;
                    keywayGrooveSketch.AxisEntity = compDef.WorkAxes[1];
                    var lines = keywayGrooveSketch.SketchLines;
                    var line1 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(keywayGrooveRoundedEnd.EdgePoint.X.InMillimeters(),
                            (-keywayGrooveRoundedEnd.Width / 2).InMillimeters()),
                        TransientGeometry.CreatePoint2d(keywayGrooveRoundedEnd.EdgePoint.X.InMillimeters(),
                            (keywayGrooveRoundedEnd.Width / 2).InMillimeters()));
                    var line2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line1.Geometry.EndPoint.X, line1.Geometry.EndPoint.Y),
                        TransientGeometry.CreatePoint2d(
                            keywayGrooveRoundedEnd.EdgePosition == EdgeFeaturePosition.FirstEdge
                                ? (keywayGrooveRoundedEnd.KeywayLength - keywayGrooveRoundedEnd.Width / 2)
                                .InMillimeters()
                                : (keywayGrooveRoundedEnd.LinkedSection.Length -
                                   keywayGrooveRoundedEnd.KeywayLength + keywayGrooveRoundedEnd.Width / 2)
                                .InMillimeters(),
                            line1.Geometry.EndPoint.Y));
                    var line4 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line2.EndSketchPoint.Geometry.X,
                            (-keywayGrooveRoundedEnd.Width / 2).InMillimeters()),
                        TransientGeometry.CreatePoint2d(line1.StartSketchPoint.Geometry.X,
                            line1.StartSketchPoint.Geometry.Y));
                    SketchArc arc = null;
                    if (keywayGrooveRoundedEnd.EdgePosition == EdgeFeaturePosition.FirstEdge)
                    {
                        arc = keywayGrooveSketch.SketchArcs.AddByCenterStartEndPoint(
                            TransientGeometry.CreatePoint2d(line2.EndSketchPoint.Geometry.X,
                                0),
                            TransientGeometry.CreatePoint2d(line4.StartSketchPoint.Geometry.X,
                                line4.StartSketchPoint.Geometry.Y),
                            TransientGeometry.CreatePoint2d(line2.EndSketchPoint.Geometry.X,
                                line2.EndSketchPoint.Geometry.Y));
                    }
                    else
                    {
                        arc = keywayGrooveSketch.SketchArcs.AddByThreePoints(
                            TransientGeometry.CreatePoint2d(line4.StartSketchPoint.Geometry.X,
                                line4.StartSketchPoint.Geometry.Y),
                            TransientGeometry.CreatePoint2d(
                                (keywayGrooveRoundedEnd.LinkedSection.Length -
                                 keywayGrooveRoundedEnd.KeywayLength).InMillimeters(), 0),
                            TransientGeometry.CreatePoint2d(line2.EndSketchPoint.Geometry.X,
                                line2.EndSketchPoint.Geometry.Y)
                        );
                    }

                    line1.StartSketchPoint.Merge(line4.EndSketchPoint);
                    line1.EndSketchPoint.Merge(line2.StartSketchPoint);
                    if (keywayGrooveRoundedEnd.EdgePosition == EdgeFeaturePosition.FirstEdge)
                    {
                        arc.StartSketchPoint.Merge(line4.StartSketchPoint);
                        arc.EndSketchPoint.Merge(line2.EndSketchPoint);
                    }
                    else
                    {
                        arc.EndSketchPoint.Merge(line4.StartSketchPoint);
                        arc.StartSketchPoint.Merge(line2.EndSketchPoint);
                    }

                    if (keywayGrooveRoundedEnd.Chamfer.IsGreaterThanZero())
                    {
                        var chamferEdge = compDef.SurfaceBodies[1]
                                                 .LocateUsingPoint(ObjectTypeEnum.kEdgeObject,
                                                     TransientGeometry.CreatePoint(
                                                         keywayGrooveRoundedEnd.EdgePoint.X.InMillimeters(),
                                                         -keywayGrooveRoundedEnd.EdgePoint.Y.InMillimeters()), 0.01);
                        var edgeColl = InvApp.TransientObjects.CreateEdgeCollection();
                        edgeColl.Add(chamferEdge);
                        compDef.Features.ChamferFeatures.AddUsingDistance(edgeColl,
                            keywayGrooveRoundedEnd.Chamfer.InMillimeters());
                    }

                    var profile = keywayGrooveSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.ExtrudeFeatures.AddByDistanceExtent(profile,
                        keywayGrooveRoundedEnd.Depth.InMillimeters(),
                        PartFeatureExtentDirectionEnum.kNegativeExtentDirection,
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"KeywayGrooveWithRoundedEnd_#{keywayGrooveNumber}";
                    if (keywayGrooveRoundedEnd.NumberOfKeys == 1)
                    {
                        continue;
                    }

                    var featuresColl = InvApp.TransientObjects.CreateObjectCollection();
                    featuresColl.Add(feature);
                    var definition =
                        compDef.Features.CircularPatternFeatures.CreateDefinition(
                            ParentFeatures: featuresColl,
                            AxisEntity: compDef.WorkAxes[1],
                            NaturalAxisDirection: true,
                            Count: keywayGrooveRoundedEnd.NumberOfKeys,
                            Angle: 0);
                    var circularPatternFeature =
                        compDef.Features.CircularPatternFeatures.AddByDefinition(definition);

                    circularPatternFeature.Angle._Value =
                        keywayGrooveRoundedEnd.NumberOfKeys == 2
                            ? MathExtensions.DegreesToRadians(keywayGrooveRoundedEnd.AngleBetweenKeys)
                            : MathExtensions.DegreesToRadians(360);
                }
                catch
                {
                    ConstructionErrors.Add(new FeatureConstructionError(keywayGrooveRoundedEnd));
                }
            }
        }

        private static void BuildReliefsASi(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsASi = new List<ReliefASIEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefASIEdgeFeature reliefASi1 &&
                    reliefASi1.ShouldBeBuilt)
                {
                    allReliefsASi.Add(reliefASi1);
                }


                if (section.SecondEdgeFeature is ReliefASIEdgeFeature reliefASi2 &&
                    reliefASi2.ShouldBeBuilt)
                {
                    allReliefsASi.Add(reliefASi2);
                }
            }

            int reliefASiNumber = 0;
            foreach (var reliefASi in allReliefsASi)
            {
                try
                {
                    reliefASiNumber++;
                    bool firstEdge = reliefASi.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefASiSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefASiSketch.AxisEntity = compDef.WorkAxes[1];

                    reliefASiSketch.Visible = false;
                    var points = reliefASiSketch.SketchPoints;
                    var lines = reliefASiSketch.SketchLines;

                    var arcs = reliefASiSketch.SketchArcs;
                    var pointFromEdgeToB1 = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefASi.EdgePoint.X + reliefASi.Width1).InMillimeters()
                            : (reliefASi.EdgePoint.X - reliefASi.Width1).InMillimeters(),
                        reliefASi.EdgePoint.Y.InMillimeters()));

                    var pointFromEdgeToB = points.Add(TransientGeometry.CreatePoint2d(
                        reliefASi.EdgePoint.X.InMillimeters(),
                        (reliefASi.EdgePoint.Y + reliefASi.Width1).InMillimeters()));
                    var pointFromEdgeToDepthPlusA = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefASi.EdgePoint.X + reliefASi.Width1 / 2).InMillimeters()
                            : (reliefASi.EdgePoint.X - reliefASi.Width1 / 2).InMillimeters(),
                        (reliefASi.EdgePoint.Y - reliefASi.ReliefDepth - reliefASi.MachiningAllowance)
                        .InMillimeters()));

                    var pointFromEdgeToDepthPlusRigth = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefASi.EdgePoint.X - reliefASi.ReliefDepth - reliefASi.MachiningAllowance)
                            .InMillimeters()
                            : (reliefASi.EdgePoint.X + reliefASi.ReliefDepth + reliefASi.MachiningAllowance)
                            .InMillimeters(), (reliefASi.EdgePoint.Y + reliefASi.Width / 2).InMillimeters()));
                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToB1);
                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToB);
                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToDepthPlusA);
                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToDepthPlusRigth);

                    var line1 = lines.AddByTwoPoints(pointFromEdgeToB1.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToB1.Geometry.X,
                            pointFromEdgeToDepthPlusA.Geometry.Y));
                    var line2 = lines.AddByTwoPoints(line1.EndSketchPoint.Geometry, pointFromEdgeToDepthPlusA.Geometry);
                    var line3 = lines.AddByTwoPoints(pointFromEdgeToDepthPlusA.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepthPlusRigth.Geometry.X,
                            pointFromEdgeToDepthPlusA.Geometry.Y));
                    var line4 = lines.AddByTwoPoints(line3.EndSketchPoint.Geometry,
                        pointFromEdgeToDepthPlusRigth.Geometry);
                    var line5 = lines.AddByTwoPoints(line4.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepthPlusRigth.Geometry.X,
                            pointFromEdgeToB.Geometry.Y));
                    var line6 = lines.AddByTwoPoints(line5.EndSketchPoint.Geometry, pointFromEdgeToB.Geometry);
                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)line1);
                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)line2);
                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)line3);

                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)line4);

                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)line5);

                    reliefASiSketch.GeometricConstraints.AddGround((SketchEntity)line6);

                    var fillet1 = arcs.AddByFillet((SketchEntity)line1, (SketchEntity)line2,
                        reliefASi.Radius.InMillimeters(), line1.StartSketchPoint.Geometry,
                        line2.EndSketchPoint.Geometry);
                    var fillet2 = arcs.AddByFillet((SketchEntity)line3, (SketchEntity)line4,
                        reliefASi.Radius.InMillimeters(), line3.StartSketchPoint.Geometry,
                        line4.EndSketchPoint.Geometry);
                    var fillet3 = arcs.AddByFillet((SketchEntity)line5, (SketchEntity)line6,
                        reliefASi.Radius.InMillimeters(), line5.StartSketchPoint.Geometry,
                        line6.EndSketchPoint.Geometry);

                    var fillet1EndFillet2StartIntersectPoint =
                        TransientGeometry.CurveCurveIntersection(
                            TransientGeometry.CreateArc3d(fillet1.Geometry3d.Center, fillet1.Geometry3d.Normal,
                                fillet1.Geometry3d.ReferenceVector, fillet1.Radius, fillet1.StartAngle,
                                fillet1.SweepAngle),
                            TransientGeometry.CreateArc3d(fillet2.Geometry3d.Center, fillet2.Geometry3d.Normal,
                                fillet2.Geometry3d.ReferenceVector, fillet2.Radius, fillet2.StartAngle,
                                fillet2.SweepAngle))[1] as Point;
                    var fillet2EndFillet3StartIntersectPoint = TransientGeometry.CurveCurveIntersection(
                        TransientGeometry.CreateArc3d(fillet2.Geometry3d.Center, fillet2.Geometry3d.Normal,
                            fillet2.Geometry3d.ReferenceVector, fillet2.Radius, fillet2.StartAngle, fillet2.SweepAngle),
                        TransientGeometry.CreateArc3d(fillet3.Geometry3d.Center, fillet3.Geometry3d.Normal,
                            fillet3.Geometry3d.ReferenceVector, fillet3.Radius, fillet3.StartAngle,
                            fillet3.SweepAngle))[1] as Point;
                    var point1 = TransientGeometry.CreatePoint2d(fillet1EndFillet2StartIntersectPoint.X,
                        fillet1EndFillet2StartIntersectPoint.Y);

                    var point2 = TransientGeometry.CreatePoint2d(fillet2EndFillet3StartIntersectPoint.X,
                        fillet2EndFillet3StartIntersectPoint.Y);
                    if (firstEdge)
                    {
                        fillet1.StartSketchPoint.MoveTo(point1);
                        fillet2.EndSketchPoint.MoveTo(point1);
                        fillet1.StartSketchPoint.Merge(fillet2.EndSketchPoint);
                        fillet2.StartSketchPoint.MoveTo(point2);
                        fillet3.EndSketchPoint.MoveTo(point2);
                        fillet2.StartSketchPoint.Merge(fillet3.EndSketchPoint);
                    }
                    else
                    {
                        fillet1.EndSketchPoint.MoveTo(point1);
                        fillet2.StartSketchPoint.MoveTo(point1);
                        fillet1.EndSketchPoint.Merge(fillet2.StartSketchPoint);
                        fillet2.EndSketchPoint.MoveTo(point2);
                        fillet3.StartSketchPoint.MoveTo(point2);
                        fillet2.EndSketchPoint.Merge(fillet3.StartSketchPoint);
                    }


                    var lockLine = lines.AddByTwoPoints(
                        firstEdge ? fillet1.EndSketchPoint.Geometry : fillet1.StartSketchPoint.Geometry,
                        firstEdge ? fillet3.StartSketchPoint.Geometry : fillet3.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(firstEdge ? fillet1.EndSketchPoint : fillet1.StartSketchPoint);
                    lockLine.EndSketchPoint.Merge(firstEdge ? fillet3.StartSketchPoint : fillet3.EndSketchPoint);
                    var profile = reliefASiSketch.Profiles.AddForSolid();
                    var reliefADinFeature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    reliefADinFeature.Name = $"ReliefASI_#{reliefASiNumber}";

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    sketch2.AxisEntity = compDef.WorkAxes[1];
                    sketch2.Visible = false;
                    var helperLine3 = sketch2.SketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(reliefASi.EdgePoint.X.InMillimeters(),
                            reliefASi.EdgePoint.Y.InMillimeters()),
                        pointFromEdgeToB1.Geometry);
                    var helperLine4 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(
                        reliefASi.EdgePoint.X.InMillimeters(), reliefASi.EdgePoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(reliefASi.EdgePoint.X.InMillimeters(),
                        (reliefASi.EdgePoint.Y + 1000).InMillimeters()));

                    var intersectPoint = helperLine3.Geometry.IntersectWithCurve(fillet2.Geometry)[1] as Point2d;
                    var intersectPoint2 = helperLine4.Geometry.IntersectWithCurve(fillet2.Geometry)[1] as Point2d;
                    var sketch2Line1 =
                        sketch2.SketchLines.AddByTwoPoints(intersectPoint,
                            TransientGeometry.CreatePoint2d(reliefASi.EdgePoint.X.InMillimeters(),
                                reliefASi.EdgePoint.Y.InMillimeters()));
                    helperLine3.Delete();
                    helperLine4.Delete();
                    var sketch2Line2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d
                        (reliefASi.EdgePoint.X.InMillimeters(), reliefASi.EdgePoint.Y.InMillimeters()),
                        intersectPoint2);

                    sketch2Line2.StartSketchPoint.Merge(sketch2Line1.EndSketchPoint);

                    var filletCenterPoint = TransientGeometry.CreatePoint2d(fillet2.CenterSketchPoint.Geometry.X,
                        fillet2.CenterSketchPoint.Geometry.Y);

                    var sketch2Arc1 = sketch2.SketchArcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge
                            ? sketch2Line2.EndSketchPoint.Geometry
                            : sketch2Line1.StartSketchPoint.Geometry,
                        firstEdge
                            ? sketch2Line1.StartSketchPoint.Geometry
                            : sketch2Line2.EndSketchPoint.Geometry);

                    sketch2Line1.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.EndSketchPoint
                        : sketch2Arc1.StartSketchPoint);
                    sketch2Line2.EndSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.StartSketchPoint
                        : sketch2Arc1.EndSketchPoint);

                    sketch2.Visible = false;
                    var axisLine2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var sketch2Profile = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(sketch2Profile, axisLine2,
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefASi));
                }
            }
        }

        private static void BuildReliefsBSi(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;

            var allReliefsBSi = new List<ReliefBSIEdgeFeature>();

            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefBSIEdgeFeature reliefBSi1 &&
                    reliefBSi1.ShouldBeBuilt)
                {
                    allReliefsBSi.Add(reliefBSi1);
                }

                if (section.SecondEdgeFeature is ReliefBSIEdgeFeature reliefBSi2 &&
                    reliefBSi2.ShouldBeBuilt)
                {
                    allReliefsBSi.Add(reliefBSi2);
                }
            }

            int reliefBSiNumber = 0;
            foreach (var reliefBSi in allReliefsBSi)
            {
                try
                {
                    reliefBSiNumber++;
                    bool firstEdge = reliefBSi.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefBSiSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefBSiSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefBSiSketch.Visible = false;
                    var pointReliefDepthPlusA = TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefBSi.EdgePoint.X + reliefBSi.Radius).InMillimeters()
                            : (reliefBSi.EdgePoint.X - reliefBSi.Radius).InMillimeters(),
                        (reliefBSi.EdgePoint.Y - reliefBSi.ReliefDepth - reliefBSi.MachiningAllowance).InMillimeters());

                    var circleCenterPoint = TransientGeometry.CreatePoint2d(pointReliefDepthPlusA.X,
                        pointReliefDepthPlusA.Y + reliefBSi.Radius.InMillimeters());
                    var circle =
                        reliefBSiSketch.SketchCircles.AddByCenterRadius(circleCenterPoint,
                            reliefBSi.Radius.InMillimeters());

                    var profile = reliefBSiSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"ReliefBSI_#{reliefBSiNumber}";

                    var circleRightTangentPoint =
                        TransientGeometry.CreatePoint2d(
                            firstEdge ? circleCenterPoint.X - circle.Radius : circleCenterPoint.X + circle.Radius,
                            circleCenterPoint.Y);
                    var helperLine = TransientGeometry.CreateLine(
                        TransientGeometry.CreatePoint(reliefBSi.EdgePoint.X.InMillimeters(),
                            reliefBSi.EdgePoint.Y.InMillimeters()),
                        TransientGeometry.CreateVector(firstEdge
                            ? reliefBSi.EdgePoint.X + reliefBSi.Radius.InMillimeters()
                            : reliefBSi.EdgePoint.X - reliefBSi.Radius.InMillimeters()));
                    var circleIntersect =
                        TransientGeometry.CreateCircle(
                            TransientGeometry.CreatePoint(circleCenterPoint.X, circleCenterPoint.Y),
                            circle.Geometry3d.Normal, reliefBSi.Radius.InMillimeters());
                    var intesectPoint =
                        TransientGeometry.CurveCurveIntersection(circleIntersect, helperLine)[firstEdge ? 2 : 1] as Point;
                    var intersectPoint2d = TransientGeometry.CreatePoint2d(intesectPoint.X, intesectPoint.Y);
                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    var line1 = sketch2.SketchLines.AddByTwoPoints(intersectPoint2d,
                        TransientGeometry.CreatePoint2d(reliefBSi.EdgePoint.X.InMillimeters(),
                            reliefBSi.EdgePoint.Y.InMillimeters()));
                    var line2 = sketch2.SketchLines.AddByTwoPoints(line1.EndSketchPoint.Geometry,
                        circleRightTangentPoint);

                    var arc = sketch2.SketchArcs.AddByCenterStartEndPoint(circleCenterPoint, 
                        firstEdge ? circleRightTangentPoint : intersectPoint2d,
                        firstEdge ? intersectPoint2d : circleRightTangentPoint);
                    line1.StartSketchPoint.Merge(firstEdge ? arc.EndSketchPoint : arc.StartSketchPoint);
                    line2.EndSketchPoint.Merge(firstEdge ? arc.StartSketchPoint : arc.EndSketchPoint);
                    line1.EndSketchPoint.Merge(line2.StartSketchPoint);
                    var p = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(p, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefBSi));
                }
            }
        }

        private static void BuildReliefsFSi(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsFSi = new List<ReliefFSIEdgeFeature>();

            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefFSIEdgeFeature reliefFSi1 &&
                    reliefFSi1.ShouldBeBuilt)
                {
                    allReliefsFSi.Add(reliefFSi1);
                }

                if (section.SecondEdgeFeature is ReliefFSIEdgeFeature reliefFSi2 &&
                    reliefFSi2.ShouldBeBuilt)
                {
                    allReliefsFSi.Add(reliefFSi2);
                }
            }

            int reliefFSiNumber = 0;
            foreach (var reliefFSi in allReliefsFSi)
            {
                try
                {
                    reliefFSiNumber++;
                    bool firstEdge = reliefFSi.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefFSiSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefFSiSketch.AxisEntity = compDef.WorkAxes[1];

                    reliefFSiSketch.Visible = false;
                    var points = reliefFSiSketch.SketchPoints;
                    var lines = reliefFSiSketch.SketchLines;
                    var arcs = reliefFSiSketch.SketchArcs;
                    var edgePoint = TransientGeometry.CreatePoint2d(reliefFSi.EdgePoint.X.InMillimeters(),
                        reliefFSi.EdgePoint.Y.InMillimeters());
                    var pointFromEdgeToB = TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefFSi.EdgePoint.X + reliefFSi.Width).InMillimeters()
                            : (reliefFSi.EdgePoint.X - reliefFSi.Width).InMillimeters(),
                        reliefFSi.EdgePoint.Y.InMillimeters());
                    points.Add(pointFromEdgeToB);
                    var helperLine = lines.AddByTwoPoints(pointFromEdgeToB,
                        TransientGeometry.CreatePoint2d(firstEdge
                            ? pointFromEdgeToB.X + 1f.InMillimeters()
                            : pointFromEdgeToB.X - 1f.InMillimeters(), pointFromEdgeToB.Y));
                    var helperLine2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y - (reliefFSi.ReliefDepth - reliefFSi.MachiningAllowance).InMillimeters()),
                        TransientGeometry.CreatePoint2d(pointFromEdgeToB.X,
                            edgePoint.Y - (reliefFSi.ReliefDepth - reliefFSi.MachiningAllowance).InMillimeters()));
                    var helperPoint = pointFromEdgeToB.Copy();
                    var vector = helperLine.Geometry.Direction.AsVector();
                    vector.ScaleBy(-1f.InMillimeters());

                    helperPoint.TranslateBy(vector);

                    var p = points.Add(pointFromEdgeToB);
                    reliefFSiSketch.GeometricConstraints.AddGround((SketchEntity)p);
                    var angledLine = lines.AddByTwoPoints(p.Geometry, helperPoint);
                    angledLine.StartSketchPoint.Merge(p);

                    angledLine.OverrideColor = InvApp.TransientObjects.CreateColor(100, 0, 0);
                    reliefFSiSketch.GeometricConstraints.AddGround((SketchEntity)helperLine);
                    var angleDimConstraint = reliefFSiSketch.DimensionConstraints.AddTwoLineAngle(firstEdge ? helperLine : angledLine,
                        firstEdge ? angledLine : helperLine,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToB.X + 0.5f.InMillimeters()
                                : pointFromEdgeToB.X - 0.5f.InMillimeters(),
                            pointFromEdgeToB.Y + 0.05f.InMillimeters()));

                    angleDimConstraint.Parameter.Value = MathExtensions.DegreesToRadians(180 - reliefFSi.IncidenceAngle);
                    var vector2 = angledLine.Geometry.Direction.AsVector();
                    vector2.ScaleBy(10f.InMillimeters());
                    var pt = angledLine.EndSketchPoint.Geometry;
                    pt.TranslateBy(vector2);

                    angledLine.EndSketchPoint.MoveTo(pt);
                    var intersectPoint = angledLine.Geometry.IntersectWithCurve(helperLine2.Geometry)?[1] as Point2d;
                    angledLine.EndSketchPoint.MoveTo(intersectPoint);
                    helperLine2.EndSketchPoint.MoveTo(intersectPoint);

                    angledLine.EndSketchPoint.Merge(helperLine2.EndSketchPoint);

                    var pointFromEdgeToB1 =
                        points.Add(TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y + (reliefFSi.ReliefDepth - reliefFSi.MachiningAllowance).InMillimeters() +
                            reliefFSi.Width1.InMillimeters()));
                    var pointFromEdgeToDepthPlusA = points.Add(TransientGeometry.CreatePoint2d(edgePoint.X,
                        edgePoint.Y - (reliefFSi.ReliefDepth - reliefFSi.MachiningAllowance).InMillimeters()));
                    reliefFSiSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToB1);

                    var helperLine3 =
                        lines.AddByTwoPoints(pointFromEdgeToDepthPlusA.Geometry, pointFromEdgeToB1.Geometry);
                    helperLine3.OverrideColor = InvApp.TransientObjects.CreateColor(190, 20, 0);
                    reliefFSiSketch.GeometricConstraints.AddGround((SketchEntity)helperLine3);
                    var angledLine2 = lines.AddByTwoPoints(helperLine3.StartSketchPoint.Geometry,
                        helperLine3.EndSketchPoint.Geometry);
                    angledLine2.OverrideColor = InvApp.TransientObjects.CreateColor(237, 209, 28);
                    angledLine2.EndSketchPoint.Merge(pointFromEdgeToB1);
                    var angleDimConstraint2 = reliefFSiSketch.DimensionConstraints.AddTwoLineAngle(helperLine3,
                         angledLine2,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? edgePoint.X + 0.1f.InMillimeters()
                                : edgePoint.X - 0.1f.InMillimeters(),
                            pointFromEdgeToDepthPlusA.Geometry.Y +
                            0.5f.InMillimeters()));
                    angleDimConstraint2.Parameter.Value = MathExtensions.DegreesToRadians(firstEdge ? 180 - 8 : 8);
                    ((PartDocument)compDef.Document).Update();
                    var line = TransientGeometry.CreateLine2d(angledLine2.StartSketchPoint.Geometry,
                        angledLine2.Geometry.Direction);
                    var line2 = TransientGeometry.CreateLine2d(helperLine2.StartSketchPoint.Geometry,
                        helperLine2.Geometry.Direction);
                    line2.Direction.Y = -line2.Direction.Y;
                    var intersectPoint2 = line.IntersectWithCurve(line2)?[1] as Point2d;
                    var p2 = points.Add(intersectPoint2);
                    ((PartDocument)compDef.Document).Update();
                    var line4 = lines.AddByTwoPoints(pointFromEdgeToDepthPlusA.Geometry, p2.Geometry);
                    line4.OverrideColor = InvApp.TransientObjects.CreateColor(239, 92, 19);
                    reliefFSiSketch.GeometricConstraints.AddGround((SketchEntity)line4);
                    angledLine2.StartSketchPoint.MoveTo(intersectPoint2);

                    var fillet2d = arcs.AddByFillet((SketchEntity)angledLine2, (SketchEntity)line4,
                        reliefFSi.Radius.InMillimeters(), angledLine2.EndSketchPoint.Geometry,
                         line4.StartSketchPoint.Geometry);
                    helperLine2.StartSketchPoint.MoveTo(firstEdge ? fillet2d.EndSketchPoint.Geometry : fillet2d.StartSketchPoint.Geometry);
                    helperLine2.StartSketchPoint.Merge(firstEdge ? fillet2d.EndSketchPoint : fillet2d.StartSketchPoint);
                    helperLine3.Delete();
                    var lockLine = lines.AddByTwoPoints(angledLine.StartSketchPoint.Geometry,
                        angledLine2.StartSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(angledLine.StartSketchPoint);
                    lockLine.EndSketchPoint.Merge(angledLine2.StartSketchPoint);
                    var filletCenterPoint = fillet2d.Geometry.Center;
                    line4.Delete();
                    fillet2d.Delete();

                    var arc = arcs.AddByCenterStartEndPoint(filletCenterPoint,
                         helperLine2.StartSketchPoint.Geometry,
                         angledLine2.StartSketchPoint.Geometry);
                    arc.StartSketchPoint.Merge(firstEdge ? angledLine2.StartSketchPoint : helperLine2.StartSketchPoint);
                    arc.EndSketchPoint.Merge(firstEdge ? helperLine2.StartSketchPoint : angledLine2.StartSketchPoint);

                    var axisLine = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));

                    var profile = reliefFSiSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.RevolveFeatures.AddFull(profile, axisLine,
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"ReliefFSI_#{reliefFSiNumber}";
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefFSi));
                }
            }
        }

        private static void BuildReliefsGSi(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsGSi = new List<ReliefGSIEdgeFeature>();

            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefGSIEdgeFeature reliefGSi1 &&
                    reliefGSi1.ShouldBeBuilt)
                {
                    allReliefsGSi.Add(reliefGSi1);
                }

                if (section.SecondEdgeFeature is ReliefGSIEdgeFeature reliefGSi2 &&
                    reliefGSi2.ShouldBeBuilt)
                {
                    allReliefsGSi.Add(reliefGSi2);
                }
            }

            int reliefGSiNumber = 0;
            foreach (var reliefGSi in allReliefsGSi)
            {
                try
                {
                    reliefGSiNumber++;
                    bool firstEdge = reliefGSi.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefGSiSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefGSiSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefGSiSketch.Visible = false;

                    var points = reliefGSiSketch.SketchPoints;
                    var lines = reliefGSiSketch.SketchLines;
                    var arcs = reliefGSiSketch.SketchArcs;
                    var edgePoint = TransientGeometry.CreatePoint2d(reliefGSi.EdgePoint.X.InMillimeters(),
                        reliefGSi.EdgePoint.Y.InMillimeters());

                    var pointFromEdgeToB = TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefGSi.EdgePoint.X + reliefGSi.Width).InMillimeters()
                            : (reliefGSi.EdgePoint.X - reliefGSi.Width).InMillimeters(),
                        reliefGSi.EdgePoint.Y.InMillimeters());
                    points.Add(pointFromEdgeToB);
                    var helperLine = lines.AddByTwoPoints(pointFromEdgeToB,
                        TransientGeometry.CreatePoint2d(firstEdge
                            ? pointFromEdgeToB.X + 1f.InMillimeters()
                            : pointFromEdgeToB.X - 1f.InMillimeters(), pointFromEdgeToB.Y));
                    var helperLine2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y - (reliefGSi.ReliefDepth - reliefGSi.MachiningAllowance).InMillimeters()),
                        TransientGeometry.CreatePoint2d(pointFromEdgeToB.X,
                            edgePoint.Y - (reliefGSi.ReliefDepth - reliefGSi.MachiningAllowance).InMillimeters()));
                    var helperPoint = pointFromEdgeToB.Copy();

                    var vector = helperLine.Geometry.Direction.AsVector();
                    vector.ScaleBy(-1f.InMillimeters());
                    helperPoint.TranslateBy(vector);
                    var p = points.Add(pointFromEdgeToB);
                    reliefGSiSketch.GeometricConstraints.AddGround((SketchEntity)p);
                    var angledLine = lines.AddByTwoPoints(p.Geometry, helperPoint);
                    angledLine.StartSketchPoint.Merge(p);
                    angledLine.OverrideColor = InvApp.TransientObjects.CreateColor(100, 0, 0);

                    reliefGSiSketch.GeometricConstraints.AddGround((SketchEntity)helperLine);

                    var angleDimConstraint = reliefGSiSketch.DimensionConstraints.AddTwoLineAngle(firstEdge ? helperLine : angledLine,
                        firstEdge ? angledLine : helperLine,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToB.X + 0.5f.InMillimeters()
                                : pointFromEdgeToB.X - 0.5f.InMillimeters(),
                            pointFromEdgeToB.Y + 0.05f.InMillimeters()));
                    angleDimConstraint.Parameter.Value = MathExtensions.DegreesToRadians(180 - reliefGSi.IncidenceAngle);
                    var vector2 = angledLine.Geometry.Direction.AsVector();
                    vector2.ScaleBy(10f.InMillimeters());
                    var pt = angledLine.EndSketchPoint.Geometry;

                    pt.TranslateBy(vector2);
                    angledLine.EndSketchPoint.MoveTo(pt);
                    var intersectPoint = angledLine.Geometry.IntersectWithCurve(helperLine2.Geometry)?[1] as Point2d;
                    angledLine.EndSketchPoint.MoveTo(intersectPoint);
                    helperLine2.EndSketchPoint.MoveTo(intersectPoint);
                    angledLine.EndSketchPoint.Merge(helperLine2.EndSketchPoint);
                    var pointToDepthPlusA = TransientGeometry.CreatePoint2d(edgePoint.X,
                        edgePoint.Y - (reliefGSi.ReliefDepth - reliefGSi.MachiningAllowance).InMillimeters());
                    var rightLine = lines.AddByTwoPoints(pointToDepthPlusA,
                        TransientGeometry.CreatePoint2d(pointToDepthPlusA.X,
                            pointToDepthPlusA.Y + reliefGSi.Radius.InMillimeters()));

                    var fillet2D = arcs.AddByFillet((SketchEntity)rightLine, (SketchEntity)helperLine2,
                        reliefGSi.Radius.InMillimeters(), rightLine.EndSketchPoint.Geometry,
                        helperLine2.EndSketchPoint.Geometry);
                    var lockLine = lines.AddByTwoPoints(angledLine.StartSketchPoint.Geometry,
                        rightLine.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(angledLine.StartSketchPoint);
                    lockLine.EndSketchPoint.Merge(rightLine.EndSketchPoint);
                    var axisLine = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var profile1 = reliefGSiSketch.Profiles.AddForSolid();
                    var feature =
                        compDef.Features.RevolveFeatures.AddFull(profile1, axisLine,
                            PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"ReliefGSI_#{reliefGSiNumber}";

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    sketch2.AxisEntity = compDef.WorkAxes[1];
                    sketch2.Visible = false;
                    var sketch2Line1 =
                        sketch2.SketchLines.AddByTwoPoints(helperLine2.StartSketchPoint.Geometry, pointToDepthPlusA);
                    var sketch2Line2 = sketch2.SketchLines.AddByTwoPoints(lockLine.EndSketchPoint.Geometry,
                        sketch2Line1.EndSketchPoint.Geometry);
                    sketch2Line2.EndSketchPoint.Merge(sketch2Line1.EndSketchPoint);
                    var filletCenterPoint =
                        TransientGeometry.CreatePoint2d(fillet2D.CenterSketchPoint.Geometry.X,
                            fillet2D.CenterSketchPoint.Geometry.Y);

                    var sketch2Arc1 = sketch2.SketchArcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge
                            ? sketch2Line2.StartSketchPoint.Geometry
                            : sketch2Line1.StartSketchPoint.Geometry,
                        firstEdge
                            ? sketch2Line1.StartSketchPoint.Geometry
                            : sketch2Line2.StartSketchPoint.Geometry);
                    sketch2Line1.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.EndSketchPoint
                        : sketch2Arc1.StartSketchPoint);
                    sketch2Line2.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.StartSketchPoint
                        : sketch2Arc1.EndSketchPoint);
                    sketch2.Visible = false;
                    var axisLine2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var sketch2Profile = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(sketch2Profile, axisLine2,
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefGSi));
                }
            }
        }

        private static void BuildRelifsADin(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsADin = new List<ReliefADinEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefADinEdgeFeature reliefADin1 &&
                    reliefADin1.ShouldBeBuilt)
                {
                    allReliefsADin.Add(reliefADin1);
                }


                if (section.SecondEdgeFeature is ReliefADinEdgeFeature reliefADin2 &&
                    reliefADin2.ShouldBeBuilt)
                {
                    allReliefsADin.Add(reliefADin2);
                }
            }

            int reliefADinNumber = 0;

            foreach (var reliefADin in allReliefsADin)
            {
                try
                {
                    reliefADinNumber++;
                    bool firstEdge = reliefADin.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefADinSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefADinSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefADinSketch.Visible = false;
                    var points = reliefADinSketch.SketchPoints;
                    var lines = reliefADinSketch.SketchLines;
                    var arcs = reliefADinSketch.SketchArcs;

                    var pointFromEdgeToB = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefADin.EdgePoint.X + reliefADin.Width).InMillimeters()
                            : (reliefADin.EdgePoint.X - reliefADin.Width).InMillimeters(),
                        reliefADin.EdgePoint.Y.InMillimeters()));
                    var pointFromEdgeToDepth = points.Add(TransientGeometry.CreatePoint2d(
                        reliefADin.EdgePoint.X.InMillimeters(),
                        (reliefADin.EdgePoint.Y - reliefADin.ReliefDepth).InMillimeters()));
                    reliefADinSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToB);
                    reliefADinSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToDepth);
                    var line1 = lines.AddByTwoPoints(pointFromEdgeToDepth.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepth.Geometry.X,
                            pointFromEdgeToDepth.Geometry.Y + reliefADin.Radius.InMillimeters()));
                    var line2 = lines.AddByTwoPoints(pointFromEdgeToDepth.Geometry,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? (pointFromEdgeToDepth.Geometry.X + reliefADin.Radius.InMillimeters())
                                : (pointFromEdgeToDepth.Geometry.X - reliefADin.Radius.InMillimeters()),
                            pointFromEdgeToDepth.Geometry.Y));
                    var fillet2D = arcs.AddByFillet((SketchEntity)line2, (SketchEntity)line1,
                        reliefADin.Radius.InMillimeters(),
                        line2.EndSketchPoint.Geometry, line1.EndSketchPoint.Geometry);
                    var line6 = lines.AddByTwoPoints(pointFromEdgeToB.Geometry,
                        firstEdge ? fillet2D.StartSketchPoint.Geometry : fillet2D.EndSketchPoint.Geometry);
                    line6.StartSketchPoint.Merge(pointFromEdgeToB);
                    line6.EndSketchPoint.Merge(firstEdge ? fillet2D.StartSketchPoint : fillet2D.EndSketchPoint);

                    var line3 = lines.AddByTwoPoints(line2.StartSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? (reliefADin.EdgePoint.X + reliefADin.Width2).InMillimeters()
                                : (reliefADin.EdgePoint.X - reliefADin.Width2).InMillimeters(),
                            pointFromEdgeToDepth.Geometry.Y));

                    reliefADinSketch.GeometricConstraints.AddGround((SketchEntity)line3);
                    var circle = reliefADinSketch.SketchCircles.AddByCenterRadius(
                        TransientGeometry.CreatePoint2d(line3.EndSketchPoint.Geometry.X,
                            line3.EndSketchPoint.Geometry.Y +
                            reliefADin.Radius.InMillimeters()),
                        reliefADin.Radius.InMillimeters());
                    reliefADinSketch.GeometricConstraints.AddTangent((SketchEntity)line3, (SketchEntity)circle);
                    reliefADinSketch.GeometricConstraints.AddGround((SketchEntity)circle);

                    var line4 = lines.AddByTwoPoints(pointFromEdgeToB.Geometry, line3.EndSketchPoint.Geometry);
                    var helperPoint = line4.StartSketchPoint.Geometry;
                    var vector = line4.Geometry.Direction.AsVector();
                    vector.ScaleBy(-1f.InMillimeters());
                    helperPoint.TranslateBy(vector);
                    line4.Delete();
                    line4 = lines.AddByTwoPoints(helperPoint, line3.EndSketchPoint.Geometry);
                    var helperLine = lines.AddByTwoPoints(pointFromEdgeToB.Geometry,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToB.Geometry.X + 1f.InMillimeters()
                                : pointFromEdgeToB.Geometry.X - 1f.InMillimeters(),
                            pointFromEdgeToB.Geometry.Y));
                    helperLine.OverrideColor = InvApp.TransientObjects.CreateColor(255, 0, 0);
                    reliefADinSketch.GeometricConstraints.AddGround((SketchEntity)helperLine);
                    var dim = reliefADinSketch.DimensionConstraints.AddTwoLineAngle(line4, helperLine,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToB.Geometry.X + 0.5f.InMillimeters()
                                : pointFromEdgeToB.Geometry.X - 0.5f.InMillimeters(),
                            pointFromEdgeToB.Geometry.Y + 0.5f.InMillimeters()));
                    dim.Parameter.Value = MathExtensions.DegreesToRadians(180 - reliefADin.Angle);

                    reliefADinSketch.GeometricConstraints.AddTangent((SketchEntity)line4, (SketchEntity)circle);
                    var tangentPoint2D = line4.Geometry.IntersectWithCurve(circle.Geometry)[1] as Point2d;
                    line4.EndSketchPoint.MoveTo(tangentPoint2D);

                    var circleCenterPoint = TransientGeometry.CreatePoint2d(circle.CenterSketchPoint.Geometry.X,
                        circle.CenterSketchPoint.Geometry.Y);
                    var intersectPoint = line6.Geometry.IntersectWithCurve(line4.Geometry)?[1] as Point2d;
                    if (intersectPoint == null)
                    {
                        intersectPoint = line4.Geometry.IntersectWithCurve(helperLine.Geometry)[1] as Point2d;
                    }
                    var sketchIntersectionPoint = points.Add(intersectPoint);
                    reliefADinSketch.GeometricConstraints.AddGround((SketchEntity)sketchIntersectionPoint);

                    var tangentPoint = line4.Geometry.IntersectWithCurve(circle.Geometry)[1] as Point2d;
                    line4.Delete();

                    line4 = lines.AddByTwoPoints(sketchIntersectionPoint.Geometry, tangentPoint);
                    line6.StartSketchPoint.Merge(line4.StartSketchPoint);
                    circle.Delete();

                    var arc = arcs.AddByCenterStartEndPoint(circleCenterPoint,
                         firstEdge ? line3.EndSketchPoint.Geometry : line4.EndSketchPoint.Geometry,
                         firstEdge ? line4.EndSketchPoint.Geometry : line3.EndSketchPoint.Geometry);
                    line4.EndSketchPoint.Merge(firstEdge ? arc.EndSketchPoint : arc.StartSketchPoint);
                    line3.EndSketchPoint.Merge(firstEdge ? arc.StartSketchPoint : arc.EndSketchPoint);
                    var filletCenterPoint = TransientGeometry.CreatePoint2d(fillet2D.CenterSketchPoint.Geometry.X,
                        fillet2D.CenterSketchPoint.Geometry.Y);
                    fillet2D.Delete();

                    var arc2 = arcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge ? line6.EndSketchPoint.Geometry : line3.StartSketchPoint.Geometry,
                        firstEdge ? line3.StartSketchPoint.Geometry : line6.EndSketchPoint.Geometry);
                    line6.EndSketchPoint.Merge(firstEdge ? arc2.StartSketchPoint : arc2.EndSketchPoint);
                    line3.StartSketchPoint.Merge(firstEdge ? arc2.EndSketchPoint : arc2.StartSketchPoint);
                    var profile = reliefADinSketch.Profiles.AddForSolid();
                    var reliefADinFeature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    reliefADinFeature.Name = $"ReliefADin_#{reliefADinNumber}";

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    var sketch2Line1 =
                        sketch2.SketchLines.AddByTwoPoints(line3.StartSketchPoint.Geometry,
                            pointFromEdgeToDepth.Geometry);
                    var sketch2Line2 = sketch2.SketchLines.AddByTwoPoints(line6.EndSketchPoint.Geometry,
                        sketch2Line1.EndSketchPoint.Geometry);
                    sketch2Line2.EndSketchPoint.Merge(sketch2Line1.EndSketchPoint);
                    var sketch2Arc1 = sketch2.SketchArcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge
                            ? sketch2Line2.StartSketchPoint.Geometry
                            : sketch2Line1.StartSketchPoint.Geometry,
                        firstEdge
                            ? sketch2Line1.StartSketchPoint.Geometry
                            : sketch2Line2.StartSketchPoint.Geometry);

                    sketch2Line1.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.EndSketchPoint
                        : sketch2Arc1.StartSketchPoint);
                    sketch2Line2.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.StartSketchPoint
                        : sketch2Arc1.EndSketchPoint);
                    partDocument.Update();
                    reliefADinSketch.GeometricConstraints.AddTangent((SketchEntity)sketch2Arc1, (SketchEntity)sketch2Line1);
                    reliefADinSketch.GeometricConstraints.AddTangent((SketchEntity)sketch2Arc1, (SketchEntity)sketch2Line2);
                    var sketch2Profile = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(sketch2Profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kJoinOperation);
                    sketch2.Visible = false;
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefADin));
                }
            }
        }

        private static void BuildRelifsBDin(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;

            var allReliefsBDin = new List<ReliefBDinEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefBDinEdgeFeature reliefBDin1 &&
                    reliefBDin1.ShouldBeBuilt)
                {
                    allReliefsBDin.Add(reliefBDin1);
                }

                if (section.SecondEdgeFeature is ReliefBDinEdgeFeature reliefBDin2 &&
                    reliefBDin2.ShouldBeBuilt)
                {
                    allReliefsBDin.Add(reliefBDin2);
                }
            }

            int reliefBDinNumber = 0;
            foreach (var reliefBDin in allReliefsBDin)
            {
                try
                {
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefBDin));
                }

                reliefBDinNumber++;
                bool firstEdge = reliefBDin.EdgePosition == EdgeFeaturePosition.FirstEdge;
                var reliefBDinSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                reliefBDinSketch.AxisEntity = compDef.WorkAxes[1];
                reliefBDinSketch.Visible = false;

                var points = reliefBDinSketch.SketchPoints;
                var lines = reliefBDinSketch.SketchLines;
                var arcs = reliefBDinSketch.SketchArcs;
                var pointDepthPlusA = points.Add(TransientGeometry.CreatePoint2d(reliefBDin.EdgePoint.X.InMillimeters(),
                    (reliefBDin.EdgePoint.Y - reliefBDin.ReliefDepth).InMillimeters()));
                var pointFromDepthPlusAToB1 = points.Add(TransientGeometry.CreatePoint2d(pointDepthPlusA.Geometry.X,
                    pointDepthPlusA.Geometry.Y + reliefBDin.Width1.InMillimeters()));

                var pointFromEdgeToB = points.Add(TransientGeometry.CreatePoint2d(firstEdge
                        ? (reliefBDin.EdgePoint.X + reliefBDin.Width).InMillimeters()
                        : (reliefBDin.EdgePoint.X - reliefBDin.Width).InMillimeters(),
                    reliefBDin.EdgePoint.Y.InMillimeters()));
                var lockLine = lines.AddByTwoPoints(pointFromEdgeToB.Geometry, pointFromDepthPlusAToB1.Geometry);
                var helperLine = lines.AddByTwoPoints(pointFromDepthPlusAToB1.Geometry,
                    TransientGeometry.CreatePoint2d(pointFromDepthPlusAToB1.Geometry.X,
                        pointFromDepthPlusAToB1.Geometry.Y + 1f.InMillimeters()));
                var angledLine = lines.AddByTwoPoints(pointFromDepthPlusAToB1.Geometry,
                    TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointFromDepthPlusAToB1.Geometry.X + 0.5f.InMillimeters()
                            : pointFromDepthPlusAToB1.Geometry.X - 0.5f.InMillimeters(),
                        helperLine.EndSketchPoint.Geometry.Y));
                reliefBDinSketch.GeometricConstraints.AddGround((SketchEntity)helperLine);
                var angleDimConstraint = reliefBDinSketch.DimensionConstraints.AddTwoLineAngle(angledLine, helperLine,
                    TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointFromDepthPlusAToB1.Geometry.X +
                              0.25f.InMillimeters()
                            : pointFromDepthPlusAToB1.Geometry.X -
                              0.25f.InMillimeters(),
                        pointFromDepthPlusAToB1.Geometry.Y +
                        0.5f.InMillimeters()));
                angleDimConstraint.Parameter.Value = MathExtensions.DegreesToRadians(180 - 20);
                var horizontalLine1 = lines.AddByTwoPoints(pointDepthPlusA.Geometry,
                    TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointDepthPlusA.Geometry.X - (reliefBDin.Radius * 3).InMillimeters()
                            : pointDepthPlusA.Geometry.X + (reliefBDin.Radius * 3).InMillimeters(),
                        pointDepthPlusA.Geometry.Y));
                reliefBDinSketch.GeometricConstraints.AddGround((SketchEntity)horizontalLine1);

                var helperPoint = angledLine.StartSketchPoint.Geometry;
                var vector = angledLine.Geometry.Direction.AsVector();
                vector.ScaleBy((-reliefBDin.Width1 * 2).InMillimeters());
                helperPoint.TranslateBy(vector);
                angledLine.Delete();
                angledLine = lines.AddByTwoPoints(pointFromDepthPlusAToB1.Geometry, helperPoint);
                var intersectPoint = angledLine.Geometry.IntersectWithCurve(horizontalLine1.Geometry)[1] as Point2d;
                points.Add(intersectPoint);
                angledLine.EndSketchPoint.MoveTo(intersectPoint);
                horizontalLine1.EndSketchPoint.MoveTo(intersectPoint);
                angledLine.EndSketchPoint.Merge(horizontalLine1.EndSketchPoint);
                reliefBDinSketch.GeometricConstraints.AddGround((SketchEntity)angledLine);
                var fillet2D = arcs.AddByFillet((SketchEntity)horizontalLine1, (SketchEntity)angledLine,
                    reliefBDin.Radius.InMillimeters(),
                    horizontalLine1.StartSketchPoint.Geometry, angledLine.StartSketchPoint.Geometry);
                var lineB2 = lines.AddByTwoPoints(
                    firstEdge ? fillet2D.EndSketchPoint.Geometry : fillet2D.StartSketchPoint.Geometry,
                    firstEdge
                        ? TransientGeometry.CreatePoint2d(
                            fillet2D.EndSketchPoint.Geometry.X + reliefBDin.Width2.InMillimeters(),
                            fillet2D.EndSketchPoint.Geometry.Y)
                        : TransientGeometry.CreatePoint2d(
                            fillet2D.StartSketchPoint.Geometry.X - reliefBDin.Width2.InMillimeters(),
                            fillet2D.StartSketchPoint.Geometry.Y));
                var circle = reliefBDinSketch.SketchCircles.AddByCenterRadius(
                    TransientGeometry.CreatePoint2d(lineB2.EndSketchPoint.Geometry.X,
                        lineB2.EndSketchPoint.Geometry.Y +
                        reliefBDin.Radius.InMillimeters()),
                    reliefBDin.Radius.InMillimeters());
                reliefBDinSketch.GeometricConstraints.AddTangent((SketchEntity)lineB2, (SketchEntity)circle);
                reliefBDinSketch.GeometricConstraints.AddGround((SketchEntity)circle);
                var optionalAngleLine = lines.AddByTwoPoints(pointFromEdgeToB.Geometry, lineB2.EndSketchPoint.Geometry);
                helperPoint = optionalAngleLine.StartSketchPoint.Geometry;
                vector = optionalAngleLine.Geometry.Direction.AsVector();
                vector.ScaleBy(-50f.InMillimeters());
                helperPoint.TranslateBy(vector);

                optionalAngleLine.StartSketchPoint.MoveTo(helperPoint);
                var helperLine2 = lines.AddByTwoPoints(pointFromEdgeToB.Geometry,
                    TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointFromEdgeToB.Geometry.X + reliefBDin.LinkedSection.Length.InMillimeters()
                            : pointFromEdgeToB.Geometry.X - reliefBDin.LinkedSection.Length.InMillimeters(),
                        pointFromEdgeToB.Geometry.Y));

                reliefBDinSketch.GeometricConstraints.AddGround((SketchEntity)helperLine2);
                var angleDimConstraint2 = reliefBDinSketch.DimensionConstraints.AddTwoLineAngle(optionalAngleLine,
                    helperLine2,
                    TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointFromEdgeToB.Geometry.X +
                              0.5f.InMillimeters()
                            : pointFromEdgeToB.Geometry.X -
                              0.5f.InMillimeters(),
                        pointFromEdgeToB.Geometry.Y + 0.5f.InMillimeters()));
                angleDimConstraint2.Parameter.Value = MathExtensions.DegreesToRadians(180 - reliefBDin.Angle);
                reliefBDinSketch.GeometricConstraints.AddTangent((SketchEntity)optionalAngleLine,
                    (SketchEntity)circle);
                var tangentPoint2D = optionalAngleLine.Geometry.IntersectWithCurve(circle.Geometry)[1] as Point2d;
                optionalAngleLine.EndSketchPoint.MoveTo(tangentPoint2D);

                var circleCenterPoint = TransientGeometry.CreatePoint2d(circle.CenterSketchPoint.Geometry.X,
                    circle.CenterSketchPoint.Geometry.Y);


                var intersectPoint2 =
                    optionalAngleLine.Geometry.IntersectWithCurve(helperLine2.Geometry)?[1] as Point2d;
                if (intersectPoint2 == null)

                {
                    intersectPoint2 = helperLine2.Geometry.IntersectWithCurve(circle.Geometry)[1] as Point2d;
                    optionalAngleLine.Delete();
                    lockLine.StartSketchPoint.MoveTo(intersectPoint2);
                    circle.Delete();
                    var arc = arcs.AddByCenterStartEndPoint(circleCenterPoint,
                        firstEdge ? lineB2.EndSketchPoint.Geometry : intersectPoint2,
                        firstEdge ? intersectPoint2 : lineB2.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(firstEdge ? arc.EndSketchPoint : arc.StartSketchPoint);
                    lineB2.EndSketchPoint.Merge(firstEdge ? arc.StartSketchPoint : arc.EndSketchPoint);
                }
                else
                {
                    optionalAngleLine.StartSketchPoint.MoveTo(intersectPoint2);
                    lockLine.StartSketchPoint.MoveTo(intersectPoint2);

                    optionalAngleLine.StartSketchPoint.Merge(lockLine.StartSketchPoint);
                    circle.Delete();
                    var arc = arcs.AddByCenterStartEndPoint(circleCenterPoint,
                        firstEdge ? lineB2.EndSketchPoint.Geometry : optionalAngleLine.EndSketchPoint.Geometry,
                        firstEdge ? optionalAngleLine.EndSketchPoint.Geometry : lineB2.EndSketchPoint.Geometry);
                    optionalAngleLine.EndSketchPoint.Merge(firstEdge ? arc.EndSketchPoint : arc.StartSketchPoint);
                    lineB2.EndSketchPoint.Merge(firstEdge ? arc.StartSketchPoint : arc.EndSketchPoint);
                }

                lockLine.EndSketchPoint.Merge(angledLine.StartSketchPoint);
                lineB2.StartSketchPoint.Merge(firstEdge ? fillet2D.EndSketchPoint : fillet2D.StartSketchPoint);
                var profile = reliefBDinSketch.Profiles.AddForSolid();

                var reliefADinFeature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                    PartFeatureOperationEnum.kCutOperation);
                reliefADinFeature.Name = $"ReliefBDin_#{reliefBDinNumber}";
            }
        }

        private static void BuildRelifsCDin(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsCDin = new List<ReliefCDinEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefCDinEdgeFeature reliefCDin1 &&
                    reliefCDin1.ShouldBeBuilt)
                {
                    allReliefsCDin.Add(reliefCDin1);
                }

                if (section.SecondEdgeFeature is ReliefCDinEdgeFeature reliefCDin2 &&
                    reliefCDin2.ShouldBeBuilt)
                {
                    allReliefsCDin.Add(reliefCDin2);
                }
            }

            int reliefCDinNumber = 0;
            foreach (var reliefFDin in allReliefsCDin)
            {
                try
                {
                    reliefCDinNumber++;
                    bool firstEdge = reliefFDin.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefCDinSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefCDinSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefCDinSketch.Visible = false;
                    var pointReliefDepthPlusA = TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefFDin.EdgePoint.X + reliefFDin.Radius).InMillimeters()
                            : (reliefFDin.EdgePoint.X - reliefFDin.Radius).InMillimeters(),
                        (reliefFDin.EdgePoint.Y - reliefFDin.ReliefDepth - reliefFDin.MachiningAllowance)
                        .InMillimeters());
                    var circleCenterPoint = TransientGeometry.CreatePoint2d(pointReliefDepthPlusA.X,
                        pointReliefDepthPlusA.Y + reliefFDin.Radius.InMillimeters());
                    var circle =
                        reliefCDinSketch.SketchCircles.AddByCenterRadius(circleCenterPoint,
                            reliefFDin.Radius.InMillimeters());
                    var profile = reliefCDinSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"ReliefCDin_#{reliefCDinNumber}";

                    var circleRightTangentPoint =
                        TransientGeometry.CreatePoint2d(
                            firstEdge ? circleCenterPoint.X - circle.Radius : circleCenterPoint.X + circle.Radius,
                            circleCenterPoint.Y);
                    var helperLine = TransientGeometry.CreateLine(
                        TransientGeometry.CreatePoint(reliefFDin.EdgePoint.X.InMillimeters(),
                            reliefFDin.EdgePoint.Y.InMillimeters()),
                        TransientGeometry.CreateVector(firstEdge
                            ? reliefFDin.EdgePoint.X + reliefFDin.Radius.InMillimeters()
                            : reliefFDin.EdgePoint.X - reliefFDin.Radius.InMillimeters()));
                    var circleIntersect =
                        TransientGeometry.CreateCircle(
                            TransientGeometry.CreatePoint(circleCenterPoint.X, circleCenterPoint.Y),
                            circle.Geometry3d.Normal, reliefFDin.Radius.InMillimeters());
                    var intesectPoint =
                        TransientGeometry.CurveCurveIntersection(circleIntersect, helperLine)[firstEdge ? 2 : 1] as Point;
                    var intersectPoint2D = TransientGeometry.CreatePoint2d(intesectPoint.X, intesectPoint.Y);

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    var line1 = sketch2.SketchLines.AddByTwoPoints(intersectPoint2D,
                        TransientGeometry.CreatePoint2d(reliefFDin.EdgePoint.X.InMillimeters(),
                            reliefFDin.EdgePoint.Y.InMillimeters()));

                    var line2 = sketch2.SketchLines.AddByTwoPoints(line1.EndSketchPoint.Geometry,
                        circleRightTangentPoint);

                    var arc = sketch2.SketchArcs.AddByCenterStartEndPoint(circleCenterPoint, firstEdge ? circleRightTangentPoint : intersectPoint2D,
                        firstEdge ? intersectPoint2D : circleRightTangentPoint);
                    line1.StartSketchPoint.Merge(arc.StartSketchPoint);

                    line2.EndSketchPoint.Merge(arc.EndSketchPoint);
                    line1.EndSketchPoint.Merge(line2.StartSketchPoint);
                    var p =
                        sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(p, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefFDin));
                }
            }
        }

        private static void BuildReliefsDDin(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsDdin = new List<ReliefDDinEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefDDinEdgeFeature reliefDDin1 &&
                    reliefDDin1.ShouldBeBuilt)
                {
                    allReliefsDdin.Add(reliefDDin1);
                }

                if (section.SecondEdgeFeature is ReliefDDinEdgeFeature reliefDDin2 &&
                    reliefDDin2.ShouldBeBuilt)
                {
                    allReliefsDdin.Add(reliefDDin2);
                }
            }

            int reliefDDinNumber = 0;
            foreach (var reliefDDin in allReliefsDdin)
            {
                try
                {
                    reliefDDinNumber++;
                    bool firstEdge = reliefDDin.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefDDinSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefDDinSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefDDinSketch.Visible = false;
                    var points = reliefDDinSketch.SketchPoints;

                    var lines = reliefDDinSketch.SketchLines;
                    var arcs = reliefDDinSketch.SketchArcs;

                    var pointFromEdgeToB1 = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefDDin.EdgePoint.X + reliefDDin.Width1).InMillimeters()
                            : (reliefDDin.EdgePoint.X - reliefDDin.Width1).InMillimeters(),
                        reliefDDin.EdgePoint.Y.InMillimeters()));

                    var pointFromEdgeToB = points.Add(TransientGeometry.CreatePoint2d(
                        reliefDDin.EdgePoint.X.InMillimeters(),
                        (reliefDDin.EdgePoint.Y + reliefDDin.Width1).InMillimeters()));

                    var pointFromEdgeToDepthPlusA = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefDDin.EdgePoint.X + reliefDDin.Width1 / 2).InMillimeters()
                            : (reliefDDin.EdgePoint.X - reliefDDin.Width1 / 2).InMillimeters(),
                        (reliefDDin.EdgePoint.Y - reliefDDin.ReliefDepth - reliefDDin.MachiningAllowance)
                        .InMillimeters()));
                    var pointFromEdgeToDepthPlusRigth = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefDDin.EdgePoint.X - reliefDDin.ReliefDepth - reliefDDin.MachiningAllowance)
                            .InMillimeters()
                            : (reliefDDin.EdgePoint.X + reliefDDin.ReliefDepth + reliefDDin.MachiningAllowance)
                            .InMillimeters(), (reliefDDin.EdgePoint.Y + reliefDDin.Width / 2).InMillimeters()));
                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToB1);
                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToB);

                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToDepthPlusA);
                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToDepthPlusRigth);
                    var line1 = lines.AddByTwoPoints(pointFromEdgeToB1.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToB1.Geometry.X,
                            pointFromEdgeToDepthPlusA.Geometry.Y));
                    var line2 = lines.AddByTwoPoints(line1.EndSketchPoint.Geometry, pointFromEdgeToDepthPlusA.Geometry);
                    var line3 = lines.AddByTwoPoints(pointFromEdgeToDepthPlusA.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepthPlusRigth.Geometry.X,
                            pointFromEdgeToDepthPlusA.Geometry.Y));


                    var line4 = lines.AddByTwoPoints(line3.EndSketchPoint.Geometry,
                        pointFromEdgeToDepthPlusRigth.Geometry);
                    var line5 = lines.AddByTwoPoints(line4.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepthPlusRigth.Geometry.X,
                            pointFromEdgeToB.Geometry.Y));
                    var line6 = lines.AddByTwoPoints(line5.EndSketchPoint.Geometry, pointFromEdgeToB.Geometry);

                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)line1);
                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)line2);
                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)line3);
                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)line4);
                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)line5);
                    reliefDDinSketch.GeometricConstraints.AddGround((SketchEntity)line6);

                    var fillet1 = arcs.AddByFillet((SketchEntity)line1, (SketchEntity)line2,
                        reliefDDin.Radius.InMillimeters(), line1.StartSketchPoint.Geometry,
                        line2.EndSketchPoint.Geometry);

                    var fillet2 = arcs.AddByFillet((SketchEntity)line3, (SketchEntity)line4,
                        reliefDDin.Radius.InMillimeters(), line3.StartSketchPoint.Geometry,
                        line4.EndSketchPoint.Geometry);

                    var fillet3 = arcs.AddByFillet((SketchEntity)line5, (SketchEntity)line6,
                        reliefDDin.Radius.InMillimeters(), line5.StartSketchPoint.Geometry,
                        line6.EndSketchPoint.Geometry);

                    points.Add(firstEdge ? fillet1.StartSketchPoint.Geometry : fillet1.EndSketchPoint.Geometry);
                    var fillet1EndFillet2StartIntersectPoint =
                         TransientGeometry.CurveCurveIntersection(
                             TransientGeometry.CreateArc3d(fillet1.Geometry3d.Center, fillet1.Geometry3d.Normal,
                                 fillet1.Geometry3d.ReferenceVector, fillet1.Radius, fillet1.StartAngle,
                                 fillet1.SweepAngle),
                             TransientGeometry.CreateArc3d(fillet2.Geometry3d.Center, fillet2.Geometry3d.Normal,
                                 fillet2.Geometry3d.ReferenceVector, fillet2.Radius, fillet2.StartAngle,
                                 fillet2.SweepAngle))[1] as Point;

                    var fillet2EndFillet3StartIntersectPoint = TransientGeometry.CurveCurveIntersection(
                        TransientGeometry.CreateArc3d(fillet2.Geometry3d.Center, fillet2.Geometry3d.Normal,
                            fillet2.Geometry3d.ReferenceVector, fillet2.Radius, fillet2.StartAngle, fillet2.SweepAngle),
                        TransientGeometry.CreateArc3d(fillet3.Geometry3d.Center, fillet3.Geometry3d.Normal,
                            fillet3.Geometry3d.ReferenceVector, fillet3.Radius, fillet3.StartAngle,
                            fillet3.SweepAngle))[1] as Point;

                    var point1 = TransientGeometry.CreatePoint2d(fillet1EndFillet2StartIntersectPoint.X,
                        fillet1EndFillet2StartIntersectPoint.Y);
                    var point2 = TransientGeometry.CreatePoint2d(fillet2EndFillet3StartIntersectPoint.X,
                        fillet2EndFillet3StartIntersectPoint.Y);

                    if (firstEdge)
                    {
                        fillet1.StartSketchPoint.MoveTo(point1);
                        fillet2.EndSketchPoint.MoveTo(point1);
                        fillet1.StartSketchPoint.Merge(fillet2
                            .EndSketchPoint);
                        fillet2.StartSketchPoint.MoveTo(point2);
                        fillet3.EndSketchPoint.MoveTo(point2);
                        fillet2.StartSketchPoint.Merge(fillet3.EndSketchPoint);
                    }
                    else
                    {
                        fillet1.EndSketchPoint.MoveTo(point1);
                        fillet2.StartSketchPoint.MoveTo(point1);
                        fillet1.EndSketchPoint.Merge(fillet2
                            .StartSketchPoint);
                        fillet2.EndSketchPoint.MoveTo(point2);
                        fillet3.StartSketchPoint.MoveTo(point2);
                        fillet2.EndSketchPoint.Merge(fillet3.StartSketchPoint);
                    }



                    var lockLine = lines.AddByTwoPoints(firstEdge ? fillet1.EndSketchPoint.Geometry : fillet1.StartSketchPoint.Geometry,
                        firstEdge ? fillet3.StartSketchPoint.Geometry : fillet3.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(firstEdge ? fillet1.EndSketchPoint : fillet1.StartSketchPoint);
                    lockLine.EndSketchPoint.Merge(firstEdge ? fillet3.StartSketchPoint : fillet3.EndSketchPoint);
                    var profile = reliefDDinSketch.Profiles.AddForSolid();
                    var reliefADinFeature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    reliefADinFeature.Name = $"ReliefDDIn_#{reliefDDinNumber}";

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    sketch2.AxisEntity = compDef.WorkAxes[1];
                    sketch2.Visible = false;
                    var helperLine3 = sketch2.SketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(
                            reliefDDin.EdgePoint.X.InMillimeters(),
                            reliefDDin.EdgePoint.Y.InMillimeters()),
                        pointFromEdgeToB1.Geometry);

                    var intersectPoint = helperLine3.Geometry.IntersectWithCurve(fillet2.Geometry)[1] as Point2d;

                    var sketch2Line1 =
                        sketch2.SketchLines.AddByTwoPoints(intersectPoint,
                            TransientGeometry.CreatePoint2d(reliefDDin.EdgePoint.X.InMillimeters(),
                                reliefDDin.EdgePoint.Y.InMillimeters()));
                    helperLine3.Delete();
                    var sketch2Line2 = sketch2.SketchLines.AddByTwoPoints(firstEdge ? fillet2.StartSketchPoint.Geometry : fillet2.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(firstEdge ? fillet2.StartSketchPoint.Geometry.X : fillet2.EndSketchPoint.Geometry.X,
                            reliefDDin.EdgePoint.Y.InMillimeters()));
                    sketch2Line1.EndSketchPoint.MoveTo(sketch2Line2.EndSketchPoint.Geometry);
                    sketch2Line2.EndSketchPoint.Merge(sketch2Line1.EndSketchPoint);
                    var filletCenterPoint = TransientGeometry.CreatePoint2d(fillet2.CenterSketchPoint.Geometry.X,
                        fillet2.CenterSketchPoint.Geometry.Y);

                    var sketch2Arc1 = sketch2.SketchArcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge
                            ? sketch2Line2.StartSketchPoint.Geometry
                            : sketch2Line1.StartSketchPoint.Geometry,
                        firstEdge
                            ? sketch2Line1.StartSketchPoint.Geometry
                            : sketch2Line2.StartSketchPoint.Geometry);

                    sketch2Line1.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.EndSketchPoint
                        : sketch2Arc1.StartSketchPoint);
                    sketch2Line2.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.StartSketchPoint
                        : sketch2Arc1.EndSketchPoint);
                    sketch2.Visible = false;
                    var axisLine2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var sketch2Profile = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(sketch2Profile, axisLine2,
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefDDin));
                }
            }
        }

        private static void BuildReliefsEDin(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsEDin = new List<ReliefEDinEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefEDinEdgeFeature reliefEDin1 &&
                    reliefEDin1.ShouldBeBuilt)
                {
                    allReliefsEDin.Add(reliefEDin1);
                }

                if (section.SecondEdgeFeature is ReliefEDinEdgeFeature reliefEDin2 &&
                    reliefEDin2.ShouldBeBuilt)
                {
                    allReliefsEDin.Add(reliefEDin2);
                }
            }

            int reliefEDinNumber = 0;
            foreach (var reliefEDin in allReliefsEDin)
            {
                try
                {
                    reliefEDinNumber++;
                    bool firstEdge = reliefEDin.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefEDinSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefEDinSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefEDinSketch.Visible = false;

                    var points = reliefEDinSketch.SketchPoints;
                    var lines = reliefEDinSketch.SketchLines;
                    var arcs = reliefEDinSketch.SketchArcs;
                    var edgePoint = TransientGeometry.CreatePoint2d(reliefEDin.EdgePoint.X.InMillimeters(),
                        reliefEDin.EdgePoint.Y.InMillimeters());

                    var pointFromEdgeToB = TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefEDin.EdgePoint.X + reliefEDin.Width).InMillimeters()
                            : (reliefEDin.EdgePoint.X - reliefEDin.Width).InMillimeters(),
                        reliefEDin.EdgePoint.Y.InMillimeters());
                    points.Add(pointFromEdgeToB);
                    var helperLine = lines.AddByTwoPoints(pointFromEdgeToB,
                        TransientGeometry.CreatePoint2d(firstEdge
                            ? pointFromEdgeToB.X + 1f.InMillimeters()
                            : pointFromEdgeToB.X - 1f.InMillimeters(), pointFromEdgeToB.Y));
                    var helperLine2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y - (reliefEDin.ReliefDepth - reliefEDin.MachiningAllowance).InMillimeters()),
                        TransientGeometry.CreatePoint2d(pointFromEdgeToB.X,
                            edgePoint.Y - (reliefEDin.ReliefDepth - reliefEDin.MachiningAllowance).InMillimeters()));
                    var helperPoint = pointFromEdgeToB.Copy();

                    var vector = helperLine.Geometry.Direction.AsVector();
                    vector.ScaleBy(-1f.InMillimeters());
                    helperPoint.TranslateBy(vector);
                    var p = points.Add(pointFromEdgeToB);
                    reliefEDinSketch.GeometricConstraints.AddGround((SketchEntity)p);
                    var angledLine = lines.AddByTwoPoints(p.Geometry, helperPoint);
                    angledLine.StartSketchPoint.Merge(p);
                    angledLine.OverrideColor = InvApp.TransientObjects.CreateColor(100, 0, 0);

                    reliefEDinSketch.GeometricConstraints.AddGround((SketchEntity)helperLine);

                    var angleDimConstraint = reliefEDinSketch.DimensionConstraints.AddTwoLineAngle(firstEdge ? helperLine : angledLine,
                        firstEdge ? angledLine : helperLine,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToB.X + 0.5f.InMillimeters()
                                : pointFromEdgeToB.X - 0.5f.InMillimeters(),
                            pointFromEdgeToB.Y + 0.05f.InMillimeters()));
                    angleDimConstraint.Parameter.Value = MathExtensions.DegreesToRadians(180 - 15);
                    var vector2 = angledLine.Geometry.Direction.AsVector();
                    vector2.ScaleBy(10f.InMillimeters());
                    var pt = angledLine.EndSketchPoint.Geometry;

                    pt.TranslateBy(vector2);
                    angledLine.EndSketchPoint.MoveTo(pt);
                    var intersectPoint = angledLine.Geometry.IntersectWithCurve(helperLine2.Geometry)?[1] as Point2d;
                    angledLine.EndSketchPoint.MoveTo(intersectPoint);
                    helperLine2.EndSketchPoint.MoveTo(intersectPoint);
                    angledLine.EndSketchPoint.Merge(helperLine2.EndSketchPoint);
                    var pointToDepthPlusA = TransientGeometry.CreatePoint2d(edgePoint.X,
                        edgePoint.Y - (reliefEDin.ReliefDepth - reliefEDin.MachiningAllowance).InMillimeters());
                    var rightLine = lines.AddByTwoPoints(pointToDepthPlusA,
                        TransientGeometry.CreatePoint2d(pointToDepthPlusA.X,
                            pointToDepthPlusA.Y + reliefEDin.Radius.InMillimeters()));

                    var fillet2D = arcs.AddByFillet((SketchEntity)rightLine, (SketchEntity)helperLine2,
                        reliefEDin.Radius.InMillimeters(), rightLine.EndSketchPoint.Geometry,
                        helperLine2.EndSketchPoint.Geometry);
                    var lockLine = lines.AddByTwoPoints(angledLine.StartSketchPoint.Geometry,
                        rightLine.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(angledLine.StartSketchPoint);
                    lockLine.EndSketchPoint.Merge(rightLine.EndSketchPoint);
                    var axisLine = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var profile1 = reliefEDinSketch.Profiles.AddForSolid();
                    var feature =
                        compDef.Features.RevolveFeatures.AddFull(profile1, axisLine,
                            PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"ReliefEDin_#{reliefEDinNumber}";

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    sketch2.AxisEntity = compDef.WorkAxes[1];
                    sketch2.Visible = false;
                    var sketch2Line1 =
                        sketch2.SketchLines.AddByTwoPoints(helperLine2.StartSketchPoint.Geometry, pointToDepthPlusA);
                    var sketch2Line2 = sketch2.SketchLines.AddByTwoPoints(lockLine.EndSketchPoint.Geometry,
                        sketch2Line1.EndSketchPoint.Geometry);
                    sketch2Line2.EndSketchPoint.Merge(sketch2Line1.EndSketchPoint);
                    var filletCenterPoint =
                        TransientGeometry.CreatePoint2d(fillet2D.CenterSketchPoint.Geometry.X,
                            fillet2D.CenterSketchPoint.Geometry.Y);

                    var sketch2Arc1 = sketch2.SketchArcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge
                            ? sketch2Line2.StartSketchPoint.Geometry
                            : sketch2Line1.StartSketchPoint.Geometry,
                        firstEdge
                            ? sketch2Line1.StartSketchPoint.Geometry
                            : sketch2Line2.StartSketchPoint.Geometry);
                    sketch2Line1.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.EndSketchPoint
                        : sketch2Arc1.StartSketchPoint);
                    sketch2Line2.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.StartSketchPoint
                        : sketch2Arc1.EndSketchPoint);
                    sketch2.Visible = false;
                    var axisLine2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var sketch2Profile = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(sketch2Profile, axisLine2,
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefEDin));
                }
            }
        }


        private static void BuildRelifsFDin(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsFDin = new List<ReliefFDinEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefFDinEdgeFeature reliefFDin1 &&
                    reliefFDin1.ShouldBeBuilt)
                {
                    allReliefsFDin.Add(reliefFDin1);
                }

                if (section.SecondEdgeFeature is ReliefFDinEdgeFeature reliefFDin2 &&
                    reliefFDin2.ShouldBeBuilt)
                {
                    allReliefsFDin.Add(reliefFDin2);
                }
            }

            int reliefFDinNumber = 0;
            foreach (var reliefFDin in allReliefsFDin)
            {
                try
                {
                    reliefFDinNumber++;
                    bool firstEdge = reliefFDin.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefFDinSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefFDinSketch.AxisEntity = compDef.WorkAxes[1];

                    reliefFDinSketch.Visible = false;
                    var points = reliefFDinSketch.SketchPoints;
                    var lines = reliefFDinSketch.SketchLines;
                    var arcs = reliefFDinSketch.SketchArcs;
                    var edgePoint = TransientGeometry.CreatePoint2d(reliefFDin.EdgePoint.X.InMillimeters(),
                        reliefFDin.EdgePoint.Y.InMillimeters());
                    var pointFromEdgeToB = TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefFDin.EdgePoint.X + reliefFDin.Width).InMillimeters()
                            : (reliefFDin.EdgePoint.X - reliefFDin.Width).InMillimeters(),
                        reliefFDin.EdgePoint.Y.InMillimeters());
                    points.Add(pointFromEdgeToB);
                    var helperLine = lines.AddByTwoPoints(pointFromEdgeToB,
                        TransientGeometry.CreatePoint2d(firstEdge
                            ? pointFromEdgeToB.X + 1f.InMillimeters()
                            : pointFromEdgeToB.X - 1f.InMillimeters(), pointFromEdgeToB.Y));
                    var helperLine2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y - (reliefFDin.ReliefDepth - reliefFDin.MachiningAllowance).InMillimeters()),
                        TransientGeometry.CreatePoint2d(pointFromEdgeToB.X,
                            edgePoint.Y - (reliefFDin.ReliefDepth - reliefFDin.MachiningAllowance).InMillimeters()));
                    var helperPoint = pointFromEdgeToB.Copy();
                    var vector = helperLine.Geometry.Direction.AsVector();
                    vector.ScaleBy(-1f.InMillimeters());

                    helperPoint.TranslateBy(vector);

                    var p = points.Add(pointFromEdgeToB);
                    reliefFDinSketch.GeometricConstraints.AddGround((SketchEntity)p);
                    var angledLine = lines.AddByTwoPoints(p.Geometry, helperPoint);
                    angledLine.StartSketchPoint.Merge(p);

                    angledLine.OverrideColor = InvApp.TransientObjects.CreateColor(100, 0, 0);
                    reliefFDinSketch.GeometricConstraints.AddGround((SketchEntity)helperLine);
                    var angleDimConstraint = reliefFDinSketch.DimensionConstraints.AddTwoLineAngle(firstEdge ? helperLine : angledLine,
                        firstEdge ? angledLine : helperLine,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToB.X + 0.5f.InMillimeters()
                                : pointFromEdgeToB.X - 0.5f.InMillimeters(),
                            pointFromEdgeToB.Y + 0.05f.InMillimeters()));

                    angleDimConstraint.Parameter.Value = MathExtensions.DegreesToRadians(180 - 15);
                    var vector2 = angledLine.Geometry.Direction.AsVector();
                    vector2.ScaleBy(10f.InMillimeters());
                    var pt = angledLine.EndSketchPoint.Geometry;
                    pt.TranslateBy(vector2);

                    angledLine.EndSketchPoint.MoveTo(pt);
                    var intersectPoint = angledLine.Geometry.IntersectWithCurve(helperLine2.Geometry)?[1] as Point2d;
                    angledLine.EndSketchPoint.MoveTo(intersectPoint);
                    helperLine2.EndSketchPoint.MoveTo(intersectPoint);

                    angledLine.EndSketchPoint.Merge(helperLine2.EndSketchPoint);

                    var pointFromEdgeToB1 =
                        points.Add(TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y + (reliefFDin.ReliefDepth - reliefFDin.MachiningAllowance).InMillimeters() +
                            reliefFDin.Width1.InMillimeters()));
                    var pointFromEdgeToDepthPlusA = points.Add(TransientGeometry.CreatePoint2d(edgePoint.X,
                        edgePoint.Y - (reliefFDin.ReliefDepth - reliefFDin.MachiningAllowance).InMillimeters()));
                    reliefFDinSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToB1);

                    var helperLine3 =
                        lines.AddByTwoPoints(pointFromEdgeToDepthPlusA.Geometry, pointFromEdgeToB1.Geometry);
                    helperLine3.OverrideColor = InvApp.TransientObjects.CreateColor(190, 20, 0);
                    reliefFDinSketch.GeometricConstraints.AddGround((SketchEntity)helperLine3);
                    var angledLine2 = lines.AddByTwoPoints(helperLine3.StartSketchPoint.Geometry,
                        helperLine3.EndSketchPoint.Geometry);
                    angledLine2.OverrideColor = InvApp.TransientObjects.CreateColor(237, 209, 28);
                    angledLine2.EndSketchPoint.Merge(pointFromEdgeToB1);
                    var angleDimConstraint2 = reliefFDinSketch.DimensionConstraints.AddTwoLineAngle(helperLine3,
                         angledLine2,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? edgePoint.X + 0.1f.InMillimeters()
                                : edgePoint.X - 0.1f.InMillimeters(),
                            pointFromEdgeToDepthPlusA.Geometry.Y +
                            0.5f.InMillimeters()));
                    angleDimConstraint2.Parameter.Value = MathExtensions.DegreesToRadians(firstEdge ? 180 - 8 : 8);
                    ((PartDocument)compDef.Document).Update();
                    var line = TransientGeometry.CreateLine2d(angledLine2.StartSketchPoint.Geometry,
                        angledLine2.Geometry.Direction);
                    var line2 = TransientGeometry.CreateLine2d(helperLine2.StartSketchPoint.Geometry,
                        helperLine2.Geometry.Direction);
                    line2.Direction.Y = -line2.Direction.Y;
                    var intersectPoint2 = line.IntersectWithCurve(line2)?[1] as Point2d;
                    var p2 = points.Add(intersectPoint2);
                    ((PartDocument)compDef.Document).Update();
                    var line4 = lines.AddByTwoPoints(pointFromEdgeToDepthPlusA.Geometry, p2.Geometry);
                    line4.OverrideColor = InvApp.TransientObjects.CreateColor(239, 92, 19);
                    reliefFDinSketch.GeometricConstraints.AddGround((SketchEntity)line4);
                    angledLine2.StartSketchPoint.MoveTo(intersectPoint2);

                    var fillet2d = arcs.AddByFillet((SketchEntity)angledLine2, (SketchEntity)line4,
                        reliefFDin.Radius.InMillimeters(), angledLine2.EndSketchPoint.Geometry,
                         line4.StartSketchPoint.Geometry);
                    helperLine2.StartSketchPoint.MoveTo(firstEdge ? fillet2d.EndSketchPoint.Geometry : fillet2d.StartSketchPoint.Geometry);
                    helperLine2.StartSketchPoint.Merge(firstEdge ? fillet2d.EndSketchPoint : fillet2d.StartSketchPoint);
                    helperLine3.Delete();
                    var lockLine = lines.AddByTwoPoints(angledLine.StartSketchPoint.Geometry,
                        angledLine2.StartSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(angledLine.StartSketchPoint);
                    lockLine.EndSketchPoint.Merge(angledLine2.StartSketchPoint);
                    var filletCenterPoint = fillet2d.Geometry.Center;
                    line4.Delete();
                    fillet2d.Delete();

                    var arc = arcs.AddByCenterStartEndPoint(filletCenterPoint,
                         helperLine2.StartSketchPoint.Geometry,
                         angledLine2.StartSketchPoint.Geometry);
                    arc.StartSketchPoint.Merge(firstEdge ? angledLine2.StartSketchPoint : helperLine2.StartSketchPoint);
                    arc.EndSketchPoint.Merge(firstEdge ? helperLine2.StartSketchPoint : angledLine2.StartSketchPoint);

                    var axisLine = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));

                    var profile = reliefFDinSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.RevolveFeatures.AddFull(profile, axisLine,
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"ReliefFDin_#{reliefFDinNumber}";
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefFDin));
                }
            }
        }

        private static void BuildRelifsAGost(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsAGost = new List<ReliefAGostEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefAGostEdgeFeature reliefAGost1 &&
                    reliefAGost1.ShouldBeBuilt)
                {
                    allReliefsAGost.Add(reliefAGost1);
                }

                if (section.SecondEdgeFeature is ReliefAGostEdgeFeature reliefAGost2 &&
                    reliefAGost2.ShouldBeBuilt)
                {
                    allReliefsAGost.Add(reliefAGost2);
                }
            }

            int reliefAGostNumber = 0;
            foreach (var reliefAGost in allReliefsAGost)
            {
                try
                {
                    reliefAGostNumber++;
                    bool firstEdge = reliefAGost.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefAGostSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefAGostSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefAGostSketch.Visible = false;
                    var points = reliefAGostSketch.SketchPoints;
                    var lines = reliefAGostSketch.SketchLines;
                    var arcs = reliefAGostSketch.SketchArcs;
                    var thisPartDoc = (PartDocument)compDef.Document;
                    var pointFromEdgeToDepth = points.Add(TransientGeometry.CreatePoint2d(
                        reliefAGost.EdgePoint.X.InMillimeters(),
                        (reliefAGost.EdgePoint.Y - reliefAGost.ReliefDepth - reliefAGost.MachiningAllowance)
                        .InMillimeters()));
                    var pointFromEdgeToB = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointFromEdgeToDepth.Geometry.X + reliefAGost.Width.InMillimeters()
                            : pointFromEdgeToDepth.Geometry.X - reliefAGost.Width.InMillimeters(),
                        pointFromEdgeToDepth.Geometry.Y));
                    var lineB = lines.AddByTwoPoints(pointFromEdgeToDepth.Geometry, pointFromEdgeToB.Geometry);

                    var rightLine = lines.AddByTwoPoints(pointFromEdgeToDepth.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepth.Geometry.X,
                            pointFromEdgeToDepth.Geometry.Y + reliefAGost.Radius.InMillimeters()));
                    var rightFillet2D = arcs.AddByFillet((SketchEntity)lineB, (SketchEntity)rightLine,
                        reliefAGost.Radius.InMillimeters(),
                        lineB.EndSketchPoint.Geometry, rightLine.EndSketchPoint.Geometry);
                    var angledLine = lines.AddByTwoPoints(lineB.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? lineB.EndSketchPoint.Geometry.X + 0.01f.InMillimeters()
                                : lineB.EndSketchPoint.Geometry.X - 0.01f.InMillimeters(),
                            lineB.EndSketchPoint.Geometry.Y));
                    reliefAGostSketch.GeometricConstraints.AddGround((SketchEntity)lineB);
                    thisPartDoc.Update();
                    var dim = reliefAGostSketch.DimensionConstraints.AddTwoLineAngle(angledLine, lineB,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToB.Geometry.X + 0.5f.InMillimeters()
                                : pointFromEdgeToB.Geometry.X - 0.5f.InMillimeters(),
                            pointFromEdgeToB.Geometry.Y + 0.5f.InMillimeters()));
                    dim.Parameter.Value = MathExtensions.DegreesToRadians(firstEdge ? 180 - 45 : 45);
                    thisPartDoc.Update();
                    var helperLine2 = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(
                            reliefAGost.EdgePoint.X.InMillimeters(),
                            reliefAGost.EdgePoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(firstEdge
                            ? (reliefAGost.EdgePoint.X + 5).InMillimeters()
                            : (reliefAGost.EdgePoint.X - 5).InMillimeters(), reliefAGost.EdgePoint.Y.InMillimeters()));


                    angledLine.StartSketchPoint.MoveTo(lineB.EndSketchPoint.Geometry);
                    var line2D1 = TransientGeometry.CreateLine2d(helperLine2.StartSketchPoint.Geometry,
                        helperLine2.Geometry.Direction);
                    var line2D2 =
                        TransientGeometry.CreateLine2d(angledLine.EndSketchPoint.Geometry,
                            angledLine.Geometry.Direction);
                    var intersectionPoint = line2D1.IntersectWithCurve(line2D2)?[1] as Point2d;
                    angledLine.EndSketchPoint.MoveTo(intersectionPoint);
                    angledLine.StartSketchPoint.Merge(lineB.EndSketchPoint);
                    reliefAGostSketch.GeometricConstraints.AddGround((SketchEntity)angledLine.EndSketchPoint);
                    arcs.AddByFillet((SketchEntity)angledLine, (SketchEntity)lineB,
                        reliefAGost.Radius1.InMillimeters(), angledLine.EndSketchPoint.Geometry,
                        lineB.StartSketchPoint.Geometry);
                    var lockLine = lines.AddByTwoPoints(angledLine.EndSketchPoint.Geometry,
                       firstEdge ? rightFillet2D.StartSketchPoint.Geometry : rightFillet2D.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(angledLine.EndSketchPoint);
                    lockLine.EndSketchPoint.Merge(firstEdge ? rightFillet2D.StartSketchPoint : rightFillet2D.EndSketchPoint);


                    var axisLine = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var profile1 = reliefAGostSketch.Profiles.AddForSolid();
                    var feature =
                        compDef.Features.RevolveFeatures.AddFull(profile1, axisLine,
                            PartFeatureOperationEnum.kCutOperation);

                    feature.Name = $"ReliefAGost_#{reliefAGostNumber}";
                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    sketch2.AxisEntity = compDef.WorkAxes[1];

                    sketch2.Visible = false;
                    var intersectionPoint2 = line2D1.IntersectWithCurve(rightFillet2D.Geometry)[1] as Point2d;
                    sketch2.SketchPoints.Add(intersectionPoint2);
                    var sketch2Line1 =
                        sketch2.SketchLines.AddByTwoPoints(helperLine2.StartSketchPoint.Geometry, intersectionPoint2);
                    var sketch2Line2 = sketch2.SketchLines.AddByTwoPoints(helperLine2.StartSketchPoint.Geometry,
                        lockLine.EndSketchPoint.Geometry);
                    sketch2Line2.StartSketchPoint.Merge(sketch2Line1.StartSketchPoint);
                    var filletCenterPoint = TransientGeometry.CreatePoint2d(rightFillet2D.CenterSketchPoint.Geometry.X,
                        rightFillet2D.CenterSketchPoint.Geometry.Y);
                    var sketch2Arc1 = sketch2.SketchArcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge ? lockLine.EndSketchPoint.Geometry : intersectionPoint2,
                       firstEdge ? intersectionPoint2 : lockLine.EndSketchPoint.Geometry);

                    sketch2Line1.EndSketchPoint.Merge(firstEdge ? sketch2Arc1.EndSketchPoint : sketch2Arc1.StartSketchPoint);
                    sketch2Line2.EndSketchPoint.Merge(firstEdge ? sketch2Arc1.StartSketchPoint : sketch2Arc1.EndSketchPoint);
                    sketch2.Visible = false;

                    var axisLine2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));

                    var sketch2Profile = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(sketch2Profile, axisLine2,
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefAGost));
                }
            }
        }

        private static void BuildRelifsBGost(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsBGost = new List<ReliefBGostEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefBGostEdgeFeature reliefBGost1 &&
                    reliefBGost1.ShouldBeBuilt)
                {
                    allReliefsBGost.Add(reliefBGost1);
                }

                if (section.SecondEdgeFeature is ReliefBGostEdgeFeature reliefBGost2 &&
                    reliefBGost2.ShouldBeBuilt)
                {
                    allReliefsBGost.Add(reliefBGost2);
                }
            }

            int reliefBGostNumber = 0;
            foreach (var reliefBGost in allReliefsBGost)
            {
                try
                {
                    reliefBGostNumber++;
                    bool firstEdge = reliefBGost.EdgePosition == EdgeFeaturePosition.FirstEdge;

                    var reliefBGostSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefBGostSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefBGostSketch.Visible = false;
                    var points = reliefBGostSketch.SketchPoints;
                    var lines = reliefBGostSketch.SketchLines;
                    var arcs = reliefBGostSketch.SketchArcs;

                    var thisPartDoc = (PartDocument)compDef.Document;
                    var pointFromEdgeToDepth = points.Add(TransientGeometry.CreatePoint2d(
                        reliefBGost.EdgePoint.X.InMillimeters(),
                        (reliefBGost.EdgePoint.Y - reliefBGost.ReliefDepth - reliefBGost.MachiningAllowance)
                        .InMillimeters()));
                    var pointFromEdgeToDepth1 = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? reliefBGost.EdgePoint.X.InMillimeters() -
                              (reliefBGost.ReliefDepth1 - reliefBGost.MachiningAllowance).InMillimeters()
                            : reliefBGost.EdgePoint.X.InMillimeters() +
                              (reliefBGost.ReliefDepth1 + reliefBGost.MachiningAllowance).InMillimeters(),
                        (reliefBGost.EdgePoint.Y - reliefBGost.ReliefDepth - reliefBGost.MachiningAllowance)
                        .InMillimeters()));
                    var pointFromEdgeToBHorizontal = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointFromEdgeToDepth1.Geometry.X + reliefBGost.Width.InMillimeters()
                            : pointFromEdgeToDepth1.Geometry.X - reliefBGost.Width.InMillimeters(),
                        pointFromEdgeToDepth1.Geometry.Y));
                    var pointFromEdgeToBVertical = points.Add(TransientGeometry.CreatePoint2d(
                        pointFromEdgeToDepth1.Geometry.X,
                        pointFromEdgeToDepth1.Geometry.Y + reliefBGost.Width.InMillimeters()));

                    var lineBHorizontal =
                        lines.AddByTwoPoints(pointFromEdgeToDepth1.Geometry, pointFromEdgeToBHorizontal.Geometry);
                    var lineBVertical =
                        lines.AddByTwoPoints(pointFromEdgeToDepth1.Geometry, pointFromEdgeToBVertical.Geometry);
                    var rightFillet2D = arcs.AddByFillet((SketchEntity)lineBHorizontal, (SketchEntity)lineBVertical,
                        reliefBGost.Radius.InMillimeters(),
                        lineBHorizontal.EndSketchPoint.Geometry, lineBVertical.EndSketchPoint.Geometry);
                    var angledLine = lines.AddByTwoPoints(lineBHorizontal.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? lineBHorizontal.EndSketchPoint.Geometry.X + 0.01f.InMillimeters()
                                : lineBHorizontal.EndSketchPoint.Geometry.X - 0.01f.InMillimeters(),
                            lineBHorizontal.EndSketchPoint.Geometry.Y));
                    reliefBGostSketch.GeometricConstraints.AddGround((SketchEntity)lineBHorizontal);
                    thisPartDoc.Update();
                    var dim = reliefBGostSketch.DimensionConstraints.AddTwoLineAngle(angledLine, lineBHorizontal,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToBHorizontal.Geometry.X +
                                  0.5f.InMillimeters()
                                : pointFromEdgeToBHorizontal.Geometry.X -
                                  0.5f.InMillimeters(),
                            pointFromEdgeToBHorizontal.Geometry.Y + 0.5f.InMillimeters()));
                    dim.Parameter.Value = MathExtensions.DegreesToRadians(firstEdge ? 180 - 45 : 45);
                    thisPartDoc.Update();
                    var helperLine2 = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(
                            reliefBGost.EdgePoint.X.InMillimeters(),
                            reliefBGost.EdgePoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(firstEdge
                            ? (reliefBGost.EdgePoint.X + 5).InMillimeters()
                            : (reliefBGost.EdgePoint.X - 5).InMillimeters(), reliefBGost.EdgePoint.Y.InMillimeters()));
                    angledLine.StartSketchPoint.MoveTo(lineBHorizontal.EndSketchPoint.Geometry);

                    var line2D1 = TransientGeometry.CreateLine2d(helperLine2.StartSketchPoint.Geometry,
                        helperLine2.Geometry.Direction);
                    var line2D2 =
                        TransientGeometry.CreateLine2d(angledLine.EndSketchPoint.Geometry,
                            angledLine.Geometry.Direction);

                    var intersectionPoint = line2D1.IntersectWithCurve(line2D2)?[1] as Point2d;
                    angledLine.EndSketchPoint.MoveTo(intersectionPoint);
                    angledLine.StartSketchPoint.Merge(lineBHorizontal.EndSketchPoint);
                    reliefBGostSketch.GeometricConstraints.AddGround((SketchEntity)angledLine.EndSketchPoint);
                    arcs.AddByFillet((SketchEntity)angledLine, (SketchEntity)lineBHorizontal,
                        reliefBGost.Radius1.InMillimeters(), angledLine.EndSketchPoint.Geometry,
                        lineBHorizontal.StartSketchPoint.Geometry);

                    var helperLine3 = lines.AddByTwoPoints(pointFromEdgeToDepth.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepth.Geometry.X,
                            (reliefBGost.EdgePoint.Y + 5).InMillimeters()));
                    var angledLine2 = lines.AddByTwoPoints(lineBVertical.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(lineBVertical.EndSketchPoint.Geometry.X,
                            lineBVertical.EndSketchPoint.Geometry.Y + 0.5f.InMillimeters()));
                    reliefBGostSketch.GeometricConstraints.AddGround((SketchEntity)lineBVertical);
                    thisPartDoc.Update();
                    var dim2 = reliefBGostSketch.DimensionConstraints.AddTwoLineAngle(angledLine2, lineBVertical,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToBVertical.Geometry.X +
                                  0.25f.InMillimeters()
                                : pointFromEdgeToBVertical.Geometry.X -
                                  0.25f.InMillimeters(),
                            pointFromEdgeToBHorizontal.Geometry.Y + 0.5f.InMillimeters()));
                    dim2.Parameter.Value = MathExtensions.DegreesToRadians(firstEdge ? 45 : 180 - 45);

                    thisPartDoc.Update();
                    angledLine2.StartSketchPoint.MoveTo(lineBVertical.EndSketchPoint.Geometry);
                    var line2D3 = TransientGeometry.CreateLine2d(helperLine3.StartSketchPoint.Geometry,
                        helperLine3.Geometry.Direction);

                    var line2D4 =
                        TransientGeometry.CreateLine2d(angledLine2.EndSketchPoint.Geometry,
                            angledLine2.Geometry.Direction);
                    var intersectionPoint2 = line2D3.IntersectWithCurve(line2D4)?[1] as Point2d;
                    angledLine2.EndSketchPoint.MoveTo(intersectionPoint2);
                    angledLine2.StartSketchPoint.Merge(lineBVertical.EndSketchPoint);
                    reliefBGostSketch.GeometricConstraints.AddGround((SketchEntity)angledLine2.EndSketchPoint);
                    arcs.AddByFillet((SketchEntity)angledLine2, (SketchEntity)lineBVertical,
                        reliefBGost.Radius1.InMillimeters(), angledLine2.EndSketchPoint.Geometry,
                        lineBVertical.StartSketchPoint.Geometry);
                    var lockLine = lines.AddByTwoPoints(angledLine.EndSketchPoint.Geometry,
                        angledLine2.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(angledLine.EndSketchPoint);

                    lockLine.EndSketchPoint.Merge(angledLine2.EndSketchPoint);
                    var axisLine = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var profile1 = reliefBGostSketch.Profiles.AddForSolid();
                    var feature =
                        compDef.Features.RevolveFeatures.AddFull(profile1, axisLine,
                            PartFeatureOperationEnum.kCutOperation);

                    feature.Name = $"ReliefBGost_#{reliefBGostNumber}";

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    sketch2.AxisEntity = compDef.WorkAxes[1];
                    sketch2.Visible = false;
                    var intersectionPoint3 = line2D1.IntersectWithCurve(rightFillet2D.Geometry)[1] as Point2d;
                    var intersectionPoint4 = line2D3.IntersectWithCurve(rightFillet2D.Geometry)[1] as Point2d;
                    sketch2.SketchPoints.Add(intersectionPoint3);
                    var sketch2Line1 =
                        sketch2.SketchLines.AddByTwoPoints(helperLine2.StartSketchPoint.Geometry, intersectionPoint3);
                    var sketch2Line2 =
                        sketch2.SketchLines.AddByTwoPoints(helperLine2.StartSketchPoint.Geometry, intersectionPoint4);
                    var sketch2Line3 = sketch2.SketchLines.AddByTwoPoints(sketch2Line1.EndSketchPoint.Geometry,
                        sketch2Line2.EndSketchPoint.Geometry);

                    sketch2Line2.StartSketchPoint.Merge(sketch2Line1.StartSketchPoint);
                    sketch2Line3.StartSketchPoint.Merge(sketch2Line1.EndSketchPoint);
                    sketch2Line3.EndSketchPoint.Merge(sketch2Line2.EndSketchPoint);
                    sketch2.Visible = false;
                    var axisLine2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var sketch2Profile = sketch2.Profiles.AddForSolid();

                    compDef.Features.RevolveFeatures.AddFull(sketch2Profile, axisLine2,
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefBGost));
                }
            }
        }

        private static void BuildRelifsCGost(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsCGost = new List<ReliefCGostEdgeFeature>();
            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefCGostEdgeFeature reliefCGost1 &&
                    reliefCGost1.ShouldBeBuilt)
                {
                    allReliefsCGost.Add(reliefCGost1);
                }

                if (section.SecondEdgeFeature is ReliefCGostEdgeFeature reliefCGost2 &&
                    reliefCGost2.ShouldBeBuilt)
                {
                    allReliefsCGost.Add(reliefCGost2);
                }
            }

            int reliefCGostNumber = 0;
            foreach (var reliefCGost in allReliefsCGost)
            {
                try
                {
                    reliefCGostNumber++;
                    bool firstEdge = reliefCGost.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefCGostSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefCGostSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefCGostSketch.Visible = false;
                    var points = reliefCGostSketch.SketchPoints;
                    var lines = reliefCGostSketch.SketchLines;
                    var arcs = reliefCGostSketch.SketchArcs;
                    var thisPartDoc = (PartDocument)compDef.Document;
                    var edgePoint = TransientGeometry.CreatePoint2d(reliefCGost.EdgePoint.X.InMillimeters(),
                        reliefCGost.EdgePoint.Y.InMillimeters());
                    var pointFromEdgeToDepth = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? edgePoint.X -
                              (reliefCGost.ReliefDepth - reliefCGost.MachiningAllowance).InMillimeters()
                            : reliefCGost.EdgePoint.X.InMillimeters() +
                              (reliefCGost.ReliefDepth + reliefCGost.MachiningAllowance).InMillimeters(), edgePoint.Y));
                    var pointFromEdgeToB = points.Add(TransientGeometry.CreatePoint2d(
                        pointFromEdgeToDepth.Geometry.X,
                        pointFromEdgeToDepth.Geometry.Y + reliefCGost.Width.InMillimeters()));

                    var lineB =
                        lines.AddByTwoPoints(pointFromEdgeToDepth.Geometry, pointFromEdgeToB.Geometry);
                    var helperLineHorizontal = lines.AddByTwoPoints(pointFromEdgeToDepth.Geometry,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToDepth.Geometry.X + (reliefCGost.Width * 2).InMillimeters()
                                : pointFromEdgeToDepth.Geometry.X - (reliefCGost.Width * 2).InMillimeters(),
                            pointFromEdgeToDepth.Geometry.Y));
                    var fillet = arcs.AddByFillet((SketchEntity)helperLineHorizontal, (SketchEntity)lineB,
                        reliefCGost.Radius.InMillimeters(),
                        helperLineHorizontal.EndSketchPoint.Geometry, lineB.EndSketchPoint.Geometry);
                    var angledLine = lines.AddByTwoPoints(lineB.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(lineB.EndSketchPoint.Geometry.X,
                            lineB.EndSketchPoint.Geometry.Y + 0.5f.InMillimeters()));
                    reliefCGostSketch.GeometricConstraints.AddGround((SketchEntity)lineB);
                    thisPartDoc.Update();
                    var dim2 = reliefCGostSketch.DimensionConstraints.AddTwoLineAngle(angledLine, lineB,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? pointFromEdgeToB.Geometry.X + 0.25f.InMillimeters()
                                : pointFromEdgeToB.Geometry.X - 0.25f.InMillimeters(),
                            pointFromEdgeToB.Geometry.Y + 0.5f.InMillimeters()));
                    dim2.Parameter.Value = MathExtensions.DegreesToRadians(firstEdge ? 45 : 180 - 45);
                    thisPartDoc.Update();
                    var helperLine2 = lines.AddByTwoPoints(edgePoint,
                        TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y + 5f.InMillimeters()));
                    angledLine.StartSketchPoint.MoveTo(lineB.EndSketchPoint.Geometry);
                    var line2D1 = TransientGeometry.CreateLine2d(helperLine2.StartSketchPoint.Geometry,
                        helperLine2.Geometry.Direction);
                    var line2D2 =
                        TransientGeometry.CreateLine2d(angledLine.EndSketchPoint.Geometry,
                            angledLine.Geometry.Direction);
                    var intersectionPoint = line2D1.IntersectWithCurve(line2D2)?[1] as Point2d;
                    angledLine.EndSketchPoint.MoveTo(intersectionPoint);
                    angledLine.StartSketchPoint.Merge(lineB.EndSketchPoint);
                    reliefCGostSketch.GeometricConstraints.AddGround((SketchEntity)angledLine.EndSketchPoint);
                    arcs.AddByFillet((SketchEntity)angledLine, (SketchEntity)lineB,
                        reliefCGost.Radius1.InMillimeters(), angledLine.EndSketchPoint.Geometry,
                        lineB.StartSketchPoint.Geometry);
                    var lockLine = lines.AddByTwoPoints(helperLineHorizontal.EndSketchPoint.Geometry,
                        angledLine.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(helperLineHorizontal.EndSketchPoint);
                    lockLine.EndSketchPoint.Merge(angledLine.EndSketchPoint);
                    var axisLine = lines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var profile1 = reliefCGostSketch.Profiles.AddForSolid();

                    var feature =
                        compDef.Features.RevolveFeatures.AddFull(profile1, axisLine,
                            PartFeatureOperationEnum.kCutOperation);

                    feature.Name = $"ReliefCGost_#{reliefCGostNumber}";
                    var filletCenterPoint = TransientGeometry.CreatePoint2d(fillet.CenterSketchPoint.Geometry.X,
                        fillet.CenterSketchPoint.Geometry.Y);
                    var intersectionPoint2 = line2D1.IntersectWithCurve(fillet.Geometry)[1] as Point2d;

                    var intersectionPoint3 = TransientGeometry.CreateLine2d(
                                                                  helperLineHorizontal.StartSketchPoint.Geometry,
                                                                  helperLineHorizontal.Geometry.Direction)
                                                              .IntersectWithCurve(fillet.Geometry)[1] as Point2d;
                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    var line1 = sketch2.SketchLines.AddByTwoPoints(edgePoint, intersectionPoint2);
                    var line2 = sketch2.SketchLines.AddByTwoPoints(edgePoint, intersectionPoint3);
                    line1.StartSketchPoint.Merge(line2.StartSketchPoint);

                    var arc2 = sketch2.SketchArcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge ? line1.EndSketchPoint : line2.EndSketchPoint,
                        firstEdge ? line2.EndSketchPoint : line1.EndSketchPoint);

                    sketch2.Visible = false;
                    var axisLine2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    compDef.Features.RevolveFeatures.AddFull(sketch2.Profiles.AddForSolid(), axisLine2,
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefCGost));
                }
            }
        }

        private static void BuildReliefsDGost(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsDGost = new List<ReliefDGostEdgeFeature>();

            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefDGostEdgeFeature reliefDGost1 &&
                    reliefDGost1.ShouldBeBuilt)
                {
                    allReliefsDGost.Add(reliefDGost1);
                }

                if (section.SecondEdgeFeature is ReliefDGostEdgeFeature reliefDGost2 &&
                    reliefDGost2.ShouldBeBuilt)
                {
                    allReliefsDGost.Add(reliefDGost2);
                }
            }

            int reliefDGostNumber = 0;
            foreach (var reliefDGost in allReliefsDGost)
            {
                try
                {
                    reliefDGostNumber++;
                    bool firstEdge = reliefDGost.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefDGostSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefDGostSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefDGostSketch.Visible = false;

                    var points = reliefDGostSketch.SketchPoints;
                    var lines = reliefDGostSketch.SketchLines;
                    var arcs = reliefDGostSketch.SketchArcs;
                    var edgePoint = TransientGeometry.CreatePoint2d(reliefDGost.EdgePoint.X.InMillimeters(),
                        reliefDGost.EdgePoint.Y.InMillimeters());
                    var pointFromEdgeToDepthPlusRigth = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? (reliefDGost.EdgePoint.X - reliefDGost.ReliefDepth1 - reliefDGost.MachiningAllowance)
                            .InMillimeters()
                            : (reliefDGost.EdgePoint.X + reliefDGost.ReliefDepth1 + reliefDGost.MachiningAllowance)
                            .InMillimeters(), (reliefDGost.EdgePoint.Y + reliefDGost.Width / 2).InMillimeters()));
                    var pointFromEdgeToDepthPlusA = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointFromEdgeToDepthPlusRigth.Geometry.X + (reliefDGost.Width / 2).InMillimeters()
                            : pointFromEdgeToDepthPlusRigth.Geometry.X - (reliefDGost.Width / 2).InMillimeters(),
                        (reliefDGost.EdgePoint.Y - reliefDGost.ReliefDepth - reliefDGost.MachiningAllowance)
                        .InMillimeters()));
                    var pointFromEdgeToBHorizontal = points.Add(TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? pointFromEdgeToDepthPlusRigth.Geometry.X + reliefDGost.Width.InMillimeters()
                            : pointFromEdgeToDepthPlusRigth.Geometry.X - reliefDGost.Width.InMillimeters(),
                        edgePoint.Y));
                    var pointFromEdgeToBVertical = points.Add(TransientGeometry.CreatePoint2d(edgePoint.X,
                        pointFromEdgeToDepthPlusA.Geometry.Y + reliefDGost.Width.InMillimeters()));

                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToBHorizontal);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToBVertical);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToDepthPlusA);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)pointFromEdgeToDepthPlusRigth);
                    var line1 = lines.AddByTwoPoints(pointFromEdgeToBHorizontal.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToBHorizontal.Geometry.X,
                            pointFromEdgeToDepthPlusA.Geometry.Y));
                    var line2 = lines.AddByTwoPoints(line1.EndSketchPoint.Geometry, pointFromEdgeToDepthPlusA.Geometry);
                    var line3 = lines.AddByTwoPoints(pointFromEdgeToDepthPlusA.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepthPlusRigth.Geometry.X,
                            pointFromEdgeToDepthPlusA.Geometry.Y));

                    var line4 = lines.AddByTwoPoints(line3.EndSketchPoint.Geometry,
                        pointFromEdgeToDepthPlusRigth.Geometry);
                    var line5 = lines.AddByTwoPoints(line4.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(pointFromEdgeToDepthPlusRigth.Geometry.X,
                            pointFromEdgeToBVertical.Geometry.Y));
                    var line6 = lines.AddByTwoPoints(line5.EndSketchPoint.Geometry, pointFromEdgeToBVertical.Geometry);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)line1);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)line2);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)line3);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)line4);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)line5);
                    reliefDGostSketch.GeometricConstraints.AddGround((SketchEntity)line6);
                    var fillet1 = arcs.AddByFillet((SketchEntity)line1, (SketchEntity)line2,
                        reliefDGost.Radius.InMillimeters(), line1.StartSketchPoint.Geometry,
                        line2.EndSketchPoint.Geometry);
                    var fillet2 = arcs.AddByFillet((SketchEntity)line3, (SketchEntity)line4,
                        reliefDGost.Radius.InMillimeters(), line3.StartSketchPoint.Geometry,
                        line4.EndSketchPoint.Geometry);
                    var fillet3 = arcs.AddByFillet((SketchEntity)line5, (SketchEntity)line6,
                        reliefDGost.Radius.InMillimeters(), line5.StartSketchPoint.Geometry,
                        line6.EndSketchPoint.Geometry);
                    var newFillet1 = arcs.AddByCenterStartEndPoint(fillet1.CenterSketchPoint.Geometry,
                        fillet1.StartSketchPoint.Geometry, fillet1.EndSketchPoint.Geometry);
                    var newFillet2 = arcs.AddByCenterStartEndPoint(fillet2.CenterSketchPoint.Geometry
                        , fillet2.StartSketchPoint.Geometry, fillet2.EndSketchPoint.Geometry);
                    var newFillet3 = arcs.AddByCenterStartEndPoint(fillet3.CenterSketchPoint.Geometry,
                        fillet3.StartSketchPoint.Geometry,
                        fillet3.EndSketchPoint.Geometry);

                    fillet1.Delete();
                    fillet2.Delete();
                    fillet3.Delete();

                    line2.StartSketchPoint.Merge(firstEdge ? newFillet1.StartSketchPoint : newFillet1.EndSketchPoint);
                    line2.EndSketchPoint.Merge(line3.StartSketchPoint);
                    line3.EndSketchPoint.Merge(firstEdge ? newFillet2.EndSketchPoint : newFillet2.StartSketchPoint);

                    line4.StartSketchPoint.Merge(firstEdge ? newFillet2.StartSketchPoint : newFillet2.EndSketchPoint);
                    line4.EndSketchPoint.Merge(line5.StartSketchPoint);
                    line5.EndSketchPoint.Merge(firstEdge ? newFillet3.EndSketchPoint : newFillet3.StartSketchPoint);
                    var lockLine = lines.AddByTwoPoints(
                        firstEdge ? newFillet1.EndSketchPoint.Geometry : newFillet1.StartSketchPoint.Geometry,
                        firstEdge ? newFillet3.StartSketchPoint.Geometry : newFillet3.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(firstEdge ? newFillet1.EndSketchPoint : newFillet1.StartSketchPoint);
                    lockLine.EndSketchPoint.Merge(firstEdge ? newFillet3.StartSketchPoint : newFillet3.EndSketchPoint);
                    var profile = reliefDGostSketch.Profiles.AddForSolid();

                    var reliefADinFeature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    reliefADinFeature.Name = $"ReliefDGost_#{reliefDGostNumber}";

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    sketch2.AxisEntity = compDef.WorkAxes[1];
                    sketch2.Visible = false;
                    var helperLine3 = sketch2.SketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y), pointFromEdgeToBHorizontal.Geometry);
                    var intersectionPoint = helperLine3.Geometry.IntersectWithCurve(newFillet2.Geometry)[1] as Point2d;

                    var sketch2Line1 =
                        sketch2.SketchLines.AddByTwoPoints(intersectionPoint,
                            TransientGeometry.CreatePoint2d(edgePoint.X,
                                edgePoint.Y));
                    helperLine3.Delete();
                    var sketch2Line2 = sketch2.SketchLines.AddByTwoPoints(
                        firstEdge ? newFillet2.StartSketchPoint.Geometry : newFillet2.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(
                            firstEdge ? newFillet2.StartSketchPoint.Geometry.X : newFillet2.EndSketchPoint.Geometry.X,
                            edgePoint.Y));
                    sketch2Line1.EndSketchPoint.MoveTo(sketch2Line2.EndSketchPoint.Geometry);
                    sketch2Line2.EndSketchPoint.Merge(sketch2Line1.EndSketchPoint);
                    var filletCenterPoint = TransientGeometry.CreatePoint2d(newFillet2.CenterSketchPoint.Geometry.X,
                        newFillet2.CenterSketchPoint.Geometry.Y);
                    var sketch2Arc1 = sketch2.SketchArcs.AddByCenterStartEndPoint(filletCenterPoint,
                        firstEdge
                            ? sketch2Line2.StartSketchPoint.Geometry
                            : sketch2Line1.StartSketchPoint.Geometry,
                        firstEdge
                            ? sketch2Line1.StartSketchPoint.Geometry
                            : sketch2Line2.StartSketchPoint.Geometry);
                    sketch2Line1.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.EndSketchPoint
                        : sketch2Arc1.StartSketchPoint);
                    sketch2Line2.StartSketchPoint.Merge(firstEdge
                        ? sketch2Arc1.StartSketchPoint
                        : sketch2Arc1.EndSketchPoint);
                    sketch2.Visible = false;
                    var axisLine2 = sketch2.SketchLines.AddByTwoPoints(TransientGeometry.CreatePoint2d(),
                        TransientGeometry.CreatePoint2d(0.1f.InMillimeters()));
                    var sketch2Profile = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(sketch2Profile, axisLine2,
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefDGost));
                }
            }
        }


        private static void BuildReliefsEGost(PartComponentDefinition compDef)

        {
            var sections = Shaft.Sections;
            var allReliefsEGost = new List<ReliefEGostEdgeFeature>();


            foreach (var section in sections)
            {
                if (section.FirstEdgeFeature is ReliefEGostEdgeFeature reliefEGost1 &&
                    reliefEGost1.ShouldBeBuilt)
                {
                    allReliefsEGost.Add(reliefEGost1);
                }

                if (section.SecondEdgeFeature is ReliefEGostEdgeFeature reliefEGost2 &&
                    reliefEGost2.ShouldBeBuilt)
                {
                    allReliefsEGost.Add(reliefEGost2);
                }
            }

            int reliefEGostNumber = 0;
            foreach (var reliefEGost in allReliefsEGost)
            {
                try
                {
                    reliefEGostNumber++;

                    bool firstEdge = reliefEGost.EdgePosition == EdgeFeaturePosition.FirstEdge;
                    var reliefEGostSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefEGostSketch.AxisEntity = compDef.WorkAxes[1];
                    reliefEGostSketch.Visible = false;

                    var points = reliefEGostSketch.SketchPoints;
                    var lines = reliefEGostSketch.SketchLines;
                    var arcs = reliefEGostSketch.SketchArcs;
                    var edgePoint = TransientGeometry.CreatePoint2d(reliefEGost.EdgePoint.X.InMillimeters(),
                        reliefEGost.EdgePoint.Y.InMillimeters());
                    var pointFromEdgeToB =
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? edgePoint.X + reliefEGost.Width.InMillimeters()
                                : edgePoint.X - reliefEGost.Width.InMillimeters(),
                            edgePoint.Y);
                    var circleCenterPoint = TransientGeometry.CreatePoint2d(
                        firstEdge
                            ? edgePoint.X + reliefEGost.Radius.InMillimeters()
                            : edgePoint.X - reliefEGost.Radius.InMillimeters(),
                        edgePoint.Y - (reliefEGost.ReliefDepth + reliefEGost.MachiningAllowance).InMillimeters() +
                        reliefEGost.Radius.InMillimeters());
                    var circle =
                        reliefEGostSketch.SketchCircles.AddByCenterRadius(circleCenterPoint,
                            reliefEGost.Radius.InMillimeters());
                    var helperLine1 = lines.AddByTwoPoints(edgePoint,
                        TransientGeometry.CreatePoint2d(edgePoint.X, edgePoint.Y - 0.1f.InMillimeters()));
                    var helperLine2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(edgePoint.X,
                            edgePoint.Y - (reliefEGost.ReliefDepth - reliefEGost.MachiningAllowance).InMillimeters()),
                        TransientGeometry.CreatePoint2d(
                            firstEdge ? edgePoint.X + 0.1f.InMillimeters() : edgePoint.X - 0.1f.InMillimeters(),
                            edgePoint.Y - (reliefEGost.ReliefDepth - reliefEGost.MachiningAllowance).InMillimeters()));

                    var line2D1 = TransientGeometry.CreateLine2d(helperLine1.StartSketchPoint.Geometry,
                        helperLine1.Geometry.Direction);

                    var line2D2 = TransientGeometry.CreateLine2d(helperLine2.StartSketchPoint.Geometry,
                        helperLine2.Geometry.Direction);

                    var intersectionPointRight = line2D1.IntersectWithCurve(circle.Geometry)[1] as Point2d;

                    var intersectionPointBottom = line2D2.IntersectWithCurve(circle.Geometry)[1] as Point2d;
                    points.Add(intersectionPointRight);

                    points.Add(intersectionPointBottom);
                    reliefEGostSketch.GeometricConstraints.AddGround((SketchEntity)helperLine1.StartSketchPoint);
                    reliefEGostSketch.GeometricConstraints.AddGround((SketchEntity)helperLine2.StartSketchPoint);
                    helperLine1.EndSketchPoint.MoveTo(intersectionPointRight);
                    helperLine2.EndSketchPoint.MoveTo(intersectionPointBottom);
                    reliefEGostSketch.GeometricConstraints.AddTangent((SketchEntity)helperLine1,
                        (SketchEntity)circle);
                    reliefEGostSketch.GeometricConstraints.AddTangent((SketchEntity)helperLine2,
                        (SketchEntity)circle);
                    var line3 = lines.AddByTwoPoints(helperLine1.StartSketchPoint.Geometry,
                        helperLine2.StartSketchPoint.Geometry);
                    var angledLine = lines.AddByTwoPoints(pointFromEdgeToB, helperLine2.EndSketchPoint.Geometry);

                    reliefEGostSketch.GeometricConstraints.AddGround((SketchEntity)angledLine.StartSketchPoint);
                    var proximityPoint = angledLine.Geometry.IntersectWithCurve(circle.Geometry)[1] as Point2d;
                    reliefEGostSketch.GeometricConstraints.AddTangent((SketchEntity)angledLine, (SketchEntity)circle,
                        proximityPoint);
                    circle.Delete();

                    var arc = arcs.AddByCenterStartEndPoint(circleCenterPoint,
                        firstEdge ? helperLine1.EndSketchPoint.Geometry : helperLine2.EndSketchPoint.Geometry,
                       firstEdge ? helperLine2.EndSketchPoint.Geometry : helperLine1.EndSketchPoint.Geometry);
                    angledLine.EndSketchPoint.Merge(arc.StartSketchPoint);
                    var lockLine =
                        lines.AddByTwoPoints(angledLine.StartSketchPoint.Geometry, arc.EndSketchPoint.Geometry);
                    lockLine.StartSketchPoint.Merge(angledLine.StartSketchPoint);
                    lockLine.EndSketchPoint.Merge(arc.EndSketchPoint);
                    var profile = reliefEGostSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"ReliefEGost_#{reliefEGostNumber}";

                    var sketch2 = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    var arc2 = sketch2.SketchArcs.AddByCenterStartEndPoint(arc.CenterSketchPoint.Geometry,
                        arc.StartSketchPoint.Geometry,
                        arc.EndSketchPoint.Geometry);
                    var line1 = sketch2.SketchLines.AddByTwoPoints(arc2.EndSketchPoint.Geometry,
                        helperLine2.StartSketchPoint.Geometry);
                    var line2 = sketch2.SketchLines.AddByTwoPoints(helperLine2.StartSketchPoint.Geometry,
                        arc2.StartSketchPoint.Geometry);
                    line1.StartSketchPoint.Merge(arc2.EndSketchPoint);
                    line1.EndSketchPoint.Merge(line2.StartSketchPoint);
                    line2.EndSketchPoint.Merge(arc2.StartSketchPoint);
                    sketch2.Visible = false;
                    var profile2 = sketch2.Profiles.AddForSolid();
                    compDef.Features.RevolveFeatures.AddFull(profile2, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kJoinOperation);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefEGost));
                }
            }
        }

        private static void BuildThroughHoles(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allThroughHoles = new List<ThroughHoleSubFeature>();
            foreach (var section in sections)

            {
                allThroughHoles.AddRange(section.SubFeatures.OfType<ThroughHoleSubFeature>());
            }

            int throughHoleNumber = 0;
            foreach (var throughHole in allThroughHoles)
            {
                try
                {
                    throughHoleNumber++;
                    var holeCenterPoint = InvApp.TransientObjects.CreateObjectCollection();
                    float distance = throughHole.LinkedSection is CylinderSection cylinder
                        ? cylinder.Diameter.InMillimeters() / 2
                        : ((PolygonSection)throughHole.LinkedSection).CircumscribedCircleDiameter.InMillimeters() / 2;


                    var helperPlane = compDef.WorkPlanes.AddByLinePlaneAndAngle(compDef.WorkAxes[1],
                        compDef.WorkPlanes[3],
                        -throughHole.Angle + "deg");
                    helperPlane.Visible = false;
                    var throughHolePlane = compDef.WorkPlanes.AddByPlaneAndOffset(helperPlane, distance);
                    throughHolePlane.Visible = false;
                    var throughHoleSketch =
                        compDef.Sketches.Add(throughHolePlane);
                    throughHoleSketch.Visible = false;
                    throughHoleSketch.AxisEntity = compDef.WorkAxes[1];

                    Point2d point2D = null;
                    switch (throughHole.DistanceFrom)
                    {
                        case DistanceFrom.FromFirstEdge:
                            point2D = TransientGeometry.CreatePoint2d(throughHole.LinkedSection.SecondLine.StartPoint.X
                                                                                 .InMillimeters() +
                                                                      throughHole.Distance.InMillimeters());
                            break;
                        case DistanceFrom.FromSecondEdge:
                            point2D = TransientGeometry.CreatePoint2d(throughHole.LinkedSection.SecondLine.EndPoint.X
                                                                                 .InMillimeters() -
                                                                      throughHole.Distance.InMillimeters());
                            break;
                        case DistanceFrom.Centered:
                            point2D = TransientGeometry.CreatePoint2d(
                                (throughHole.LinkedSection.SecondLine.StartPoint.X +
                                 throughHole.LinkedSection.Length / 2)
                                .InMillimeters() + throughHole.Distance.InMillimeters());
                            break;
                    }

                    var point = throughHoleSketch.SketchPoints.Add(point2D);
                    holeCenterPoint.Add(point);
                    var holeFeature = compDef.Features.HoleFeatures.AddDrilledByThroughAllExtent(holeCenterPoint,
                        throughHole.HoleDiameter.InMillimeters(),
                        PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
                    holeFeature.Name = $"ThroughHole_#{throughHoleNumber}";
                    if (!throughHole.Chamfering.IsGreaterThanZero() ||
                        (throughHole.LinkedSection is PolygonSection polygonSection &&
                         (polygonSection.CircumscribedCircleDiameter / 2).NearlyEqual(throughHole.HoleDiameter,
                             float.Epsilon) && (throughHole.Angle.NearlyEqual(0, float.Epsilon) ||
                                                throughHole.Angle.NearlyEqual(90, float.Epsilon) ||
                                                throughHole.Angle.NearlyEqual(180, float.Epsilon) ||
                                                throughHole.Angle.NearlyEqual(270, float.Epsilon) ||
                                                throughHole.Angle.NearlyEqual(360, float.Epsilon))))
                    {
                        continue;
                    }

                    var faces = holeFeature.Faces;

                    Face neededFace = null;
                    foreach (Face face in faces)
                    {
                        if (face.SurfaceType == SurfaceTypeEnum.kCylinderSurface)
                        {
                            neededFace = face;
                            break;
                        }
                    }

                    var edges = neededFace.Edges;
                    var chamferEdgesColl = InvApp.TransientObjects.CreateEdgeCollection();
                    int[] edgeIndices = null;
                    switch (throughHole.DistanceFrom)
                    {
                        case DistanceFrom.FromFirstEdge when throughHole.LinkedSection is CylinderSection:
                        case DistanceFrom.FromSecondEdge when throughHole.LinkedSection is CylinderSection:
                            {
                                if ((throughHole.Distance + throughHole.HoleDiameter / 2) >
                                    throughHole.LinkedSection.Length ||
                                    (throughHole.Distance + throughHole.HoleDiameter / 2) < 0)

                                {
                                    edgeIndices = new[] { 1, 2, 4 };
                                }
                                else

                                {
                                    edgeIndices = new[] { 1, 2, 3 };
                                }

                                break;
                            }
                        case DistanceFrom.Centered when throughHole.LinkedSection is CylinderSection:
                            {
                                if ((throughHole.LinkedSection.Length / 2 + throughHole.Distance +
                                     throughHole.HoleDiameter / 2) > throughHole.LinkedSection.Length ||
                                    (throughHole.LinkedSection.Length / 2 + throughHole.Distance +
                                     throughHole.HoleDiameter / 2) < 0)

                                {
                                    edgeIndices = new[] { 1, 2, 4 };
                                }
                                else

                                {
                                    edgeIndices = new[] { 1, 2, 3 };
                                }

                                break;
                            }
                    }

                    if (throughHole.LinkedSection is PolygonSection)
                    {
                        foreach (Edge edge in edges)
                        {
                            if (edge.CurveType == CurveTypeEnum.kCircleCurve ||
                                edge.CurveType == CurveTypeEnum.kEllipseFullCurve)

                            {
                                chamferEdgesColl.Add(edge);
                            }
                        }
                    }
                    else
                    {
                        foreach (var index in edgeIndices)
                        {
                            var edge = edges[index];
                            chamferEdgesColl.Add(edge);
                        }
                    }

                    compDef.Features.ChamferFeatures.AddUsingDistance(chamferEdgesColl,
                            throughHole.Chamfering.InMillimeters()).Name =
                        $"Chamfer_ThroughHole_#{throughHoleNumber}";
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(throughHole));
                }
            }
        }

        private static void BuildWrenches(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;

            var allWrenches = new List<WrenchSubFeature>();
            foreach (var section in sections)
            {
                allWrenches.AddRange(section.SubFeatures.OfType<WrenchSubFeature>());
            }

            int wrenchNumber = 0;
            foreach (var wrench in allWrenches)
            {
                try
                {
                    wrenchNumber++;

                    var cylinderSection = (CylinderSection)wrench.LinkedSection;


                    float distance = cylinderSection.Diameter.InMillimeters() / 2;


                    var helperPlane = compDef.WorkPlanes.AddByLinePlaneAndAngle(compDef.WorkAxes[1],
                        compDef.WorkPlanes[3],
                        -wrench.Angle + "deg");

                    helperPlane.Visible = false;
                    var wrenchPlane = compDef.WorkPlanes.AddByPlaneAndOffset(helperPlane, distance);
                    wrenchPlane.Visible = false;
                    var wrenchSketch =
                        compDef.Sketches.Add(wrenchPlane);
                    wrenchSketch.Visible = false;

                    wrenchSketch.AxisEntity = compDef.WorkAxes[1];
                    var lines = wrenchSketch.SketchLines;
                    float startPointX = 0;
                    bool firstEdge = wrench.DistanceFrom == DistanceFrom.FromFirstEdge;
                    bool centered = wrench.DistanceFrom == DistanceFrom.Centered;
                    switch (wrench.DistanceFrom)
                    {
                        case DistanceFrom.FromFirstEdge:
                            startPointX = wrench.Distance + wrench.LinkedSection.SecondLine.StartPoint.X;
                            break;
                        case DistanceFrom.FromSecondEdge:
                            startPointX = wrench.LinkedSection.Length - wrench.Distance +
                                          wrench.LinkedSection.SecondLine.StartPoint.X;
                            break;
                        case DistanceFrom.Centered:
                            startPointX = wrench.LinkedSection.Length / 2 + wrench.Distance +
                                          wrench.LinkedSection.SecondLine.StartPoint.X;
                            break;
                    }

                    var sketchLine1 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(startPointX.InMillimeters(),
                            cylinderSection.Diameter.InMillimeters() / -2),
                        TransientGeometry.CreatePoint2d(startPointX.InMillimeters(),
                            cylinderSection.Diameter.InMillimeters() / 2));
                    var sketchLine2 = lines.AddByTwoPoints(sketchLine1.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? sketchLine1.EndSketchPoint.Geometry.X + wrench.WrenchLength.InMillimeters()
                                : centered
                                    ? sketchLine1.EndSketchPoint.Geometry.X + wrench.WrenchLength.InMillimeters()
                                    : sketchLine1.EndSketchPoint.Geometry.X - wrench.WrenchLength.InMillimeters(),
                            sketchLine1.EndSketchPoint.Geometry.Y));
                    var sketchLine3 = lines.AddByTwoPoints(sketchLine2.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(sketchLine2.EndSketchPoint.Geometry.X,
                            sketchLine1.StartSketchPoint.Geometry.Y));
                    var sketchLine4 = lines.AddByTwoPoints(sketchLine3.EndSketchPoint.Geometry,
                        sketchLine1.StartSketchPoint.Geometry);

                    sketchLine1.EndSketchPoint.Merge(sketchLine2.StartSketchPoint);

                    sketchLine2.EndSketchPoint.Merge(sketchLine3.StartSketchPoint);

                    sketchLine3.EndSketchPoint.Merge(sketchLine4.StartSketchPoint);

                    sketchLine4.EndSketchPoint.Merge(sketchLine1.StartSketchPoint);
                    var profile = wrenchSketch.Profiles.AddForSolid();
                    var extrudeDistance = (cylinderSection.Diameter - wrench.WidthAcrossFlats) / 2;
                    var extrudeWrench = compDef.Features.ExtrudeFeatures.AddByDistanceExtent(profile,
                        extrudeDistance.InMillimeters(),
                        PartFeatureExtentDirectionEnum.kNegativeExtentDirection,
                        PartFeatureOperationEnum.kCutOperation);
                    extrudeWrench.Name = $"WrenchOpening_#{wrenchNumber}";

                    var featureColl = InvApp.TransientObjects.CreateObjectCollection();
                    featureColl.Add(extrudeWrench);
                    var definition = compDef.Features.MirrorFeatures.CreateDefinition(featureColl, helperPlane,
                        PatternComputeTypeEnum.kIdenticalCompute);
                    compDef.Features.MirrorFeatures.AddByDefinition(definition);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(wrench));
                }
            }
        }

        private static void BuildRetainingRings(PartComponentDefinition compDef, bool boreSections = false,
            BoreFromEdge? boreFromEdge = null)
        {
            var sections = boreSections
                ? (boreFromEdge == BoreFromEdge.FromLeft ? Shaft.BoreOnTheLeft : Shaft.BoreOnTheRight)
                : Shaft.Sections;

            var allRetainingRings = new List<RetainingRingGrooveSubFeature>();

            foreach (var section in sections)
            {
                allRetainingRings.AddRange(section.SubFeatures.OfType<RetainingRingGrooveSubFeature>());
            }

            int retainingRingNumber = 0;

            foreach (var retainingRing in allRetainingRings)
            {
                try
                {
                    retainingRingNumber++;

                    var retainingRingSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    retainingRingSketch.Visible = false;
                    retainingRingSketch.AxisEntity = compDef.WorkAxes[1];

                    var lines = retainingRingSketch.SketchLines;
                    var cylinderSection = (CylinderSection)retainingRing.LinkedSection;
                    float startPointX = 0;

                    bool firstEdge = retainingRing.DistanceFrom == DistanceFrom.FromFirstEdge;
                    bool centered =
                        retainingRing.DistanceFrom == DistanceFrom.Centered;
                    switch (retainingRing.DistanceFrom)
                    {
                        case DistanceFrom.FromFirstEdge:
                            startPointX = retainingRing.Distance + retainingRing.LinkedSection.SecondLine.StartPoint.X;
                            break;
                        case DistanceFrom.FromSecondEdge:
                            startPointX = retainingRing.LinkedSection.Length - retainingRing.Distance +
                                          retainingRing.LinkedSection.SecondLine.StartPoint.X;
                            break;
                        case DistanceFrom.Centered:
                            startPointX = retainingRing.LinkedSection.Length / 2 + retainingRing.Distance +
                                          retainingRing.LinkedSection.SecondLine.StartPoint.X;
                            break;
                    }

                    var line1 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(startPointX.InMillimeters(),
                            boreSections
                                ? retainingRing.Diameter1.InMillimeters() / 2
                                : cylinderSection.Diameter.InMillimeters() / 2),
                        TransientGeometry.CreatePoint2d(
                            firstEdge ? (startPointX + retainingRing.Width).InMillimeters() :
                            centered ? (startPointX + retainingRing.Width).InMillimeters() :
                            (startPointX - retainingRing.Width).InMillimeters(),
                            boreSections
                                ? retainingRing.Diameter1.InMillimeters() / 2
                                : cylinderSection.Diameter.InMillimeters() / 2));
                    var line2 = lines.AddByTwoPoints(line1.EndSketchPoint.Geometry,
                        TransientGeometry.CreatePoint2d(line1.EndSketchPoint.Geometry.X,
                            boreSections
                                ? 0
                                : line1.EndSketchPoint.Geometry.Y -
                                  (cylinderSection.Diameter - retainingRing.Diameter1).InMillimeters() / 2));

                    var line3 = lines.AddByTwoPoints(line2.EndSketchPoint.Geometry, TransientGeometry.CreatePoint2d(
                        line1.StartSketchPoint.Geometry.X, boreSections
                            ? 0
                            : line1.StartSketchPoint.Geometry.Y -
                              (cylinderSection.Diameter - retainingRing.Diameter1)
                              .InMillimeters() / 2));

                    var line4 = lines.AddByTwoPoints(line3.EndSketchPoint.Geometry, line1.StartSketchPoint.Geometry);


                    line1.EndSketchPoint.Merge(line2.StartSketchPoint);

                    line2.EndSketchPoint.Merge(line3.StartSketchPoint);

                    line3.EndSketchPoint.Merge(line4.StartSketchPoint);

                    line4.EndSketchPoint.Merge(line1.StartSketchPoint);

                    var profile = retainingRingSketch.Profiles.AddForSolid();

                    var revolveRetainingRingFeature =
                        compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                            PartFeatureOperationEnum.kCutOperation);

                    revolveRetainingRingFeature.Name = boreSections
                        ? $"RetainingRing_#{retainingRingNumber}"
                        : $"RetainingRingGroove_#{retainingRingNumber}";
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(retainingRing));
                }
            }
        }

        private static void BuildKeywayGrooves(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allKeywayGrooves = new List<KeywayGrooveSubFeature>();
            foreach (var section in sections)
            {
                allKeywayGrooves.AddRange(section.SubFeatures.OfType<KeywayGrooveSubFeature>());
            }

            int keywayGrooveNumber = 0;
            foreach (var keywayGroove in allKeywayGrooves)

            {
                try
                {
                    keywayGrooveNumber++;
                    var cylinderSection = (CylinderSection)keywayGroove.LinkedSection;

                    WorkPlane keywayGrooveWorkPlane = null;
                    if (keywayGroove.Angle > 0 || keywayGroove.Angle < 0)
                    {
                        var helperWorkPlane =
                            compDef.WorkPlanes.AddByLinePlaneAndAngle(compDef.WorkAxes[1], compDef.WorkPlanes[3],
                                -keywayGroove.Angle + "deg");
                        helperWorkPlane.Visible = false;
                        keywayGrooveWorkPlane = compDef.WorkPlanes.AddByPlaneAndOffset(helperWorkPlane,
                            (cylinderSection.Diameter / 2).InMillimeters());
                    }
                    else
                    {
                        keywayGrooveWorkPlane = compDef.WorkPlanes.AddByPlaneAndOffset(compDef.WorkPlanes[3],
                            (cylinderSection.Diameter / 2).InMillimeters());
                    }


                    keywayGrooveWorkPlane.Visible = false;


                    var keywayGrooveSketch = compDef.Sketches.Add(keywayGrooveWorkPlane);
                    keywayGrooveSketch.Visible = false;
                    keywayGrooveSketch.AxisEntity = compDef.WorkAxes[1];
                    var arcs = keywayGrooveSketch.SketchArcs;
                    float startPointX = 0;
                    bool firstEdge = keywayGroove.DistanceFrom == DistanceFrom.FromFirstEdge;
                    bool centered = keywayGroove.DistanceFrom == DistanceFrom.Centered;

                    switch (keywayGroove.DistanceFrom)
                    {
                        case DistanceFrom.FromFirstEdge:
                            startPointX = keywayGroove.Distance + keywayGroove.LinkedSection.SecondLine.StartPoint.X;
                            break;
                        case DistanceFrom.FromSecondEdge:
                            startPointX = keywayGroove.LinkedSection.Length - keywayGroove.Distance +
                                          keywayGroove.LinkedSection.SecondLine.StartPoint.X;
                            break;

                        case DistanceFrom.Centered:
                            startPointX = keywayGroove.LinkedSection.Length / 2 + keywayGroove.Distance +
                                          keywayGroove.LinkedSection.SecondLine.StartPoint.X;

                            break;
                    }

                    var lines = keywayGrooveSketch.SketchLines;

                    var line1 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(
                            firstEdge
                                ? (startPointX + keywayGroove.Width / 2).InMillimeters()
                                : centered
                                    ? (startPointX + keywayGroove.Width / 2 - keywayGroove.KeywayLength / 2)
                                    .InMillimeters()
                                    : (startPointX - keywayGroove.Width / 2).InMillimeters(),
                            (-keywayGroove.Width / 2).InMillimeters()),
                        TransientGeometry.CreatePoint2d(firstEdge
                                ? (startPointX + keywayGroove.KeywayLength - keywayGroove.Width / 2).InMillimeters()
                                : centered
                                    ? (startPointX + keywayGroove.KeywayLength / 2 - keywayGroove.Width / 2)
                                    .InMillimeters()
                                    : (startPointX - keywayGroove.KeywayLength + keywayGroove.Width / 2)
                                    .InMillimeters(),
                            -keywayGroove.Width.InMillimeters() / 2));

                    var line2 = lines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(line1.Geometry.StartPoint.X,
                            keywayGroove.Width.InMillimeters() / 2), TransientGeometry.CreatePoint2d(
                            line1.EndSketchPoint.Geometry.X,
                            keywayGroove.Width.InMillimeters() / 2));
                    var arc1 = arcs.AddByCenterStartEndPoint(
                        TransientGeometry.CreatePoint2d(line2.StartSketchPoint.Geometry.X, 0),
                        firstEdge ? line2.StartSketchPoint.Geometry :
                        centered ? line2.StartSketchPoint.Geometry : line1.StartSketchPoint.Geometry,
                        firstEdge ? line1.StartSketchPoint.Geometry :
                        centered ? line1.StartSketchPoint.Geometry : line2.StartSketchPoint.Geometry);
                    var arc2 = arcs.AddByCenterStartEndPoint(
                        TransientGeometry.CreatePoint2d(line2.EndSketchPoint.Geometry.X, 0),
                        firstEdge ? line1.EndSketchPoint.Geometry :
                        centered ? line1.EndSketchPoint.Geometry : line2.EndSketchPoint.Geometry,
                        firstEdge ? line2.EndSketchPoint.Geometry :
                        centered ? line2.EndSketchPoint.Geometry : line1.EndSketchPoint.Geometry);
                    arc1.StartSketchPoint.Merge(firstEdge ? line2.StartSketchPoint :
                        centered ? line2.StartSketchPoint : line1.StartSketchPoint);
                    arc1.EndSketchPoint.Merge(firstEdge ? line1.StartSketchPoint :
                        centered ? line1.StartSketchPoint : line2.StartSketchPoint);
                    arc2.StartSketchPoint.Merge(firstEdge ? line1.EndSketchPoint :
                        centered ? line1.EndSketchPoint : line2.EndSketchPoint);
                    arc2.EndSketchPoint.Merge(firstEdge ? line2.EndSketchPoint :
                        centered ? line2.EndSketchPoint : line1.EndSketchPoint);
                    Profile profile;
                    try
                    {
                        profile = keywayGrooveSketch.Profiles.AddForSolid();
                    }
                    catch
                    {
                        var intersectionPoints = TransientGeometry.CurveCurveIntersection(
                            TransientGeometry.CreateArc3d(arc1.Geometry3d.Center, arc1.Geometry3d.Normal,
                                arc1.Geometry3d.ReferenceVector, arc1.Radius, arc1.StartAngle, arc1.SweepAngle),
                            TransientGeometry.CreateArc3d(arc2.Geometry3d.Center, arc2.Geometry3d.Normal,
                                arc2.Geometry3d.ReferenceVector, arc2.Radius, arc2.StartAngle,
                                arc2.SweepAngle));
                        Point point1 = intersectionPoints[1] as Point;
                        Point point2 = intersectionPoints[2] as Point;
                        var sketchPoint1 =
                            keywayGrooveSketch.SketchPoints.Add(TransientGeometry.CreatePoint2d(point1.X, point1.Y));
                        var sketchPoint2 =
                            keywayGrooveSketch.SketchPoints.Add(TransientGeometry.CreatePoint2d(point2.X, point2.Y));
                        keywayGrooveSketch.GeometricConstraints.AddGround((SketchEntity)sketchPoint1);
                        keywayGrooveSketch.GeometricConstraints.AddGround((SketchEntity)sketchPoint2);
                        arc1.EndSketchPoint.Merge(sketchPoint1);
                        arc2.StartSketchPoint.Merge(sketchPoint1);

                        arc1.StartSketchPoint.Merge(sketchPoint2);
                        arc2.EndSketchPoint.Merge(sketchPoint2);
                        profile = keywayGrooveSketch.Profiles.AddForSolid();
                    }


                    var feature = compDef.Features.ExtrudeFeatures.AddByDistanceExtent(profile,
                        keywayGroove.Depth.InMillimeters(),
                        PartFeatureExtentDirectionEnum.kNegativeExtentDirection,
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"KeywayGroove_#{keywayGrooveNumber}";

                    if (keywayGroove.NumberOfKeys == 1)
                    {
                        continue;
                    }

                    var featuresColl = InvApp.TransientObjects.CreateObjectCollection();
                    featuresColl.Add(feature);
                    var definition =
                        compDef.Features.CircularPatternFeatures.CreateDefinition(
                            ParentFeatures: featuresColl,
                            AxisEntity: compDef.WorkAxes[1],
                            NaturalAxisDirection: true,
                            Count: keywayGroove.NumberOfKeys,
                            Angle: 0);
                    var circularPatternFeature =
                        compDef.Features.CircularPatternFeatures.AddByDefinition(definition);
                    circularPatternFeature.Angle._Value = keywayGroove.NumberOfKeys == 2
                        ? MathExtensions.DegreesToRadians(keywayGroove.AngleBetweenKeys)
                        : MathExtensions.DegreesToRadians(360);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(keywayGroove));
                }
            }
        }

        private static void BuildGroovesA(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allGroovesA = new List<GrooveASubFeature>();
            foreach (var section in sections)
            {
                allGroovesA.AddRange(section.SubFeatures.OfType<GrooveASubFeature>());
            }


            int grooveANumber = 0;
            foreach (var grooveA in allGroovesA)
            {
                try
                {
                    grooveANumber++;

                    var grooveASketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    grooveASketch.Visible = false;
                    grooveASketch.AxisEntity = compDef.WorkAxes[1];
                    var cylinderSection = (CylinderSection)grooveA.LinkedSection;

                    float startPointX = 0;
                    switch (grooveA.DistanceFrom)
                    {
                        case DistanceFrom.FromFirstEdge:
                            startPointX = grooveA.Distance + grooveA.LinkedSection.SecondLine.StartPoint.X;
                            break;
                        case DistanceFrom.FromSecondEdge:
                            startPointX = grooveA.LinkedSection.Length - grooveA.Distance +
                                          grooveA.LinkedSection.SecondLine.StartPoint.X;
                            break;
                        case DistanceFrom.Centered:
                            startPointX = grooveA.LinkedSection.Length / 2 + grooveA.Distance +
                                          grooveA.LinkedSection.SecondLine.StartPoint.X;
                            break;
                    }

                    var circleCenterPoint = TransientGeometry.CreatePoint2d(startPointX.InMillimeters(),
                        (cylinderSection.Diameter / 2 + grooveA.Radius - grooveA.Depth).InMillimeters());
                    grooveASketch.SketchCircles.AddByCenterRadius(circleCenterPoint, grooveA.Radius.InMillimeters());
                    var profile = grooveASketch.Profiles.AddForSolid();
                    var feature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"GrooveA_#{grooveANumber}";
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(grooveA));
                }
            }
        }

        private static void BuildGroovesB(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allGroovesB = new List<GrooveBSubFeature>();
            foreach (var section in sections)
            {
                allGroovesB.AddRange(section.SubFeatures.OfType<GrooveBSubFeature>());
            }


            int grooveBNumber = 0;
            foreach (var grooveB in allGroovesB)
            {
                try
                {
                    grooveBNumber++;
                    var cylinderSection = (CylinderSection)grooveB.LinkedSection;


                    var helperPlane = compDef.WorkPlanes.AddByLinePlaneAndAngle(compDef.WorkAxes[1],
                        compDef.WorkPlanes[2],
                        -grooveB.Angle + "deg");
                    helperPlane.Visible = false;

                    var grooveBSketch =
                        compDef.Sketches.Add(helperPlane);
                    grooveBSketch.Visible = false;

                    float startPointX = 0;
                    switch (grooveB.DistanceFrom)
                    {
                        case DistanceFrom.FromFirstEdge:
                            startPointX = grooveB.Distance + grooveB.LinkedSection.SecondLine.StartPoint.X;
                            break;
                        case DistanceFrom.FromSecondEdge:
                            startPointX = grooveB.LinkedSection.Length - grooveB.Distance +
                                          grooveB.LinkedSection.SecondLine.StartPoint.X;
                            break;

                        case DistanceFrom.Centered:
                            startPointX = grooveB.LinkedSection.Length / 2 + grooveB.Distance +
                                          grooveB.LinkedSection.SecondLine.StartPoint.X;
                            break;
                    }

                    var circleCenterPoint = TransientGeometry.CreatePoint2d(startPointX.InMillimeters(),
                        (cylinderSection.Diameter / 2 + grooveB.Radius - grooveB.Depth).InMillimeters());
                    grooveBSketch.SketchCircles.AddByCenterRadius(circleCenterPoint, grooveB.Radius.InMillimeters());
                    var profile = grooveBSketch.Profiles.AddForSolid();
                    var feature = compDef.Features.ExtrudeFeatures.AddByThroughAllExtent(profile,
                        PartFeatureExtentDirectionEnum.kSymmetricExtentDirection,
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"GrooveB_#{grooveBNumber}";
                    var objColl = InvApp.TransientObjects.CreateObjectCollection();
                    objColl.Add(feature);
                    var mirrorPlane = compDef.WorkPlanes.AddByLinePlaneAndAngle(compDef.WorkAxes[1],
                        compDef.WorkPlanes[3],
                        -grooveB.Angle + "deg");
                    mirrorPlane.Visible = false;
                    compDef.Features.MirrorFeatures.Add(objColl, mirrorPlane,
                        ComputeType: PatternComputeTypeEnum.kIdenticalCompute);
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(grooveB));
                }
            }
        }

        private static void BuildReliefsDSI(PartComponentDefinition compDef)
        {
            var sections = Shaft.Sections;
            var allReliefsDSI = new List<ReliefDSISubFeature>();

            foreach (var section in sections)
            {
                allReliefsDSI.AddRange(section.SubFeatures.OfType<ReliefDSISubFeature>());
            }

            int reliefDSINumber = 0;

            foreach (var reliefDSI in allReliefsDSI)
            {
                try
                {
                    reliefDSINumber++;
                    var cylinderSection = (CylinderSection)reliefDSI.LinkedSection;
                    var reliefDSISketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
                    reliefDSISketch.Visible = false;
                    var splines = reliefDSISketch.SketchSplines;
                    var lines = reliefDSISketch.SketchLines;
                    float startPointX = 0;
                    switch (reliefDSI.DistanceFrom)
                    {
                        case DistanceFrom.FromFirstEdge:
                            startPointX = reliefDSI.Distance + reliefDSI.LinkedSection.SecondLine.StartPoint.X;
                            break;

                        case DistanceFrom.FromSecondEdge:
                            startPointX = reliefDSI.LinkedSection.Length - reliefDSI.Distance +
                                          reliefDSI.LinkedSection.SecondLine.StartPoint.X;
                            break;

                        case DistanceFrom.Centered:
                            startPointX = reliefDSI.LinkedSection.Length / 2 + reliefDSI.Distance +
                                          reliefDSI.LinkedSection.SecondLine.StartPoint.X;
                            break;
                    }

                    var pointCenter = TransientGeometry.CreatePoint2d(startPointX.InMillimeters(),
                        (cylinderSection.Diameter / 2 - reliefDSI.ReliefDepth - reliefDSI.MachiningAllowance)
                        .InMillimeters());
                    var leftPoint = TransientGeometry.CreatePoint2d((startPointX - reliefDSI.Width / 2).InMillimeters(),
                        cylinderSection.Diameter.InMillimeters() / 2);
                    var rightPoint = TransientGeometry.CreatePoint2d(leftPoint.X + reliefDSI.Width.InMillimeters(),
                        cylinderSection.Diameter.InMillimeters() / 2);
                    var pointsColl = InvApp.TransientObjects.CreateObjectCollection();
                    pointsColl.Add(leftPoint);
                    pointsColl.Add(pointCenter);
                    pointsColl.Add(rightPoint);
                    var spline = splines.Add(pointsColl, SplineFitMethodEnum.kSmoothSplineFit);
                    var line = lines.AddByTwoPoints(spline.StartSketchPoint.Geometry, spline.EndSketchPoint.Geometry);
                    line.StartSketchPoint.Merge(spline.StartSketchPoint);
                    line.EndSketchPoint.Merge(spline.EndSketchPoint);
                    var profile = reliefDSISketch.Profiles.AddForSolid();
                    var feature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                        PartFeatureOperationEnum.kCutOperation);
                    feature.Name = $"ReliefDSi_#{reliefDSINumber}";
                }
                catch (Exception)
                {
                    ConstructionErrors.Add(new FeatureConstructionError(reliefDSI));
                }
            }
        }


        private static void BuildLeftBore(PartComponentDefinition compDef)
        {
            var sections = Shaft.BoreOnTheLeft;


            if (sections.Count == 0)
            {
                return;
            }

            PlanarSketch mainSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
            mainSketch.NaturalAxisDirection = true;
            var sketchLines = mainSketch.SketchLines;
            var axisParams = Shaft.BoreOnTheLeftRevolveAxis;
            sketchLines.AddByTwoPoints(
                TransientGeometry.CreatePoint2d(axisParams.StartPoint.X.InMillimeters(),
                    axisParams.StartPoint.Y.InMillimeters()),
                TransientGeometry.CreatePoint2d(axisParams.EndPoint.X.InMillimeters(),
                    axisParams.EndPoint.Y.InMillimeters()));


            for (var index = 0; index < sections.Count; index++)
            {
                var section = sections[index];

                if (section.FirstLine != null)

                {
                    var firstLineParams = section.FirstLine;
                    Point2d firstPoint = TransientGeometry.CreatePoint2d(firstLineParams.StartPoint.X.InMillimeters(),
                        firstLineParams.StartPoint.Y.InMillimeters());
                    Point2d secondPoint =
                        TransientGeometry.CreatePoint2d(firstLineParams.EndPoint.X.InMillimeters(),
                            firstLineParams.EndPoint.Y.InMillimeters());
                    sketchLines.AddByTwoPoints(firstPoint, secondPoint);
                }

                var secondLineParams = section.SecondLine;

                if (Shaft.BoreOnTheLeft.IndexOf(section) != 0)

                {
                    var previousSectionSecondLine = sections[index - 1].SecondLine;
                    if (previousSectionSecondLine.EndPoint != secondLineParams.StartPoint)
                    {
                        sketchLines.AddByTwoPoints(
                            TransientGeometry.CreatePoint2d(previousSectionSecondLine.EndPoint.X.InMillimeters(),
                                previousSectionSecondLine.EndPoint.Y.InMillimeters()),
                            TransientGeometry.CreatePoint2d(secondLineParams.StartPoint.X.InMillimeters(),
                                secondLineParams.StartPoint.Y.InMillimeters()));
                    }
                }

                sketchLines.AddByTwoPoints(
                    TransientGeometry.CreatePoint2d(secondLineParams.StartPoint.X.InMillimeters(),
                        secondLineParams.StartPoint.Y.InMillimeters()),
                    TransientGeometry.CreatePoint2d(secondLineParams.EndPoint.X.InMillimeters(),
                        secondLineParams.EndPoint.Y.InMillimeters()));

                if (section.ThirdLine != null)

                {
                    var thirdLineParams = section.ThirdLine;
                    sketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(thirdLineParams.StartPoint.X.InMillimeters(),
                            thirdLineParams.StartPoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(thirdLineParams.EndPoint.X.InMillimeters(),
                            thirdLineParams.EndPoint.Y.InMillimeters()));
                }
            }

            sketchLines[1].StartSketchPoint.Merge(sketchLines[2].StartSketchPoint);

            for (int i = 2; i < sketchLines.Count; i++)
            {
                sketchLines[i].EndSketchPoint.Merge(sketchLines[i + 1].StartSketchPoint);
            }

            sketchLines[1].EndSketchPoint.Merge(sketchLines[sketchLines.Count].EndSketchPoint);
            mainSketch.Color = InvApp.TransientObjects.CreateColor(102, 158, 97);
            var profile = mainSketch.Profiles.AddForSolid();
            var revolveFeature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                PartFeatureOperationEnum.kCutOperation);
            revolveFeature.Name = "BoreOnTheLeft";
        }


        private static void BuildRightBore(PartComponentDefinition compDef)
        {
            var sections = Shaft.BoreOnTheRight;


            if (sections.Count == 0)
            {
                return;
            }

            PlanarSketch mainSketch = compDef.Sketches.Add(compDef.WorkPlanes[3]);
            mainSketch.NaturalAxisDirection = true;
            var sketchLines = mainSketch.SketchLines;
            var axisParams = Shaft.BoreOnTheRightRevolveAxis;
            var shaftLength = Shaft.Sections.Sum(section => section.Length);
            sketchLines.AddByTwoPoints(
                TransientGeometry.CreatePoint2d(shaftLength.InMillimeters() - axisParams.StartPoint.X.InMillimeters(),
                    axisParams.StartPoint.Y.InMillimeters()),
                TransientGeometry.CreatePoint2d(shaftLength.InMillimeters() - axisParams.EndPoint.X.InMillimeters(),
                    axisParams.EndPoint.Y.InMillimeters()));


            for (var index = 0; index < sections.Count; index++)
            {
                var section = sections[index];

                if (section.FirstLine != null)

                {
                    var firstLineParams = section.FirstLine;
                    Point2d firstPoint = TransientGeometry.CreatePoint2d(
                        shaftLength.InMillimeters() - firstLineParams.StartPoint.X.InMillimeters(),
                        firstLineParams.StartPoint.Y.InMillimeters());
                    Point2d secondPoint =
                        TransientGeometry.CreatePoint2d(
                            shaftLength.InMillimeters() - firstLineParams.EndPoint.X.InMillimeters(),
                            firstLineParams.EndPoint.Y.InMillimeters());
                    sketchLines.AddByTwoPoints(firstPoint, secondPoint);
                }

                var secondLineParams = section.SecondLine;

                if (Shaft.BoreOnTheRight.IndexOf(section) != 0)

                {
                    var previousSectionSecondLine = sections[index - 1].SecondLine;
                    if (previousSectionSecondLine.EndPoint != secondLineParams.StartPoint)
                    {
                        sketchLines.AddByTwoPoints(
                            TransientGeometry.CreatePoint2d(
                                shaftLength.InMillimeters() - previousSectionSecondLine.EndPoint.X.InMillimeters(),
                                previousSectionSecondLine.EndPoint.Y.InMillimeters()),
                            TransientGeometry.CreatePoint2d(
                                shaftLength.InMillimeters() - secondLineParams.StartPoint.X.InMillimeters(),
                                secondLineParams.StartPoint.Y.InMillimeters()));
                    }
                }

                sketchLines.AddByTwoPoints(
                    TransientGeometry.CreatePoint2d(
                        shaftLength.InMillimeters() - secondLineParams.StartPoint.X.InMillimeters(),
                        secondLineParams.StartPoint.Y.InMillimeters()),
                    TransientGeometry.CreatePoint2d(
                        shaftLength.InMillimeters() - secondLineParams.EndPoint.X.InMillimeters(),
                        secondLineParams.EndPoint.Y.InMillimeters()));
                if (section.ThirdLine != null)
                {
                    var thirdLineParams = section.ThirdLine;
                    sketchLines.AddByTwoPoints(
                        TransientGeometry.CreatePoint2d(
                            shaftLength.InMillimeters() - thirdLineParams.StartPoint.X.InMillimeters(),
                            thirdLineParams.StartPoint.Y.InMillimeters()),
                        TransientGeometry.CreatePoint2d(
                            shaftLength.InMillimeters() - thirdLineParams.EndPoint.X.InMillimeters(),
                            thirdLineParams.EndPoint.Y.InMillimeters()));
                }
            }

            sketchLines[1].StartSketchPoint.Merge(sketchLines[2].StartSketchPoint);
            for (int i = 2; i < sketchLines.Count; i++)
            {
                sketchLines[i].EndSketchPoint.Merge(sketchLines[i + 1].StartSketchPoint);
            }

            sketchLines[1].EndSketchPoint.Merge(sketchLines[sketchLines.Count].EndSketchPoint);
            mainSketch.Color = InvApp.TransientObjects.CreateColor(102, 158, 97);
            var profile = mainSketch.Profiles.AddForSolid();
            var revolveFeature = compDef.Features.RevolveFeatures.AddFull(profile, compDef.WorkAxes[1],
                PartFeatureOperationEnum.kCutOperation);
            revolveFeature.Name = "BoreOnTheRight";
        }
    }
}