using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using InventorShaftGenerator.Extensions;

namespace InventorShaftGenerator.CustomizedUIElements
{
    public class CenteringCanvas : Canvas
    {
        public static readonly DependencyProperty ItemsControlProperty = DependencyProperty.RegisterAttached(
            "ItemsControl", typeof(ItemsControl), typeof(CenteringCanvas), new PropertyMetadata(default(ItemsControl)));

        public static readonly DependencyProperty AxisLineProperty = DependencyProperty.RegisterAttached(
            "AxisLine", typeof(Line), typeof(CenteringCanvas), new PropertyMetadata(default(Line)));

        public ItemsControl ItemsControl
        {
            get => (ItemsControl) GetValue(ItemsControlProperty);
            set => SetValue(ItemsControlProperty, value);
        }

        public Line AxisLine
        {
            get => (Line) GetValue(AxisLineProperty);
            set => SetValue(AxisLineProperty, value);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var paths = this.ItemsControl.FindChildren<Path>().ToList();

            foreach (UIElement element in this.InternalChildren)
            {
                if (element == null)
                {
                    continue;
                }

                double x = 0.0;
                double y = 0.0;
                double left = GetLeft(element);
                if (!double.IsNaN(left))
                {
                    x = left - element.DesiredSize.Width / 2;
                }

                double top = GetTop(element);
                if (!double.IsNaN(top))
                {
                    y = top - element.DesiredSize.Height / 2;
                }

                element.Arrange(new Rect(new Point(x, y), element.DesiredSize));
            }

            var boundingRect = VisualTreeHelper.GetDescendantBounds(this);

            var boundingWidth = this.ItemsControl.ActualWidth - 10;
            var boundingHeight = this.ItemsControl.ActualHeight - 10;

            if (boundingRect.Width > boundingWidth || boundingRect.Height > boundingHeight)
            {
                var scaleX = ((ScaleTransform) paths.Last().LayoutTransform).ScaleX;
                var scaleY = ((ScaleTransform) paths.Last().LayoutTransform).ScaleY;

                do
                {
                    scaleX -= 0.01;
                    scaleY -= 0.01;
                    boundingRect.Scale(scaleX, scaleY);
                } while (boundingRect.Width > boundingWidth || boundingRect.Height > boundingHeight);

                foreach (var path in paths)
                {
                    path.LayoutTransform = new ScaleTransform(scaleX, scaleY);
                }
            }

            this.AxisLine.X1 = boundingRect.IsEmpty ? 0 : boundingRect.Left;
            this.AxisLine.X2 = boundingRect.IsEmpty ? 0 : boundingRect.Right;

            return arrangeSize;
        }
    }
}