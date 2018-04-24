using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using InventorShaftGenerator.AttachedProperties;
using InventorShaftGenerator.CustomizedUIElements.Extensions;
using InventorShaftGenerator.Extensions;


namespace InventorShaftGenerator.Behaviors
{
    public class ShowHideSectionFeatureButtons : Behavior<Border>
    {
        public static readonly DependencyProperty RelativePathProperty = DependencyProperty.RegisterAttached(
            "RelativePath", typeof(Path), typeof(ShowHideSectionFeatureButtons), new PropertyMetadata(default(Path)));

        private static readonly SolidColorBrush ErrorColor =
            (SolidColorBrush) new BrushConverter().ConvertFrom("#fdadad");

        private TreeView parentTreeView;
        private TreeViewItem parentTreeViewItem;
        private IEnumerable<StackPanel> childStackPanels;

        private static readonly ThicknessAnimation AppearBorderThicknessAnimation =
            new ThicknessAnimation(new Thickness(0), new Thickness(1), new Duration(TimeSpan.FromSeconds(0.125)),
                FillBehavior.HoldEnd);

        private static readonly ThicknessAnimation FadeBorderThicknessAnimation =
            new ThicknessAnimation(new Thickness(1), new Thickness(0), new Duration(TimeSpan.FromSeconds(0.125)),
                FillBehavior.HoldEnd);

