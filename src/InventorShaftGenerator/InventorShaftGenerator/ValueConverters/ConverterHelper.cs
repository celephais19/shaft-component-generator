using System;
using System.ComponentModel;
using System.Linq;

namespace InventorShaftGenerator.ValueConverters
{
    public static class ConverterHelper
    {
        public static object Result(bool? comparedResult, Type targetType, object parameter, object nullValue = null,
                                    object trueValue = null, object falseValue = null)
        {
            var parameterString = parameter is string ? parameter.ToString() : string.Empty;
            if (parameterString.Contains("?"))
                parameterString = parameterString.Substring(parameterString.IndexOf("?") + 1);
            if (parameterString == string.Empty) return comparedResult;
            falseValue = falseValue ?? "false";
            trueValue = trueValue ?? "true";
            if (parameterString.ToLower() == "reverse")
            {
                var temp = falseValue;
                falseValue = trueValue;
                trueValue = temp;
            }
            else if (parameterString.Contains(":"))
            {
                var values = parameterString.Split(':');
                trueValue = values[0];
                falseValue = values[1];
                nullValue = values.Length > 2 ? values[2] : nullValue;
            }

            if (nullValue == null && comparedResult == null) return null;
            var returnValue = (comparedResult.HasValue ? (comparedResult.Value ? trueValue : falseValue) : nullValue)
                .ToString();

            return ConvertToType(returnValue, targetType);
        }

        public static object Result<T>(object value, Func<T, bool?> comparer, Type targetType, object parameter,
                                       object nullValue = null, object trueValue = null, object falseValue = null)
        {
            var comparedResult = value is T variable ? comparer(variable) : null;
            return Result(comparedResult, targetType, parameter, nullValue, trueValue, falseValue);
        }

        public static object ResultWithParameterValue(Func<string, bool?> comparer, Type targetType,
                                                      object parameter, object nullValue = null,
                                                      object trueValue = null, object falseValue = null)
        {
            var parameterString = parameter.ToString();
            var compareItems = parameterString.Split(':').Select(i => i.Split('?').ToArray()).ToArray();
            if (compareItems.Length > 1)
            {
                for (int i = 0; i < compareItems.Length - 1; i++)
                {
                    for (int j = 0; j < compareItems[i].Length - 1; j++)
                    {
                        var returnValue = Result(comparer(compareItems[i][j]), typeof(string), compareItems[i][j],
                            null, compareItems[i][compareItems[i].Length - 1], ":");
                        if (returnValue.ToString().ToLower() != ":") return ConvertToType(returnValue, targetType);
                    }
                }

                return ConvertToType(compareItems.Last().First(), targetType);
            }

            return ConvertToType(parameterString, targetType);
        }

        private static object ConvertToType(object value, Type targetType)
        {
            try
            {
                if (targetType == typeof(object)) return value;
                TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                return converter.ConvertFrom(value);
            }
            catch
            {
                var errorValue = value;
                throw new Exception(
                    $"Failed in attempt to convert {errorValue} to type {targetType.Name} using TypeConverter in class TrueFalseValues");
            }
        }
    }
}