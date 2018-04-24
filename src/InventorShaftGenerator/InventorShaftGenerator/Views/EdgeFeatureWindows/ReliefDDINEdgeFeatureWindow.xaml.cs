namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefDDinEdgeFeatureWindow : IDialogView
    {
        public ReliefDDinEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}