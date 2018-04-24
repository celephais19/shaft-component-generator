using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.Behaviors
{
    public class SectionHasErrorStackPanelBehavior : Behavior<StackPanel>
    {
        private Brush oldBackground;

        protected override void OnAttached()
        {
            ShaftSection shaftSection = (ShaftSection) this.AssociatedObject.DataContext;
            shaftSection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(ShaftSection.FirstEdgeFeatureHasErrors) &&
                    args.PropertyName != nameof(ShaftSection.SecondEdgeFeatureHasErrors))
                {
                    return;
                }

                var sectionSender = (ShaftSection)sender;
                if (sectionSender.FirstEdgeFeatureHasErrors || sectionSender.SecondEdgeFeatureHasErrors)
                {
                    this.oldBackground = this.AssociatedObject.Background;
                    this.AssociatedObject.Background = new BrushConverter().ConvertFrom("#fdadad") as SolidColorBrush;
                }
                else
                {
                    this.AssociatedObject.Background = this.oldBackground;
                }
            };
        }
    }
}