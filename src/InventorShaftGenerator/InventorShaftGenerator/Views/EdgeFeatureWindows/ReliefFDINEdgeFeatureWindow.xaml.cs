namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefFDinEdgeFeatureWindow : IDialogView
    {
        public ReliefFDinEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}