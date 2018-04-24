using InventorShaftGenerator.Commands;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private bool is2DPreviewEnabled = Settings.Is2DPreviewEnabled;
        private bool is3DPreviewEnabled = Settings.Is3DPreviewEnabled;
        private bool isDimensionsPanelEnabled = Settings.IsDimensionsPanelEnabled;

        public bool Is2DPreviewEnabled
        {
            get => this.is2DPreviewEnabled;
            set => SetProperty(ref this.is2DPreviewEnabled, value);
        }

        public bool Is3DPreviewEnabled
        {
            get => this.is3DPreviewEnabled;
            set => SetProperty(ref this.is3DPreviewEnabled, value);
        }

        public bool IsDimensionsPanelEnabled
        {
            get => this.isDimensionsPanelEnabled;
            set => SetProperty(ref this.isDimensionsPanelEnabled, value);
        }

        public RelayCommand SaveCommand => new RelayCommand(o => SaveSettings((IDialogView) o));

        public RelayCommand CancelCommand => new RelayCommand(o => CancelChanges((IDialogView) o));

        private void SaveSettings(IDialogView view)
        {
            Settings.Is3DPreviewEnabled = this.Is3DPreviewEnabled;
            Settings.Is2DPreviewEnabled = this.Is2DPreviewEnabled;
            Settings.IsDimensionsPanelEnabled = this.IsDimensionsPanelEnabled;

            view?.Close();
        }

        private void CancelChanges(IDialogView view)
        {
            view?.Close();
        }
    }
}