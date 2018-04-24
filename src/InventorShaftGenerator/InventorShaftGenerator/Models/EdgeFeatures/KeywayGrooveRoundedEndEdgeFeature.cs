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
    public class KeywayGrooveRoundedEndEdgeFeature : KeywayGrooveFeature, ISectionEdgeFeature
    {
        private CylinderSection cylinderSection;
        private float chamfer;

        private readonly Dictionary<KeywayGrooveRoundedEndFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<KeywayGrooveRoundedEndFeatureError, ShaftSectionFeatureError>();

        public KeywayGrooveRoundedEndEdgeFeature()
        {
            this.KeywaysDimensions = JsonSerializer.Deserialize<List<KeywayDimensions>>(Resources.KeywaysDimensions);
            this.Keyways = JsonSerializer.Deserialize<List<Keyway>>(Resources.KeywaysRoundedEnd);
            this.Keyway = this.Keyways.Single(keyway => keyway.Name == "ISO 2491 C");
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

            this.errorsRepo.Add(KeywayGrooveRoundedEndFeatureError.WidthIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Width"));
            this.errorsRepo.Add(KeywayGrooveRoundedEndFeatureError.DepthIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Depth"));
            this.errorsRepo.Add(KeywayGrooveRoundedEndFeatureError.AngleIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Angle"));
            this.errorsRepo.Add(KeywayGrooveRoundedEndFeatureError.ChamferIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Chamfer"));
            this.errorsRepo.Add(KeywayGrooveRoundedEndFeatureError.KeywayLengthIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Keyway length"));
            this.errorsRepo.Add(KeywayGrooveRoundedEndFeatureError.NumberOfKeysIncorrectRange,
                new ShaftSectionFeatureError(this.cylinderSection, this, "Incorrect range: Number of keys"));
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
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[KeywayGrooveRoundedEndFeatureError.WidthIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveRoundedEndFeatureError.WidthIncorrectRange],
                    () =>
                        this.EdgePosition == EdgeFeaturePosition.FirstEdge
                            ? (this.Width <= this.cylinderSection.Diameter / 2 ||
                               this.LinkedSection.FirstEdgeFeature != this)
                            : (this.Width <= this.cylinderSection.Diameter / 2 ||
                               this.LinkedSection.SecondEdgeFeature != this));
            }

            if (this.Depth > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[KeywayGrooveRoundedEndFeatureError.DepthIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveRoundedEndFeatureError.DepthIncorrectRange],
                    () =>
                        this.EdgePosition == EdgeFeaturePosition.FirstEdge
                            ? (this.Depth <= this.cylinderSection.Diameter / 2 ||
                               this.LinkedSection.FirstEdgeFeature != this)
                            : (this.Depth <= this.cylinderSection.Diameter / 2 ||
                               this.LinkedSection.SecondEdgeFeature != this));
            }

            if (this.Chamfer > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[KeywayGrooveRoundedEndFeatureError.ChamferIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveRoundedEndFeatureError.ChamferIncorrectRange],
                    () => this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Chamfer <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Chamfer <= this.cylinderSection.Diameter / 2 ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }

            if (this.KeywayLength > this.LinkedSection.Length &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[KeywayGrooveRoundedEndFeatureError.KeywayLengthIncorrectRange]))
            {
                this.FeatureErrors.AddError(
                    this.errorsRepo[KeywayGrooveRoundedEndFeatureError.KeywayLengthIncorrectRange],
                    () => this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.KeywayLength <= this.LinkedSection.Length ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.KeywayLength <= this.LinkedSection.Length ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }

            if ((this.Angle < -360 || this.Angle > 360) &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[KeywayGrooveRoundedEndFeatureError.AngleIncorrectRange]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveRoundedEndFeatureError.AngleIncorrectRange],
                    () => this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.Angle >= -360 && this.Angle <= 360 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.Angle >= -360 && this.Angle <= 360 ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }

            if ((this.NumberOfKeys < 1 || this.NumberOfKeys > 4) &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[KeywayGrooveRoundedEndFeatureError.NumberOfKeysIncorrectRange]))
            {
                this.FeatureErrors.AddError(
                    this.errorsRepo[KeywayGrooveRoundedEndFeatureError.NumberOfKeysIncorrectRange],
                    () => this.EdgePosition == EdgeFeaturePosition.FirstEdge
                        ? (this.NumberOfKeys >= 1 || this.NumberOfKeys <= 4 ||
                           this.LinkedSection.FirstEdgeFeature != this)
                        : (this.NumberOfKeys >= 1 || this.NumberOfKeys <= 4 ||
                           this.LinkedSection.SecondEdgeFeature != this));
            }
        }

        private enum KeywayGrooveRoundedEndFeatureError
        {
            WidthIncorrectRange,
            DepthIncorrectRange,
            ChamferIncorrectRange,
            KeywayLengthIncorrectRange,
            AngleIncorrectRange,
            NumberOfKeysIncorrectRange
        }
    }
}