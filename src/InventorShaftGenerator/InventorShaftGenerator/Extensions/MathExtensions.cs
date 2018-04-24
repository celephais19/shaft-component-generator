using System;

namespace InventorShaftGenerator.Extensions
{
    public static class MathExtensions
    {
        public static double DegreesToRadians(double angle)
        {
            return Math.PI * angle / 180;
        }
    }
}
