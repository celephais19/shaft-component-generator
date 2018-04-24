namespace InventorShaftGenerator.Views.SubFeatureWindows
{
    public partial class WrenchSubFeatureWindow : IDialogView
    {
        public WrenchSubFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}