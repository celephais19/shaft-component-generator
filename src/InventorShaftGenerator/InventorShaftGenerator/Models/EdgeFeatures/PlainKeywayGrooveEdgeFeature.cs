using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.EdgeFeatures
{
    [EdgeFeature]
    public class PlainKeywayGrooveEdgeFeature : KeywayGrooveFeature, ISectionEdgeFeature
    {
        private CylinderSection cylinderSection;

        private readonly Dictionary<PlainKeywayGrooveFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<PlainKeywayGrooveFeatureError, ShaftSectionFeatureError>();

        private float radius;
        private float chamfer;

        public PlainKeywayGrooveEdgeFeature()
        {
            this.KeywaysDimensions = JsonSerializer
                .Deserialize<List<KeywayDimensions>>(Resources.KeywaysDimensions);
            this.Keyways = JsonSerializer.Deserialize<List<Keyway>>(Resources.Keyways);
            this.Keyway = this.Keyways.Single(k => k.Name == "ISO 2491 B");
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

        public float Chamfer
        {
            get => this.chamfer;
            set
            {
                SetProperty(ref this.chamfer, value);
                NotifyForAnyErrors();
            }
        }

        public EdgeFeaturePosition EdgePosition { get; set; }
        public PointF EdgePoint { get; set; }

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;
            var selectedKeywayDimensions = this.KeywaysDimensions.First(dimensions =>
                this.cylinderSection.Diameter >= dimensions.MinInclusive &&
                this.cylinderSection.Diameter <= dimensions.MaxInclusive &&
                dimensions.Depths.Any(depth =>
                    depth.DepthType == this.Keyway.DepthType));
            this.Depth = selectedKeywayDimensions.Depths.First(depth => depth.DepthType == this.Keyway.DepthType).Value;
            this.Radius = this.cylinderSection.Diameter * 0.2f;
            this.Width = selectedKeywayDimensions.Width;
            this.KeywayLength = this.cylinderSection.Length * 0.36f;
            this.AngleBetweenKeys = 360;
            this.NumberOfKeys = 1;
            this.Chamfer = this.cylinderSection.Diameter * 0.02f;

            switch (edgeFeaturePosition)
            {
                case EdgeFeaturePosition.FirstEdge:
                    this.EdgePoint = section.SecondLine.StartPoint;
                    break;

                case EdgeFeaturePosition.SecondEdge:
                    this.EdgePoint = section.SecondLine.EndPoint;
                    break;

                case null:
                    throw new ArgumentNullException("An edge feature position must be specified",
                        nameof(edgeFeaturePosition));
            }

            this.EdgePosition = edgeFeaturePosition.Value;

            this.LinkedSection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(CylinderSection.Diameter) ||
                    args.PropertyName == nameof(CylinderSection.Length))
                {
                    NotifyForAnyErrors();
                }
            };

            this.errorsRepo.Add(PlainKeywayGrooveFeatureError.WidthIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Width"));
            this.errorsRepo.Add(PlainKeywayGrooveFeatureError.DepthIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Depth"));
            this.errorsRepo.Add(PlainKeywayGrooveFeatureError.AngleIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Angle"));
            this.errorsRepo.Add(PlainKeywayGrooveFeatureError.ChamferIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Chamfer"));
            this.errorsRepo.Add(PlainKeywayGrooveFeatureError.KeywayLengthIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Keyway length"));
            this.errorsRepo.Add(PlainKeywayGrooveFeatureError.NumberOfKeysIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Number of keys"));
            this.errorsRepo.Add(PlainKeywayGrooveFeatureError.RadiusIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Radius"));
        }

        public override void UpdateFeatureParameters()
        {
            base.UpdateFeatureParameters();

            this.EdgePoint = this.EdgePosition == EdgeFeaturePosition.FirstEdge
                ? this.LinkedSection.SecondLine.StartPoint
                : this.LinkedSection.SecondLine.EndPoint;
        }

        protected override void NotifyForAnyErrors()
        {
            if (this.EdgePoint.IsEmpty)
            {
                return;
            }

            if (this.Width > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[PlainKeywayGrooveFeatureError.WidthIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[PlainKeywayGrooveFeatureError.WidthIncorrectRange], () =>
                    this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Width <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Width <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }

            if (this.Depth > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[PlainKeywayGrooveFeatureError.DepthIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[PlainKeywayGrooveFeatureError.DepthIncorrectRange], () =>
                    this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Depth <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Depth <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }

            if (this.Chamfer > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[PlainKeywayGrooveFeatureError.ChamferIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[PlainKeywayGrooveFeatureError.ChamferIncorrectRange],
                    () => this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Chamfer <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Chamfer <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }

            if (this.Radius <= this.Depth &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[PlainKeywayGrooveFeatureError.RadiusIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[PlainKeywayGrooveFeatureError.RadiusIncorrectRange], () =>
                    this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Radius >= this.Depth || this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Radius >= this.Depth || this.LinkedSection.SecondEdgeFeature != this));
            }

            if (this.KeywayLength > this.LinkedSection.Length &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[PlainKeywayGrooveFeatureError.KeywayLengthIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[PlainKeywayGrooveFeatureError.KeywayLengthIncorrectRange],
                    () => this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.KeywayLength <= this.LinkedSection.Length ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.KeywayLength <= this.LinkedSection.Length ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }

            if ((this.Angle < -360 || this.Angle > 360) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[PlainKeywayGrooveFeatureError.AngleIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[PlainKeywayGrooveFeatureError.AngleIncorrectRange],
                    () => this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Angle >= -360 && this.Angle <= 360 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Angle >= -360 && this.Angle <= 360 ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }

            if ((this.NumberOfKeys < 1 || this.NumberOfKeys > 4) &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[PlainKeywayGrooveFeatureError.NumberOfKeysIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[PlainKeywayGrooveFeatureError.NumberOfKeysIncorrectRange],
                    () => this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.NumberOfKeys >= 1 || this.NumberOfKeys <= 4 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.NumberOfKeys >= 1 || this.NumberOfKeys <= 4 ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }
        }

        private enum PlainKeywayGrooveFeatureError
        {
            WidthIncorrectRange,
            DepthIncorrectRange,
            ChamferIncorrectRange,
            RadiusIncorrectRange,
            KeywayLengthIncorrectRange,
            AngleIncorrectRange,
            NumberOfKeysIncorrectRange
        }
    }
}