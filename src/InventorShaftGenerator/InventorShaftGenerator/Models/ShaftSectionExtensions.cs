using System.Collections.Generic;
using System.Linq;
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
                sections = shaftSections.Where(section =>
                    section.SecondLine.StartPoint.X.NearlyEqual(startPointX) &&
                    section.SecondLine.EndPoint.X.NearlyEqual(endPointX) ||
                    section.SecondLine.StartPoint.X.NearlyEqual(startPointX) &&
                    section.SecondLine.EndPoint.X > endPointX ||
                    section.SecondLine.StartPoint.X < startPointX &&
                    section.SecondLine.EndPoint.X.NearlyEqual(endPointX) ||
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
                sections = shaftSections.Where(section =>
                    section.SecondLine.StartPoint.X.NearlyEqual(endPointX) &&
                    section.SecondLine.EndPoint.X.NearlyEqual(startPointX) ||
                    section.SecondLine.StartPoint.X < endPointX &&
                    section.SecondLine.EndPoint.X > startPointX ||
                    section.SecondLine.StartPoint.X < endPointX &&
                    section.SecondLine.EndPoint.X < startPointX ||
                    section.SecondLine.StartPoint.X > endPointX &&
                    section.SecondLine.StartPoint.X < startPointX
                ).ToList();
            }

            return sections;
        }
    }
}