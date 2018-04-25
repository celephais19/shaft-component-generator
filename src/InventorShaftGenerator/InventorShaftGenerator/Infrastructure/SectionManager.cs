using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Infrastructure
{
    public sealed class SectionManager<TSection>
        where TSection : ShaftSection, new()
    {
        static SectionManager()
        {
            Settings.SettingsChanged += OnSettingsChanged;
            Shaft.Sections.CollectionChanged += SectionsOnCollectionChanged;
            Shaft.BoreOnTheLeft.CollectionChanged += SectionsOnCollectionChanged;
            Shaft.BoreOnTheRight.CollectionChanged += SectionsOnCollectionChanged;
        }

        private static void SectionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var sections = ((ObservableCollectionEx<ShaftSection>) sender).ToList();
            bool isBore = false;
            BoreFromEdge? boreFromEdge = null;

            if (sections.Count == 0)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newSection = (ShaftSection) e.NewItems[0];
                    isBore = newSection.IsBore;
                    boreFromEdge = newSection.BoreFromEdge;
                    ResolveSketchLines(resolveAsStandardAppendage: true, isBoreSection: isBore,
                        borePosition: boreFromEdge);

                    break;

                case NotifyCollectionChangedAction.Remove:
                    var removedSection = (ShaftSection) e.OldItems[0];
                    isBore = removedSection.IsBore;
                    if (removedSection.BoreFromEdge != null)
                    {
                        boreFromEdge = removedSection.BoreFromEdge.Value;
                    }

                    var sectionBeforeRemoved =
                        sections.SingleOrDefault(section => section.NextSection?.Id == removedSection.Id);
                    var sectionAfterRemoved =
                        sections.SingleOrDefault(section => section.PreviousSection?.Id == removedSection.Id);

                    if (sectionAfterRemoved != null)
                    {
                        sectionAfterRemoved.PreviousSection = sectionBeforeRemoved;

                        if (sectionBeforeRemoved != null)
                        {
                            sectionBeforeRemoved.NextSection = sectionAfterRemoved;
                        }
                    }
                    else if (sectionBeforeRemoved != null)
                    {
                        sectionBeforeRemoved.NextSection = null;
                    }

                    break;
            }


            ResolveSketchLines(isBoreSection: isBore, borePosition: boreFromEdge);
            ResolveEdgeFeatures(isBoreSection: isBore, borePosition: boreFromEdge);
        }

        private static void ResolveSketchLines(bool resolveAsStandardAppendage = false, bool isBoreSection = false,
                                               BoreFromEdge? borePosition = null)
        {
            ShaftSection previousSection = null;

            ObservableCollectionEx<ShaftSection> items;
            if (isBoreSection)
            {
                items = borePosition == BoreFromEdge.FromLeft ? Shaft.BoreOnTheLeft : Shaft.BoreOnTheRight;
            }
            else
            {
                items = Shaft.Sections;
            }

            foreach (var section in items)
            {
                if (section.IsFirst)
                {
                    switch (section)
                    {
                        case CylinderSection cylinderSection:
                            cylinderSection.FirstLine = new SketchLineSimple(
                                startPoint: PointF.Empty,
                                endPoint: new PointF(0, cylinderSection.Diameter / 2));

                            break;

                        case ConeSection coneSection:
                            coneSection.FirstLine = new SketchLineSimple(
                                startPoint: PointF.Empty,
                                endPoint: new PointF(0, coneSection.Diameter1 / 2));

                            break;

                        case PolygonSection polygonSection:
                            polygonSection.FirstLine = new SketchLineSimple(
                                startPoint: PointF.Empty,
                                endPoint: new PointF(0, polygonSection.InscribedCircleDiameter / 2));
                            break;
                    }
                }

                switch (section)
                {
                    case CylinderSection cylinderSection:
                        cylinderSection.SecondLine = new SketchLineSimple(
                            startPoint: cylinderSection.FirstLine?.EndPoint ?? (resolveAsStandardAppendage
                                            ? previousSection.SecondLine.EndPoint
                                            : new PointF(previousSection.SecondLine.EndPoint.X,
                                                cylinderSection.Diameter / 2)),
                            endPoint: new PointF(
                                cylinderSection.FirstLine?.EndPoint.X + cylinderSection.Length ??
                                previousSection.SecondLine.EndPoint.X + cylinderSection.Length,
                                cylinderSection.FirstLine?.EndPoint.Y ?? (resolveAsStandardAppendage
                                    ? previousSection.SecondLine.EndPoint.Y
                                    : cylinderSection.Diameter / 2)));

                        break;

                    case ConeSection coneSection:
                        coneSection.SecondLine = new SketchLineSimple(
                            startPoint: coneSection.FirstLine?.EndPoint ?? (resolveAsStandardAppendage
                                            ? previousSection.SecondLine.EndPoint
                                            : new PointF(previousSection.SecondLine.EndPoint.X,
                                                coneSection.Diameter1 / 2)),
                            endPoint: new PointF(coneSection.FirstLine?.EndPoint.X + coneSection.Length ??
                                                 previousSection.SecondLine.EndPoint.X + coneSection.Length,
                                coneSection.Diameter2 / 2));

                        break;

                    case PolygonSection polygonSection:
                        polygonSection.SecondLine = new SketchLineSimple(
                            startPoint: polygonSection.FirstLine?.EndPoint ?? (resolveAsStandardAppendage
                                            ? previousSection.SecondLine.EndPoint
                                            : new PointF(previousSection.SecondLine.EndPoint.X,
                                                polygonSection.InscribedCircleDiameter / 2)),
                            endPoint: new PointF(
                                polygonSection.FirstLine?.EndPoint.X + polygonSection.Length ??
                                previousSection.SecondLine.EndPoint.X + polygonSection.Length,
                                polygonSection.FirstLine?.EndPoint.Y ??
                                polygonSection.InscribedCircleDiameter / 2));
                        break;
                }

                if (section.IsLast)
                {
                    switch (section)
                    {
                        case CylinderSection cylinderSection:
                            cylinderSection.ThirdLine = new SketchLineSimple(
                                startPoint: new PointF(
                                    previousSection?.SecondLine.EndPoint.X + cylinderSection.Length ??
                                    section.Length,
                                    cylinderSection.Diameter / 2),
                                endPoint: new PointF(
                                    previousSection?.SecondLine.EndPoint.X + cylinderSection.Length ??
                                    cylinderSection.Length,
                                    0));

                            break;

                        case ConeSection coneSection:
                            coneSection.ThirdLine = new SketchLineSimple(
                                startPoint: new PointF(
                                    previousSection?.SecondLine.EndPoint.X + coneSection.Length ?? section.Length,
                                    coneSection.Diameter2 / 2),
                                endPoint: new PointF(
                                    previousSection?.SecondLine.EndPoint.X + coneSection.Length ??
                                    coneSection.Length,
                                    0));

                            break;

                        case PolygonSection polygonSection:
                            polygonSection.ThirdLine = new SketchLineSimple(
                                startPoint: new PointF(
                                    previousSection?.SecondLine.EndPoint.X + polygonSection.Length ?? section.Length,
                                    polygonSection.InscribedCircleDiameter / 2),
                                endPoint: new PointF(
                                    previousSection?.SecondLine.EndPoint.X + polygonSection.Length ??
                                    polygonSection.Length,
                                    0));
                            break;
                    }
                }

                previousSection = section;
                section.FirstEdgeFeature?.UpdateFeatureParameters();
                section.SecondEdgeFeature?.UpdateFeatureParameters();
            }
        }

        private static void ResolveEdgeFeatures(bool isBoreSection = false, BoreFromEdge? borePosition = null)
        {
            void SetIfEdgeFeatureWillBeBuilt(ShaftSection section)
            {
                /*if (section.AvailableFirstEdgeFeatures.First() == EdgeFeature.NotAvailable)
                {
                    section.FirstEdgeFeature.ShouldBeBuilt = false;
                }

                if (section.AvailableSecondEdgeFeatures.First() == EdgeFeature.NotAvailable)
                {
                    section.SecondEdgeFeature.ShouldBeBuilt = false;
                }*/

                if (section.FirstEdgeFeature != null)
                {
                    section.FirstEdgeFeature.ShouldBeBuilt = section.AvailableFirstEdgeFeatures.Any(
                        ef => ef == section
                                    .FirstEdgeFeature
                                    .ToEdgeFeatureEnumMember());
                }

                if (section.SecondEdgeFeature != null)
                {
                    section.SecondEdgeFeature.ShouldBeBuilt = section.AvailableSecondEdgeFeatures.Any(
                        ef => ef == section
                                    .SecondEdgeFeature
                                    .ToEdgeFeatureEnumMember());
                }
            }

            ObservableCollectionEx<ShaftSection> items;
            if (isBoreSection)
            {
                items = borePosition == BoreFromEdge.FromLeft ? Shaft.BoreOnTheLeft : Shaft.BoreOnTheRight;
            }
            else
            {
                items = Shaft.Sections;
            }

            foreach (var section in items)
            {
                float previousSectionDiameter;
                switch (section.PreviousSection)
                {
                    case CylinderSection cylinderSection:
                        previousSectionDiameter = cylinderSection.Diameter;
                        break;
                    case ConeSection coneSection:
                        previousSectionDiameter = coneSection.Diameter2;
                        break;

                    case PolygonSection polygonSection:
                        previousSectionDiameter = polygonSection.CircumscribedCircleDiameter;
                        break;

                    default:
                        previousSectionDiameter = 0;
                        break;
                }

                float nextSectionDiameter;
                switch (section.NextSection)
                {
                    case CylinderSection cylinderSection:
                        nextSectionDiameter = cylinderSection.Diameter;
                        break;

                    case ConeSection coneSection:
                        nextSectionDiameter = coneSection.Diameter1;
                        break;

                    case PolygonSection polygonSection:
                        nextSectionDiameter = polygonSection.CircumscribedCircleDiameter;
                        break;

                    default:
                        nextSectionDiameter = 0;
                        break;
                }

                switch (section)
                {
                    case CylinderSection cylinderSection:
                        if (cylinderSection.PreviousSection != null &&
                            previousSectionDiameter > cylinderSection.Diameter)
                        {
                            if (cylinderSection.IsBore)
                            {
                                cylinderSection.AvailableFirstEdgeFeatures = new[]
                                        {EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet, EdgeFeature.Thread}
                                    .ToObservableCollection();
                            }
                            else
                            {
                                cylinderSection.AvailableFirstEdgeFeatures = new[]
                                {
                                    EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet, EdgeFeature.ReliefSI,
                                    EdgeFeature.ReliefDIN, EdgeFeature.ReliefGOST
                                }.ToObservableCollection();
                            }
                        }
                        else if (cylinderSection.PreviousSection == null)
                        {
                            if (cylinderSection.IsBore)
                            {
                                cylinderSection.AvailableFirstEdgeFeatures = new[]
                                    {
                                        EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                        EdgeFeature.Thread
                                    }
                                    .ToObservableCollection();
                            }
                            else
                            {
                                cylinderSection.AvailableFirstEdgeFeatures = new[]
                                {
                                    EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                    EdgeFeature.LockNutGroove,
                                    EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                    EdgeFeature.KeywayGrooveRoundedEnd
                                }.ToObservableCollection();
                            }
                        }
                        else if (previousSectionDiameter < cylinderSection.Diameter)
                        {
                            /*cylinderSection.PropertyChanged += (sender, args) =>
                            {
                                if (args.PropertyName == nameof(ShaftSection.SecondEdgeFeature))
                                {
                                    if (((ShaftSection) sender).SecondEdgeFeature?.ToEdgeFeatureEnumMember() ==
                                        EdgeFeature.LockNutGroove)
                                    {
                                        cylinderSection.AvailableFirstEdgeFeatures = new[]
                                        {
                                            EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                            EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                            EdgeFeature.KeywayGrooveRoundedEnd
                                        }.ToObservableCollection();
                                    }
                                    else
                                    {
                                        cylinderSection.AvailableFirstEdgeFeatures = new[]
                                        {
                                            EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                            EdgeFeature.LockNutGroove,
                                            EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                            EdgeFeature.KeywayGrooveRoundedEnd
                                        }.ToObservableCollection();
                                    }
                                }
                            };*/

                            if (cylinderSection.SecondEdgeFeature?.ToEdgeFeatureEnumMember() ==
                                EdgeFeature.LockNutGroove)
                            {
                                cylinderSection.AvailableFirstEdgeFeatures = new[]
                                {
                                    EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                    EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                    EdgeFeature.KeywayGrooveRoundedEnd
                                }.ToObservableCollection();
                            }
                            else
                            {
                                if (cylinderSection.IsBore)
                                {
                                    cylinderSection.AvailableFirstEdgeFeatures = new[]
                                        {
                                            EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                            EdgeFeature.Thread
                                        }
                                        .ToObservableCollection();
                                }
                                else
                                {
                                    cylinderSection.AvailableFirstEdgeFeatures = new[]
                                    {
                                        EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                        EdgeFeature.LockNutGroove,
                                        EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                        EdgeFeature.KeywayGrooveRoundedEnd
                                    }.ToObservableCollection();
                                }
                            }
                        }
                        else if (cylinderSection.PreviousSection != null &&
                                 previousSectionDiameter.NearlyEqual(cylinderSection.Diameter))
                        {
                            if (cylinderSection.IsBore)
                            {
                                cylinderSection.AvailableFirstEdgeFeatures =
                                    new[] {EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet}
                                        .ToObservableCollection();
                            }
                            else
                            {
                                cylinderSection.AvailableFirstEdgeFeatures =
                                    new[] {EdgeFeature.NotAvailable}.ToObservableCollection();
                            }
                        }

                        if (cylinderSection.NextSection != null && nextSectionDiameter > cylinderSection.Diameter)
                        {
                            if (cylinderSection.IsBore)
                            {
                                cylinderSection.AvailableSecondEdgeFeatures = new[]
                                    {
                                        EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                        EdgeFeature.Thread
                                    }
                                    .ToObservableCollection();
                            }
                            else
                            {
                                cylinderSection.AvailableSecondEdgeFeatures = new[]
                                {
                                    EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet, EdgeFeature.ReliefSI,
                                    EdgeFeature.ReliefDIN, EdgeFeature.ReliefGOST
                                }.ToObservableCollection();
                            }
                        }
                        else if (nextSectionDiameter < cylinderSection.Diameter)
                        {
                            /*cylinderSection.PropertyChanged += (sender, args) =>
                            {
                                if (args.PropertyName == nameof(ShaftSection.FirstEdgeFeature))
                                {
                                    if (((ShaftSection) sender).FirstEdgeFeature?.ToEdgeFeatureEnumMember() ==
                                        EdgeFeature.LockNutGroove)
                                    {

                                        cylinderSection.AvailableSecondEdgeFeatures = new[]
                                        {
                                            EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                            EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                            EdgeFeature.KeywayGrooveRoundedEnd
                                        }.ToObservableCollection();
                                    }
                                    else
                                    {
                                        cylinderSection.AvailableSecondEdgeFeatures = new[]
                                        {
                                            EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                            EdgeFeature.LockNutGroove,
                                            EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                            EdgeFeature.KeywayGrooveRoundedEnd
                                        }.ToObservableCollection();
                                    }
                                }
                            }*/
                            ;

                            if (cylinderSection.FirstEdgeFeature?.ToEdgeFeatureEnumMember() ==
                                EdgeFeature.LockNutGroove)
                            {
                                cylinderSection.AvailableSecondEdgeFeatures = new[]
                                {
                                    EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                    EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                    EdgeFeature.KeywayGrooveRoundedEnd
                                }.ToObservableCollection();
                            }
                            else
                            {
                                if (cylinderSection.IsBore)
                                {
                                    cylinderSection.AvailableSecondEdgeFeatures = new[]
                                        {
                                            EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                            EdgeFeature.Thread
                                        }
                                        .ToObservableCollection();
                                }
                                else
                                {
                                    cylinderSection.AvailableSecondEdgeFeatures = new[]
                                    {
                                        EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet,
                                        EdgeFeature.LockNutGroove,
                                        EdgeFeature.Thread, EdgeFeature.PlainKeywayGroove,
                                        EdgeFeature.KeywayGrooveRoundedEnd
                                    }.ToObservableCollection();
                                }
                            }
                        }
                        else if (cylinderSection.NextSection != null &&
                                 nextSectionDiameter.NearlyEqual(cylinderSection.Diameter))
                        {
                            if (cylinderSection.IsBore && cylinderSection.NextSection is ConeSection)
                            {
                                cylinderSection.AvailableSecondEdgeFeatures = new[]
                                        {EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet}
                                    .ToObservableCollection();
                            }
                            else
                            {
                                cylinderSection.AvailableSecondEdgeFeatures =
                                    new[] {EdgeFeature.NotAvailable}.ToObservableCollection();
                            }
                        }

                        break;

                    case ConeSection coneSection:
                        if (coneSection.PreviousSection != null ||
                            previousSectionDiameter.NearlyEqual(coneSection.Diameter1))
                        {
                            if (coneSection.IsBore && coneSection.PreviousSection is CylinderSection)
                            {
                                coneSection.AvailableFirstEdgeFeatures = new[]
                                        {EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet}
                                    .ToObservableCollection();
                            }
                            else
                            {
                                coneSection.AvailableFirstEdgeFeatures = new[]
                                    {EdgeFeature.NotAvailable}.ToObservableCollection();
                            }
                        }
                        else if (coneSection.PreviousSection == null)
                        {
                            coneSection.AvailableFirstEdgeFeatures =
                                new[] {EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet}
                                    .ToObservableCollection();
                        }

                        if (coneSection.NextSection != null ||
                            nextSectionDiameter.NearlyEqual(coneSection.Diameter2))
                        {
                            if (coneSection.IsBore && coneSection.NextSection is CylinderSection)
                            {
                                coneSection.AvailableSecondEdgeFeatures = new[]
                                        {EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet}
                                    .ToObservableCollection();
                            }
                            else
                            {
                                coneSection.AvailableSecondEdgeFeatures = new[]
                                    {EdgeFeature.NotAvailable}.ToObservableCollection();
                            }
                        }
                        else if (coneSection.NextSection == null)
                        {
                            coneSection.AvailableSecondEdgeFeatures =
                                new[] {EdgeFeature.None, EdgeFeature.Chamfer, EdgeFeature.Fillet}
                                    .ToObservableCollection();
                        }

                        break;

                    case PolygonSection polygonSection:
                        if (polygonSection.PreviousSection != null &&
                            previousSectionDiameter > polygonSection.CircumscribedCircleDiameter)
                        {
                            polygonSection.AvailableFirstEdgeFeatures = new[]
                                {EdgeFeature.NotAvailable}.ToObservableCollection();
                        }
                        else if (previousSectionDiameter < polygonSection.CircumscribedCircleDiameter)
                        {
                            polygonSection.AvailableFirstEdgeFeatures = new[]
                                {EdgeFeature.None, EdgeFeature.Chamfer}.ToObservableCollection();
                        }
                        else if (polygonSection.PreviousSection != null &&
                                 previousSectionDiameter.NearlyEqual(polygonSection.CircumscribedCircleDiameter))
                        {
                            polygonSection.AvailableFirstEdgeFeatures =
                                new[] {EdgeFeature.NotAvailable}.ToObservableCollection();
                        }

                        if (polygonSection.NextSection != null &&
                            nextSectionDiameter > polygonSection.CircumscribedCircleDiameter)
                        {
                            polygonSection.AvailableSecondEdgeFeatures = new[]
                                {EdgeFeature.NotAvailable}.ToObservableCollection();
                        }
                        else if (nextSectionDiameter < polygonSection.CircumscribedCircleDiameter)
                        {
                            polygonSection.AvailableSecondEdgeFeatures = new[]
                                {EdgeFeature.None, EdgeFeature.Chamfer}.ToObservableCollection();
                        }
                        else if (polygonSection.NextSection != null &&
                                 nextSectionDiameter.NearlyEqual(polygonSection.CircumscribedCircleDiameter))
                        {
                            polygonSection.AvailableSecondEdgeFeatures =
                                new[] {EdgeFeature.NotAvailable}.ToObservableCollection();
                        }

                        break;
                }

                SetIfEdgeFeatureWillBeBuilt(section);
            }
        }

        private static void OnSettingsChanged(object sender, EventArgs eventArgs)
        {
            if (Settings.Is3DPreviewEnabled)
            {
                BuildShaft();
            }
        }

        private static ShaftSection CreateSection(float startPosition)
        {
            ShaftSection createdSection = null;
            if (typeof(TSection) == typeof(CylinderSection))
            {
                var newCylinderSection = new CylinderSection();
                createdSection = CreateCylinder(newCylinderSection, startPosition);
            }
            else if (typeof(TSection) == typeof(ConeSection))
            {
                var newConeSection = new ConeSection();
                createdSection = CreateCone(newConeSection, startPosition);
            }
            else if (typeof(TSection) == typeof(PolygonSection))
            {
                var newPolygonSection = new PolygonSection();
                createdSection = CreatePolygon(newPolygonSection, startPosition);
            }

            return createdSection;
        }

        private static void ResolveRevolveAxis(float startPosition, ShaftSection createdSection)
        {
            if (createdSection.IsBore)
            {
                if (createdSection.BoreFromEdge == BoreFromEdge.FromLeft)
                {
                    Shaft.BoreOnTheLeftRevolveAxis.EndPoint = Math.Abs(startPosition) > 0
                        ? new PointF(Shaft.BoreOnTheLeftRevolveAxis.EndPoint.X + createdSection.Length, 0)
                        : new PointF(createdSection.Length, 0);
                }
                else
                {
                    Shaft.BoreOnTheRightRevolveAxis.EndPoint = Math.Abs(startPosition) > 0
                        ? new PointF(Shaft.BoreOnTheRightRevolveAxis.EndPoint.X + createdSection.Length, 0)
                        : new PointF(createdSection.Length, 0);
                }
            }
            else
            {
                Shaft.RevolveAxis.EndPoint = Math.Abs(startPosition) > 0
                    ? new PointF(Shaft.RevolveAxis.EndPoint.X + createdSection.Length, 0)
                    : new PointF(createdSection.Length, 0);
            }
        }

        private static void InsertCreatedSection(ShaftSection createdSection, ShaftSection sectionBeforeNew,
                                                 bool replace = false)
        {
            bool isBoreFromTheLeft = createdSection.IsBore && createdSection.BoreFromEdge == BoreFromEdge.FromLeft;
            bool isBoreFromTheRigth =
                createdSection.IsBore && createdSection.BoreFromEdge == BoreFromEdge.FromRight;

            if (createdSection.IsBore)
            {
                createdSection.Length = (Shaft.Sections.Sum(section => section.Length) -
                                         Shaft.BoreOnTheLeft.Sum(section => section.Length) -
                                         Shaft.BoreOnTheRight.Sum(section => section.Length) -
                                         (replace ? sectionBeforeNew.Length : 0)) / 3;
            }

            if (sectionBeforeNew != null)
            {
                int indexOfSectionBeforeNew;
                if (createdSection.IsBore)
                {
                    indexOfSectionBeforeNew = isBoreFromTheLeft
                        ? Shaft.BoreOnTheLeft.IndexOf(sectionBeforeNew)
                        : Shaft.BoreOnTheRight.IndexOf(sectionBeforeNew);
                }
                else
                {
                    indexOfSectionBeforeNew = Shaft.Sections.IndexOf(sectionBeforeNew);
                }

                createdSection.PreviousSection = replace ? sectionBeforeNew.PreviousSection : sectionBeforeNew;
                switch (createdSection)
                {
                    case CylinderSection newCylinderSection:
                        switch (sectionBeforeNew)
                        {
                            case CylinderSection cylinderSectionBeforeNew:
                                newCylinderSection.Diameter = cylinderSectionBeforeNew.Diameter;
                                break;

                            case ConeSection coneSectionBeforeNew:
                                newCylinderSection.Diameter = coneSectionBeforeNew.Diameter2;
                                break;

                            case PolygonSection polygonSectionBeforeNew:
                                newCylinderSection.Diameter = polygonSectionBeforeNew.CircumscribedCircleDiameter;
                                break;
                        }

                        break;

                    case ConeSection newConeSection:
                        switch (sectionBeforeNew)
                        {
                            case CylinderSection cylinderSectionBeforeNew:
                                newConeSection.Diameter1 = cylinderSectionBeforeNew.Diameter;
                                newConeSection.Diameter2 = newConeSection.Diameter1 * 1.6f;
                                break;

                            case ConeSection coneSectionBeforeNew:
                                newConeSection.Diameter1 = coneSectionBeforeNew.Diameter2;
                                newConeSection.Diameter2 = newConeSection.Diameter1 * 1.6f;
                                break;

                            case PolygonSection polygonSectionBeforeNew:
                                newConeSection.Diameter1 = polygonSectionBeforeNew.InscribedCircleDiameter;
                                newConeSection.Diameter2 = newConeSection.Diameter1 * 1.6f;
                                break;
                        }

                        break;

                    case PolygonSection newPolygonSection:
                        switch (sectionBeforeNew)
                        {
                            case CylinderSection cylinderSectionBeforeNew:
                                newPolygonSection.InscribedCircleDiameter =
                                    newPolygonSection.CalculateInscribedRadius(cylinderSectionBeforeNew.Diameter,
                                        newPolygonSection.NumberOfEdges);
                                break;

                            case ConeSection coneSectionBeforeNew:
                                newPolygonSection.CircumscribedCircleDiameter = coneSectionBeforeNew.Diameter2;
                                break;

                            case PolygonSection polygonSectionBeforeNew:
                                newPolygonSection.CircumscribedCircleDiameter =
                                    polygonSectionBeforeNew.CircumscribedCircleDiameter;
                                break;
                        }

                        break;
                }

                if (replace)
                {
                    if (sectionBeforeNew.PreviousSection != null)
                    {
                        sectionBeforeNew.PreviousSection.NextSection = createdSection;
                    }
                }
                else
                {
                    sectionBeforeNew.ThirdLine = null;
                    sectionBeforeNew.NextSection = createdSection;
                }

                if (!sectionBeforeNew.IsLast)
                {
                    var nextSectionForNew =
                        isBoreFromTheLeft ? Shaft.BoreOnTheLeft[indexOfSectionBeforeNew + 1] :
                        isBoreFromTheRigth ? Shaft.BoreOnTheRight[indexOfSectionBeforeNew + 1] :
                        Shaft.Sections[indexOfSectionBeforeNew + 1];
                    createdSection.NextSection = nextSectionForNew;
                    /*isBoreFromTheLeft ? Shaft.BoreOnTheLeft[indexOfSectionBeforeNew + 1] :
                    isBoreFromTheRigth ? Shaft.BoresOnTheRight[indexOfSectionBeforeNew + 1] :
                    Shaft.Sections[indexOfSectionBeforeNew + 1];*/

                    nextSectionForNew.PreviousSection = createdSection;
                    createdSection.ThirdLine = null;
                }

                if (isBoreFromTheLeft)
                {
                    Shaft.BoreOnTheLeft.Insert(replace ? indexOfSectionBeforeNew : indexOfSectionBeforeNew + 1,
                        createdSection);
                }
                else if (isBoreFromTheRigth)
                {
                    Shaft.BoreOnTheRight.Insert(replace ? indexOfSectionBeforeNew : indexOfSectionBeforeNew + 1,
                        createdSection);
                }
                else
                {
                    Shaft.Sections.Insert(replace ? indexOfSectionBeforeNew : indexOfSectionBeforeNew + 1,
                        createdSection);
                }

                if (replace)
                {
                    if (sectionBeforeNew.IsBore)
                    {
                        switch (sectionBeforeNew.BoreFromEdge)
                        {
                            case BoreFromEdge.FromLeft:
                                Shaft.BoreOnTheLeft.Remove(sectionBeforeNew);
                                break;

                            case BoreFromEdge.FromRight:
                                Shaft.BoreOnTheRight.Remove(sectionBeforeNew);
                                break;
                        }
                    }
                    else
                    {
                        Shaft.Sections.Remove(sectionBeforeNew);
                    }
                }
            }
            else
            {
                if (createdSection.IsBore)
                {
                    var firstOuterSection = createdSection.BoreFromEdge == BoreFromEdge.FromLeft
                        ? Shaft.Sections.First()
                        : Shaft.Sections.Last();

                    switch (createdSection)
                    {
                        case CylinderSection innerCylinder:
                            switch (firstOuterSection)
                            {
                                case CylinderSection outerCylinder:
                                    innerCylinder.Diameter = outerCylinder.Diameter / 2;
                                    break;

                                case ConeSection outerCone:
                                    innerCylinder.Diameter = Math.Min(outerCone.Diameter1, outerCone.Diameter2) / 2;
                                    break;

                                case PolygonSection outerPolygon:
                                    innerCylinder.Diameter = outerPolygon.InscribedCircleDiameter / 2;
                                    break;
                            }

                            break;

                        case ConeSection innerCone:
                            switch (firstOuterSection)
                            {
                                case CylinderSection outerCylinder:
                                    innerCone.Diameter2 = outerCylinder.Diameter / 2;
                                    break;

                                case ConeSection outerCone:
                                    innerCone.Diameter2 = Math.Min(outerCone.Diameter1, outerCone.Diameter2) / 2;
                                    break;

                                case PolygonSection outerPolygon:
                                    innerCone.Diameter2 = outerPolygon.InscribedCircleDiameter / 2;
                                    break;
                            }

                            innerCone.Diameter1 = innerCone.Diameter2 / 1.6f;

                            break;
                    }
                }

                if (isBoreFromTheLeft)
                {
                    Shaft.BoreOnTheLeft.Add(createdSection);
                }
                else if (isBoreFromTheRigth)
                {
                    Shaft.BoreOnTheRight.Add(createdSection);
                }
                else
                {
                    Shaft.Sections.Add(createdSection);
                }
            }

            if (createdSection.IsBore)
            {
                if (sectionBeforeNew != null)
                {
                    float startPointX = replace
                        ? sectionBeforeNew.SecondLine.StartPoint.X
                        : sectionBeforeNew.SecondLine.EndPoint.X;
                    float endPointX = startPointX + createdSection.Length;
                    var outerSections = GetOuterSections(startPointX, endPointX,
                        createdSection.BoreFromEdge.Value).ToList();

                    bool CheckForDiameterError(ShaftSection outerSection)
                    {
                        switch (outerSection)
                        {
                            case CylinderSection outerCylinder:
                            {
                                switch (createdSection)
                                {
                                    case CylinderSection innerCylinder:
                                        return innerCylinder.Diameter >= outerCylinder.Diameter;

                                    case ConeSection innerCone:
                                        return
                                            Math.Max(innerCone.Diameter1, innerCone.Diameter2) >=
                                            outerCylinder.Diameter;
                                }

                                break;
                            }

                            case ConeSection outerCone:
                            {
                                switch (createdSection)
                                {
                                    case CylinderSection innerCylinder:
                                        return innerCylinder.Diameter >=
                                               Math.Min(outerCone.Diameter1, outerCone.Diameter2);

                                    case ConeSection innerCone:
                                        return
                                            Math.Max(innerCone.Diameter1, innerCone.Diameter2) >=
                                            Math.Min(outerCone.Diameter1, outerCone.Diameter2);
                                }

                                break;
                            }

                            case PolygonSection outerPolygon:
                            {
                                switch (createdSection)
                                {
                                    case CylinderSection innerCylinder:
                                        return
                                            innerCylinder.Diameter >= outerPolygon.InscribedCircleDiameter;

                                    case ConeSection innerCone:
                                        return
                                            Math.Max(innerCone.Diameter1, innerCone.Diameter2) >=
                                            outerPolygon.InscribedCircleDiameter;
                                }

                                break;
                            }
                        }

                        return false;
                    }

                    if (CheckForDiameterError(outerSections.First()))
                    {
                        createdSection.BoreDiameterCollisionError =
                            new BoreDiameterCollisionError(createdSection, outerSections.First());
                    }
                    else if (outerSections.Count > 1 && CheckForDiameterError(outerSections.Last()))
                    {
                        createdSection.BoreDiameterCollisionError =
                            new BoreDiameterCollisionError(createdSection, outerSections.Last());
                    }
                }
            }

            createdSection.PropertyChanged += CreatedSectionOnPropertyChanged;
        }

        public static IEnumerable<ShaftSection> GetOuterSections(float startPointX, float endPointX,
                                                                 BoreFromEdge boreFromEdge)
        {
            List<ShaftSection> sections;

            if (boreFromEdge == BoreFromEdge.FromLeft)
            {
                sections = Shaft.Sections.Where(section =>
                    section.SecondLine.StartPoint.X <= startPointX &&
                    section.SecondLine.EndPoint.X >= endPointX ||
                    section.SecondLine.StartPoint.X <= startPointX &&
                    section.SecondLine.EndPoint.X <= endPointX ||
                    section.SecondLine.StartPoint.X >= startPointX &&
                    section.SecondLine.EndPoint.X >= endPointX).ToList();
            }
            else
            {
                sections = Shaft.Sections.Where(section =>
                    section.SecondLine.StartPoint.X <= endPointX &&
                    section.SecondLine.EndPoint.X >= startPointX ||
                    section.SecondLine.StartPoint.X <= endPointX &&
                    section.SecondLine.EndPoint.X <= startPointX ||
                    section.SecondLine.StartPoint.X >= endPointX &&
                    section.SecondLine.EndPoint.X >= startPointX).ToList();
            }

            return sections;
        }

        public static void InstallSection(ShaftSection sectionBeforeNew, bool isBoreSection = false,
                                          BoreFromEdge? borePosition = null, bool replace = false)
        {
            if (sectionBeforeNew == null)
            {
                if (isBoreSection)
                {
                    sectionBeforeNew = borePosition == BoreFromEdge.FromLeft
                        ? Shaft.BoreOnTheLeft.LastOrDefault()
                        : Shaft.BoreOnTheRight.LastOrDefault();
                }
                else
                {
                    sectionBeforeNew = Shaft.Sections.LastOrDefault();
                }
            }

            float startPosition = replace
                ? sectionBeforeNew.SecondLine.StartPoint.X
                : sectionBeforeNew?.SecondLine.EndPoint.X ?? 0;

            var createdSection = CreateSection(startPosition);

            if (isBoreSection)
            {
                createdSection.IsBore = true;
                createdSection.BoreFromEdge = borePosition;
            }

            ResolveRevolveAxis(startPosition, createdSection);

            InsertCreatedSection(createdSection, sectionBeforeNew, replace: replace);

            if (Settings.Is3DPreviewEnabled)
            {
                BuildShaft();
            }
        }

        private static void ResolveBoresDiameterErrors()
        {
            bool CheckForDiameterError(ShaftSection outerSection, ShaftSection innerSection)
            {
                switch (outerSection)
                {
                    case CylinderSection outerCylinder:
                    {
                        switch (innerSection)
                        {
                            case CylinderSection innerCylinder:
                                return innerCylinder.Diameter >= outerCylinder.Diameter - 0.1;

                            case ConeSection innerCone:
                                return
                                    Math.Max(innerCone.Diameter1, innerCone.Diameter2) >=
                                    outerCylinder.Diameter - 0.1;
                        }

                        break;
                    }

                    case ConeSection outerCone:
                    {
                        switch (innerSection)
                        {
                            case CylinderSection innerCylinder:
                                return innerCylinder.Diameter >=
                                       Math.Min(outerCone.Diameter1, outerCone.Diameter2) - 0.1;

                            case ConeSection innerCone:
                                return
                                    Math.Max(innerCone.Diameter1, innerCone.Diameter2) >=
                                    Math.Min(outerCone.Diameter1, outerCone.Diameter2) - 0.1;
                        }

                        break;
                    }

                    case PolygonSection outerPolygon:
                    {
                        switch (innerSection)
                        {
                            case CylinderSection innerCylinder:
                                return
                                    innerCylinder.Diameter >= outerPolygon.InscribedCircleDiameter - 0.1;

                            case ConeSection innerCone:
                                return
                                    Math.Max(innerCone.Diameter1, innerCone.Diameter2) >=
                                    outerPolygon.InscribedCircleDiameter - 0.1;
                        }

                        break;
                    }
                }

                return false;
            }

            foreach (var leftBore in Shaft.BoreOnTheLeft)
            {
                var outerSections = leftBore.GetOuterSections()
                                            .ToList();

                if (CheckForDiameterError(outerSections.First(), leftBore))
                {
                    leftBore.BoreDiameterCollisionError =
                        new BoreDiameterCollisionError(leftBore, outerSections.First());
                }
                else if (outerSections.Count > 1 && CheckForDiameterError(outerSections.Last(), leftBore))
                {
                    leftBore.BoreDiameterCollisionError =
                        new BoreDiameterCollisionError(leftBore, outerSections.Last());
                }
            }

            foreach (var rightBore in Shaft.BoreOnTheRight)
            {
                var outerSections = rightBore.GetOuterSections()
                                             .ToList();

                if (CheckForDiameterError(outerSections.First(), rightBore))
                {
                    rightBore.BoreDiameterCollisionError =
                        new BoreDiameterCollisionError(rightBore, outerSections.First());
                }
                else if (outerSections.Count > 1 && CheckForDiameterError(outerSections.Last(), rightBore))
                {
                    rightBore.BoreDiameterCollisionError =
                        new BoreDiameterCollisionError(rightBore, outerSections.Last());
                }
            }
        }

        private static void CreatedSectionOnPropertyChanged(object sender,
                                                            PropertyChangedEventArgs e)
        {
            var changedSection = (ShaftSection) sender;

            switch (e.PropertyName)
            {
                case nameof(ShaftSection.Length):
                    ResolveSketchLines(isBoreSection: changedSection.IsBore, borePosition: changedSection.BoreFromEdge);
                    break;

                case nameof(CylinderSection.Diameter):
                case nameof(ConeSection.Diameter1):
                case nameof(ConeSection.Diameter2):
                case nameof(PolygonSection.CircumscribedCircleDiameter):
                case nameof(PolygonSection.InscribedCircleDiameter):
                {
                    ResolveSketchLines(isBoreSection: changedSection.IsBore,
                        borePosition: changedSection.BoreFromEdge);
                    ResolveEdgeFeatures(isBoreSection: changedSection.IsBore,
                        borePosition: changedSection.BoreFromEdge);
                    ResolveBoresDiameterErrors();
                    break;
                }
            }
        }

        private static CylinderSection CreateCylinder(CylinderSection newSection, float startPosition)
        {
            if (Math.Abs(startPosition) > 0)
            {
                newSection.SecondLine = new SketchLineSimple(
                    startPoint: new PointF(startPosition, newSection.Diameter / 2),
                    endPoint: new PointF(startPosition + newSection.Length, newSection.Diameter / 2));

                newSection.ThirdLine = new SketchLineSimple(
                    startPoint: newSection.SecondLine.EndPoint,
                    endPoint: new PointF(newSection.SecondLine.EndPoint.X, 0));
            }
            // Then, that's the first section
            else
            {
                newSection.FirstLine = new SketchLineSimple(
                    startPoint: PointF.Empty,
                    endPoint: new PointF(0, newSection.Diameter / 2));

                newSection.SecondLine = new SketchLineSimple(
                    startPoint: newSection.FirstLine.EndPoint,
                    endPoint: new PointF(newSection.Length, newSection.Diameter / 2));

                newSection.ThirdLine = new SketchLineSimple(
                    startPoint: newSection.SecondLine.EndPoint,
                    endPoint: new PointF(newSection.Length, 0));
            }

            return newSection;
        }

        private static ConeSection CreateCone(ConeSection newConeSection, float startPosition)
        {
            if (Math.Abs(startPosition) > 0)
            {
                newConeSection.SecondLine = new SketchLineSimple(
                    startPoint: new PointF(startPosition, newConeSection.Diameter1 / 2),
                    endPoint: new PointF(startPosition + newConeSection.Length, newConeSection.Diameter2 / 2));

                newConeSection.ThirdLine = new SketchLineSimple(
                    startPoint: newConeSection.SecondLine.EndPoint,
                    endPoint: new PointF(newConeSection.SecondLine.EndPoint.X, 0));
            }
            // Then, that's the first section
            else
            {
                newConeSection.FirstLine = new SketchLineSimple(
                    startPoint: PointF.Empty,
                    endPoint: new PointF(0, newConeSection.Diameter1 / 2));

                newConeSection.SecondLine = new SketchLineSimple(
                    startPoint: newConeSection.FirstLine.EndPoint,
                    endPoint: new PointF(newConeSection.Length, newConeSection.Diameter2 / 2));

                newConeSection.ThirdLine = new SketchLineSimple(
                    startPoint: newConeSection.SecondLine.EndPoint,
                    endPoint: new PointF(newConeSection.Length, 0));
            }

            return newConeSection;
        }

        private static PolygonSection CreatePolygon(PolygonSection newPolygonSection, float startPosition)
        {
            if (Math.Abs(startPosition) > 0)
            {
                newPolygonSection.SecondLine = new SketchLineSimple(
                    startPoint: new PointF(startPosition, newPolygonSection.InscribedCircleDiameter / 2),
                    endPoint: new PointF(startPosition + newPolygonSection.Length,
                        newPolygonSection.InscribedCircleDiameter / 2));

                newPolygonSection.ThirdLine = new SketchLineSimple(
                    startPoint: newPolygonSection.SecondLine.EndPoint,
                    endPoint: new PointF(newPolygonSection.SecondLine.EndPoint.X, 0));
            }
            // Then, that's the first section
            else
            {
                newPolygonSection.FirstLine = new SketchLineSimple(
                    startPoint: PointF.Empty,
                    endPoint: new PointF(0, newPolygonSection.InscribedCircleDiameter / 2));

                newPolygonSection.SecondLine = new SketchLineSimple(
                    startPoint: newPolygonSection.FirstLine.EndPoint,
                    endPoint: new PointF(newPolygonSection.Length, newPolygonSection.InscribedCircleDiameter / 2));

                newPolygonSection.ThirdLine = new SketchLineSimple(
                    startPoint: newPolygonSection.SecondLine.EndPoint,
                    endPoint: new PointF(newPolygonSection.Length, 0));
            }

            return newPolygonSection;
        }

        public static void SplitCylinderSection(CylinderSection selectedCylinder, float mainDiameter1,
                                                float mainDiameter2,
                                                float sectionLength1, float sectionLength2)
        {
            int selectedCylinderIndex = Shaft.Sections.IndexOf(selectedCylinder);
            selectedCylinder.Diameter = mainDiameter1;
            selectedCylinder.Length = sectionLength1;

            var cylinder2 = (CylinderSection) CreateSection(selectedCylinder.SecondLine.EndPoint.X);
            cylinder2.PropertyChanged += CreatedSectionOnPropertyChanged;
            cylinder2.PreviousSection = selectedCylinder;
            cylinder2.NextSection = selectedCylinder.NextSection;
            selectedCylinder.NextSection = cylinder2;
            cylinder2.Diameter = mainDiameter2;
            cylinder2.Length = sectionLength2;
            Shaft.Sections.Insert(selectedCylinderIndex + 1, cylinder2);
            InstallSection(cylinder2);
            Shaft.Sections.RemoveAt(selectedCylinderIndex + 2);
        }

        private static void BuildShaft()
        {
        }
    }
}