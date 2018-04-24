using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures
{
    [EdgeFeature]
    public class LockNutGrooveEdgeFeature : NotifyPropertyChanged, ISectionEdgeFeature
    {
        private CylinderSection cylinderSection;
        private float width;
        private float depth;
        private float grooveLength;
        private float angle;
        private float radius;

        public float Width
        {
            get => this.width;
            set
            {
                SetProperty(ref this.width, value);
                NotifyForAnyErrors();
            }
        }

        public float Depth
        {
            get => this.depth;
            set
            {
                SetProperty(ref this.depth, value);
                NotifyForAnyErrors();
            }
        }

        public float GrooveLength
        {
            get => this.grooveLength;
            set
            {
                SetProperty(ref this.grooveLength, value);
                NotifyForAnyErrors();
            }
        }

        public float Angle
        {
            get => this.angle;
            set
            {
                SetProperty(ref this.angle, value);
                NotifyForAnyErrors();
            }
        }

        public float Radius
        {
            get => this.radius;
            set
            {
                SetProperty(ref this.radius, value);
                NotifyForAnyErrors();
            }
        }

        public ThreadEdgeFeature ThreadEdgeFeature { get; set; } = new ThreadEdgeFeature();

        public static List<ThreadType> Threads => JsonSerializer
                                                  .Deserialize<List<ThreadType>>(Resources.Threads).Where(t =>
                                                      t.Name == "ANSI Unified Screw Threads" ||
                                                      t.Name == "ANSI Metric M Profile" ||
                                                      t.Name == "ISO Metric profile" ||
                                                      t.Name == "ISO Metric Trapezoidal Threads" ||
                                                      t.Name == "AFBMA Standard Locknuts").ToList();

        public EdgeFeaturePosition EdgePosition { get; set; }
        public PointF EdgePoint { get; private set; }
        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; } = true;

        public ShaftSection LinkedSection { get; set; }

        private readonly Dictionary<LockNutGrooveFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<LockNutGrooveFeatureError, ShaftSectionFeatureError>();


        public void UpdateFeatureParameters()
        {
            this.EdgePoint = this.ThreadEdgeFeature.EdgePoint = this.EdgePosition == EdgeFeaturePosition.FirstEdge
                ? this.LinkedSection.SecondLine.StartPoint
                : this.LinkedSection.SecondLine.EndPoint;
        }

        public string DisplayName => $"Lock nut groove [Width: {this.width} Depth: {this.depth}]";

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
                                                                [NotNull] EdgeFeaturePosition? edgeFeaturePosition)
        {
            ;
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;
            this.Radius = this.cylinderSection.Diameter * 0.2f;
            this.Width = this.cylinderSection.Diameter * 0.2f;
            this.Depth = this.cylinderSection.Diameter * 0.1f;
            this.GrooveLength = this.cylinderSection.Length * 0.15f;
            this.ThreadEdgeFeature.InitializeInAccordanceWithSectionParameters(section, edgeFeaturePosition);
            this.ThreadEdgeFeature.Chamfer = this.cylinderSection.Diameter * 0.05f;
            this.ThreadEdgeFeature.ThreadLength = this.cylinderSection.Length * 0.25f;

            switch (edgeFeaturePosition)
            {
                case EdgeFeaturePosition.FirstEdge:
                    this.EdgePoint = this.ThreadEdgeFeature.EdgePoint = section.SecondLine.StartPoint;
                    break;

                case EdgeFeaturePosition.SecondEdge:
                    this.EdgePoint = this.ThreadEdgeFeature.EdgePoint = section.SecondLine.EndPoint;
                    break;

                case null:
                    throw new ArgumentNullException("An edge feature position must be specified",
                        nameof(edgeFeaturePosition));
            }

            this.EdgePosition = this.ThreadEdgeFeature.EdgePosition = edgeFeaturePosition.Value;

            this.LinkedSection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(CylinderSection.Diameter) ||
                    args.PropertyName == nameof(CylinderSection.Length))
                {
                    NotifyForAnyErrors();
                }
            };


            this.errorsRepo.Add(LockNutGrooveFeatureError.WidthInvalidRange,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Width"));
            this.errorsRepo.Add(LockNutGrooveFeatureError.DepthInvalidRange,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Depth"));
            this.errorsRepo.Add(LockNutGrooveFeatureError.RadiusInvalidRange,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Radius"));
            this.errorsRepo.Add(LockNutGrooveFeatureError.GrooveLengthInvalidRange,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Groove length"));
            this.errorsRepo.Add(LockNutGrooveFeatureError.AngleInvalidRange,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Angle"));
        }

        private void NotifyForAnyErrors()
        {
            if (this.EdgePoint == PointF.Empty)
            {
                return;
            }

            if (this.Width > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[LockNutGrooveFeatureError.WidthInvalidRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[LockNutGrooveFeatureError.WidthInvalidRange], () =>
                {
                    return this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Width <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Width <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.SecondEdgeFeature != this);
                });
            }

            if (this.Depth > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[LockNutGrooveFeatureError.DepthInvalidRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[LockNutGrooveFeatureError.DepthInvalidRange], () =>
                {
                    return this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Depth <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Depth <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.SecondEdgeFeature != this);
                });
            }

            if (this.Radius < this.Depth &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[LockNutGrooveFeatureError.RadiusInvalidRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[LockNutGrooveFeatureError.RadiusInvalidRange], () =>
                {
                    return this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Radius >= this.Depth || this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Radius >= this.Depth || this.LinkedSection.SecondEdgeFeature != this);
                });
            }

            if (this.GrooveLength > this.cylinderSection.Length &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[LockNutGrooveFeatureError.GrooveLengthInvalidRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[LockNutGrooveFeatureError.GrooveLengthInvalidRange], () =>
                {
                    return this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.GrooveLength <= this.cylinderSection.Length ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.GrooveLength <= this.cylinderSection.Length ||
                           this.LinkedSection.SecondEdgeFeature != this);
                });
            }

            if ((this.Angle < -360 || this.Angle > 360) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[LockNutGrooveFeatureError.AngleInvalidRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[LockNutGrooveFeatureError.AngleInvalidRange], () =>
                {
                    return this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Angle >= -360 && this.Angle <= 360 || this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Angle >= -360 && this.Angle <= 360 || this.LinkedSection.SecondEdgeFeature != this);
                });
            }
        }

        private enum LockNutGrooveFeatureError
        {
            WidthInvalidRange,
            DepthInvalidRange,
            RadiusInvalidRange,
            GrooveLengthInvalidRange,
            AngleInvalidRange,
            ThreadLengthInvalidRange,
            ThreadChamferInvalidRange
        }
    }
}