using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InventorShaftGenerator.CustomizedUIElements
{
    public class UnitTextBox : TextBox
    {
        private FormattedText unitText;
        private Rect unitTextBounds;

        public static DependencyProperty UnitTextProperty =
            DependencyProperty.Register(
                name: "UnitText",
                propertyType: typeof(string),
                ownerType: typeof(UnitTextBox),
                typeMetadata: new FrameworkPropertyMetadata(
                    default(string),
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public string UnitText
        {
            get => (string) GetValue(UnitTextProperty);
            set => SetValue(UnitTextProperty, value);
        }

        public static DependencyProperty UnitPaddingProperty =
            DependencyProperty.Register(
                "UnitPadding",
                typeof(Thickness),
                typeof(UnitTextBox),
                new FrameworkPropertyMetadata(
                    new Thickness(5d, 0d, 0d, 0d),
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public Thickness UnitPadding
        {
            get => (Thickness) GetValue(UnitPaddingProperty);
            set => SetValue(UnitPaddingProperty, value);
        }

        public static DependencyProperty TextBoxWidthProperty =
            DependencyProperty.Register(
                "TextBoxWidth",
                typeof(double),
                typeof(UnitTextBox),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty AllowNegativeProperty = DependencyProperty.Register(
            "AllowNegative", typeof(bool), typeof(UnitTextBox), new PropertyMetadata(default(bool)));

        public bool AllowNegative
        {
            get => (bool) GetValue(AllowNegativeProperty);
            set => SetValue(AllowNegativeProperty, value);
        }

        public double TextBoxWidth
        {
            get => (double) GetValue(TextBoxWidthProperty);
            set => SetValue(TextBoxWidthProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ForegroundProperty)
            {
                EnsureUnitText(invalidate: true);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var textBoxWidth = this.TextBoxWidth;
            var unit = EnsureUnitText(invalidate: true);
            var padding = this.UnitPadding;

            if (unit != null)
            {
                var unitWidth = unit.Width + padding.Left + padding.Right;
                var unitHeight = unit.Height + padding.Top + padding.Bottom;

                constraint = new Size(
                    constraint.Width - unitWidth,
                    Math.Max(constraint.Height, unitHeight));
            }

            var hasFixedTextBoxWidth = !double.IsNaN(textBoxWidth) &&
                                       !double.IsInfinity(textBoxWidth);

            if (hasFixedTextBoxWidth)
            {
                constraint = new Size(textBoxWidth, constraint.Height);
            }

            var baseSize = base.MeasureOverride(constraint);
            var baseWidth = hasFixedTextBoxWidth ? textBoxWidth : baseSize.Width;

            if (unit != null)
            {
                var unitWidth = unit.Width + padding.Left + padding.Right;
                var unitHeight = unit.Height + padding.Top + padding.Bottom;

                return new Size(
                    baseWidth + unitWidth,
                    Math.Max(baseSize.Height, unitHeight));
            }

            return new Size(baseWidth, baseSize.Height);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var textSize = arrangeBounds;
            var unit = EnsureUnitText();
            var padding = this.UnitPadding;

            if (unit != null)
            {
                var unitWidth = unit.Width + padding.Left + padding.Right;
                var unitHeight = unit.Height + padding.Top + padding.Bottom;

                textSize.Width -= unitWidth;

                this.unitTextBounds = new Rect(
                    textSize.Width + padding.Left,
                    (arrangeBounds.Height - unitHeight) / 2 + padding.Top,
                    textSize.Width,
                    textSize.Height);
            }

            var baseSize = base.ArrangeOverride(textSize);

            if (unit != null)
            {
                var unitWidth = unit.Width + padding.Left + padding.Right;
                var unitHeight = unit.Height + padding.Top + padding.Bottom;

                return new Size(
                    baseSize.Width + unitWidth,
                    Math.Max(baseSize.Height, unitHeight));
            }

            return baseSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var unitText = EnsureUnitText();
            if (unitText != null)
                drawingContext.DrawText(unitText, this.unitTextBounds.Location);
        }

        private FormattedText EnsureUnitText(bool invalidate = false)
        {
            if (invalidate)
            {
                this.unitText = null;
            }

            if (this.unitText != null)
            {
                return this.unitText;
            }

            var unit = this.UnitText;

            if (!string.IsNullOrEmpty(unit))
            {
                this.unitText = new FormattedText(
                    unit,
                    CultureInfo.InvariantCulture,
                    this.FlowDirection,
                    new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                    this.FontSize,
                    this.Foreground);
            }

            return this.unitText;
        }
    }
}