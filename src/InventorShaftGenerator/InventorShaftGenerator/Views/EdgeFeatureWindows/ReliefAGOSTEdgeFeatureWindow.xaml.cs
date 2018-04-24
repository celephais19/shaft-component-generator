namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefAGostEdgeFeatureWindow : IDialogView
    {
        public ReliefAGostEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}