using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Threading;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.Behaviors
{
    public class ItemsControlItemsSourceUpdatedRefreshBehavior : Behavior<ItemsControl>
    {
        protected override void OnAttached()
        {
            ((INotifyCollectionChanged) this.AssociatedObject.ItemsSource).CollectionChanged += (sender, args) =>
            {
                this.AssociatedObject.Items.Refresh();
            };

            if (this.AssociatedObject.ItemsSource is ObservableCollectionEx<ShaftSection> sections)
            {
                sections.ItemPropertyChanged += (sender, args) => this.AssociatedObject.Items.Refresh();
            }
        }
    }
}