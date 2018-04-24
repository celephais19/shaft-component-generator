namespace InventorShaftGenerator.Views.SubFeatureWindows
{
    public partial class GrooveASubFeatureWindow : IDialogView
    {
        public GrooveASubFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}