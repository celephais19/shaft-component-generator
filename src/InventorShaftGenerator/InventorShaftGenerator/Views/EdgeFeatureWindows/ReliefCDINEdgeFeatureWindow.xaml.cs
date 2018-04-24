namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefCDinEdgeFeatureWindow : IDialogView
    {
        public ReliefCDinEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}