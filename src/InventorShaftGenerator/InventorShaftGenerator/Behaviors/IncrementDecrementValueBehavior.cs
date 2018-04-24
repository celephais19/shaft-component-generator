using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using InventorShaftGenerator.CustomizedUIElements;
using InventorShaftGenerator.Extensions;

namespace InventorShaftGenerator.Behaviors
{
    public class IncrementDecrementValueBehavior : Behavior<RepeatButton>
    {
        private static Grid ancestorGrid;

        private static readonly DependencyProperty DecrementProperty =
            DependencyProperty.RegisterAttached(
                "Decrement", typeof(bool), typeof(IncrementDecrementValueBehavior),
                new PropertyMetadata(false));

        public static bool GetDecrement(DependencyObject obj)
        {
            return (bool) obj.GetValue(DecrementProperty);
        }

        public static void SetDecrement(DependencyObject obj, bool value)
        {
            obj.SetValue(DecrementProperty, value);
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.Click += (sender, args) =>
            {
                ancestorGrid = this.AssociatedObject.FindAncestor<Grid>(2);
                ButtonOnClick(this.AssociatedObject);
            };
        }

        private static void ButtonOnClick(DependencyObject sender)
        {
            var grid = ancestorGrid;
            var textBox = grid.FindChild<NumericUnitTextBox>();
            var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
            float value = float.Parse(textBox.Text, CultureInfo.InvariantCulture);

            bool decrementButtonClicked = GetDecrement(sender);
            if (decrementButtonClicked)
            {
                if (--value < 0 && !textBox.AllowNegative)
                {
                    value = 0;
                }
            }
            else
            {
                value++;
            }


            string stringFormat = bindingExpression.ParentBinding.StringFormat;

            textBox.Text = value.ToString(stringFormat, CultureInfo.InvariantCulture);
        }
    }
}