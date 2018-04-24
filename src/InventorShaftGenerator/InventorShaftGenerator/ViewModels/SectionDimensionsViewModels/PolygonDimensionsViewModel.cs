using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels.SectionDimensionsViewModels
{
    public class PolygonDimensionsViewModel : SectionDimensionsViewModel<PolygonSection>
    {
        private float circumscribedCircleDiameter;
        private float inscribedCircleDiameter;
        private int numberOfEdges;
        private float sectionAngle;
        private float length;
        private bool inscribedActive;
        private bool oneWaySettingActive;
        private bool initializationActive;
        private bool invalidNumberOfEdgesRange;

        public float CircumscribedCircleDiameter
        {
            get => this.circumscribedCircleDiameter;
            set
            {
                SetProperty(ref this.circumscribedCircleDiameter, value);

                if (this.oneWaySettingActive)
                {
                    return;
                }

                this.oneWaySettingActive = true;
                this.InscribedCircleDiameter = this.Section.CalculateInscribedRadius(value, this.initializationActive
                    ? this.Section.NumberOfEdges
                    : this.numberOfEdges);
                this.oneWaySettingActive = false;
            }
        }

        public float InscribedCircleDiameter
        {
            get => this.inscribedCircleDiameter;
            set
            {
                SetProperty(ref this.inscribedCircleDiameter, value);

                if (this.oneWaySettingActive)
                {
                    return;
                }

                this.oneWaySettingActive = true;
                this.CircumscribedCircleDiameter = this.Section.CalculateCircumscribedRadius(value,
                    this.initializationActive
                        ? this.Section.NumberOfEdges
                        : this.numberOfEdges);
                this.oneWaySettingActive = false;
            }
        }

        public int NumberOfEdges
        {
            get => this.numberOfEdges;
            set
            {
                SetProperty(ref this.numberOfEdges, value);
                if (value >= 3 && value <= 40)
                {
                    this.InvalidNumberOfEdges = false;
                    if (this.inscribedCircleDiameter >= 0.1 && this.circumscribedCircleDiameter >= 0.1)
                    {
                        this.oneWaySettingActive = true;
                        this.CircumscribedCircleDiameter =
                            this.Section.CalculateCircumscribedRadius(this.inscribedCircleDiameter, value);
                        this.InscribedCircleDiameter =
                            this.Section.CalculateInscribedRadius(this.circumscribedCircleDiameter, value);
                        this.oneWaySettingActive = false;
                    }
                }
                else
                {
                    this.InvalidNumberOfEdges = true;
                }
            }
        }

        public float SectionAngle
        {
            get => this.sectionAngle;
            set => SetProperty(ref this.sectionAngle, value);
        }

        public float Length
        {
            get => this.length;
            set => SetProperty(ref this.length, value);
        }

        public bool InscribedActive
        {
            get => this.inscribedActive;
            set => SetProperty(ref this.inscribedActive, value);
        }

        public bool InvalidNumberOfEdges
        {
            get => this.invalidNumberOfEdgesRange;
            set => SetProperty(ref this.invalidNumberOfEdgesRange, value);
        }

        protected override void InitializeDimensions()
        {
            this.initializationActive = true;
            this.CircumscribedCircleDiameter = this.Section.CircumscribedCircleDiameter;
            this.InscribedCircleDiameter = this.Section.InscribedCircleDiameter;
            this.NumberOfEdges = this.Section.NumberOfEdges;
            this.SectionAngle = this.Section.SectionAngle;
            this.Length = this.Section.Length;
            this.initializationActive = false;
        }

        protected override void SaveDimensions()
        {
            this.Section.CircumscribedCircleDiameter = this.circumscribedCircleDiameter;
            this.Section.InscribedCircleDiameter = this.inscribedCircleDiameter;
            this.Section.NumberOfEdges = this.numberOfEdges;
            this.Section.SectionAngle = this.sectionAngle;
            this.Section.Length = this.length;
        }

        public override string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(this.InscribedCircleDiameter):
                        if (this.inscribedCircleDiameter < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.InscribedCircleDiameter));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.InscribedCircleDiameter));
                        }

                        break;

                    case nameof(this.CircumscribedCircleDiameter):
                        if (this.CircumscribedCircleDiameter < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.CircumscribedCircleDiameter));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.CircumscribedCircleDiameter));
                        }

                        break;

                    case nameof(this.Length):
                        if (this.Length < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.Length));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.Length));
                        }

                        break;

                    case nameof(this.NumberOfEdges):
                        if (this.numberOfEdges < 3 || this.numberOfEdges > 40)
                        {
                            error = $"Valid range is < 3 ul ; 40 ul >";
                            this.ErrorFields.Add(nameof(this.NumberOfEdges));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.NumberOfEdges));
                        }

                        break;

                    case nameof(this.SectionAngle):
                        if (this.sectionAngle < -360 || this.sectionAngle > 360)
                        {
                            error = $"Valid range is < {-360:F2} deg ; {360:F2} deg >";
                            this.ErrorFields.Add(nameof(this.SectionAngle));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.SectionAngle));
                        }

                        break;
                }

                return error;
            }
        }
    }
}