using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ThreadEdgeFeatureViewModel : SectionFeatureViewModel, IEdgeFeatureViewModel, IDataErrorInfo
    {
        private string threadType;
        private float chamfer;
        private string designation;
        private string threadClass;
        private float threadLength;
        private string threadDirection;
        private string size;
        private ObservableCollection<string> sizes = new ObservableCollection<string>();
        private ObservableCollection<string> designations = new ObservableCollection<string>();
        private ObservableCollection<string> classes = new ObservableCollection<string>();
        private bool areSizesChanging;
        private bool areDesignationsChanging;
        private float sectionLength;
        private bool isSaveEnabled;
        private IChamferFeatureParamsValidator chamferParamsValidator;
        private bool boreSection;

        protected CylinderSection CylinderSection { get; set; }

        protected virtual List<ThreadType> Threads { get; } = ThreadEdgeFeature.Threads;

        protected ThreadEdgeFeature ThreadEdgeFeature { get; set; }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.ThreadType):
                    if (this.threadType == "No thread")
                    {
                        return;
                    }

                    this.areSizesChanging = true;
                    var sizes = this.Threads.Single(t => t.Name == this.threadType).Sizes.Select(s => s.Value)
                                    .ToObservableCollection();
                    this.Sizes = sizes;
                    this.areSizesChanging = false;
                    this.Size = sizes.First();

                    break;

                case nameof(this.Size):
                    if (this.areSizesChanging)
                    {
                        return;
                    }

                    this.areDesignationsChanging = true;
                    var designations = this.Threads.Single(t => t.Name == this.threadType).Sizes
                                           .Single(s => s.Value == this.size).Designations.Select(d => d.Value)
                                           .ToObservableCollection();
                    this.Designations = designations;
                    this.areDesignationsChanging = false;
                    this.Designation = designations.First();
                    break;

                case nameof(this.Designation):
                    if (this.areDesignationsChanging)
                    {
                        return;
                    }

                    var classes = this.Threads.Single(t => t.Name == this.threadType).Sizes
                                      .Single(s => s.Value == this.size)
                                      .Designations.Single(d => d.Value == this.designation).Classes
                                      .Select(c => c.Name).ToObservableCollection();
                    this.Classes = classes;
                    this.Class = classes.First();

                    break;

                default:
                    return;
            }
        }

        public ObservableCollection<string> ThreadTypes { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> Sizes
        {
            get => this.sizes;
            private set => SetProperty(ref this.sizes, value);
        }


        public ObservableCollection<string> Designations
        {
            get => this.designations;
            private set => SetProperty(ref this.designations, value);
        }

        public ObservableCollection<string> Classes
        {
            get => this.classes;
            private set => SetProperty(ref this.classes, value);
        }

        public List<string> ThreadDirections => new List<string>
        {
            "Left hand",
            "Right hand"
        };

        public string ThreadType
        {
            get => this.threadType;
            set => SetProperty(ref this.threadType, value);
        }

        public string Size
        {
            get => this.size;
            set
            {
                SetProperty(ref this.size, value);
                float floatValue = float.Parse(value);
                if (this.ThreadType == "ANSI Unified Screw Threads" || this.ThreadType == "Inch Tapping Threads" ||
                    this.ThreadType == "AFBMA Standard Locknuts")
                {
                    floatValue = floatValue * 25.4f;
                }

                this.CylinderSection.Diameter = floatValue;
                if (this.CylinderSection != null)
                {
                    ValidateProperties();
                }
            }
        }

        public string Designation
        {
            get => this.designation;
            set => SetProperty(ref this.designation, value);
        }

        public string Class
        {
            get => this.threadClass;
            set => SetProperty(ref this.threadClass, value);
        }

        public float Chamfer
        {
            get => this.chamfer;
            set => SetProperty(ref this.chamfer, value);
        }

        public float ThreadLength
        {
            get => this.threadLength;
            set => SetProperty(ref this.threadLength, value);
        }

        public string ThreadDirection
        {
            get => this.threadDirection;
            set => SetProperty(ref this.threadDirection, value);
        }

        public float SectionLength
        {
            get => this.sectionLength;
            set => SetProperty(ref this.sectionLength, value);
        }

        public EdgeFeaturePosition EdgeFeaturePosition { get; set; }

        protected virtual void InitializeThreadEdgeFeature()
        {
            if (this.EdgeFeaturePosition == EdgeFeaturePosition.FirstEdge)
            {
                this.ThreadEdgeFeature = (ThreadEdgeFeature) this.Section.FirstEdgeFeature;
            }
            else
            {
                this.ThreadEdgeFeature = (ThreadEdgeFeature) this.Section.SecondEdgeFeature;
            }
        }

        protected override void InitilizeFeatureParameters()
        {
            InitializeThreadEdgeFeature();
            this.CylinderSection = (CylinderSection) this.Section;

            if (this.CylinderSection.IsBore)
            {
                this.boreSection = true;
                var chamferEdgeFeature = new ChamferEdgeFeature()
                {
                    LinkedSection = this.CylinderSection,
                    Distance = this.ThreadEdgeFeature.Chamfer,
                    ChamferType = ChamferType.Distance,
                    EdgePoint = this.ThreadEdgeFeature.EdgePoint,
                    EdgePosition = this.ThreadEdgeFeature.EdgePosition
                };

                this.chamferParamsValidator = new BoreChamferFeatureParamsValidator(chamferEdgeFeature);
            }

            this.ThreadType = this.ThreadEdgeFeature.ThreadType;
            this.Size = this.ThreadEdgeFeature.Size;
            this.Designation = this.ThreadEdgeFeature.Designation;
            this.Class = this.ThreadEdgeFeature.Class;
            this.SectionLength = this.Section.Length;
            this.Chamfer = this.ThreadEdgeFeature.Chamfer;
            this.ThreadLength = this.ThreadEdgeFeature.ThreadLength;
            this.ThreadDirection =
                this.ThreadEdgeFeature.ThreadDirection == Models.EdgeFeatures.ThreadDirection.LeftHand
                    ? "Left hand"
                    : "Right hand";

            foreach (var threadType in this.Threads.Select(t => t.Name))
            {
                this.ThreadTypes.Add(threadType);
            }

            var sizes = this.Threads.Single(t => t.Name == this.threadType).Sizes;
            foreach (var size in sizes.Select(s => s.Value))
            {
                this.Sizes.Add(size);
            }

            var designations = sizes.Single(s => s.Value == this.size).Designations;
            foreach (var designation in designations.Select(d => d.Value))
            {
                this.Designations.Add(designation);
            }

            var classes = designations.First().Classes.Select(c => c.Name);
            foreach (var threadTypeClass in classes)
            {
                this.Classes.Add(threadTypeClass);
            }

            this.PropertyChanged += OnPropertyChanged;
        }

        protected override void SaveFeatureParameters()
        {
            this.ThreadEdgeFeature.ThreadType = this.ThreadType;
            this.ThreadEdgeFeature.Size = this.Size;
            this.ThreadEdgeFeature.Designation = this.Designation;
            this.ThreadEdgeFeature.Class = this.Class;
            this.ThreadEdgeFeature.Chamfer = this.Chamfer;
            this.Section.Length = this.sectionLength;
            this.ThreadEdgeFeature.ThreadLength = this.ThreadLength;
            this.ThreadEdgeFeature.ThreadDirection = this.ThreadDirection == "Left hand"
                ? Models.EdgeFeatures.ThreadDirection.LeftHand
                : Models.EdgeFeatures.ThreadDirection.RightHand;
        }

        protected override void SaveChanges(IDialogView dialogView)
        {
            this.chamferParamsValidator?.Dispose();

            base.SaveChanges(dialogView);
        }

        protected override void CancelChanges(IDialogView dialogView)
        {
            this.chamferParamsValidator?.Dispose();

            base.CancelChanges(dialogView);
        }

        protected virtual void ValidateProperties()
        {
            OnPropertyChanged(nameof(this.SectionLength));
            OnPropertyChanged(nameof(this.Chamfer));
            OnPropertyChanged(nameof(this.ThreadLength));
        }

        public virtual string this[string columnName]
        {
            get
            {
                if (this.CylinderSection == null)
                {
                    return null;
                }

                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(this.SectionLength):
                        if (this.sectionLength.NearlyEqual(0))
                        {
                            error = $"Valid range is <{0:F3} mm ; ~ >";
                            this.IsSaveEnabled = false;
                        }
                        else
                        {
                            this.IsSaveEnabled = true;
                        }

                        OnPropertyChanged(nameof(this.ThreadLength));
                        OnPropertyChanged(nameof(this.Chamfer));

                        break;

                    case nameof(this.Chamfer):
                        if (this.boreSection
                            ? !this.chamferParamsValidator.ValidateDistance(this.chamfer)
                            : this.chamfer > this.CylinderSection.Diameter / 2)
                        {
                            error = "The chamfer distance is greater than the section radius";
                        }
                        else if (this.chamfer > this.sectionLength)
                        {
                            error = "The chamfer distance is greater that the section length";
                        }

                        break;

                    case nameof(this.ThreadLength):
                        if (this.threadLength > this.sectionLength)
                        {
                            error = "The thread length is greater than the section length";
                        }

                        break;
                }

                return error;
            }
        }

        public string Error => null;
    }
}