using System;
using System.Windows;
using System.Windows.Media;

namespace InventorShaftGenerator.Views.Extensions
{
    public class VisualTargetPresentationSource : PresentationSource
    {
        private readonly VisualTarget visualTarget;
        private bool isDisposed;

        public VisualTargetPresentationSource(HostVisual hostVisual)
        {
            this.visualTarget = new VisualTarget(hostVisual);
            AddSource();
        }

        public Size DesiredSize { get; private set; }

        public override Visual RootVisual
        {
            get => this.visualTarget.RootVisual;
            set
            {
                Visual oldRoot = this.visualTarget.RootVisual;

                // Set the root visual of the VisualTarget.  This visual will
                // now be used to visually compose the scene.
                this.visualTarget.RootVisual = value;

                // Tell the PresentationSource that the root visual has
                // changed.  This kicks off a bunch of stuff like the
                // Loaded event.
                RootChanged(oldRoot, value);

                // Kickoff layout...
                if (value is UIElement rootElement)
                {
                    rootElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));

                    this.DesiredSize = rootElement.DesiredSize;
                }
                else
                {
                    this.DesiredSize = new Size(0, 0);
                }
            }
        }

        protected override CompositionTarget GetCompositionTargetCore()
        {
            return this.visualTarget;
        }

        public override bool IsDisposed => this.isDisposed;

        internal void Dispose()
        {
            RemoveSource();
            this.isDisposed = true;
        }
    }
}
