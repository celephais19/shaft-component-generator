using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;
using Microsoft.Expression.Interactivity.Media;
using EventTrigger = System.Windows.Interactivity.EventTrigger;

namespace InventorShaftGenerator.Behaviors
{
    public class SecitonPathBehavior : Behavior<Path>
    {
        private static Path oldSelectedPath;

        public static readonly DependencyProperty InstalledPathsItemsControlProperty =
            DependencyProperty.RegisterAttached(
                "InstalledPathsItemsControl", typeof(ItemsControl), typeof(SecitonPathBehavior));

        public static readonly DependencyProperty InstalledSectionsTreeViewProperty =
            DependencyProperty.RegisterAttached(
                "InstalledSectionsTreeView", typeof(TreeView), typeof(SecitonPathBehavior));

        public static readonly DependencyProperty SectionsComboboxProperty = DependencyProperty.RegisterAttached(
            "SectionsCombobox", typeof(ComboBox), typeof(SecitonPathBehavior));

        private static readonly ColorAnimation FadeAnimation = new ColorAnimation(Color.FromRgb(37, 161, 218),
            Color.FromRgb(160, 160, 160), new Duration(TimeSpan.FromSeconds(0.1)));

        public ItemsControl InstalledPathsItemsControl
        {
            get => (ItemsControl) GetValue(InstalledPathsItemsControlProperty);
            set => SetValue(InstalledPathsItemsControlProperty, value);
        }

        public ComboBox SectionsCombobox
        {
            get => (ComboBox) GetValue(SectionsComboboxProperty);
            set => SetValue(SectionsComboboxProperty, value);
        }

        public TreeView InstalledSectionsTreeView
        {
            get => (TreeView) GetValue(InstalledSectionsTreeViewProperty);
            set => SetValue(InstalledSectionsTreeViewProperty, value);
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.Loaded += (sender, args) =>
            {
                SetTreeViewItemSectioRelativePath();
                SetupEventTriggers();
            };
            this.SectionsCombobox.SelectionChanged += (sender, args) =>
            {
                if ((string) this.SectionsCombobox.SelectedValue == "Sections")
                {
                    this.InstalledPathsItemsControl.Items.Refresh();
                }
            };
        }

