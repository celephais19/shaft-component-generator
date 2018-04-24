namespace InventorShaftGenerator.Views.SectionDimensionsWindows
{
    public partial class CylinderDimensionsWindow : IDialogView
    {
        public CylinderDimensionsWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}