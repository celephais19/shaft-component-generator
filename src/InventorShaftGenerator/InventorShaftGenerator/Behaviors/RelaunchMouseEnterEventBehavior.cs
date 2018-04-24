using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using InventorShaftGenerator.Extensions;

namespace InventorShaftGenerator.Behaviors
{
    public class RelaunchMouseEnterEventBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            TreeViewItem thisTreeViewItem;
            if (this.AssociatedObject is TreeViewItem treeViewItem)
            {
                thisTreeViewItem = treeViewItem;
            }
            else
            {
                thisTreeViewItem = this.AssociatedObject.FindAncestor<TreeViewItem>();
            }   

            var parentTreeViewItem = thisTreeViewItem.FindAncestor<TreeViewItem>();

            thisTreeViewItem.MouseEnter += (sender, args) =>
            {
                parentTreeViewItem.RaiseEvent(args);
            };
        }
    }
}