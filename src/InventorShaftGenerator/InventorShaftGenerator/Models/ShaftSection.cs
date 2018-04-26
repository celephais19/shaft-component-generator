using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using InventorShaftGenerator.Infrastructure;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.SubFeatures;
using InventorShaftGenerator.Properties;

namespace InventorShaftGenerator.Models
{
    public abstract class ShaftSection : INotifyPropertyChanged, IEquatable<ShaftSection>
    {
        private SketchLineSimple firstLine;
        private SketchLineSimple secondLine;
        private SketchLineSimple thirdLine;
        private float length;
        private ISectionFeature firstEdgeFeature;
        private ISectionFeature secondEdgeFeature;
        private ShaftSection nextSection;
        private ShaftSection previousSection;
        private ObservableCollection<EdgeFeature> availableFirstEdgeFeatures = new ObservableCollection<EdgeFeature>();
        private ObservableCollection<EdgeFeature> availableSecondEdgeFeatures = new ObservableCollection<EdgeFeature>();
        private bool firstEdgeFeatureHasErrors;
        private bool secondEdgeFeatureHasErrors;
        private bool isBore;
        private BoreFromEdge? boreFromEdge;
        private bool hasDiameterError;
        private float boreSectionMaxDiameter;
        private BoreDiameterCollisionError boreDiameterCollisionError;

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id { get; } = Guid.NewGuid();

        public virtual SketchLineSimple FirstLine
        {
            get => this.firstLine;
            set => SetProperty(ref this.firstLine, value);
        }

        public virtual SketchLineSimple SecondLine
        {
            get => this.secondLine;
            set => SetProperty(ref this.secondLine, value);
        }

        public virtual SketchLineSimple ThirdLine
        {
            get => this.thirdLine;
            set => SetProperty(ref this.thirdLine, value);
        }

        public float Length
        {
            get => this.length;
            set => SetProperty(ref this.length, value);
        }

        public bool IsBore
        {
            get => this.isBore;
            set => SetProperty(ref this.isBore, value);
        }

        public BoreFromEdge? BoreFromEdge
        {
            get => this.boreFromEdge;
            set => SetProperty(ref this.boreFromEdge, value);
        }

        public ISectionFeature FirstEdgeFeature
        {
            get => this.firstEdgeFeature;
            set
            {
                SetProperty(ref this.firstEdgeFeature, value);

                if (this.firstEdgeFeature == null)
                {
                    return;
                }

                this.FirstEdgeFeature.FeatureErrors.CollectionChanged += (sender, args) =>
                {
                    var errors = (ObservableCollection<ShaftSectionFeatureError>) sender;
                    this.FirstEdgeFeatureHasErrors = errors.Any();
                };

                this.FirstEdgeFeature.PropertyChanged += (sender, args) =>
                {
                    var errors = ((ISectionFeature) sender).FeatureErrors;
                    this.FirstEdgeFeatureHasErrors = errors.Any();
                };
            }
        }

        public ISectionFeature SecondEdgeFeature
        {
            get => this.secondEdgeFeature;
            set
            {
                SetProperty(ref this.secondEdgeFeature, value);

                if (this.secondEdgeFeature == null)
                {
                    return;
                }

                this.SecondEdgeFeature.FeatureErrors.CollectionChanged += (sender, args) =>
                {
                    var errors = (ObservableCollection<ShaftSectionFeatureError>) sender;
                    this.SecondEdgeFeatureHasErrors = errors.Any();
                };
            }
        }

        public ShaftSection PreviousSection
        {
            get => this.previousSection;
            set => SetProperty(ref this.previousSection, value);
        }

        public ShaftSection NextSection
        {
            get => this.nextSection;
            set => SetProperty(ref this.nextSection, value);
        }

        public bool HasDiameterError
        {
            get => this.hasDiameterError;
            set => SetProperty(ref this.hasDiameterError, value);
        }

        public float BoreSectionMaxDiameter
        {
            get => this.boreSectionMaxDiameter;
            set => SetProperty(ref this.boreSectionMaxDiameter, value - 0.1f);
        }

        public BoreDiameterCollisionError BoreDiameterCollisionError
        {
            get => this.boreDiameterCollisionError;
            set => SetProperty(ref this.boreDiameterCollisionError, value);
        }

        public ObservableCollection<ISectionSubFeature> SubFeatures { get; set; } =
            new ObservableCollection<ISectionSubFeature>();

        public ObservableCollection<EdgeFeature> AvailableFirstEdgeFeatures
        {
            get => this.availableFirstEdgeFeatures;
            set => SetProperty(ref this.availableFirstEdgeFeatures, value);
        }

        public ObservableCollection<EdgeFeature> AvailableSecondEdgeFeatures
        {
            get => this.availableSecondEdgeFeatures;
            set => SetProperty(ref this.availableSecondEdgeFeatures, value);
        }

        public bool FirstEdgeFeatureHasErrors
        {
            get => this.firstEdgeFeatureHasErrors;
            set => SetProperty(ref this.firstEdgeFeatureHasErrors, value);
        }

        public bool SecondEdgeFeatureHasErrors
        {
            get => this.secondEdgeFeatureHasErrors;
            set => SetProperty(ref this.secondEdgeFeatureHasErrors, value);
        }

        public abstract string DisplayName { get; }

        public bool IsFirst
        {
            get
            {
                if (!this.isBore)
                {
                    return Shaft.Sections.IndexOf(this) == 0;
                }

                if (this.boreFromEdge == Models.BoreFromEdge.FromLeft)
                {
                    return Shaft.BoreOnTheLeft.IndexOf(this) == 0;
                }

                return Shaft.BoreOnTheRight.IndexOf(this) == 0;
            }
        }

        public bool IsLast
        {
            get
            {
                if (!this.isBore)
                {
                    return Shaft.Sections.IndexOf(this) == Shaft.Sections.Count - 1;
                }

                if (this.boreFromEdge == Models.BoreFromEdge.FromLeft)
                {
                    return Shaft.BoreOnTheLeft.IndexOf(this) == Shaft.BoreOnTheLeft.Count - 1;
                }

                return Shaft.BoreOnTheRight.IndexOf(this) == Shaft.BoreOnTheRight.Count - 1;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;

            this.OnPropertyChanged(propertyName);
            this.OnPropertyChanged(nameof(this.DisplayName));
            return true;
        }

        public bool Equals(ShaftSection other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ShaftSection) obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() * 17;
        }
    }
}