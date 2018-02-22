using System.Windows;

namespace InventorShaftGenerator.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : IDialogView
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Owner = StandardAddInServer.MainWindow;
        }
    }
}
