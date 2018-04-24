namespace InventorShaftGenerator.Views.SectionDimensionsWindows
{
    public partial class PolygonDimensionsWindow : IDialogView
    {
        public PolygonDimensionsWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}