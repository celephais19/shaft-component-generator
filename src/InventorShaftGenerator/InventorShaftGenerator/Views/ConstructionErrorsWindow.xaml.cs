namespace InventorShaftGenerator.Views
{
    public partial class ConstructionErrorsWindow : IView
    {
        public ConstructionErrorsWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;

            this.Loaded += (sender, args) =>
            {
                var mainWindow = StandardAddInServer.MainWindow;
                mainWindow.BusyIndicator.IsBusyIndicatorShowing = false;
                mainWindow.MainGrid.Opacity = 1;
            };
        }
    }
}