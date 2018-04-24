using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Models.SubFeatures
{
    public class GrooveASubFeature : NotifyPropertyChanged, ISectionSubFeature
    {
        private float depth;
        private float radius;
        private float distance;
        private CylinderSection cylinderSection;

        private readonly Dictionary<GrooveAFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<GrooveAFeatureError, ShaftSectionFeatureError>();

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

        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; } = true;

        public ShaftSection LinkedSection { get; set; }

        public void UpdateFeatureParameters()
        {
        }

        public string DisplayName => $"Groove A [Depth: {this.Depth}]";

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
            EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;

            this.errorsRepo.Add(GrooveAFeatureError.InvalidDepth,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Depth"));
            this.errorsRepo.Add(GrooveAFeatureError.InvalidDistance,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Distance"));
            this.errorsRepo.Add(GrooveAFeatureError.InvalidRadius,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Radius"));

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
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveAFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveAFeatureError.InvalidDistance],
                    () => this.distance <= this.LinkedSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.DistanceFrom == DistanceFrom.Centered &&
                (this.distance > this.LinkedSection.Length / 2 || this.distance < this.LinkedSection.Length / -2) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveAFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveAFeatureError.InvalidDistance],
                    () => this.distance >= this.LinkedSection.Length / -2 &&
                          this.distance <= this.LinkedSection.Length / 2 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.radius > this.LinkedSection.Length / 2 || this.radius.NearlyEqual(0)) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveAFeatureError.InvalidRadius]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveAFeatureError.InvalidRadius],
                    () => this.radius <= this.LinkedSection.Length / 2 && this.radius >= 0.01 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.depth > this.cylinderSection.Diameter / 2 || this.depth.NearlyEqual(0)) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[GrooveAFeatureError.InvalidDepth]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[GrooveAFeatureError.InvalidDepth],
                    () => this.depth <= this.cylinderSection.Diameter / 2 && this.depth >= 0.01 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }
        }

        private enum GrooveAFeatureError
        {
            InvalidDepth,
            InvalidRadius,
            InvalidDistance
        }
    }
}