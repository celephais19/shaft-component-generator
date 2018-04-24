namespace InventorShaftGenerator.Models.Sections
{
    public class CylinderSection : ShaftSection
    {
        private float diameter = 50;

        public CylinderSection()
        {
            this.Length = 100;
        }

        public float Diameter
        {
            get => this.diameter;
            set => SetProperty(ref this.diameter, value);
        }

        public override string DisplayName => $"Cylinder {this.Diameter:0.###} x {this.Length:0.###}";
    }
}