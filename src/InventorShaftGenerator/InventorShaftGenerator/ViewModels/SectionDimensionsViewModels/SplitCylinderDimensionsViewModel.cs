using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels.SectionDimensionsViewModels
{
    public class SplitCylinderDimensionsViewModel : SectionDimensionsViewModel<CylinderSection>
    {
        private float initialDiameter;
        private float initialLength;
        private float mainDiameter1;
        private float mainDiameter2;
        private float sectionLength1;
        private float sectionLength2;
        private bool updating;

        public float MainDiameter1
        {
            get => this.mainDiameter1;
            set => SetProperty(ref this.mainDiameter1, value);
        }

        public float MainDiameter2
        {
            get => this.mainDiameter2;
            set => SetProperty(ref this.mainDiameter2, value);
        }

        public float SectionLength1
        {
            get => this.sectionLength1;
            set
            {
                SetProperty(ref this.sectionLength1, value);

                if (this.updating)
                {
                    return;
                }

                this.updating = true;
                this.SectionLength2 = this.initialLength - value;
                this.updating = false;
            }
        }

        public float SectionLength2
        {
            get => this.sectionLength2;
            set
            {
                SetProperty(ref this.sectionLength2, value);

                if (this.updating)
                {
                    return;
                }

                this.updating = true;
                this.SectionLength1 = this.initialLength - value;
                this.updating = false;
            }
        }

        protected override void InitializeDimensions()
        {
            this.initialDiameter = this.Section.Diameter;
            this.initialLength = this.Section.Length;
            this.MainDiameter1 = this.initialDiameter;
            this.MainDiameter2 = this.initialDiameter;
            this.SectionLength1 = this.initialLength / 2;
            this.SectionLength2 = this.initialLength / 2;
        }

        protected override void SaveDimensions()
        {
            // Ignored
        }

        public override string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(this.MainDiameter1):
                        if (this.mainDiameter1 < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.MainDiameter1));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.MainDiameter1));
                        }

                        break;

                    case nameof(this.MainDiameter2):
                        if (this.mainDiameter2 < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.MainDiameter2));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.MainDiameter2));
                        }

                        break;

                    case nameof(this.SectionLength1):
                        if (this.sectionLength1 < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.SectionLength1));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.SectionLength1));
                        }

                        break;

                    case nameof(this.SectionLength2):
                        if (this.sectionLength1 < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.SectionLength2));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.SectionLength2));
                        }

                        break;
                }

                return error;
            }
        }
    }
}