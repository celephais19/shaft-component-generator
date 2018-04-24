namespace InventorShaftGenerator.Views
{
    public partial class RemoveSubFeatureDialogWindow : IDialogView
    {
        public RemoveSubFeatureDialogWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}