namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefASIEdgeFeatureWindow : IDialogView
    {
        public ReliefASIEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}