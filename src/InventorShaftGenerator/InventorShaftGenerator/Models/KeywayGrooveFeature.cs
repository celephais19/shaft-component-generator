using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using InventorShaftGenerator.Models.EdgeFeatures;

namespace InventorShaftGenerator.Models
{
    public abstract class KeywayGrooveFeature : NotifyPropertyChanged, ISectionFeature
    {
        private float width;
        private float keywayLength;
        private float depth;
        private int numberOfKeys;
        private float angle;
        private float angleBetweenKeys;
        public List<KeywayDimensions> KeywaysDimensions { get; protected set; }

        public List<Keyway> Keyways { get; protected set; }

        public Keyway Keyway { get; set; }

        public float Width
        {
            get => this.width;
            set
            {
                SetProperty(ref this.width, value);
                NotifyForAnyErrors();
            }
        }

        public float KeywayLength
        {
            get => this.keywayLength;
            set
            {
                SetProperty(ref this.keywayLength, value);
                NotifyForAnyErrors();
            }
        }

        public float Depth
        {
            get => this.depth;
            set
            {
                SetProperty(ref this.depth, value);
                NotifyForAnyErrors();
            }
        }

        public int NumberOfKeys
        {
            get => this.numberOfKeys;
            set
            {
                SetProperty(ref this.numberOfKeys, value);
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

        public float AngleBetweenKeys
        {
            get => this.angleBetweenKeys;
            set
            {
                SetProperty(ref this.angleBetweenKeys, value);
                NotifyForAnyErrors();
            }
        }

        public ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public Guid Id { get; } = Guid.NewGuid();

        public bool ShouldBeBuilt { get; set; } = true;

        public ShaftSection LinkedSection { get; set; }
        public bool CustomParameters { get; set; }

        public virtual void UpdateFeatureParameters()
        {
        }

        public string DisplayName
        {
            get
            {
                var regexToSplit = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
                return $"{regexToSplit.Replace(this.GetType().Name, " ")} [Keyway: {this.Keyway.Name}]";
            }
        }

        public abstract void InitializeInAccordanceWithSectionParameters(
            ShaftSection section, EdgeFeaturePosition? edgeFeaturePosition);

        protected abstract void NotifyForAnyErrors();
    }
}