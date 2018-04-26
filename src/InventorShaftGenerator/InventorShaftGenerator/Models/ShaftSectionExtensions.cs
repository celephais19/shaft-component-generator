using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DeepCopyUsingExpressionTrees;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure;

namespace InventorShaftGenerator.Models
{
    public static class ShaftSectionExtensions
    {
        public static IEnumerable<ShaftSection> GetOuterSections(this ShaftSection shaftSection,
                                                                 IEnumerable<ShaftSection> fromSections = null)
        {
            if (!shaftSection.IsBore)
            {
                return null;
            }

            if (fromSections == null)
            {
                fromSections = Shaft.Sections;
            }

            var shaftSections = fromSections.ToList();
            float shaftLength = shaftSections.Sum(section => section.Length);

            List<ShaftSection> sections;
            float startPointX = shaftSection.SecondLine.StartPoint.X;
            float endPointX = shaftSection.SecondLine.EndPoint.X;

            if (shaftSection.BoreFromEdge == BoreFromEdge.FromLeft)
            {
                sections = shaftSections.SkipWhile(section => section.SecondLine.EndPoint.X < startPointX).Where(
                    section =>
                        section.SecondLine.StartPoint.X.NearlyEqual(startPointX) &&
                        section.SecondLine.EndPoint.X.NearlyEqual(endPointX) ||
                        section.SecondLine.StartPoint.X.NearlyEqual(startPointX) &&
                        section.SecondLine.EndPoint.X > endPointX ||
                        section.SecondLine.StartPoint.X.NearlyEqual(startPointX) &&
                        section.SecondLine.EndPoint.X < endPointX ||
                        section.SecondLine.StartPoint.X < startPointX &&
                        section.SecondLine.EndPoint.X > endPointX ||
                        section.SecondLine.StartPoint.X < startPointX &&
                        section.SecondLine.EndPoint.X < endPointX ||
                        section.SecondLine.StartPoint.X > startPointX &&
                        section.SecondLine.StartPoint.X < endPointX
                ).ToList();
            }
            else
            {
                endPointX = shaftLength - endPointX;
                shaftSections = shaftSections.SkipWhile(section => section.SecondLine.EndPoint.X < endPointX).ToList();
                sections = Inverse(shaftSections);
            }

            List<ShaftSection> Inverse(List<ShaftSection> source)
            {
                List<ShaftSection> list = new List<ShaftSection>();
                foreach (var section in source.DeepCopyByExpressionTree().FastReverse())
                {
                    PointF startPoint;
                    PointF endPoint;
                    if (section.FirstLine != null)
                    {
                        startPoint = section.FirstLine.StartPoint;
                        endPoint = section.FirstLine.EndPoint;
                        section.ThirdLine = new SketchLineSimple(new PointF(shaftLength - endPoint.X,
                            endPoint.Y), new PointF(shaftLength - startPoint.X,
                            startPoint.Y));
                    }

                    startPoint = section.SecondLine.StartPoint;
                    endPoint = section.SecondLine.EndPoint;

                    section.SecondLine.EndPoint = new PointF(shaftLength - startPoint.X, startPoint.Y);
                    section.SecondLine.StartPoint = new PointF(shaftLength - endPoint.X, endPoint.Y);

                    if (section.ThirdLine != null)
                    {
                        startPoint = section.ThirdLine.StartPoint;
                        endPoint = section.ThirdLine.EndPoint;
                        section.FirstLine = new SketchLineSimple(new PointF(shaftLength - endPoint.X,
                            endPoint.Y), new PointF(shaftLength - startPoint.X,
                            startPoint.Y));
                    }

                    list.Add(section);
                }

                return list;
            }

            return sections;
        }
    }
}