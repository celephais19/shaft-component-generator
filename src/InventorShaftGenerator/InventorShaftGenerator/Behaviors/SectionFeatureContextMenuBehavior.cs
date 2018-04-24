using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.ViewModels;

namespace InventorShaftGenerator.Behaviors
{
    public class SectionFeatureContextMenuBehavior
    {
        private static readonly DoubleAnimation AppearAnimation =
            new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.HoldEnd);

        private static readonly DoubleAnimation FadeAnimation =
            new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.HoldEnd);

        private static readonly DependencyProperty PlacementAtProperty =
            DependencyProperty.RegisterAttached(
                "PlacementAt", typeof(UIElement), typeof(SectionFeatureContextMenuBehavior),
                new PropertyMetadata(HandlePropertyChanged));

        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.RegisterAttached(
                "ContextMenu", typeof(ContextMenu), typeof(SectionFeatureContextMenuBehavior));

        public static readonly DependencyProperty SectionProperty = DependencyProperty.RegisterAttached("Section",
            typeof(ShaftSection), typeof(SectionFeatureContextMenuBehavior));

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.RegisterAttached("DataContext", typeof(MainViewModel),
                typeof(SectionFeatureContextMenuBehavior));

        public static readonly DependencyProperty IsFeatureAvailableProperty =
            DependencyProperty.RegisterAttached("IsFeatureAvailable", typeof(EdgeFeature?),
                typeof(SectionFeatureContextMenuBehavior));

        public static UIElement GetPlacementAt(UIElement obj)
        {
            return (UIElement) obj.GetValue(PlacementAtProperty);
        }

        public static void SetPlacementAt(UIElement obj, UIElement value)
        {
            obj.SetValue(PlacementAtProperty, value);
        }

        public static ContextMenu GetContextMenu(UIElement element)
        {
            return (ContextMenu) element.GetValue(ContextMenuProperty);
        }

        public static void SetContextMenu(UIElement element, ContextMenu value)
        {
            element.SetValue(ContextMenuProperty, value);
        }

        public static ShaftSection GetSection(UIElement element)
        {
            return (ShaftSection) element.GetValue(SectionProperty);
        }

        public static void SetSection(UIElement element, ShaftSection value)
        {
            element.SetValue(SectionProperty, value);
        }

        public static MainViewModel GetDataContext(UIElement element)
        {
            return (MainViewModel) element.GetValue(DataContextProperty);
        }

        public static void SetDataContext(UIElement element, MainViewModel value)
        {
            element.SetValue(DataContextProperty, value);
        }

        public static EdgeFeature? GetIsFeatureAvailable(UIElement obj)
        {
            return (EdgeFeature?) obj.GetValue(IsFeatureAvailableProperty);
        }

        public static void SetIsFeatureAvailable(UIElement obj, EdgeFeature? value)
        {
            obj.SetValue(IsFeatureAvailableProperty, value);
        }

        private static void HandlePropertyChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is Button button)
            {
                button.PreviewMouseLeftButtonDown -= ExecuteMouseLeftButtonDown;
                button.PreviewMouseLeftButtonDown += ExecuteMouseLeftButtonDown;
            }
        }

        private static void ExecuteMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            UIElement button = sender as UIElement;

            if (GetIsFeatureAvailable(button).HasValue && GetIsFeatureAvailable(button) == EdgeFeature.NotAvailable)
            {
                return;
            }

            ContextMenu contextMenu = GetContextMenu(button);
            contextMenu.Placement = PlacementMode.Bottom;
            var placementTarget = GetPlacementAt(button);
            contextMenu.PlacementTarget = placementTarget;
            var mainViewModel = GetDataContext(button);
            var section = GetSection(button);
            mainViewModel.SelectedSection = section;
            contextMenu.Opacity = 0;

            switch (contextMenu.Name)
            {
                case "FirstEdgeContextMenu":
                    mainViewModel.ActiveEdgeFeaturePosition = EdgeFeaturePosition.FirstEdge;
                    break;
                case "SecondEdgeContextMenu":
                    mainViewModel.ActiveEdgeFeaturePosition = EdgeFeaturePosition.SecondEdge;
                    break;
                case "SubFeatureContextMenu":
                    var subfeatures = Enum.GetValues(typeof(SubFeature)).Cast<SubFeature>().ToList();
                    switch (mainViewModel.SelectedSection)
                    {
                        case CylinderSection cylinderSection when cylinderSection.IsBore:
                            contextMenu.ItemsSource = new[]
                                {subfeatures.Single(s => s == SubFeature.RetainingRingBore)};
                            break;
                        
                        case CylinderSection _:
                            subfeatures.Remove(SubFeature.RetainingRingBore);
                            contextMenu.ItemsSource = subfeatures;
                            break;

                        case ConeSection _:
                            contextMenu.ItemsSource = null;
                            break;
                        case PolygonSection _:
                            contextMenu.ItemsSource = new[] {subfeatures.Single(s => s == SubFeature.ThroughHole)};
                            break;
                    }

                    break;
            }

            if (sender is Button)
            {
                contextMenu.IsOpen = true;
                contextMenu.BeginAnimation(UIElement.OpacityProperty, AppearAnimation);
            }
        }
    }
}