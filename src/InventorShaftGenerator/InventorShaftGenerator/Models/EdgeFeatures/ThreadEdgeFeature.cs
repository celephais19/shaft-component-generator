using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;
using InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels;

namespace InventorShaftGenerator.Models.EdgeFeatures
{
    [EdgeFeature]
    public class ThreadEdgeFeature : NotifyPropertyChanged, ISectionEdgeFeature
    {
        private CylinderSection cylinderSection;

        public static readonly List<ThreadType> Threads =
            JsonSerializer.Deserialize<List<ThreadType>>(Resources.Threads);

        private float threadLength;
        private float chamfer = 0.1f;

        public PointF EdgePoint { get; set; }

        public string ThreadType { get; set; } = "ANSI Metric M Profile";

        public string Size { get; set; }

        public string Designation { get; set; } = "M50x4";

        public string Class { get; set; } = "4g6g";

        private readonly Dictionary<ThreadFeatureError, ShaftSectionFeatureError> errorsRepo =
            new Dictionary<ThreadFeatureError, ShaftSectionFeatureError>();

        public float Chamfer
        {
            get => this.chamfer;
            set
            {
                SetProperty(ref this.chamfer, value);
                NotifyForAnyErrors();
            }
        }

        public float ThreadLength
        {
            get => this.threadLength;
            set
            {
                SetProperty(ref this.threadLength, value);
                NotifyForAnyErrors();
            }
        }

        public ThreadDirection ThreadDirection { get; set; } = ThreadDirection.RightHand;

        public EdgeFeaturePosition EdgePosition { get; set; }

        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; } = true;

        public ShaftSection LinkedSection { get; set; }

        public void UpdateFeatureParameters()
        {
            this.EdgePoint =
                this.EdgePosition == EdgeFeaturePosition.FirstEdge
                    ? this.LinkedSection.SecondLine.StartPoint
                    : this.LinkedSection.SecondLine.EndPoint;
        }

        public string DisplayName => $"Thread [Type: {this.ThreadType} Size: {this.Size}]";

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
                                                                EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;
            this.ThreadLength = section.Length * 0.2f;
            this.Size = this.cylinderSection.Diameter.ToString(CultureInfo.InvariantCulture);
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

            this.errorsRepo.Add(ThreadFeatureError.ThreadLengthIsGreaterThanSectionLengthError,
                new ShaftSectionFeatureError(
                    section: this.LinkedSection,
                    feature: this,
                    errorMessage: "A thread length is greater than the section length"));

            this.errorsRepo.Add(ThreadFeatureError.ChamferIsGreaterThanSectionRadiusError, new ShaftSectionFeatureError(
                this.LinkedSection, this,
                "A chamfer distance is greater than the section radius"));

            this.errorsRepo.Add(ThreadFeatureError.ChamferIsGreaterThanSectionLengthError, new ShaftSectionFeatureError(
                this.LinkedSection, this,
                "A chamfer distance is greater than the section length"));
        }

        private void NotifyForAnyErrors()
        {
            if (this.EdgePoint.IsEmpty)
            {
                return;
            }

            bool boreSection = this.LinkedSection.IsBore;

            var chamferEdgeFeature = new ChamferEdgeFeature()
            {
                LinkedSection = this.LinkedSection,
                Distance = this.Chamfer,
                ChamferType = ChamferType.Distance,
                EdgePoint = this.EdgePoint,
                EdgePosition = this.EdgePosition
            };

            IChamferFeatureParamsValidator validator = null;
            if (boreSection)
            {
                validator = new BoreChamferFeatureParamsValidator(chamferEdgeFeature);
            }

            bool firstEdge = this.EdgePosition == EdgeFeaturePosition.FirstEdge;

            if (this.ThreadLength > this.LinkedSection.Length &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[ThreadFeatureError.ThreadLengthIsGreaterThanSectionLengthError]))
            {
                this.FeatureErrors.AddError(
                    this.errorsRepo[ThreadFeatureError.ThreadLengthIsGreaterThanSectionLengthError],
                    () => this.ThreadLength <= this.LinkedSection.Length ||
                          (firstEdge
                        ? this.LinkedSection.FirstEdgeFeature != this
                        : this.LinkedSection.SecondEdgeFeature != this));
            }

            if ((boreSection
                    ? !validator.ValidateDistance(this.chamfer)
                    : this.Chamfer > this.cylinderSection.Diameter / 2) &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[ThreadFeatureError.ChamferIsGreaterThanSectionRadiusError]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[ThreadFeatureError.ChamferIsGreaterThanSectionRadiusError],
                    () =>
                    {
                        if (firstEdge
                            ? this.LinkedSection.FirstEdgeFeature != this
                            : this.LinkedSection.SecondEdgeFeature != this)
                        {
                            return true;
                        }

                        IChamferFeatureParamsValidator v = null;
                        if (boreSection)
                        {
                            v = new BoreChamferFeatureParamsValidator(chamferEdgeFeature);
                        }

                        bool satisfies = boreSection
                            ? validator.ValidateDistance(this.chamfer)
                            : this.Chamfer <= this.cylinderSection.Diameter / 2;

                        v.Dispose();
                        return satisfies;
                    });
            }

            if (this.chamfer > this.cylinderSection.Length &&
                !this.FeatureErrors.ContainsError(
                    this.errorsRepo[ThreadFeatureError.ChamferIsGreaterThanSectionLengthError]))
            {
                this.FeatureErrors.AddError(this.errorsRepo[ThreadFeatureError.ChamferIsGreaterThanSectionLengthError],
                    () => this.chamfer <= this.cylinderSection.Length ||
                          (firstEdge
                        ? this.LinkedSection.FirstEdgeFeature != this
                        : this.LinkedSection.SecondEdgeFeature != this));
            }
        }

        private enum ThreadFeatureError
        {
            ThreadLengthIsGreaterThanSectionLengthError,
            ChamferIsGreaterThanSectionRadiusError,
            ChamferIsGreaterThanSectionLengthError
        }
    }


    public enum ThreadDirection
    {
        LeftHand,
        RightHand
    }

    public class ThreadType
    {
        public string Name { get; set; }
        public List<Size> Sizes { get; set; }
    }

    public class Size
    {
        public string Value { get; set; }
        public List<Designation> Designations { get; set; }
    }

    public class Designation
    {
        public string Value { get; set; }
        public List<ThreadTypeClass> Classes { get; set; }
    }

    public class ThreadTypeClass
    {
        public string Name;
    }
}