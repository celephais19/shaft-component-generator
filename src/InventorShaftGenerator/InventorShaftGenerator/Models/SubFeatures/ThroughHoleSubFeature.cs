using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Models.SubFeatures
{
    public class ThroughHoleSubFeature : NotifyPropertyChanged, ISectionSubFeature
    {
        private float holeDiameter;
        private float chamfering;
        private float angle;
        private float distance;

        private readonly Dictionary<ThroughHoleFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<ThroughHoleFeatureError, ShaftSectionFeatureError>();

        public Guid Id { get; } = Guid.NewGuid();

        public float HoleDiameter
        {
            get => this.holeDiameter;
            set
            {
                SetProperty(ref this.holeDiameter, value);
                NotifyForAnyErrors();
            }
        }

        public float Chamfering
        {
            get => this.chamfering;
            set
            {
                SetProperty(ref this.chamfering, value);
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

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; } = true;

        public ShaftSection LinkedSection { get; set; }

        public void UpdateFeatureParameters()
        {
        }

        public string DisplayName => $"Through hole [Hole diameter: {this.HoleDiameter}]";

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
            EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = section;
            switch (section)
            {
                case CylinderSection cylinderSection:
                    this.HoleDiameter = cylinderSection.Diameter / 5;
                    this.Chamfering = this.HoleDiameter / 10;
                    this.Distance = cylinderSection.Length / 2;
                    break;

                case PolygonSection polygonSection:
                    this.HoleDiameter = polygonSection.CircumscribedCircleDiameter / 5;
                    this.Chamfering = this.HoleDiameter / 10;
                    this.Distance = polygonSection.Length / 2;
                    break;
            }

            this.LinkedSection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(CylinderSection.Diameter) ||
                    args.PropertyName == nameof(CylinderSection.Length))
                {
                    NotifyForAnyErrors();
                }
            };

            this.errorsRepo.Add(ThroughHoleFeatureError.InvalidHoleDiameter,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Hole diameter"));
            this.errorsRepo.Add(ThroughHoleFeatureError.InvalidChamfering,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Chamfering"));
            this.errorsRepo.Add(ThroughHoleFeatureError.InvalidDistance,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Distance"));
            this.errorsRepo.Add(ThroughHoleFeatureError.InvalidAngle,
                new ShaftSectionFeatureError(this.LinkedSection, this, "Invalid range: Angle"));
        }


        private void NotifyForAnyErrors()
        {
            switch (this.LinkedSection)
            {
                case CylinderSection cylinderSection when this.holeDiameter > cylinderSection.Diameter &&
                                                          !this.FeatureErrors.ContainsError(
                                                              this.errorsRepo[
                                                                  ThroughHoleFeatureError.InvalidHoleDiameter]):
                {
                    this.FeatureErrors.AddError(this.errorsRepo[ThroughHoleFeatureError.InvalidHoleDiameter], () =>
                        this.holeDiameter <= cylinderSection.Diameter ||
                        !this.LinkedSection.SubFeatures.Contains(this));
                    break;
                }

                case PolygonSection polygonSection
                    when this.holeDiameter > polygonSection.CircumscribedCircleDiameter &&
                         !this.FeatureErrors.ContainsError(this.errorsRepo[ThroughHoleFeatureError.InvalidHoleDiameter])
                    :
                {
                    this.FeatureErrors.AddError(this.errorsRepo[ThroughHoleFeatureError.InvalidHoleDiameter], () =>
                        this.holeDiameter <= polygonSection.CircumscribedCircleDiameter ||
                        !this.LinkedSection.SubFeatures.Contains(this));
                    break;
                }
            }

            switch (this.LinkedSection)
            {
                case CylinderSection cylinderSection
                    when this.chamfering > (cylinderSection.Diameter - this.holeDiameter) / 2 &&
                         !this.FeatureErrors.ContainsError(this.errorsRepo[ThroughHoleFeatureError.InvalidChamfering]):
                {
                    this.FeatureErrors.AddError(this.errorsRepo[ThroughHoleFeatureError.InvalidChamfering], () =>
                        this.chamfering <= (cylinderSection.Diameter - this.holeDiameter) / 2 ||
                        !this.LinkedSection.SubFeatures.Contains(this));
                    break;
                }

                case PolygonSection polygonSection when this.chamfering >
                                                        (polygonSection.CircumscribedCircleDiameter -
                                                         this.holeDiameter) / 2 - 0.1 &&
                                                        !this.FeatureErrors.ContainsError(
                                                            this.errorsRepo
                                                                [ThroughHoleFeatureError.InvalidChamfering]):
                {
                    this.FeatureErrors.AddError(this.errorsRepo[ThroughHoleFeatureError.InvalidChamfering], () =>
                        this.chamfering <= (polygonSection.CircumscribedCircleDiameter - this.holeDiameter) / 2 ||
                        !this.LinkedSection.SubFeatures.Contains(this));
                    break;
                }
            }

            if ((this.DistanceFrom == DistanceFrom.FromFirstEdge || this.DistanceFrom == DistanceFrom.FromSecondEdge) &&
                this.distance > this.LinkedSection.Length &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[ThroughHoleFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[ThroughHoleFeatureError.InvalidDistance],
                    () => this.distance <= this.LinkedSection.Length ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if (this.DistanceFrom == DistanceFrom.Centered &&
                (this.distance > this.LinkedSection.Length / 2 || this.distance < this.LinkedSection.Length / -2) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[ThroughHoleFeatureError.InvalidDistance]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[ThroughHoleFeatureError.InvalidDistance],
                    () => this.distance >= this.LinkedSection.Length / -2 &&
                          this.distance <= this.LinkedSection.Length / 2 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }

            if ((this.angle < -360 || this.angle > 360) &&
                !this.FeatureErrors.ContainsError(this.errorsRepo[ThroughHoleFeatureError.InvalidAngle]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[ThroughHoleFeatureError.InvalidAngle],
                    () => this.angle >= -360 && this.angle <= 360 ||
                          !this.LinkedSection.SubFeatures.Contains(this));
            }
        }

        private enum ThroughHoleFeatureError
        {
            InvalidHoleDiameter,
            InvalidChamfering,
            InvalidDistance,
            InvalidAngle
        }
    }
}