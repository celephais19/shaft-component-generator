namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefFSIEdgeFeatureWindow : IDialogView
    {
        public ReliefFSIEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}