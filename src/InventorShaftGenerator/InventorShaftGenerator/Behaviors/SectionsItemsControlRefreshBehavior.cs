using System.Windows.Controls;
using System.Windows.Interactivity;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.Behaviors
{
    public class SectionsItemsControlRefreshBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            if (this.AssociatedObject.ItemsSource is ObservableCollectionEx<ShaftSection> sections)
            {
                sections.ItemPropertyChanged += (sender, args) =>
                {
                    if (((ShaftSection)sender).IsLast)
                    {
                        if (args.PropertyName == nameof(ShaftSection.SecondLine))
                        {
                            this.AssociatedObject.Items.Refresh();
                        }
                    }
                };
            }
        }
    }
}