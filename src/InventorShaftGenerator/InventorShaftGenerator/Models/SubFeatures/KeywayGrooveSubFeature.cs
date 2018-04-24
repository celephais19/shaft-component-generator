using System.Collections.Generic;
using System.Linq;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.SubFeatures
{
    public class KeywayGrooveSubFeature : KeywayGrooveFeature, ISectionSubFeature
    {
        private CylinderSection cylinderSection;

        private readonly Dictionary<KeywayGrooveFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<KeywayGrooveFeatureError, ShaftSectionFeatureError>();

        private float distance;

        public KeywayGrooveSubFeature()
        {
            this.KeywaysDimensions = JsonSerializer.Deserialize<List<KeywayDimensions>>(Resources.KeywaysDimensions);
            this.Keyways = JsonSerializer.Deserialize<List<Keyway>>(Resources.KeywaysRoundedEnd);
            this.Keyway = this.Keyways.Single(keyway => keyway.Name == "ISO 2491 A");
        }

        public float Distance
        {
            get => this.distance;
            set
            {
                SetProperty(ref this.distance, value);
                NotifyForAnyErrors();
            }
        }

        public DistanceFrom DistanceFrom { get; set; }

        public override void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;

            this.errorsRepo.Add(KeywayGrooveFeatureError.InvalidWidth,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Width"));
            this.errorsRepo.Add(KeywayGrooveFeatureError.InvalidDepth,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Depth"));
            this.errorsRepo.Add(KeywayGrooveFeatureError.InvalidAngle,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Angle"));
            this.errorsRepo.Add(KeywayGrooveFeatureError.InvalidDistance,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Distance"));
            this.errorsRepo.Add(KeywayGrooveFeatureError.InvalidAngleBetweenKeys,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Angle between keys"));
            this.errorsRepo.Add(KeywayGrooveFeatureError.InvalidKeywayLength,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Keyway length"));
            this.errorsRepo.Add(KeywayGrooveFeatureError.InvalidNumberOfKeys,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Number of keys"));

            if (this.cylinderSection.Diameter > 150)
            {
                this.Width = 36;
                this.Depth = 7.5f;
            }
            else
            {
                var keywayDimensions = GetKeywayDimensions(this.cylinderSection.Diameter);
                this.Width = keywayDimensions.Width;
                this.Depth = keywayDimensions
                             .Depths.Single(d => d.DepthType == this.Keyway.DepthType).Value;
            }

            this.KeywayLength = this.cylinderSection.Length / 3;
            this.Distance = (this.cylinderSection.Length - this.KeywayLength) / 2;
            this.NumberOfKeys = 1;
            this.AngleBetweenKeys = 360;

            this.LinkedSection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(CylinderSection.Diameter) ||
                    args.PropertyName == nameof(CylinderSection.Length))
                {
                    NotifyForAnyErrors();
                }
            };
        }

        protected override void NotifyForAnyErrors()
        {
            if (this.Width > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[KeywayGrooveFeatureError.InvalidWidth]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveFeatureError.InvalidWidth],
                    () =>
                        this.Width <= this.cylinderSection.Diameter / 2 ||
                        !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.Depth > this.cylinderSection.Diameter / 2 &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[KeywayGrooveFeatureError.InvalidDepth]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveFeatureError.InvalidDepth],
                    () =>
                        this.Depth <= this.cylinderSection.Diameter / 2 ||
                        !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.DistanceFrom == DistanceFrom.FromFirstEdge || this.DistanceFrom == DistanceFrom.FromSecondEdge) &&
                this.Distance > this.LinkedSection.Length &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[KeywayGrooveFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveFeatureError.InvalidDistance],
                    () => this.Distance <= this.LinkedSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.DistanceFrom == DistanceFrom.Centered &&
                (this.Distance > this.LinkedSection.Length / 2 || this.Distance < this.LinkedSection.Length / -2) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[KeywayGrooveFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveFeatureError.InvalidDistance],
                    () => this.Distance >= this.LinkedSection.Length / -2 &&
                          this.Distance <= this.LinkedSection.Length / 2 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.KeywayLength > this.LinkedSection.Length &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[KeywayGrooveFeatureError.InvalidKeywayLength]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveFeatureError.InvalidKeywayLength],
                    () => this.KeywayLength <= this.LinkedSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.Angle < -360 || this.Angle > 360) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[KeywayGrooveFeatureError.InvalidAngle]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[KeywayGrooveFeatureError.InvalidAngle],
                    () => this.Angle >= -360 && this.Angle <= 360 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }
        }

        private KeywayDimensions GetKeywayDimensions(float diameter)
        {
            var selectedKeywayDimensions = this.KeywaysDimensions
                                               .Where(kd =>
                                                   diameter >= kd.MinInclusive &&
                                                   diameter <= kd.MaxInclusive)
                                               .FirstOrDefault(kd =>
                                                   kd.Depths.Any(d => d.DepthType == this.Keyway.DepthType));
            return selectedKeywayDimensions;
        }

        private enum KeywayGrooveFeatureError
        {
            InvalidWidth,
            InvalidDepth,
            InvalidKeywayLength,
            InvalidAngle,
            InvalidNumberOfKeys,
            InvalidDistance,
            InvalidAngleBetweenKeys
        }
    }
}