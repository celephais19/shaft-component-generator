using System.Drawing;

namespace InventorShaftGenerator.Models
{
    public class SketchLineSimple
    {
        public PointF StartPoint { get; set; }
        public PointF EndPoint { get; set; }

        public SketchLineSimple(PointF startPoint, PointF endPoint)
        {
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }
    }
}