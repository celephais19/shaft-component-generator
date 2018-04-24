using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace InventorShaftGenerator.CustomizedUIElements.Extensions
{
    public class BrushEqualityComparer : IEqualityComparer<Brush>
    {
        public bool Equals(Brush x, Brush y)
        {
            var scb1 = (SolidColorBrush) x;
            var scb2 = (SolidColorBrush) y;
            return scb1 != null && scb2 != null && scb1.Color == scb2.Color &&
                   Math.Abs(scb1.Opacity - scb2.Opacity) < Double.Epsilon;
        }

        public int GetHashCode(Brush obj)
        {
            var scb = (SolidColorBrush) obj;
            return new {scb.Color, scb.Opacity}.GetHashCode();
        }
    }
}