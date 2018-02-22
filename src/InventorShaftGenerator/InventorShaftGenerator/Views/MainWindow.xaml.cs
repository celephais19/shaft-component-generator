using System.Windows;
using System.Windows.Controls;

namespace InventorShaftGenerator.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SectionsComboBox.SelectionChanged += OnSectionsComboboxSelectionChanged;
        }

        private void SwitchDimensionsPanelVisibility()
        {
            if (Settings.IsDimensionsPanelEnabled)
            {
                PlacementGroupBox.SetValue(Grid.ColumnSpanProperty, 1);
                SectionsGroupBox.SetValue(Grid.ColumnSpanProperty, 1);
                DimensionsGroupBox.Visibility = Visibility.Visible;
            }
            else
            {
                DimensionsGroupBox.Visibility = Visibility.Hidden;
                PlacementGroupBox.SetValue(Grid.ColumnSpanProperty, 2);
                SectionsGroupBox.SetValue(Grid.ColumnSpanProperty, 2);
            }
        }

        private void Switch2DPreviewPanelVisibility()
        {
            if (Settings.Is2DPreviewEnabled)
            {
                InstalledSectionsPanel.Margin = new Thickness(0);
                InstalledSectionsPanel.SetValue(Grid.RowProperty, 2);
                InstalledSectionsPanel.SetValue(Grid.RowSpanProperty, 1);
                Preview2DPanel.Visibility = Visibility.Visible;
            }
            else
            {
                Preview2DPanel.Visibility = Visibility.Hidden;
                InstalledSectionsPanel.Margin = new Thickness(0, 3, 0, 0);
                InstalledSectionsPanel.SetValue(Grid.RowProperty, 1);
                InstalledSectionsPanel.SetValue(Grid.RowSpanProperty, 2);
            }
        }

        private void OnSectionsComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIdx = ((ComboBox) sender).SelectedIndex;
            switch (selectedIdx)
            {
                case 0:
                    BoreLeftButtonsPanel.Visibility = Visibility.Collapsed;
                    BoreRightButtonsPanel.Visibility = Visibility.Collapsed;
                    SectionButtonsPanel.Visibility = Visibility.Visible;
                    break;
                case 1:
                    SectionButtonsPanel.Visibility = Visibility.Collapsed;
                    BoreRightButtonsPanel.Visibility = Visibility.Collapsed;
                    BoreLeftButtonsPanel.Visibility = Visibility.Visible;
                    break;
                case 2:
                    SectionButtonsPanel.Visibility = Visibility.Collapsed;
                    BoreLeftButtonsPanel.Visibility = Visibility.Collapsed;
                    BoreRightButtonsPanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        {
//            SettingsWindow window = new SettingsWindow(OnSettingsChanged) {Owner = this};
//            window.ShowDialog();
        }

        private void OnSettingsChanged()
        {
            Switch2DPreviewPanelVisibility();
            SwitchDimensionsPanelVisibility();
        }
    }
}