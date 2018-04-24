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

            this.BuildButton.PreviewMouseLeftButtonDown += (sender, args) =>
            {
                this.OpacityMask = Brushes.White;
                this.Opacity = 0.96;
                this.BusyIndicator.IsBusyIndicatorShowing = true;
            };

            this.IsVisibleChanged += (sender, args) =>
            {
                if (args.NewValue is false)
                {
                    this.BusyIndicator.IsBusyIndicatorShowing = false;
                    this.Opacity = 1;
                }
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            this.Visibility = Visibility.Hidden;
        }
    }
}