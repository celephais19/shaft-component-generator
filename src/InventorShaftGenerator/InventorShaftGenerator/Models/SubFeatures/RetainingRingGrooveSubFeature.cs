using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Models.SubFeatures
{
    public class RetainingRingGrooveSubFeature : NotifyPropertyChanged, ISectionSubFeature
    {
        private float distance;
        private float width;
        private float diameter1;
        private CylinderSection cylinderSection;

        private readonly Dictionary<RetainingRingFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<RetainingRingFeatureError, ShaftSectionFeatureError>();


        public float Distance
        {
            get => this.distance;
            set
            {
                SetProperty(ref this.distance, value);
                NotifyForAnyErrors();
            }
        }

        public float Width
        {
            get => this.width;
            set
            {
                SetProperty(ref this.width, value);
                NotifyForAnyErrors();
            }
        }

        public float Diameter1
        {
            get => this.diameter1;
            set
            {
                SetProperty(ref this.diameter1, value);
                NotifyForAnyErrors();
            }
        }

        public float ActiveLength { get; set; }

        public DistanceFrom DistanceFrom { get; set; }

        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; }

        public ShaftSection LinkedSection { get; set; }

        public void UpdateFeatureParameters()
        {
        }

        public string DisplayName => $"Retaining ring [Diameter: {this.Diameter1}]";

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
                                                                EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;

            this.errorsRepo.Add(RetainingRingFeatureError.InvalidDistance,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Distance"));
            this.errorsRepo.Add(RetainingRingFeatureError.InvalidWidth,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Width"));
            this.errorsRepo.Add(RetainingRingFeatureError.InvalidDiameter,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Diameter"));

            this.Diameter1 = this.cylinderSection.Diameter * (this.cylinderSection.IsBore ? 1.2f : 0.925f);
            this.Distance = this.cylinderSection.Length * 0.05f;
            this.Width = this.cylinderSection.Length * 0.0175f;

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
                !this.FeatureErrors.ContainsError(this.errorsRepo[RetainingRingFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[RetainingRingFeatureError.InvalidDistance],
                    () => this.distance <= this.LinkedSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.DistanceFrom == DistanceFrom.Centered &&
                (this.distance > this.LinkedSection.Length / 2 || this.distance < this.LinkedSection.Length / -2) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[RetainingRingFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[RetainingRingFeatureError.InvalidDistance],
                    () => this.distance >= this.LinkedSection.Length / -2 &&
                          this.distance <= this.LinkedSection.Length / 2 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.width < 0.01f || this.width > this.LinkedSection.Length) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[RetainingRingFeatureError.InvalidWidth]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[RetainingRingFeatureError.InvalidWidth],
                    () => this.width >= 0.01f && this.width <= this.LinkedSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.LinkedSection.IsBore)
            {
                if (this.diameter1 < this.cylinderSection.Diameter &&
                    !this.FeatureErrors.ContainsError(this.errorsRepo[RetainingRingFeatureError.InvalidDiameter]))
                {
                    this.FeatureErrors.AddError(this.errorsRepo[RetainingRingFeatureError.InvalidDiameter],
                        () => (this.diameter1 >= this.cylinderSection.Diameter) ||
                              !this.LinkedSection.SubFeatures.Contains(this));
                }
            }
            else if ((this.diameter1 < 0.01f || this.diameter1 > this.cylinderSection.Diameter) &&
                     !this.FeatureErrors.ContainsError(this.errorsRepo[RetainingRingFeatureError.InvalidDiameter]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[RetainingRingFeatureError.InvalidDiameter],
                    () => this.diameter1 >= 0.01f && this.diameter1 <= this.cylinderSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }
        }

        private enum RetainingRingFeatureError
        {
            InvalidDistance,
            InvalidWidth,
            InvalidDiameter
        }
    }
}