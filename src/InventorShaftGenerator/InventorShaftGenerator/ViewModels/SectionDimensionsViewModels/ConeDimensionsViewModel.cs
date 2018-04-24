using InventorShaftGenerator.Models.Sections;

namespace InventorShaftGenerator.ViewModels.SectionDimensionsViewModels
{
    public class ConeDimensionsViewModel : SectionDimensionsViewModel<ConeSection>
    {
        private float diameter1;
        private float diameter2;
        private float length;
        private bool isBore;

        public float Diameter1
        {
            get => this.diameter1;
            set => SetProperty(ref this.diameter1, value);
        }

        public float Diameter2
        {
            get => this.diameter2;
            set => SetProperty(ref this.diameter2, value);
        }

        public bool IsBore
        {
            get => this.isBore;
            set => SetProperty(ref this.isBore, value);
        }

        public float Length
        {
            get => this.length;
            set => SetProperty(ref this.length, value);
        }

        public override string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case nameof(this.Diameter1):
                        if (this.Diameter1 < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.Diameter1));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.Diameter1));
                        }

                        break;

                    case nameof(this.Diameter2):
                        if (this.Diameter2 < 0.1)
                        {
                            error = $"Valid range is < {0.1:F3} mm ; ~ >";
                            this.ErrorFields.Add(nameof(this.Diameter2));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.Diameter2));
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
            this.Diameter1 = this.Section.Diameter1;
            this.Diameter2 = this.Section.Diameter2;
            this.Length = this.Section.Length;
            this.IsBore = this.Section.IsBore;
        }

        protected override void SaveDimensions()
        {
            this.Section.Diameter1 = this.Diameter1;
            this.Section.Diameter2 = this.Diameter2;
            this.Section.Length = this.Length;
        }
    }
}