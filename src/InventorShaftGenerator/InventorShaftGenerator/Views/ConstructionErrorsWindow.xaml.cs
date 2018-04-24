namespace InventorShaftGenerator.Views
{
    public partial class ConstructionErrorsWindow : IView
    {
        public ConstructionErrorsWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;

            this.Loaded += (sender, args) =>
                StandardAddInServer.MainWindow.BusyIndicator.IsBusyIndicatorShowing = false;
        }
    }
}