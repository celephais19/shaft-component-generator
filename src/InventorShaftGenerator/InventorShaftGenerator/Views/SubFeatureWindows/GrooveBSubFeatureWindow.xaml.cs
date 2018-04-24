namespace InventorShaftGenerator.Views.SubFeatureWindows
{
    public partial class GrooveBSubFeatureWindow : IDialogView
    {
        public GrooveBSubFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}