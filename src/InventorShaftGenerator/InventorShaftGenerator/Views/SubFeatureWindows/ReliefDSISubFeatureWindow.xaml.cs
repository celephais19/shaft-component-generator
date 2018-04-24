namespace InventorShaftGenerator.Views.SubFeatureWindows
{
    public partial class ReliefDSISubFeatureWindow : IDialogView
    {
        public ReliefDSISubFeatureWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}