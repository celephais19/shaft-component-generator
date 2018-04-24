namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefEGostEdgeFeatureWindow : IDialogView
    {
        public ReliefEGostEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}