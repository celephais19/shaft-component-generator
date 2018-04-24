namespace InventorShaftGenerator.Models.Sections
{
    public class ConeSection : ShaftSection
    {
        private float diameter1 = 50;
        private float diameter2 = 80;

        public ConeSection()
        {
            this.Length = 100;
        }

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

        public override string DisplayName =>
            $"Cone {this.Diameter1:0.###} / {this.Diameter2:0.###} x {this.Length:0.###}";
    }
}