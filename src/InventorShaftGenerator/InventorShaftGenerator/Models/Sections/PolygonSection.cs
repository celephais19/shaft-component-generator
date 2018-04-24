using System;
using System.Collections.Generic;
using System.Drawing;
using InventorShaftGenerator.Extensions;

namespace InventorShaftGenerator.Models.Sections
{
    public class PolygonSection : ShaftSection
    {
        private float circumscribedCircleDiameter = 92.376f;
        private float inscribedCircleDiameter = 80;
        private int numberOfEdges = 6;
        private float sectionAngle;
        private bool mainDiameterLocked = true;

        public PolygonSection()
        {
            this.Length = 100;
        }

        public float CircumscribedCircleDiameter
        {
            get => this.circumscribedCircleDiameter;
            set
            {
                this.inscribedCircleDiameter = CalculateInscribedRadius(value, this.numberOfEdges);
                SetProperty(ref this.circumscribedCircleDiameter, value);
            }
        }

        public float InscribedCircleDiameter
        {
            get => this.inscribedCircleDiameter;
            set
            {
                this.circumscribedCircleDiameter = CalculateCircumscribedRadius(value, this.numberOfEdges);
                SetProperty(ref this.inscribedCircleDiameter, value);
            }
        }

        public bool MainDiameterLocked
        {
            get => this.mainDiameterLocked;
            set => SetProperty(ref this.mainDiameterLocked, value);
        }

        public int NumberOfEdges
        {
            get => this.numberOfEdges;
            set => SetProperty(ref this.numberOfEdges, value);
        }

        public float SectionAngle
        {
            get => this.sectionAngle;
            set => SetProperty(ref this.sectionAngle, value);
        }

        public List<PointF> PolygonVertices { get; set; }

        public override string DisplayName => $"Polygon {this.InscribedCircleDiameter:0.###} x {this.Length:0.###}";

        public float CalculateInscribedRadius(double circumscribedRadius, int edgesCount)
        {
            float inscribedRadius =
                (float) (circumscribedRadius * Math.Cos(MathExtensions.DegreesToRadians(180f / edgesCount)));
            return (float) Math.Round(inscribedRadius, 4);
        }

        public float CalculateCircumscribedRadius(double inscribedRadius, int edgesCount)
        {
            float circumscribedRadius =
                (float) (inscribedRadius * (1 / Math.Cos(MathExtensions.DegreesToRadians(180f / edgesCount))));
            return (float) Math.Round(circumscribedRadius, 4);
        }

        public List<PointF> CalculateVertices(double inscribedRadius, int edgesCount, double startingAngle,
            PointF centerPoint)
        {
            double circumscribedRadius = CalculateCircumscribedRadius(inscribedRadius, edgesCount);
            List<PointF> vertices = new List<PointF>();
            double step = 360d / edgesCount;
            double angle = startingAngle;
            for (double i = startingAngle; i < startingAngle + 360d; i += step)
            {
                vertices.Add(DegreesToPointF(angle, circumscribedRadius, centerPoint));
                angle += step;
            }

            return vertices;
        }

        private PointF DegreesToPointF(double degrees, double radius, PointF origin)
        {
            double radians = MathExtensions.DegreesToRadians(degrees);
            PointF xy = new PointF(
                x: (float) (Math.Cos(radians) * radius + origin.X),
                y: (float) (Math.Sin(-radians) * radius + origin.Y)
            );
            return xy;
        }
    }
}