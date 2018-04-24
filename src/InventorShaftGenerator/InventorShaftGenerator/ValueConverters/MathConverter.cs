using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InventorShaftGenerator.ValueConverters
{
    public class MathConverter : ValueConverterWithAttachedParameter
    {
        private static readonly char[] AllOperators = {'+', '-', '*', '/', '%', '(', ')'};

        private static readonly List<string> Grouping = new List<string> {"(", ")"};
        private static readonly List<string> Operators = new List<string> {"+", "-", "*", "/", "%"};

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mathEquation = parameter as string;
            mathEquation = mathEquation.Replace(" ", "");
            mathEquation = mathEquation.Replace("@VALUE", value.ToString());

            var numbers = new List<double>();

            foreach (string s in mathEquation.Split(AllOperators))
            {
                if (s == string.Empty)
                {
                    continue;
                }

                if (double.TryParse(s, out double tmp))
                {
                    numbers.Add(tmp);
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            EvaluateMathString(ref mathEquation, ref numbers, 0);

            return numbers[0];
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void EvaluateMathString(ref string mathEquation, ref List<double> numbers, int index)
        {
            string token = GetNextToken(mathEquation);

            while (token != string.Empty)
            {
                mathEquation = mathEquation.Remove(0, token.Length);

                if (Grouping.Contains(token))
                {
                    switch (token)
                    {
                        case "(":
                            EvaluateMathString(ref mathEquation, ref numbers, index);
                            break;

                        case ")":
                            return;
                    }
                }

                if (Operators.Contains(token))
                {
                    string nextToken = GetNextToken(mathEquation);
                    if (nextToken == "(")
                    {
                        EvaluateMathString(ref mathEquation, ref numbers, index + 1);
                    }

                    // Verify that enough numbers exist in the List<double> to complete the operation
                    // and that the next token is either the number expected, or it was a ( meaning
                    // that this was called recursively and that the number changed
                    if (numbers.Count > (index + 1) &&
                        (Math.Abs(double.Parse(nextToken) - numbers[index + 1]) < double.Epsilon || nextToken == "("))
                    {
                        switch (token)
                        {
                            case "+":
                                numbers[index] = numbers[index] + numbers[index + 1];
                                break;
                            case "-":
                                numbers[index] = numbers[index] - numbers[index + 1];
                                break;
                            case "*":
                                numbers[index] = numbers[index] * numbers[index + 1];
                                break;
                            case "/":
                                numbers[index] = numbers[index] / numbers[index + 1];
                                break;
                            case "%":
                                numbers[index] = numbers[index] % numbers[index + 1];
                                break;
                        }

                        numbers.RemoveAt(index + 1);
                    }
                    else
                    {
                        throw new FormatException("Next token is not the expected number");
                    }
                }

                token = GetNextToken(mathEquation);
            }
        }

        private string GetNextToken(string mathEquation)
        {
            if (mathEquation == string.Empty)
            {
                return string.Empty;
            }

            string tmp = "";
            foreach (char c in mathEquation)
            {
                if (AllOperators.Contains(c))
                {
                    return (tmp == "" ? c.ToString() : tmp);
                }

                tmp += c;
            }

            return tmp;
        }
    }
}