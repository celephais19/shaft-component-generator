using InventorShaftGenerator.Commands;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ViewModels
{
    public abstract class RemoveItemViewModel : ViewModelBase, IViewModelWithSection
    {
        private RelayCommand removeCommand;
        private RelayCommand cancelCommand;

        public ShaftSection Section { get; set; }

        public RelayCommand RemoveItemCommand =>
            this.removeCommand ?? (this.removeCommand = new RelayCommand(o => Accept((IDialogView) o)));

        public RelayCommand CancelCommand =>
            this.cancelCommand ?? (this.cancelCommand = new RelayCommand(o => Cancel((IDialogView) o)));

        private void Accept(IDialogView dialogView)
        {
            RemoveItem();

            dialogView.Close();
        }

        private void Cancel(IDialogView dialogView)
        {
            dialogView.Close();
        }

        protected abstract void RemoveItem();
    }
}