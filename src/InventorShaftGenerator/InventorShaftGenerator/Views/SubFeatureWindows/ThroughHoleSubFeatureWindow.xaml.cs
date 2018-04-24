namespace InventorShaftGenerator.Views.SubFeatureWindows
{
    public partial class ThroughHoleSubFeatureWindow : IDialogView
    {
        public ThroughHoleSubFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}