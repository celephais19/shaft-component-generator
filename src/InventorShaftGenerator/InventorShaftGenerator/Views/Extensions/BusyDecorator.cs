using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InventorShaftGenerator.Views.Extensions
{
    [StyleTypedProperty(Property = "BusyStyle", StyleTargetType = typeof(Control))]
    public class BusyDecorator : Decorator
    {
        private readonly BackgroundVisualHost busyHost = new BackgroundVisualHost();

        public static readonly DependencyProperty IsBusyIndicatorShowingProperty = DependencyProperty.Register(
            "IsBusyIndicatorShowing",
            typeof(bool),
            typeof(BusyDecorator),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

        public bool IsBusyIndicatorShowing
        {
            get => (bool) GetValue(IsBusyIndicatorShowingProperty);
            set => SetValue(IsBusyIndicatorShowingProperty, value);
        }


        public static readonly DependencyProperty BusyStyleProperty =
            DependencyProperty.Register(
                "BusyStyle",
                typeof(Style),
                typeof(BusyDecorator),
                new FrameworkPropertyMetadata(OnBusyStyleChanged));


        public Style BusyStyle
        {
            get => (Style) GetValue(BusyStyleProperty);
            set => SetValue(BusyStyleProperty, value);
        }

        private static void OnBusyStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BusyDecorator bd = (BusyDecorator) d;
            Style nVal = (Style) e.NewValue;
            bd.busyHost.CreateContent = () => new Control {Style = nVal};
        }

        public static readonly DependencyProperty BusyHorizontalAlignmentProperty = DependencyProperty.Register(
            "BusyHorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(BusyDecorator),
            new FrameworkPropertyMetadata(HorizontalAlignment.Center));

        public HorizontalAlignment BusyHorizontalAlignment
        {
            get => (HorizontalAlignment) GetValue(BusyHorizontalAlignmentProperty);
            set => SetValue(BusyHorizontalAlignmentProperty, value);
        }

        public static readonly DependencyProperty BusyVerticalAlignmentProperty = DependencyProperty.Register(
            "BusyVerticalAlignment",
            typeof(VerticalAlignment),
            typeof(BusyDecorator),
            new FrameworkPropertyMetadata(VerticalAlignment.Center));

        public VerticalAlignment BusyVerticalAlignment
        {
            get => (VerticalAlignment) GetValue(BusyVerticalAlignmentProperty);
            set => SetValue(BusyVerticalAlignmentProperty, value);
        }

        static BusyDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(BusyDecorator),
                new FrameworkPropertyMetadata(typeof(BusyDecorator)));
        }

        protected override int VisualChildrenCount => this.Child != null ? 2 : 1;

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                if (this.Child != null)
                {
                    yield return this.Child;
                }

                yield return this.busyHost;
            }
        }

        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            if (this.Child != null)
            {
                switch (index)
                {
                    case 0:
                        return this.Child;

                    case 1:
                        return this.busyHost;
                }
            }
            else if (index == 0)
            {
                return this.busyHost;
            }

            throw new IndexOutOfRangeException("index");
        }

        public BusyDecorator()
        {
            AddLogicalChild(this.busyHost);
            AddVisualChild(this.busyHost);

            SetBinding(this.busyHost, IsBusyIndicatorShowingProperty, BackgroundVisualHost.IsContentShowingProperty);
            SetBinding(this.busyHost, BusyHorizontalAlignmentProperty,
                BackgroundVisualHost.HorizontalAlignmentProperty);
            SetBinding(this.busyHost, BusyVerticalAlignmentProperty, BackgroundVisualHost.VerticalAlignmentProperty);
        }

        private void SetBinding(DependencyObject obj, DependencyProperty source, DependencyProperty target)
        {
            Binding b = new Binding
            {
                Source = this,
                Path = new PropertyPath(source)
            };
            BindingOperations.SetBinding(obj, target, b);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size ret = new Size(0, 0);
            if (this.Child != null)
            {
                this.Child.Measure(constraint);
                ret = this.Child.DesiredSize;
            }

            this.busyHost.Measure(constraint);

            return new Size(Math.Max(ret.Width, this.busyHost.DesiredSize.Width),
                Math.Max(ret.Height, this.busyHost.DesiredSize.Height));
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size ret = new Size(0, 0);
            if (this.Child != null)
            {
                this.Child.Arrange(new Rect(arrangeSize));
                ret = this.Child.RenderSize;
            }

            this.busyHost.Arrange(new Rect(arrangeSize));

            return new Size(Math.Max(ret.Width, this.busyHost.RenderSize.Width),
                Math.Max(ret.Height, this.busyHost.RenderSize.Height));
        }
    }
}