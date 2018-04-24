using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using InventorShaftGenerator.Extensions;

namespace InventorShaftGenerator.Behaviors
{
    // ReSharper disable once InconsistentNaming
    public class ShowHideUIElementBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty AncestorGridLevelProperty = DependencyProperty.RegisterAttached(
            "AncestorGridLevel", typeof(int), typeof(ShowHideUIElementBehavior),
            new UIPropertyMetadata(1));

        public static int GetAncestorGridLevel(DependencyObject dependencyObject)
        {
            return (int) dependencyObject.GetValue(AncestorGridLevelProperty);
        }

        public static void SetAncestorGridLevel(DependencyObject dependencyObject, int gridLevel)
        {
            dependencyObject.SetValue(AncestorGridLevelProperty, gridLevel);
        }

        private readonly DoubleAnimation appearAnimation =
            new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.HoldEnd);

        private readonly DoubleAnimation fadeAnimation =
            new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.2)), FillBehavior.HoldEnd);

        protected override void OnAttached()
        {
            this.AssociatedObject.Visibility = Visibility.Hidden;

            var ancestorGrid =
                this.AssociatedObject.FindAncestor<Grid>(ancestorLevel: GetAncestorGridLevel(this));

            ancestorGrid.MouseEnter += (sender, args) =>
            {
                this.AssociatedObject.Visibility = Visibility.Visible;
                this.AssociatedObject.BeginAnimation(UIElement.OpacityProperty, this.appearAnimation);
            };
            ancestorGrid.MouseLeave += (sender, args) =>
            {
                this.AssociatedObject.BeginAnimation(UIElement.OpacityProperty, this.fadeAnimation);
                this.AssociatedObject.Visibility = Visibility.Hidden;
            };
        }
    }
}