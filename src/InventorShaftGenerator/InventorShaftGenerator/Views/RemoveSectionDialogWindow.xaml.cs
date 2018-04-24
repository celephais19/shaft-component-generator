namespace InventorShaftGenerator.Views
{
    public partial class RemoveSectionDialogWindow : IDialogView
    {
        public RemoveSectionDialogWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}