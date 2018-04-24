using System.Windows;

namespace InventorShaftGenerator.AttachedProperties
{
    public class ControlIndicator
    {
        public static readonly DependencyProperty IsSectionFeatureStackPanelProperty =
            DependencyProperty.RegisterAttached(
                "IsSectionFeatureStackPanel", typeof(bool), typeof(ControlIndicator),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsSectionGridProperty =
            DependencyProperty.RegisterAttached(
                "IsSectionGrid", typeof(bool), typeof(ControlIndicator),
                new PropertyMetadata(false));

        public static bool GetIsSectionFeatureStackPanel(DependencyObject dependencyObject)
        {
            return (bool) dependencyObject.GetValue(IsSectionFeatureStackPanelProperty);
        }

        public static void SetIsSectionFeatureStackPanel(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsSectionFeatureStackPanelProperty, value);
        }

        public static bool GetIsSectionGrid(DependencyObject dependencyObject)
        {
            return dependencyObject != null && (bool) dependencyObject.GetValue(IsSectionGridProperty);
        }

        public static void SetIsSectionGrid(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsSectionGridProperty, value);
        }
    }
}