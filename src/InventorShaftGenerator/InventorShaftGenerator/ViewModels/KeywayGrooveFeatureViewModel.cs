using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels
{
    public abstract class KeywayGrooveFeatureViewModel<TKeywayGrooveFeature> : SectionFeatureViewModel, IDataErrorInfo
        where TKeywayGrooveFeature : KeywayGrooveFeature
    {
        private float mainDiameter;
        private float sectionLength;
        private float width;
        private float keywayLength;
        private float depth;
        private int numberOfKeys;
        private float angle;
        private Keyway selectedKeyway;
        private int selectedIndex;
        private bool noMatchingSizeError;
        private ObservableCollection<KeywayDimensions> keywaysDimensions;
        private ObservableCollection<Keyway> keyways;
        private bool customParameters;
        private float angleBetweenKeys;
        protected CylinderSection CylinderSection { get; private set; }

        protected TKeywayGrooveFeature KeywayGrooveFeature { get; set; }

        public float MainDiameter
        {
            get => this.mainDiameter;
            set => SetProperty(ref this.mainDiameter, value);
        }

        public float SectionLength
        {
            get => this.sectionLength;
            set
            {
                SetProperty(ref this.sectionLength, value);
                OnPropertyChanged(nameof(this.KeywayLength));
            }
        }

        public float Width
        {
            get => this.width;
            set => SetProperty(ref this.width, value);
        }

        public float KeywayLength
        {
            get => this.keywayLength;
            set => SetProperty(ref this.keywayLength, value);
        }

        public float Depth
        {
            get => this.depth;
            set => SetProperty(ref this.depth, value);
        }

        public int NumberOfKeys
        {
            get => this.numberOfKeys;
            set
            {
                SetProperty(ref this.numberOfKeys, value);
                this.AngleBetweenKeys = 360f / value;
            }
        }

        public float Angle
        {
            get => this.angle;
            set => SetProperty(ref this.angle, value);
        }

        public Keyway SelectedKeyway
        {
            get => this.selectedKeyway;
            set
            {
                if (value != null)
                {
                    this.SelectedIndex = this.Keyways
                                             .IndexOf(this.Keyways
                                                          .Single(keyway => keyway.Name == value.Name));
                }

                SetProperty(ref this.selectedKeyway, value);
            }
        }

        public float AngleBetweenKeys
        {
            get => this.angleBetweenKeys;
            set => SetProperty(ref this.angleBetweenKeys, value);
        }

        public int SelectedIndex
        {
            get => this.selectedIndex;
            set => SetProperty(ref this.selectedIndex, value);
        }


        public bool NoMatchingSizeError
        {
            get => this.noMatchingSizeError;
            set => SetProperty(ref this.noMatchingSizeError, value);
        }

        protected ObservableCollection<KeywayDimensions> KeywaysDimensions
        {
            get => this.keywaysDimensions;
            private set => SetProperty(ref this.keywaysDimensions, value);
        }

        public ObservableCollection<Keyway> Keyways
        {
            get => this.keyways;
            private set => SetProperty(ref this.keyways, value);
        }

        public bool CustomParameters
        {
            get => this.customParameters;
            set => SetProperty(ref this.customParameters, value);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.mainDiameter < this.selectedKeyway.MinMainDiameter ||
                this.mainDiameter > this.selectedKeyway.MaxMainDiameter)
            {
                this.NoMatchingSizeError = true;
                return;
            }

            var selectedKeywayDimensions = GetSelectedKeywayDimensions();

            if (selectedKeywayDimensions == null)
            {
                this.NoMatchingSizeError = true;
                return;
            }

            this.NoMatchingSizeError = false;

            switch (e.PropertyName)
            {
                case nameof(this.MainDiameter):
                case nameof(this.SelectedKeyway):
                    this.Width = selectedKeywayDimensions.Width;
                    this.Depth = selectedKeywayDimensions
                                 .Depths.Single(d => d.DepthType == this.SelectedKeyway.DepthType).Value;
                    break;
            }
        }

        private KeywayDimensions GetSelectedKeywayDimensions()
        {
            var selectedKeywayDimensions = this.KeywaysDimensions
                                               .Where(kd =>
                                                   this.MainDiameter >= kd.MinInclusive &&
                                                   this.MainDiameter <= kd.MaxInclusive)
                                               .FirstOrDefault(kd =>
                                                   kd.Depths.Any(d => d.DepthType == this.SelectedKeyway.DepthType));
            return selectedKeywayDimensions;
        }


        protected abstract void InitializeKeywayGrooveFeature();

        protected override void InitilizeFeatureParameters()
        {
            InitializeKeywayGrooveFeature();

            this.KeywaysDimensions = this.KeywayGrooveFeature.KeywaysDimensions.ToObservableCollection();
            this.Keyways = this.KeywayGrooveFeature.Keyways.ToObservableCollection();

            this.CylinderSection = (CylinderSection) this.Section;

            this.SelectedKeyway = this.KeywayGrooveFeature.Keyway;

            this.MainDiameter = this.CylinderSection.Diameter;
            this.SectionLength = this.CylinderSection.Length;
            this.CustomParameters = this.KeywayGrooveFeature.CustomParameters;
            var selectedKeywayDimensions = this.KeywaysDimensions
                                               .Where(kd =>
                                                   this.mainDiameter >= kd.MinInclusive &&
                                                   this.mainDiameter <= kd.MaxInclusive).First(kd =>
                                                   kd.Depths.Any(d => d.DepthType == this.selectedKeyway.DepthType));

            this.Width = this.KeywayGrooveFeature.Width >= 0
                ? this.KeywayGrooveFeature.Width
                : selectedKeywayDimensions.Width;

            this.KeywayLength = this.KeywayGrooveFeature.KeywayLength;
            this.Depth = this.KeywayGrooveFeature.Depth >= 0
                ? this.KeywayGrooveFeature.Depth
                : selectedKeywayDimensions.Depths.Single(d => d.DepthType == this.selectedKeyway.DepthType).Value;
            this.NumberOfKeys = this.KeywayGrooveFeature.NumberOfKeys;
            this.Angle = this.KeywayGrooveFeature.Angle;
            this.AngleBetweenKeys = this.KeywayGrooveFeature.AngleBetweenKeys;

            this.PropertyChanged += OnPropertyChanged;
        }

        protected override void SaveFeatureParameters()
        {
            var cylinderSection = (CylinderSection) this.Section;
            cylinderSection.Diameter = this.MainDiameter;
            cylinderSection.Length = this.SectionLength;
            this.KeywayGrooveFeature.Keyway = this.selectedKeyway;
            this.KeywayGrooveFeature.Width = this.width;
            this.KeywayGrooveFeature.KeywayLength = this.keywayLength;
            this.KeywayGrooveFeature.Depth = this.depth;
            this.KeywayGrooveFeature.NumberOfKeys = this.numberOfKeys;
            this.KeywayGrooveFeature.Angle = this.angle;
            this.KeywayGrooveFeature.CustomParameters = this.customParameters;
            this.KeywayGrooveFeature.AngleBetweenKeys = this.angleBetweenKeys;
        }

        public virtual string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(this.MainDiameter):
                        if (this.MainDiameter < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.MainDiameter));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.MainDiameter));
                        }

                        break;

                    case nameof(this.SectionLength):
                        if (this.sectionLength < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.SectionLength));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.SectionLength));
                        }

                        break;

                    case nameof(this.KeywayLength):
                        if (this.keywayLength > this.sectionLength)
                        {
                            error = $"Valid range is < {0:F3} mm; {this.sectionLength:F3} mm >";
                        }

                        break;

                    case nameof(this.Angle):
                        if (this.angle < -360 || this.angle > 360)
                        {
                            error = $"Valid range is < {-360:F2} deg ; {360:F2} deg >";
                        }

                        break;

                    case nameof(this.AngleBetweenKeys):
                        if (this.angleBetweenKeys < -360 || this.angleBetweenKeys > 360)
                        {
                            error = $"Valid range is < {-360:F2} deg ; {360:F2} deg >";
                            this.ErrorFields.Add(nameof(this.AngleBetweenKeys));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.AngleBetweenKeys));
                        }

                        break;

                    case nameof(this.NumberOfKeys):
                        if (this.NumberOfKeys < 1 || this.NumberOfKeys > 4)
                        {
                            error = $"Valid range is < 1 ul ; 4 ul >";
                        }

                        break;
                }

                return error;
            }
        }

        public string Error => null;
    }
}