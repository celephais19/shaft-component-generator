using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;
using InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels;

namespace InventorShaftGenerator.Models.EdgeFeatures
{
    [EdgeFeature]
    public class ChamferEdgeFeature : NotifyPropertyChanged, ISectionEdgeFeature
    {
        public PointF EdgePoint { get; set; }
        public ChamferType ChamferType { get; set; }

        public float Distance
        {
            get => this.distance;
            set
            {
                SetProperty(ref this.distance, value);
                if (this.ChamferType == ChamferType.Distance)
                {
                    NotifyForAnyErrors();
                }
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

        public float Distance1
        {
            get => this.distance1;
            set
            {
                SetProperty(ref this.distance1, value);
                NotifyForAnyErrors();
            }
        }

        public float Distance2
        {
            get => this.distance2;
            set
            {
                SetProperty(ref this.distance2, value);
                NotifyForAnyErrors();
            }
        }

        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; } = true;

        public ShaftSection LinkedSection { get; set; }

        public EdgeFeaturePosition EdgePosition { get; set; }

        private readonly Dictionary<ChamferFeatureError, ShaftSectionFeatureError> errors =
            new Dictionary<ChamferFeatureError, ShaftSectionFeatureError>();

        private float distance;
        private float angle;
        private float distance1;
        private float distance2;

        public void UpdateFeatureParameters()
        {
            this.EdgePoint = this.EdgePosition == EdgeFeaturePosition.FirstEdge
                ? this.LinkedSection.SecondLine.StartPoint
                : this.LinkedSection.SecondLine.EndPoint;
        }

        public string DisplayName
        {
            get
            {
                switch (this.ChamferType)
                {
                    case ChamferType.Distance:
                        return $"Chamfer [Distance: {this.distance}]";
                    case ChamferType.DistanceAndAngle:
                        return $"Chamfer [Distance: {this.distance} Angle: {this.angle}]";
                    default:
                        return $"Chamfer [Distance1: {this.distance1} Distance2: {this.distance2}]";
                }
            }
        }

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
                                                                [NotNull] EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = section;
            this.Distance = this.Distance1 = this.Distance2 = 0.5f;
            this.Angle = 45;
            this.ChamferType = ChamferType.Distance;

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
                    args.PropertyName == nameof(ConeSection.Diameter1) ||
                    args.PropertyName == nameof(ConeSection.Diameter2) ||
                    args.PropertyName == nameof(PolygonSection.CircumscribedCircleDiameter) ||
                    args.PropertyName == nameof(PolygonSection.InscribedCircleDiameter))
                {
                    NotifyForAnyErrors();
                }
            };

