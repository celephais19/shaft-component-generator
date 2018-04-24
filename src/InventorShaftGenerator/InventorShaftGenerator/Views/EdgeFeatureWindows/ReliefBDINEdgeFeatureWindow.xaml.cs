namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefBDinEdgeFeatureWindow : IDialogView
    {
        public ReliefBDinEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}