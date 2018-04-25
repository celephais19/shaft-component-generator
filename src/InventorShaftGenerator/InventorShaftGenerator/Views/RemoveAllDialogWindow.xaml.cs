namespace InventorShaftGenerator.Views
{
    public partial class RemoveAllDialogWindow : IDialogView
    {
        public RemoveAllDialogWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}
