namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ReliefGSIEdgeFeatureWindow : IDialogView
    {
        public ReliefGSIEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}