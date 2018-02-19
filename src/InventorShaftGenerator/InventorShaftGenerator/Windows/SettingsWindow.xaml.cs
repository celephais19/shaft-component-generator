using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InventorShaftGenerator.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly Action onSettingsChanged;

        public SettingsWindow(Action onSettingsChanged)
        {
            this.onSettingsChanged = onSettingsChanged;
            InitializeComponent();
            Is2DPreviewVisibleCheckBox.IsChecked = MainWindowSettings.Is2DPreviewPanelVisible;
            IsDimensionsSidebarVisibleCheckBox.IsChecked = MainWindowSettings.IsDimensionsPanelVisible;
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindowSettings.Is2DPreviewPanelVisible = (bool)Is2DPreviewVisibleCheckBox.IsChecked;
            MainWindowSettings.Is3DPreviewVisible = (bool)Is3DPreviewVisibleCheckBox.IsChecked;
            MainWindowSettings.IsDimensionsPanelVisible = (bool)IsDimensionsSidebarVisibleCheckBox.IsChecked;

            onSettingsChanged.Invoke();

            this.Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
