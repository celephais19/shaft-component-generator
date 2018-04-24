namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class ThreadEdgeFeatureWindow : IDialogView
    {
        public ThreadEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}