using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models.SubFeatures
{
    public class ReliefDSISubFeature : NotifyPropertyChanged, ISectionSubFeature
    {
        private float distance;
        private float width;
        private float reliefDepth;
        private float radius;
        private float machiningAllowance;
        private CylinderSection cylinderSection;

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

        public float Width
        {
            get => this.width;
            set
            {
                SetProperty(ref this.width, value);
                NotifyForAnyErrors();
            }
        }

        public float ReliefDepth
        {
            get => this.reliefDepth;
            set
            {
                SetProperty(ref this.reliefDepth, value);
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

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set
            {
                SetProperty(ref this.machiningAllowance, value);
                NotifyForAnyErrors();
            }
        }

        public Guid Id { get; } = Guid.NewGuid();

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public bool ShouldBeBuilt { get; set; } = true;

        public ShaftSection LinkedSection { get; set; }

        public List<ReliefDimensions> Dimensions { get; } =
            JsonSerializer.Deserialize<List<ReliefDimensions>>(Resources.reliefDSIDimensions);

        public void UpdateFeatureParameters()
        {
        }

        public string DisplayName => $"Relief D SI [Depth: {this.ReliefDepth}]";

        public void InitializeInAccordanceWithSectionParameters(ShaftSection section,
            EdgeFeaturePosition? edgeFeaturePosition)
        {
            this.LinkedSection = this.cylinderSection = (CylinderSection) section;

            var neededDimensions = CalculateDimensions(this.cylinderSection.Diameter);

            this.Width = neededDimensions.Width;
            this.ReliefDepth = neededDimensions.ReliefDepth;
            this.Radius = neededDimensions.Radius;
        }

        public ReliefDimensions CalculateDimensions(float mainDiameter)
        {
            var diameter = mainDiameter < 1 ? 1 : mainDiameter;
            var neededDimensions = this.Dimensions.Single(dimensions =>
                dimensions.MinInclusive <= diameter &&
                dimensions.MaxInclusive >= diameter);

            return neededDimensions;
        }

        private void NotifyForAnyErrors()
        {
        }
    }
}