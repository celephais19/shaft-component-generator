namespace InventorShaftGenerator.Views.EdgeFeatureWindows
{
    public partial class FilletEdgeFeatureWindow : IDialogView
    {
        public FilletEdgeFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}