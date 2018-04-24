namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefEDinEdgeFeatureWindow : IDialogView
    {
        public ReliefEDinEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}