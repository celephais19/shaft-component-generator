using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using InventorShaftGenerator.ViewModels;

namespace InventorShaftGenerator.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var mainViewModel = (MainViewModel) this.DataContext;
            mainViewModel.ConstructionErrorsViewType = typeof(ConstructionErrorsWindow);

            this.MainGrid.OpacityMask = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            this.BuildButton.PreviewMouseLeftButtonDown += (sender, args) =>
            {
                this.MainGrid.Opacity = 0.8;
                this.BusyIndicator.IsBusyIndicatorShowing = true;
            };

            this.IsVisibleChanged += (sender, args) =>
            {
                if (this.Visibility == Visibility.Collapsed)
                {
                    this.BusyIndicator.IsBusyIndicatorShowing = false;
                    this.MainGrid.Opacity = 1;
                }
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            this.Visibility = Visibility.Collapsed;
        }
    }
}