            this.errors.Add(ChamferFeatureError.IncorrectDistance,
                new ShaftSectionFeatureError(this.LinkedSection, this, "An incorrect distance"));
            this.errors.Add(ChamferFeatureError.IncorrectAngle,
                new ShaftSectionFeatureError(this.LinkedSection, this, "An incorrect angle"));
            this.errors.Add(ChamferFeatureError.IncorrectDistance1,
                new ShaftSectionFeatureError(this.LinkedSection, this, "An incorrect first distance"));
            this.errors.Add(ChamferFeatureError.IncorrectDistance2,
                new ShaftSectionFeatureError(this.LinkedSection, this, "An incorrect second distance"));
            this.errors.Add(ChamferFeatureError.IncorrectDistances,
                new ShaftSectionFeatureError(this.LinkedSection, this, "An incorrect distances"));
        }

        private void NotifyForAnyErrors()
        {
            if (this.EdgePoint == PointF.Empty)
            {
                return;
            }

            bool firstEdge = this.EdgePosition == EdgeFeaturePosition.FirstEdge;
            bool boreSection = this.LinkedSection.IsBore;

            IChamferFeatureParamsValidator validator = null;
            if (boreSection)
            {
                validator = new BoreChamferFeatureParamsValidator(this);
            }
            else if (this.LinkedSection is ConeSection || this.LinkedSection is PolygonSection)
            {
                validator = new SectionChamferFeatureParamsValidator(this);
            }

            switch (this.ChamferType)
            {
                case ChamferType.Distance
                    when !this.FeatureErrors.ContainsError(this.errors[ChamferFeatureError.IncorrectDistance]):
                {
                    switch (this.LinkedSection)
                    {
                        case CylinderSection cylinderSection
                            when this.distance > cylinderSection.Diameter / 2 || this.distance.NearlyEqual(0):
                        {
                            this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectDistance],
                                () =>
                                {
                                    bool satisfies =
                                        this.distance <= ((CylinderSection) this.LinkedSection).Diameter / 2 &&
                                        this.distance > 0 ||
                                        (firstEdge
                                            ? this.LinkedSection.FirstEdgeFeature != this
                                            : this.LinkedSection.SecondEdgeFeature != this);
                                    return satisfies;
                                });
                            break;
                        }

                        case ConeSection _:
                        {
                            if (!validator.ValidateDistance(this.distance))
                            {
                                this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectDistance],
                                    () =>
                                    {
                                        if (firstEdge
                                            ? this.LinkedSection.FirstEdgeFeature != this
                                            : this.LinkedSection.SecondEdgeFeature != this)
                                        {
                                            return true;
                                        }

                                        using (var ccpv =
                                            new SectionChamferFeatureParamsValidator(this))
                                        {
                                            return ccpv.ValidateDistance(this.distance);
                                        }
                                    });
                            }


                            break;
                        }

                        case PolygonSection polygonSection:
                        {
                            if (!validator.ValidateDistance(this.distance) || this.distance.NearlyEqual(0))
                            {
                                this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectDistance], () =>
                                {
                                    if (firstEdge
                                        ? this.LinkedSection.FirstEdgeFeature != this
                                        : this.LinkedSection.SecondEdgeFeature != this)
                                    {
                                        return true;
                                    }

                                    using (var v = new SectionChamferFeatureParamsValidator(this))
                                    {
                                        return v.ValidateDistance(this.distance) || this.distance > 0;
                                    }
                                });
                            }

                            break;
                        }
                    }

                    break;
                }
                case ChamferType.DistanceAndAngle:
                {
                    switch (this.LinkedSection)
                    {
                        case CylinderSection cylinderSection:
                        {
                            if (this.distance > cylinderSection.Diameter / 2 || this.distance.NearlyEqual(0))
                            {
                                if (!this.FeatureErrors.ContainsError(
                                    this.errors[ChamferFeatureError.IncorrectDistance]))
                                {
                                    this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectDistance],
                                        () =>
                                            this.distance <= ((CylinderSection) this.LinkedSection).Diameter / 2 &&
                                            this.distance > 0 ||
                                            (firstEdge
                                                ? this.LinkedSection.FirstEdgeFeature != this
                                                : this.LinkedSection.SecondEdgeFeature != this));
                                }
                            }

                            if (this.angle >= 90 || !this.angle.IsGreaterThanZero())
                            {
                                if (!this.FeatureErrors.ContainsError(this.errors[ChamferFeatureError.IncorrectAngle]))
                                {
                                    this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectAngle], () =>
                                        this.angle < 90 &&
                                        this.angle > 0 ||
                                        (firstEdge
                                            ? this.LinkedSection.FirstEdgeFeature != this
                                            : this.LinkedSection.SecondEdgeFeature != this));
                                }
                            }

                            break;
                        }

                        case ConeSection _:
                        {
                            if (!validator.ValidateDistance(this.distance))
                            {
                                if (!this.FeatureErrors.ContainsError(
                                    this.errors[ChamferFeatureError.IncorrectDistance])
                                )
                                {
                                    this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectDistance],
                                        () =>
                                        {
                                            if (firstEdge
                                                ? this.LinkedSection.FirstEdgeFeature != this
                                                : this.LinkedSection.SecondEdgeFeature != this)
                                            {
                                                return true;
                                            }

                                            using (var ccpv =
                                                new SectionChamferFeatureParamsValidator(this))
                                            {
                                                return ccpv.ValidateDistance(this.distance) || this.distance > 0;
                                            }
                                        });
                                }
                            }

                            if (this.angle >= 90 || !this.angle.IsGreaterThanZero())
                            {
                                if (!this.FeatureErrors.ContainsError(
                                    this.errors[ChamferFeatureError.IncorrectAngle]))
                                {
                                    this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectAngle],
                                        () =>
                                            this.angle < 90 &&
                                            this.angle > 0 ||
                                            (firstEdge
                                                ? this.LinkedSection.FirstEdgeFeature != this
                                                : this.LinkedSection.SecondEdgeFeature != this));
                                }
                            }

                            break;
                        }
                    }

                    break;
                }
                case ChamferType.TwoDistances:
                {
                    switch (this.LinkedSection)
                    {
                        case CylinderSection cylinderSection:
                        {
                            if (this.distance1 > cylinderSection.Diameter / 2)
                            {
                                if (!this.FeatureErrors.ContainsError(
                                    this.errors[ChamferFeatureError.IncorrectDistance1]))
                                {
                                    this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectDistance1],
                                        () =>
                                            this.distance1 <=
                                            ((CylinderSection) this.LinkedSection).Diameter / 2 ||
                                            (firstEdge
                                                ? this.LinkedSection.FirstEdgeFeature != this
                                                : this.LinkedSection.SecondEdgeFeature != this));
                                }
                            }
                            else if (this.distance2 > cylinderSection.Diameter / 2)
                            {
                                if (!this.FeatureErrors.ContainsError(
                                    this.errors[ChamferFeatureError.IncorrectDistance2]))
                                {
                                    this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectDistance2],
                                        () =>
                                            this.distance2 <=
                                            ((CylinderSection) this.LinkedSection).Diameter / 2 ||
                                           (firstEdge
                                                ? this.LinkedSection.FirstEdgeFeature != this
                                                : this.LinkedSection.SecondEdgeFeature != this));
                                }
                            }

                            break;
                        }

                        case ConeSection _:
                        {
                            if (!validator.ValidateTwoDistances(this.distance1, this.distance2))
                            {
                                if (!this.FeatureErrors.ContainsError(
                                    this.errors[ChamferFeatureError.IncorrectDistances]))
                                {
                                    this.FeatureErrors.AddError(this.errors[ChamferFeatureError.IncorrectDistances],
                                        () =>
                                        {
                                            if (firstEdge
                                                ? this.LinkedSection.FirstEdgeFeature != this
                                                : this.LinkedSection.SecondEdgeFeature != this)
                                            {
                                                return true;
                                            }

                                            using (var ccpv =
                                                new SectionChamferFeatureParamsValidator(this))
                                            {
                                                return ccpv.ValidateTwoDistances(this.distance1,
                                                    this.distance2);
                                            }
                                        });
                                }
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            validator?.Dispose();
        }

        private enum ChamferFeatureError
        {
            IncorrectDistance,
            IncorrectDistance1,
            IncorrectDistance2,
            IncorrectDistances,
            IncorrectAngle
        }
    }

    public enum ChamferType
    {
        Distance,
        DistanceAndAngle,
        TwoDistances
    }
}