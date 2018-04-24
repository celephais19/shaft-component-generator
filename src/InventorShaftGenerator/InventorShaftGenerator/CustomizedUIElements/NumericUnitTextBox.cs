using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using InventorShaftGenerator.CustomizedUIElements.Extensions;

namespace InventorShaftGenerator.CustomizedUIElements
{
    public class NumericUnitTextBox : UnitTextBox
    {
        public NumericUnitTextBox()
        {
            this.Loaded += TextBox_Loaded;
            CommandManager.AddPreviewExecutedHandler(this, OnPreviewExecuted);
            this.MaxLength = 9;
            this.UnitPadding = new Thickness(0.5);
        }

        protected bool IsManagingInput { get; set; }

        protected bool RequireNegativeHandling { get; set; }

        protected bool NegativeIntegerPending { get; set; }

        protected bool NegativeFractionPending { get; set; }

        protected void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsManagingInput = false;

            var binding = BindingOperations.GetBindingBase(this, TextBox.TextProperty);
            var stringFormat = binding?.StringFormat;

            if (!string.IsNullOrWhiteSpace(stringFormat))
            {
                if (stringFormat[0] == '{')
                {
                    var begin = stringFormat.IndexOf(':');
                    var end = stringFormat.IndexOf('}');
                    if (begin != -1 && end != -1)
                    {
                        stringFormat = stringFormat.Substring(begin + 1, end - begin - 1);
                    }
                }

                switch (stringFormat[0])
                {
                    case 'f':
                    case 'F':
                    case 'n':
                    case 'N':
                        this.IsManagingInput = true;
                        break;

                    case '0':
                    case '#':
                        bool foundDecimalPoint = false;
                        bool foundZeroAfterDecimalPoint = false;

                        for (int i = 1; i < stringFormat.Length; i++)
                        {
                            if (stringFormat[i] == '.')
                            {
                                foundDecimalPoint = true;
                            }
                            else if (stringFormat[i] != '0' && stringFormat[i] != '#')
                            {
                                break;
                            }
                            else if (foundDecimalPoint)
                            {
                                if (stringFormat[i] == '0')
                                {
                                    foundZeroAfterDecimalPoint = true;
                                }
                            }
                        }

                        if (foundDecimalPoint && foundZeroAfterDecimalPoint)
                        {
                            this.IsManagingInput = true;

                            if (stringFormat[stringFormat.Length - 1] != '.')
                            {
                                this.RequireNegativeHandling = true;
                            }
                        }

                        break;
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (this.IsManagingInput)
            {
                this.NegativeIntegerPending = false;
                this.NegativeFractionPending = false;

                switch (e.Key)
                {
                    case Key.OemPeriod:
                    case Key.Decimal:
                        if (this.CaretIndex < this.Text.Length && this.Text[this.CaretIndex] == '.')
                        {
                            this.CaretIndex++;
                            e.Handled = true;
                        }
                        else if (this.Text.Contains(".") && !this.SelectedText.Contains("."))
                        {
                            e.Handled = true;
                        }

                        break;

                    case Key.Back:
                        if (this.CaretIndex > 0 && this.CaretIndex <= this.Text.Length && this.Text[this.CaretIndex - 1] == '.' &&
                            this.Text.HasOnlyOne('.'))
                        {
                            this.CaretIndex--;
                            e.Handled = true;
                        }

                        break;

                    case Key.Delete:
                        if (this.CaretIndex < this.Text.Length && this.Text[this.CaretIndex] == '.' && this.Text.HasOnlyOne('.'))
                        {
                            this.CaretIndex++;
                            e.Handled = true;
                        }

                        break;

                    case Key.D0:
                    case Key.NumPad0:
                    case Key.D1:
                    case Key.NumPad1:
                    case Key.D2:
                    case Key.NumPad2:
                    case Key.D3:
                    case Key.NumPad3:
                    case Key.D4:
                    case Key.NumPad4:
                    case Key.D5:
                    case Key.NumPad5:
                    case Key.D6:
                    case Key.NumPad6:
                    case Key.D7:
                    case Key.NumPad7:
                    case Key.D8:
                    case Key.NumPad8:
                    case Key.D9:
                    case Key.NumPad9:
                        if (this.RequireNegativeHandling)
                        {
                            if (this.Text == "-")
                            {
                                this.NegativeIntegerPending = true;
                            }
                            else if (this.Text == "-.")
                            {
                                this.NegativeFractionPending = true;
                            }
                        }

                        break;

                    case Key.OemMinus:
                    case Key.Subtract:
                        if (!((this.CaretIndex == 0 && this.Text.IndexOf('-') == -1) ||
                              this.SelectionLength == this.Text.Length))
                        {
                            e.Handled = true;
                        }

                        break;
                }
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (e.Text == "-")
            {
                if (!this.AllowNegative || this.Text[0] == '-')
                {
                    e.Handled = true;
                }

                return;
            }

            Regex regex = new Regex("[0-9]");
            bool isMatch = regex.IsMatch(e.Text);
            e.Handled = !isMatch;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            int decimalIndex = this.Text.IndexOf('.');
            if (this.NegativeIntegerPending && decimalIndex > -1)
            {
                this.CaretIndex = decimalIndex;
            }
            else if (this.NegativeFractionPending && decimalIndex > -1)
            {
                this.CaretIndex = decimalIndex + 2;
            }
        }

        private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (executedRoutedEventArgs.Command == ApplicationCommands.Paste)
            {
                executedRoutedEventArgs.Handled = true;
            }
        }
    }
}