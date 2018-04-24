namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefBSIEdgeFeatureWindow : IDialogView
    {
        public ReliefBSIEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}