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
    public class FilletEdgeFeature : NotifyPropertyChanged, ISectionEdgeFeature
    {
        private float radius;

        public PointF EdgePoint { get; private set; }

        public float Radius
        {
            get => this.radius;
            set
            {
                SetProperty(ref this.radius, value);
                NotifyForAnyErrors();
            }
        }

        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; } = true;
        public ShaftSection LinkedSection { get; set; }

        public EdgeFeaturePosition EdgePosition { get; set; }

        private readonly Dictionary<FilletFeatureError, ShaftSectionFeatureError> errors =
            new Dictionary<FilletFeatureError, ShaftSectionFeatureError>();

        public void UpdateFeatureParameters()
        {
            this.EdgePoint =
                this.EdgePosition == EdgeFeaturePosition.FirstEdge
                    ? this.LinkedSection.SecondLine.StartPoint
                    : this.LinkedSection.SecondLine.EndPoint;
        }

        public string DisplayName => $"Fillet [Radius: {this.Radius}]";

        private void NotifyForAnyErrors()
        {
            if (this.EdgePoint == PointF.Empty)
            {
                return;
            }


            bool firstEdge = this.EdgePosition == EdgeFeaturePosition.FirstEdge;
            bool boreSection = this.LinkedSection.IsBore;


            if (this.radius.NearlyEqual(0) &&
                !this.FeatureErrors.ContainsError(this.errors[FilletFeatureError.RadiusIsEqualZeroError]))
            {
                this.FeatureErrors.AddError(
                    this.errors[FilletFeatureError.RadiusIsEqualZeroError],
                    () => this.radius > 0 || (firstEdge
                        ? this.LinkedSection.FirstEdgeFeature != this
                        : this.LinkedSection.SecondEdgeFeature != this));
            }

            if (this.FeatureErrors.ContainsError(this.errors[FilletFeatureError.IncorrectFilletRadiusError]))
            {
                return;
            }

            IFilletFeatureParamsValidator validator = null;
            if (boreSection)
            {
                validator = new BoreFilletFeatureParamsValidator(this);
            }
            else if (this.LinkedSection is ConeSection)
            {
                validator = new SectionFilletFeatureParamsValidator(this);
            }

            switch (this.LinkedSection)
            {
                case CylinderSection cylinderSection:
                {
                    if (boreSection
                        ? !validator.ValidateRadius(this.radius)
                        : this.Radius > cylinderSection.Diameter / 2)
                    {
                        this.FeatureErrors.AddError(this.errors[FilletFeatureError.IncorrectFilletRadiusError],
                            errorCancellationCondition: () =>
                            {
                                IFilletFeatureParamsValidator v = null;
                                if (boreSection)
                                {
                                    v = new BoreFilletFeatureParamsValidator(this);
                                }

                                bool satisfies = boreSection
                                    ? !v.ValidateRadius(this.radius)
                                    : this.radius <= cylinderSection.Diameter / 2 ||
                                      (firstEdge
                                        ? this.LinkedSection.FirstEdgeFeature != this
                                        : this.LinkedSection.SecondEdgeFeature != this);

                                if (boreSection)
                                {
                                    v.Dispose();
                                }

                                return satisfies;
                            });
                    }

                    break;
                }

                case ConeSection _:
                    if (!validator.ValidateRadius(this.radius))
                    {
                        this.FeatureErrors.AddError(this.errors[FilletFeatureError.IncorrectFilletRadiusError],
                            () =>
                            {
                                if (firstEdge
                                    ? this.LinkedSection.FirstEdgeFeature != this
                                    : this.LinkedSection.SecondEdgeFeature != this)
                                {
                                    return true;
                                }

                                using (SectionFilletFeatureParamsValidator v =
                                    new SectionFilletFeatureParamsValidator(this))
                                {
                                    return v.ValidateRadius(this.radius);
                                }
                            });
                    }

                    break;
            }

            validator?.Dispose();
        }

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
            [NotNull] EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = section;
            this.Radius = 0.5f;

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

            this.errors.Add(FilletFeatureError.IncorrectFilletRadiusError, new ShaftSectionFeatureError(
                section: this.LinkedSection,
                feature: this,
                errorMessage: "An incorrect radius of the fillet"));
            this.errors.Add(FilletFeatureError.RadiusIsEqualZeroError, new ShaftSectionFeatureError(this.LinkedSection,
                this,
                "The radius of fillet must be greater than zero"));
        }

        private enum FilletFeatureError
        {
            IncorrectFilletRadiusError,
            RadiusIsEqualZeroError
        }
    }
}