using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;

namespace InventorShaftGenerator.Extensions
{
    public class SetterAction : TargetedTriggerAction<FrameworkElement>
    {
        public string PropertyName
        {
            get => (string) GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(SetterAction),
                new PropertyMetadata(String.Empty));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SetterAction),
                new PropertyMetadata(null));

        protected override void Invoke(object parameter)
        {
            var target = this.TargetObject ?? this.AssociatedObject;

            var targetType = target.GetType();

            var property = targetType.GetProperty(this.PropertyName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"Property not found: {this.PropertyName}");
            }

            if (property.CanWrite == false)
            {
                throw new ArgumentException($"Property is not settable: {this.PropertyName}");
            }

            object convertedValue;

            if (this.Value == null)
            {
                convertedValue = null;
            }
            else
            {
                var typeOfValue = this.Value.GetType();
                var typeOfProperty = property.PropertyType;

                if (typeOfValue == typeOfProperty)
                {
                    convertedValue = this.Value;
                }
                else
                {
                    var propertyConverter = TypeDescriptor.GetConverter(typeOfProperty);

                    if (propertyConverter.CanConvertFrom(typeOfValue))
                    {
                        convertedValue = propertyConverter.ConvertFrom(this.Value);
                    }
                    else if (typeOfValue.IsSubclassOf(typeOfProperty))
                    {
                        convertedValue = this.Value;
                    }
                    else
                    {
                        throw new ArgumentException($"Cannot convert type '{typeOfValue}' to '{typeOfProperty}'.");
                    }
                }
            }

            property.SetValue(target, convertedValue);
        }
    }
}