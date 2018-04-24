using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.Models
{
    [EdgeFeature]
    public abstract class ReliefEdgeFeature : NotifyPropertyChanged, ISectionEdgeFeature
    {
        private float width;
        private float reliefDepth;
        private float radius;

        public float Width
        {
            get => this.width;
            set => SetProperty(ref this.width, value);
        }

        public float ReliefDepth
        {
            get => this.reliefDepth;
            set => SetProperty(ref this.reliefDepth, value);
        }

        public float Radius
        {
            get => this.radius;
            set => SetProperty(ref this.radius, value);
        }

        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        protected abstract List<ReliefDimensions> Dimensions { get; }

        public bool ShouldBeBuilt { get; set; } = true;
        public ShaftSection LinkedSection { get; set; }
        public EdgeFeaturePosition EdgePosition { get; set; }
        public PointF EdgePoint { get; set; }

        public virtual void InitializeInAccordanceWithSectionParameters(ShaftSection section,
                                                                        EdgeFeaturePosition? edgeFeaturePosition = null)
        {
            this.LinkedSection = section;

            var cylinderSection = (CylinderSection) section;
            var neededDimensions = CalculateReliefDimensions(cylinderSection.Diameter);

            this.Width = neededDimensions.Width;
            this.Radius = neededDimensions.Radius;
            this.ReliefDepth = neededDimensions.ReliefDepth;

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
            this.EdgePoint = this.EdgePosition == EdgeFeaturePosition.FirstEdge
                ? this.LinkedSection.SecondLine.StartPoint
                : this.LinkedSection.SecondLine.EndPoint;
        }

        public virtual void UpdateFeatureParameters()
        {
            this.EdgePoint = this.EdgePosition == EdgeFeaturePosition.FirstEdge
                ? this.LinkedSection.SecondLine.StartPoint
                : this.LinkedSection.SecondLine.EndPoint;
        }

        public string DisplayName
        {
            get
            {
                var regexToSplit = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
                return $"{regexToSplit.Replace(this.GetType().Name, " ")} [Width: {this.Width}]";
            }
        }

        protected virtual void NotifyForAnyErrors()
        {
        }

        public ReliefDimensions CalculateReliefDimensions(float mainDiameter)
        {
            var diameter = mainDiameter < 1 ? 1 : mainDiameter;
            var neededDimensions = this.Dimensions.Single(dimensions =>
                dimensions.MinInclusive <= diameter &&
                dimensions.MaxInclusive >= diameter);

            return neededDimensions;
        }
    }
}