        private static readonly DoubleAnimation AppearAnimation =
            new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.125)), FillBehavior.HoldEnd);

        private static readonly DoubleAnimation FadeAnimation =
            new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.125)), FillBehavior.HoldEnd);

        private static readonly BrushEqualityComparer BrushEqualityComparer = new BrushEqualityComparer();

        public static Path GetRelativePath(DependencyObject dependencyObject)
        {
            return (Path) dependencyObject.GetValue(RelativePathProperty);
        }

        public static void SetRelativePath(DependencyObject dependencyObject, Path relativePath)
        {
            dependencyObject.SetValue(RelativePathProperty, relativePath);
        }

        protected override void OnAttached()
        {
            var ancestorGrid = this.AssociatedObject.FindAncestor<Grid>(ancestorLevel: 2);
            ancestorGrid.Loaded += (sender, args) =>
            {
                var thisBorder = this.AssociatedObject;
                this.childStackPanels = ancestorGrid.FindChildren<StackPanel>()
                                                    .Where(ControlIndicator
                                                        .GetIsSectionFeatureStackPanel).ToList();
                this.parentTreeViewItem = ancestorGrid.FindAncestor<TreeViewItem>();
                this.parentTreeView = this.parentTreeViewItem.FindAncestor<TreeView>();

                this.parentTreeViewItem.Selected += (o, eventArgs) =>
                {
                    if (this.parentTreeViewItem.FindChildren<TreeViewItem>()
                            .Contains(eventArgs.OriginalSource as TreeViewItem))
                    {
                        foreach (var stackPanel in this.childStackPanels)
                        {
                            if (!BrushEqualityComparer.Equals(stackPanel.Background, ErrorColor))
                            {
                                stackPanel.Background = Brushes.White;
                            }
                        }

                        return;
                    }

                    foreach (var stackPanel in this.childStackPanels)
                    {
                        if (!BrushEqualityComparer.Equals(stackPanel.Background, ErrorColor))
                        {
                            stackPanel.Background =
                                this.parentTreeViewItem.IsMouseOver ? Brushes.White : Brushes.Transparent;
                        }
                    }
                };

                this.parentTreeViewItem.Unselected += (o, eventArgs) =>
                {
                    foreach (var stackPanel in this.childStackPanels)
                    {
                        if (!BrushEqualityComparer.Equals(stackPanel.Background, ErrorColor))
                        {
                            stackPanel.Background = Brushes.Transparent;
                        }
                    }
                };

                this.parentTreeViewItem.MouseEnter += OnParentTreeViewItemMouseEnter;
                /*this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Path relativePath = GetRelativePath(thisBorder);
                    while (relativePath == null)
                    {
                        relativePath = GetRelativePath(thisBorder);
                    }

                    relativePath.MouseEnter += OnParentTreeViewItemMouseEnter;
                }));*/

                thisBorder.BorderThickness = new Thickness(0);
                List<Button> contextMenuButtons = ancestorGrid.FindChildren<Button>().ToList();
                List<ContextMenu> contextMenus = new List<ContextMenu>(3);

                foreach (var contextMenuButton in contextMenuButtons)
                {
                    if (SectionFeatureContextMenuBehavior.GetContextMenu(contextMenuButton) is ContextMenu
                        contextMenu)
                    {
                        contextMenu.Closed += (o, eventArgs) =>
                        {
                            var depObj = Mouse.DirectlyOver as DependencyObject;
                            bool isOverGrid;
                            if (depObj is ContextMenu cm)
                            {
                                Grid parentGrid = cm.PlacementTarget.FindAncestor<Grid>();

                                isOverGrid = Equals(parentGrid, ancestorGrid);
                            }
                            else
                            {
                                Grid parentGrid = depObj.FindAncestor<Grid>();

                                isOverGrid = Equals(parentGrid, ancestorGrid);
                            }

                            if (isOverGrid && this.parentTreeViewItem.IsSelected)
                            {
                                foreach (var panel in this.childStackPanels)
                                {
                                    if (!BrushEqualityComparer.Equals(panel.Background, ErrorColor))
                                    {
                                        panel.Background = Brushes.White;
                                    }
                                }
                            }
                            else if (contextMenuButtons.Any(button => button.IsMouseOver) &&
                                     this.parentTreeViewItem.IsSelected)
                            {
                                foreach (var panel in this.childStackPanels)
                                {
                                    if (!BrushEqualityComparer.Equals(panel.Background, ErrorColor))
                                    {
                                        panel.Background = this.parentTreeViewItem.IsMouseOver
                                            ? Brushes.White
                                            : Brushes.Transparent;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var panel in this.childStackPanels)
                                {
                                    if (!BrushEqualityComparer.Equals(panel.Background, ErrorColor))
                                    {
                                        panel.Background = Brushes.Transparent;
                                    }
                                }
                            }
                        };
                        contextMenus.Add(contextMenu);
                    }
                }

                foreach (var button in contextMenuButtons)
                {
                    button.Visibility = Visibility.Hidden;
                }

                ancestorGrid.MouseEnter += (s, a) =>
                {
                    thisBorder.BeginAnimation(Border.BorderThicknessProperty, AppearBorderThicknessAnimation);
                    foreach (var contextMenuButton in contextMenuButtons)
                    {
                        contextMenuButton.BeginAnimation(UIElement.OpacityProperty, AppearAnimation);
                        contextMenuButton.Visibility = Visibility.Visible;
                    }

                    foreach (var panel in this.childStackPanels)
                    {
                        if (!BrushEqualityComparer.Equals(panel.Background, ErrorColor))
                        {
                            panel.Background = Brushes.White;
                        }
                    }
                };

                contextMenus.ForEach(menu => menu.Closed += (o, eventArgs) =>
                {
                    if (ancestorGrid.IsMouseOver || contextMenuButtons.Any(button => button.IsMouseOver) ||
                        contextMenus.Any(contextMenu => contextMenu.IsOpen))
                    {
                        return;
                    }

                    thisBorder.BeginAnimation(Border.BorderThicknessProperty, FadeBorderThicknessAnimation);
                    foreach (var button in contextMenuButtons)
                    {
                        button.BeginAnimation(UIElement.OpacityProperty, FadeAnimation);
                        button.Visibility = Visibility.Hidden;
                    }
                });

                ancestorGrid.MouseLeave += (s, a) =>
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (contextMenus.Any(menu => menu.IsOpen))
                        {
                            return;
                        }

                        thisBorder.BeginAnimation(Border.BorderThicknessProperty, FadeBorderThicknessAnimation);
                        foreach (var button in contextMenuButtons)
                        {
                            button.BeginAnimation(UIElement.OpacityProperty, FadeAnimation);
                            button.Visibility = Visibility.Hidden;
                        }

                        foreach (var panel in this.childStackPanels)
                        {
                            if (!BrushEqualityComparer.Equals(panel.Background, ErrorColor))
                            {
                                panel.Background = Brushes.Transparent;
                            }
                        }
                    }));
                };
            };
        }

        private void OnParentTreeViewItemMouseEnter(object sender, MouseEventArgs eventArgs)
        {
            if (this.parentTreeViewItem.IsSelected)
            {
                return;
            }

            if (Equals(this.parentTreeView.SelectedItem as TreeViewItem, this.parentTreeViewItem) ||
                Equals(this.parentTreeViewItem, eventArgs.OriginalSource))
            {
                return;
            }

            foreach (var panel in this.childStackPanels)
            {
                if (!BrushEqualityComparer.Equals(panel.Background, ErrorColor))
                {
                    panel.Background = this.parentTreeViewItem.IsMouseOver ? Brushes.White : Brushes.Transparent;
                }
            }
        }
    }
}