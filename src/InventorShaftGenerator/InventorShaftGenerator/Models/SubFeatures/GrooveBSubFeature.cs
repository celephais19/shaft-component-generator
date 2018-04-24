using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Models.SubFeatures
{
    public class GrooveBSubFeature : NotifyPropertyChanged, ISectionSubFeature
    {
        private float depth;
        private float radius;
        private float distance;
        private float angle;
        private CylinderSection cylinderSection;

        private readonly Dictionary<GrooveBFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<GrooveBFeatureError, ShaftSectionFeatureError>();

        public float Depth
        {
            get => this.depth;
            set
            {
                SetProperty(ref this.depth, value);
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

        public float Angle
        {
            get => this.angle;
            set
            {
                SetProperty(ref this.angle, value);
                NotifyForAnyErrors();
            }
        }

        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; }

        public ShaftSection LinkedSection { get; set; }

        public void UpdateFeatureParameters()
        {
        }

        public string DisplayName => $"Groove B [Depth: {this.depth}]";

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
            EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;

            this.errorsRepo.Add(GrooveBFeatureError.InvalidDepth,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Depth"));
            this.errorsRepo.Add(GrooveBFeatureError.InvalidDistance,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Distance"));
            this.errorsRepo.Add(GrooveBFeatureError.InvalidRadius,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Radius"));
            this.errorsRepo.Add(GrooveBFeatureError.InvalidAngle,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Angle"));

            this.Depth = this.cylinderSection.Diameter * 0.125f;
            this.Radius = this.cylinderSection.Diameter * 0.25f;
            this.Distance = this.cylinderSection.Length / 2;

            this.LinkedSection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(CylinderSection.Diameter) ||
                    args.PropertyName == nameof(CylinderSection.Length))
                {
                    NotifyForAnyErrors();
                }
            };
        }

        private void NotifyForAnyErrors()
        {
            if ((this.DistanceFrom == DistanceFrom.FromFirstEdge || this.DistanceFrom == DistanceFrom.FromSecondEdge) &&
                this.distance > this.LinkedSection.Length &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveBFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveBFeatureError.InvalidDistance],
                    () => this.distance <= this.LinkedSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.DistanceFrom == DistanceFrom.Centered &&
                (this.distance > this.LinkedSection.Length / 2 || this.distance < this.LinkedSection.Length / -2) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveBFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveBFeatureError.InvalidDistance],
                    () => this.distance >= this.LinkedSection.Length / -2 &&
                          this.distance <= this.LinkedSection.Length / 2 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.radius > this.LinkedSection.Length / 2 || this.radius.NearlyEqual(0)) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveBFeatureError.InvalidRadius]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveBFeatureError.InvalidRadius],
                    () => this.radius <= this.LinkedSection.Length / 2 && this.radius >= 0.01 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.depth > this.cylinderSection.Diameter / 2 || this.depth.NearlyEqual(0)) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveBFeatureError.InvalidDepth]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveBFeatureError.InvalidDepth],
                    () => this.depth <= this.cylinderSection.Diameter / 2 && this.depth >= 0.01 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.angle < -360 || this.angle > 360) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveBFeatureError.InvalidAngle]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveBFeatureError.InvalidAngle],
                    () => this.angle >= -360 && this.angle <= 360 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }
        }

        private enum GrooveBFeatureError
        {
            InvalidDepth,
            InvalidRadius,
            InvalidDistance,
            InvalidAngle
        }
    }
}