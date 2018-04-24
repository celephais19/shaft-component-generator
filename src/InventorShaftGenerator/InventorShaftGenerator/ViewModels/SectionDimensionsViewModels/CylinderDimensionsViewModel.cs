using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels.SectionDimensionsViewModels
{
    public class CylinderDimensionsViewModel : SectionDimensionsViewModel<CylinderSection>
    {
        private float diameter;
        private float length;
        private bool isBore;

        public float Diameter
        {
            get => this.diameter;
            set => SetProperty(ref this.diameter, value);
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

        public override string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(this.Diameter):
                        if (this.Diameter < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.Diameter));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.Diameter));
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
                }

                return error;
            }
        }

        protected override void InitializeDimensions()
        {
            this.Diameter = this.Section.Diameter;
            this.Length = this.Section.Length;
            this.IsBore = this.Section.IsBore;
        }

        protected override void SaveDimensions()
        {
            this.Section.Diameter = this.Diameter;
            this.Section.Length = this.Length;
        }
    }
}