namespace InventorShaftGenerator.Views.SectionDimensionsWindows
{
    public partial class ConeDimensionsWindow : IDialogView
    {
        public ConeDimensionsWindow()
        {
            InitializeComponent();
            this.Owner = StandardAddInServer.MainWindow;
        }
    }
}