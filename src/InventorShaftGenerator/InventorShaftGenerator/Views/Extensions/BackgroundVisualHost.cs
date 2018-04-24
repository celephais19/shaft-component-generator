using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace InventorShaftGenerator.Views.Extensions
{
    public delegate Visual CreateContentFunction();

    public class BackgroundVisualHost : FrameworkElement
    {
        private ThreadedVisualHelper threadedHelper;
        private HostVisual hostVisual;

        public static readonly DependencyProperty IsContentShowingProperty = DependencyProperty.Register(
            "IsContentShowing",
            typeof(bool),
            typeof(BackgroundVisualHost),
            new FrameworkPropertyMetadata(false, OnIsContentShowingChanged));

        public bool IsContentShowing
        {
            get => (bool) GetValue(IsContentShowingProperty);
            set => SetValue(IsContentShowingProperty, value);
        }

        private static void OnIsContentShowingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackgroundVisualHost bvh = (BackgroundVisualHost) d;

            if (bvh.CreateContent != null)
            {
                if ((bool) e.NewValue)
                {
                    bvh.CreateContentHelper();
                }
                else
                {
                    bvh.HideContentHelper();
                }
            }
        }

        public static readonly DependencyProperty CreateContentProperty = DependencyProperty.Register(
            "CreateContent",
            typeof(CreateContentFunction),
            typeof(BackgroundVisualHost),
            new FrameworkPropertyMetadata(OnCreateContentChanged));

        public CreateContentFunction CreateContent
        {
            get => (CreateContentFunction) GetValue(CreateContentProperty);
            set => SetValue(CreateContentProperty, value);
        }

        private static void OnCreateContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackgroundVisualHost bvh = (BackgroundVisualHost) d;

            if (bvh.IsContentShowing)
            {
                bvh.HideContentHelper();
                if (e.NewValue != null)
                {
                    bvh.CreateContentHelper();
                }
            }
        }

        protected override int VisualChildrenCount => this.hostVisual != null ? 1 : 0;

        protected override Visual GetVisualChild(int index)
        {
            if (this.hostVisual != null && index == 0)
            {
                return this.hostVisual;
            }

            throw new IndexOutOfRangeException("index");
        }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                if (this.hostVisual != null)
                {
                    yield return this.hostVisual;
                }
            }
        }

        private void CreateContentHelper()
        {
            this.threadedHelper = new ThreadedVisualHelper(this.CreateContent, SafeInvalidateMeasure);
            this.hostVisual = this.threadedHelper.HostVisual;
        }

        private void SafeInvalidateMeasure()
        {
            this.Dispatcher.BeginInvoke(new Action(InvalidateMeasure), DispatcherPriority.Loaded);
        }

        private void HideContentHelper()
        {
            if (this.threadedHelper == null)
            {
                return;
            }

            this.threadedHelper.Exit();
            this.threadedHelper = null;
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.threadedHelper != null)
            {
                return this.threadedHelper.DesiredSize;
            }

            return base.MeasureOverride(availableSize);
        }

        private class ThreadedVisualHelper
        {
            private readonly AutoResetEvent sync =
                new AutoResetEvent(false);

            private readonly CreateContentFunction createContent;
            private readonly Action invalidateMeasure;

            public HostVisual HostVisual { get; }

            public Size DesiredSize { get; private set; }
            private Dispatcher Dispatcher { get; set; }

            public ThreadedVisualHelper(
                CreateContentFunction createContent,
                Action invalidateMeasure)
            {
                this.HostVisual = new HostVisual();
                this.createContent = createContent;
                this.invalidateMeasure = invalidateMeasure;

                Thread backgroundUi = new Thread(CreateAndShowContent);
                backgroundUi.SetApartmentState(ApartmentState.STA);
                backgroundUi.Name = "BackgroundVisualHostThread";
                backgroundUi.IsBackground = true;
                backgroundUi.Start();

                this.sync.WaitOne();
            }

            public void Exit()
            {
                this.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            }

            private void CreateAndShowContent()
            {
                this.Dispatcher = Dispatcher.CurrentDispatcher;
                VisualTargetPresentationSource source =
                    new VisualTargetPresentationSource(this.HostVisual);
                this.sync.Set();
                source.RootVisual = this.createContent();
                this.DesiredSize = source.DesiredSize;
                this.invalidateMeasure();

                Dispatcher.Run();
                source.Dispose();
            }
        }
    }
}