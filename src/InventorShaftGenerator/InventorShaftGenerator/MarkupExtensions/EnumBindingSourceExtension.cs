using System;
using System.Windows.Markup;

namespace InventorShaftGenerator.MarkupExtensions
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type enumType;

        public EnumBindingSourceExtension()
        {
        }

        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        public Type EnumType
        {
            get => this.enumType;
            set
            {
                if (value == this.enumType)
                {
                    return;
                }

                if (null != value)
                {
                    Type enType = Nullable.GetUnderlyingType(value) ?? value;
                    if (!enType.IsEnum)
                    {
                        throw new ArgumentException("Type must be for an Enum");
                    }
                }

                this.enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this.enumType)
            {
                throw new InvalidOperationException("The EnumType must be specified");
            }

            Type actualEnumType = Nullable.GetUnderlyingType(this.enumType) ?? this.enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == this.enumType)
            {
                return enumValues;
            }


            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}