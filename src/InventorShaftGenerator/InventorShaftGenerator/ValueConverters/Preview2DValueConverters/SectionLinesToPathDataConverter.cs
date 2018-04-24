using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.ValueConverters.Preview2DValueConverters
{
    public class SectionLinesToPathDataConverter : DependencyObject, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var section = values[0] as ShaftSection;
            var itemsControl = (ItemsControl) values[1];
            var paths = itemsControl.FindChildren<Path>().ToList();
            var previousPath = paths?.SingleOrDefault(path =>
                section.PreviousSection?.Equals((
                    (ShaftSection) path.DataContext)) ?? false);

            TranslatePrevious(itemsControl, section);
            TranslateNext(itemsControl, section);

            double startPointX =
                section.IsFirst
                    ? section.FirstLine.StartPoint.X - section.Length / 2
                    : previousPath.Data.Bounds.BottomRight.X;
            float startPointY = -section.SecondLine.StartPoint.Y;
            startPointX += 1;

            PathFigure figure = new PathFigure {StartPoint = new Point(startPointX, startPointY)};
            
            figure.Segments.Add(new LineSegment(
                new Point(startPointX,
                    -startPointY), false));
            figure.Segments.Add(new LineSegment(
                new Point(
                    startPointX + section.Length,
                    section.SecondLine.EndPoint.Y), false));
            figure.Segments.Add(new LineSegment(
                new Point(
                    startPointX + section.Length,
                    -section.SecondLine.EndPoint.Y), false));

            var geometry = new PathGeometry(new[] {figure});

            return geometry;
        }

        private void TranslatePrevious(ItemsControl itemsControl, ShaftSection currentSection)
        {
            var paths = itemsControl.FindChildren<Path>()?.ToList();

            Path previousPath = paths?.SingleOrDefault(path =>
                currentSection.PreviousSection?.Equals((ShaftSection) path.DataContext) ?? false);

            if (previousPath == null)
            {
                return;
            }

            float offset = -currentSection.Length / 2;
            while (previousPath != null)
            {
                var previousPathSection = (ShaftSection) previousPath.DataContext;
                previousPath.Data.Transform = new TranslateTransform(offset, 0);
                previousPath = paths.SingleOrDefault(path =>
                    previousPathSection.PreviousSection?.Equals((ShaftSection) path.DataContext) ?? false);
                offset -= previousPathSection.Length / 2;
            }
        }

        private void TranslateNext(ItemsControl itemsControl, ShaftSection currentSection)
        {
            var paths = itemsControl.FindChildren<Path>()?.ToList();

            Path nextPath = paths?.SingleOrDefault(path =>
                currentSection.NextSection?.Equals((ShaftSection) path.DataContext) ?? false);

            if (nextPath == null)
            {
                return;
            }

            float offset = currentSection.Length / 2;
            while (nextPath != null)
            {
                var nextPathSection = (ShaftSection) nextPath.DataContext;
                nextPath.Data.Transform = new TranslateTransform(offset, 0);
                nextPath = paths.SingleOrDefault(path =>
                    nextPathSection.NextSection?.Equals((ShaftSection) path.DataContext) ?? false);
                offset += nextPathSection.Length / 2;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}