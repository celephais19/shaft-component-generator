using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Models.SubFeatures
{
    public class WrenchSubFeature : NotifyPropertyChanged, ISectionSubFeature
    {
        private float wrenchLength;
        private float widthAcrossFlats;
        private float angle;
        private float distance;
        private CylinderSection cylinderSection;

        private readonly Dictionary<WrenchFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<WrenchFeatureError, ShaftSectionFeatureError>();

        public float WrenchLength
        {
            get => this.wrenchLength;
            set
            {
                SetProperty(ref this.wrenchLength, value);
                NotifyForAnyErrors();
            }
        }

        public float WidthAcrossFlats
        {
            get => this.widthAcrossFlats;
            set
            {
                SetProperty(ref this.widthAcrossFlats, value);
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

        public string DisplayName => $"Wrench [Length: {this.WrenchLength}]";

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
            EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;

            this.errorsRepo.Add(WrenchFeatureError.InvalidDistance,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Distance"));
            this.errorsRepo.Add(WrenchFeatureError.InvalidWidthAcrossFlats,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Width across flats"));
            this.errorsRepo.Add(WrenchFeatureError.InvalidWrenchLength,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Wrench length"));
            this.errorsRepo.Add(WrenchFeatureError.InvalidAngle,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Angle"));

            this.WrenchLength = this.cylinderSection.Length / 2;
            this.WidthAcrossFlats = this.cylinderSection.Diameter / 2;
            this.Distance = this.cylinderSection.Diameter * 0.25f;

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
                !this.FeatureErrors.ContainsError(this.errorsRepo[WrenchFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[WrenchFeatureError.InvalidDistance],
                    () => this.distance <= this.LinkedSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.DistanceFrom == DistanceFrom.Centered &&
                (this.distance > this.LinkedSection.Length / 2 || this.distance < this.LinkedSection.Length / -2) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[WrenchFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[WrenchFeatureError.InvalidDistance],
                    () => this.distance >= this.LinkedSection.Length / -2 &&
                          this.distance <= this.LinkedSection.Length / 2 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.WrenchLength > this.LinkedSection.Length || this.WrenchLength < 0.01f) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[WrenchFeatureError.InvalidWrenchLength]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[WrenchFeatureError.InvalidWrenchLength],
                    () => this.wrenchLength <= this.LinkedSection.Length && this.wrenchLength >= 0.01f ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.widthAcrossFlats < 0.01f || this.widthAcrossFlats > this.cylinderSection.Diameter) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[WrenchFeatureError.InvalidWidthAcrossFlats]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[WrenchFeatureError.InvalidWidthAcrossFlats],
                    () =>
                        this.widthAcrossFlats >= 0.01f && this.widthAcrossFlats <= this.cylinderSection.Diameter ||
                        !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.angle < -360 || this.angle > 360) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[WrenchFeatureError.InvalidAngle]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[WrenchFeatureError.InvalidAngle],
                    () => this.angle >= -360 && this.angle <= 360 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }
        }

        private enum WrenchFeatureError
        {
            InvalidDistance,
            InvalidWrenchLength,
            InvalidWidthAcrossFlats,
            InvalidAngle
        }
    }
}