        private void SetupEventTriggers()
        {
            var thisPathEventTriggers = Interaction.GetTriggers(this.AssociatedObject).OfType<EventTrigger>().ToList();

            EventTrigger mouseEnterEventTrigger =
                thisPathEventTriggers.Single(trigger => trigger.EventName == nameof(UIElement.MouseEnter));
            EventTrigger mouseLeaveEventTrigger =
                thisPathEventTriggers.Single(trigger => trigger.EventName == nameof(UIElement.MouseLeave));
            EventTrigger mouseLeftButtonDownEventTrigger =
                thisPathEventTriggers.Single(trigger => trigger.EventName == nameof(UIElement.MouseLeftButtonDown));

            var mouseEnterStoryboardAction = new ControlStoryboardAction
                {Storyboard = (Storyboard) this.InstalledPathsItemsControl.FindResource("OnMouseEnter")};
            var mouseLeaveStoryboardAction = new ControlStoryboardAction
                {Storyboard = (Storyboard) this.InstalledPathsItemsControl.FindResource("OnMouseLeave")};
            var mouseLeaveFromSelectedStoryboardAction = new ControlStoryboardAction
                {Storyboard = (Storyboard) this.InstalledPathsItemsControl.FindResource("OnSelectedMouseLeave")};
            var mouseLeftButtonDownStoryboardAction = new ControlStoryboardAction
                {Storyboard = (Storyboard) this.InstalledPathsItemsControl.FindResource("OnMouseLeftButtonDown")};

            mouseEnterEventTrigger.Actions.Add(mouseEnterStoryboardAction);
            mouseLeaveEventTrigger.Actions.Add(mouseLeaveStoryboardAction);
            mouseLeftButtonDownEventTrigger.Actions.Add(mouseLeftButtonDownStoryboardAction);

            mouseLeftButtonDownEventTrigger.PreviewInvoke += (sender, args) =>
            {
                if (oldSelectedPath != null && !Equals(oldSelectedPath, this.AssociatedObject))
                {
                    oldSelectedPath.Tag = null;
                    var oldSelectedPathTrigger = Interaction.GetTriggers(oldSelectedPath).SingleOrDefault(trigger =>
                        trigger is EventTrigger eventTrigger &&
                        eventTrigger.EventName ==
                        nameof(UIElement.MouseLeave));
                    oldSelectedPathTrigger.Actions.RemoveAt(oldSelectedPathTrigger.Actions.Count - 1);
                    var newAction = new ControlStoryboardAction
                        {Storyboard = (Storyboard) this.InstalledPathsItemsControl.FindResource("OnMouseLeave")};
                    ColorAnimation colorAnimation = (ColorAnimation) newAction.Storyboard.Children[0];
                    colorAnimation.SetValue(Storyboard.TargetProperty, oldSelectedPath);
                    oldSelectedPathTrigger.Actions.Add(newAction);
                    oldSelectedPath.Fill.BeginAnimation(SolidColorBrush.ColorProperty, FadeAnimation);
                }

                oldSelectedPath = this.AssociatedObject;
                this.AssociatedObject.Tag = "Selected";
                mouseLeaveEventTrigger.Actions.RemoveAt(mouseLeaveEventTrigger.Actions.Count - 1);
                var ca = (ColorAnimation) mouseLeaveFromSelectedStoryboardAction.Storyboard.Children[0];
                ca.SetValue(Storyboard.TargetProperty, this.AssociatedObject);
                mouseLeaveEventTrigger.Actions.Add(mouseLeaveFromSelectedStoryboardAction);
            };

            var associatedTreeViewItem = this.InstalledSectionsTreeView.FindChild<TreeViewItem>(item =>
                ((ShaftSection) item.DataContext).Equals(
                    (ShaftSection) this.AssociatedObject.DataContext));

            if (associatedTreeViewItem == null)
            {
                return;
            }

            associatedTreeViewItem.MouseEnter += (sender, args) =>
                this.AssociatedObject.RaiseEvent(
                    new MouseEventArgs(Mouse.PrimaryDevice, new TimeSpan(DateTime.Now.Ticks).Milliseconds)
                        {RoutedEvent = UIElement.MouseEnterEvent});
            associatedTreeViewItem.MouseLeave += (sender, args) =>
                this.AssociatedObject.RaiseEvent(
                    new MouseEventArgs(Mouse.PrimaryDevice, new TimeSpan(DateTime.Now.Ticks).Milliseconds)
                        {RoutedEvent = UIElement.MouseLeaveEvent});
            associatedTreeViewItem.Selected += (sender, args) =>
                this.AssociatedObject.RaiseEvent(
                    new MouseButtonEventArgs(Mouse.PrimaryDevice, new TimeSpan(DateTime.Now.Ticks).Milliseconds,
                            MouseButton.Left)
                        {RoutedEvent = UIElement.MouseLeftButtonDownEvent});
        }

        private void SetTreeViewItemSectioRelativePath()
        {
            var pathAssociatedSection = (ShaftSection) this.AssociatedObject.DataContext;

            const string borderTag = "SectionTreeViewItemButtonBorder";
            var relativeTreeViewItemSectionBorders = this.InstalledSectionsTreeView
                                                         .FindChild<TreeViewItem>(
                                                             item => ((ShaftSection) item.DataContext)
                                                                 .Equals(pathAssociatedSection))
                                                         .FindChildren<Border>(border =>
                                                             (string) border.Tag == borderTag);

            foreach (var border in relativeTreeViewItemSectionBorders)
            {
                ShowHideSectionFeatureButtons.SetRelativePath(border,
                    relativePath: this.AssociatedObject);
            }
        }
    }
}