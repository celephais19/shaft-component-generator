using System.ComponentModel;
using InventorShaftGenerator.Commands;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ChamferEdgeFeatureViewModel : SectionFeatureViewModel, IEdgeFeatureViewModel, IDataErrorInfo
    {
        private ChamferType chamferType;
        private float distance;
        private float angle;
        private float distance1;
        private float distance2;
        private ChamferEdgeFeature chamferEdgeFeature;
        private RelayCommand setChamferTypeCommand;
        private IChamferFeatureParamsValidator chamferParamsValidator;
        private bool boreSection;

        public EdgeFeaturePosition EdgeFeaturePosition { get; set; }

        public ChamferType ChamferType
        {
            get => this.chamferType;
            set => SetProperty(ref this.chamferType, value);
        }

        public float Distance
        {
            get => this.distance;
            set => SetProperty(ref this.distance, value, nameof(this.Distance));
        }

        public float Angle
        {
            get => this.angle;
            set => SetProperty(ref this.angle, value);
        }

        public float Distance1
        {
            get => this.distance1;
            set => SetProperty(ref this.distance1, value);
        }

        public float Distance2
        {
            get => this.distance2;
            set => SetProperty(ref this.distance2, value);
        }

        public RelayCommand SetChamferTypeCommand => this.setChamferTypeCommand ??
                                                     (this.setChamferTypeCommand = new RelayCommand(o =>
                                                         this.ChamferType = (ChamferType) o));

        protected override void InitilizeFeatureParameters()
        {
            if (this.EdgeFeaturePosition == EdgeFeaturePosition.FirstEdge)
            {
                this.chamferEdgeFeature = (ChamferEdgeFeature) this.Section.FirstEdgeFeature;
            }
            else
            {
                this.chamferEdgeFeature = (ChamferEdgeFeature) this.Section.SecondEdgeFeature;
            }

            if (this.Section.IsBore)
            {
                this.boreSection = true;
            }

            if (this.Section is ConeSection && !this.Section.IsBore)
            {
                this.chamferParamsValidator = new SectionChamferFeatureParamsValidator(this.chamferEdgeFeature);
            }
            else if (this.Section.IsBore)
            {
                this.chamferParamsValidator = new BoreChamferFeatureParamsValidator(this.chamferEdgeFeature);
            }

            this.ChamferType = this.chamferEdgeFeature.ChamferType;

            this.Distance = this.chamferEdgeFeature.Distance;
            this.Distance1 = this.chamferEdgeFeature.Distance1;
            this.Distance2 = this.chamferEdgeFeature.Distance2;
            this.Angle = this.chamferEdgeFeature.Angle;
        }

        protected override void SaveFeatureParameters()
        {
            this.chamferEdgeFeature.ChamferType = this.chamferType;

            switch (this.ChamferType)
            {
                case ChamferType.Distance:
                    this.chamferEdgeFeature.Distance = this.distance;
                    break;

                case ChamferType.TwoDistances:
                    this.chamferEdgeFeature.Distance1 = this.distance1;
                    this.chamferEdgeFeature.Distance2 = this.distance2;
                    break;

                case ChamferType.DistanceAndAngle:
                    this.chamferEdgeFeature.Distance = this.distance;
                    this.chamferEdgeFeature.Angle = this.angle;
                    break;
            }
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

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(this.Distance) when this.chamferType == ChamferType.Distance:
                    {
                        if (this.Section is CylinderSection cylinderSection && (this.boreSection
                                ? !this.chamferParamsValidator.ValidateDistance(this.distance)
                                : this.distance > cylinderSection.Diameter / 2) || this.distance.NearlyEqual(0))
                        {
                            error = "An incorrect distance";
                        }
                        else if (this.Section is ConeSection &&
                                 !this.chamferParamsValidator.ValidateDistance(this.distance))
                        {
                            error = "An incorrect distance";
                        }
                        else if (this.Section is PolygonSection polygonSection &&
                                 this.distance > polygonSection.Length || this.distance.NearlyEqual(0))
                        {
                            error = "An incorrect distance";
                        }

                        break;
                    }

                    case nameof(this.Distance) when this.chamferType == ChamferType.DistanceAndAngle:
                    {
                        switch (this.Section)
                        {
                            case CylinderSection cylinderSection:
                            {
                                if ((this.boreSection
                                        ? !this.chamferParamsValidator.ValidateDistanceAndAngle(this.distance,
                                            this.angle)
                                        : this.distance > cylinderSection.Diameter / 2) ||
                                    this.distance.NearlyEqual(0))
                                {
                                    error = "An incorrect distance";
                                }

                                if (this.angle >= 90 || !this.angle.IsGreaterThanZero())
                                {
                                    OnPropertyChanged(nameof(this.Angle));
                                }

                                break;
                            }

                            case ConeSection _
                                when !this.chamferParamsValidator.ValidateDistanceAndAngle(this.distance, this.angle):
                            {
                                if (!this.chamferParamsValidator.ValidateDistance(this.distance))
                                {
                                    error = "An incorrect distance";
                                }

                                if (this.angle >= 90 || !this.angle.IsGreaterThanZero())
                                {
                                    OnPropertyChanged(nameof(this.Angle));
                                }

                                break;
                            }
                        }

                        break;
                    }

                    case nameof(this.Angle):
                        if (this.angle >= 90 || !this.angle.IsGreaterThanZero())
                        {
                            error = "An incorrect angle";
                        }

                        break;

                    case nameof(this.Distance1):
                    {
                        switch (this.Section)
                        {
                            case CylinderSection cylinderSection when (this.boreSection
                                ? !this.chamferParamsValidator.ValidateTwoDistances(this.distance1, this.distance2)
                                : this.distance1 > cylinderSection.Diameter / 2):
                            {
                                error = "An incorrect first distance";
                                break;
                            }

                            case ConeSection _
                                when !this.chamferParamsValidator.ValidateTwoDistances(this.distance1, this.distance2):
                            {
                                error = "An incorrect first distance";
                                break;
                            }
                        }

                        break;
                    }

                    case nameof(this.Distance2):
                    {
                        switch (this.Section)
                        {
                            case CylinderSection cylinderSection when (this.boreSection
                                ? !this.chamferParamsValidator.ValidateTwoDistances(this.distance1, this.distance2)
                                : this.distance2 > cylinderSection.Diameter / 2):
                            {
                                error = "An incorrect second distance";
                                break;
                            }

                            case ConeSection _
                                when !this.chamferParamsValidator.ValidateTwoDistances(this.distance1, this.distance2):
                            {
                                error = "An incorrect second distance";
                                break;
                            }
                        }

                        break;
                    }
                }

                return error;
            }
        }

        public string Error => null;
    }
}