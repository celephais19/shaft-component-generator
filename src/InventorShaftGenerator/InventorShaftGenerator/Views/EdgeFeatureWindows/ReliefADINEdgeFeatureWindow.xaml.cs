namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefADinEdgeFeatureWindow : IDialogView
    {
        public ReliefADinEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}