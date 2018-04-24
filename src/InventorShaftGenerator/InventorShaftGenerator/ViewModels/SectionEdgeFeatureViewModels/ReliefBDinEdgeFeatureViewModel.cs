using InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits;

namespace InventorShaftGenerator.ViewModels.SectionEdgeFeatureViewModels
{
    public class ReliefBDinEdgeFeatureViewModel : ReliefEdgeFeatureViewModel<ReliefBDinEdgeFeature>
    {
        private float width1;
        private float width2;
        private float angle;
        private float machiningAllowance;

        public float MachiningAllowance
        {
            get => this.machiningAllowance;
            set => SetProperty(ref this.machiningAllowance, value);
        }

        public float Width1
        {
            get => this.width1;
            set => SetProperty(ref this.width1, value);
        }

        public float Width2
        {
            get => this.width2;
            set => SetProperty(ref this.width2, value);
        }

        public float Angle
        {
            get => this.angle;
            set => SetProperty(ref this.angle, value);
        }
        

        protected override void InitilizeFeatureParameters()
        {
            base.InitilizeFeatureParameters();

            this.Width1 = this.ReliefEdgeFeature.Width1;
            this.Width2 = this.ReliefEdgeFeature.Width2;
            this.Angle = this.ReliefEdgeFeature.Angle;
            this.MachiningAllowance = this.ReliefEdgeFeature.MachiningAllowance;
        }

        protected override void SaveFeatureParameters()
        {
            base.SaveFeatureParameters();

            this.ReliefEdgeFeature.Width1 = this.width1;
            this.ReliefEdgeFeature.Width2 = this.width2;
            this.ReliefEdgeFeature.Angle = this.angle;
            this.ReliefEdgeFeature.MachiningAllowance = this.machiningAllowance;
        }
        
        public override string this[string columnName]
        {
            get
            {
                string error = base[columnName];
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }

                switch (columnName)
                {
                    case nameof(this.Angle):
                        if (this.angle < 30 || this.angle > 60)
                        {
                            error = $"Valid range is < {30:F2} deg ; {60:F2} deg >";
                            this.ErrorFields.Add(nameof(this.Angle));
                        }
                        else
                        {
                            this.ErrorFields.Remove(nameof(this.Angle));
                        }

                        break;
                }

                return error;
            }
        }
    }
}