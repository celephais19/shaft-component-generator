namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefBGostEdgeFeatureWindow : IDialogView
    {
        public ReliefBGostEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}