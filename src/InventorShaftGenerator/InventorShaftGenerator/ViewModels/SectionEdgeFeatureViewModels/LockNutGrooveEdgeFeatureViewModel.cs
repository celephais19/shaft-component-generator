using System.Collections.Generic;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class LockNutGrooveEdgeFeatureViewModel : ThreadEdgeFeatureViewModel
    {
        protected override List<ThreadType> Threads { get; } = LockNutGrooveEdgeFeature.Threads;
        private LockNutGrooveEdgeFeature lockNutGrooveEdgeFeature;
        private float width;
        private float depth;
        private float grooveLength;
        private float angle;
        private float radius;
        private bool threadShouldBeBuilt = true;

        public LockNutGrooveEdgeFeatureViewModel()
        {
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(this.ThreadType))
                {
                    this.ThreadShouldBeBuilt = this.ThreadType != "No thread";
                }
            };
        }

        public float Width
        {
            get => this.width;
            set => SetProperty(ref this.width, value);
        }

        public float Depth
        {
            get => this.depth;
            set => SetProperty(ref this.depth, value);
        }

        public float GrooveLength
        {
            get => this.grooveLength;
            set => SetProperty(ref this.grooveLength, value);
        }

        public float Angle
        {
            get => this.angle;
            set => SetProperty(ref this.angle, value);
        }

        public float Radius
        {
            get => this.radius;
            set => SetProperty(ref this.radius, value);
        }

        public bool ThreadShouldBeBuilt
        {
            get => this.threadShouldBeBuilt;
            set => SetProperty(ref this.threadShouldBeBuilt, value);
        }

        protected override void InitializeThreadEdgeFeature()
        {
            this.ThreadEdgeFeature = this.lockNutGrooveEdgeFeature.ThreadEdgeFeature;
        }

        protected override void InitilizeFeatureParameters()
        {
            if (this.EdgeFeaturePosition == EdgeFeaturePosition.FirstEdge)
            {
                this.lockNutGrooveEdgeFeature = (LockNutGrooveEdgeFeature) this.Section.FirstEdgeFeature;
            }
            else
            {
                this.lockNutGrooveEdgeFeature = (LockNutGrooveEdgeFeature) this.Section.SecondEdgeFeature;
            }

            this.CylinderSection = (CylinderSection) this.Section;
            this.Angle = this.lockNutGrooveEdgeFeature.Angle;
            this.Width = this.lockNutGrooveEdgeFeature.Width;
            this.Depth = this.lockNutGrooveEdgeFeature.Depth;
            this.GrooveLength = this.lockNutGrooveEdgeFeature.GrooveLength;
            this.Radius = this.lockNutGrooveEdgeFeature.Radius;

            base.InitilizeFeatureParameters();

            this.ThreadTypes.Insert(0, "No thread");
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();
            this.lockNutGrooveEdgeFeature.Angle = this.Angle;
            this.lockNutGrooveEdgeFeature.Width = this.Width;
            this.lockNutGrooveEdgeFeature.Depth = this.Depth;
            this.lockNutGrooveEdgeFeature.GrooveLength = this.GrooveLength;
            this.lockNutGrooveEdgeFeature.Radius = this.Radius;
        }

        protected override void ValidateProperties()
        {
            base.ValidateProperties();
            OnPropertyChanged(nameof(this.Width));
            OnPropertyChanged(nameof(this.Depth));
            OnPropertyChanged(nameof(this.Radius));
            OnPropertyChanged(nameof(this.GrooveLength));
            OnPropertyChanged(nameof(this.Angle));
        }

        public override string this[string columnName]
        {
            get
            {
                if (this.CylinderSection == null)
                {
                    return string.Empty;
                }

                string error = base[columnName];
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }

                switch (columnName)
                {
                    case nameof(this.Width):
                    case nameof(this.Depth):
                        if (this.width > this.CylinderSection.Diameter / 2 ||
                            this.depth > this.CylinderSection.Diameter / 2)
                        {
                            error = $"Valid range is <{0:F3} mm ; {this.CylinderSection.Diameter / 2:F3} mm>";
                        }

                        break;

                    case nameof(this.Radius):
                        if (this.radius < this.depth)
                        {
                            error = $"Valid range is <{this.depth:F3} mm ; ~ >";
                        }

                        break;

                    case nameof(this.GrooveLength):
                        if (this.grooveLength > this.CylinderSection.Length)
                        {
                            error = $"Valid range is <{0:F3} mm ; {this.CylinderSection.Length:F3} mm";
                        }

                        break;

                    case nameof(this.Angle):
                        if (this.angle < -360 || this.angle > 360)
                        {
                            error = $"Valid range is <{-360:F2} deg ; {360:F2} deg>";
                        }

                        break;
                }

                return error;
            }
        }
    }
}