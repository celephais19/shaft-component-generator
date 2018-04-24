namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ChamferEdgeFeatureWindow : IDialogView
    {
        public ChamferEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}