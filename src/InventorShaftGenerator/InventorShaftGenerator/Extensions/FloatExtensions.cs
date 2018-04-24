using System;
using System.Diagnostics;

namespace InventorShaftGenerator.Extensions
{
    public static class FloatExtensions
    {
        public static bool IsGreaterThanZero(this float value)
        {
            bool isGreaterThanZero = Math.Abs(value) > float.Epsilon;
            return isGreaterThanZero;
        }

        public static bool NearlyEqual(this float a, float b, float epsilon = float.Epsilon)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            {
                return true;
            }

            if (a == 0 || b == 0 || diff < float.Epsilon)
            {
                return diff < epsilon;
            }

            return (diff / (absA + absB)) < epsilon;
        }

        [DebuggerStepThrough]
        public static float InMillimeters(this float f)
        {
            return f / 10;
        }
    }
}
