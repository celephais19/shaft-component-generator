using System;
using InventorShaftGenerator.Commands;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public bool Is2DPreviewEnabled { get; set; } = Settings.Is2DPreviewEnabled;
        public bool Is3DPreviewEnabled { get; set; } = Settings.Is3DPreviewEnabled;
        public bool IsDimensionsPanelEnabled { get; set; } = Settings.IsDimensionsPanelEnabled;

        public RelayCommand SaveCommand => new RelayCommand(o => SaveSettings((IDialogView) o));

        public RelayCommand CancelCommand => new RelayCommand(o => CancelChanges((IDialogView) o));

        public RelayCommand OpenDialogViewCommand => new RelayCommand(o => OpenDialogView((IDialogView) o));

        private void OpenDialogView(IDialogView dialogView)
        {
            dialogView = (IDialogView) Activator.CreateInstance(dialogView.GetType());
            dialogView.ShowDialog();
        }

        private void SaveSettings(IDialogView view)
        {
            Settings.Is3DPreviewEnabled = Is3DPreviewEnabled;
            Settings.Is2DPreviewEnabled = Is2DPreviewEnabled;
            Settings.IsDimensionsPanelEnabled = IsDimensionsPanelEnabled;

            view?.Close();
        }

        private void CancelChanges(IDialogView view)
        {
            view?.Close();
        }
    }
}