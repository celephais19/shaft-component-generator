using InventorShaftGenerator.Commands;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ViewModels
{
    public abstract class SectionFeatureViewModel : ViewModelBase, IViewModelWithSection
    {
        private ShaftSection section;
        private RelayCommand saveCommand;
        private RelayCommand cancelCommand;
        private bool isSaveEnabled = true;

        protected SectionFeatureViewModel()
        {
            this.ErrorFields.CollectionChanged += (sender, args) =>
            {
                this.IsSaveEnabled = this.ErrorFields.Count == 0;
            };
        }

        protected ObservableSet<string> ErrorFields { get; } = new ObservableSet<string>();

        public ShaftSection Section
        {
            get => this.section;
            set
            {
                SetProperty(ref this.section, value);
                InitializeViewModel();
            }
        }

        public RelayCommand SaveCommand =>
            this.saveCommand ?? (this.saveCommand = new RelayCommand(o => SaveChanges((IDialogView) o)));

        public RelayCommand CancelCommand =>
            this.cancelCommand ?? (this.cancelCommand = new RelayCommand(o => CancelChanges((IDialogView) o)));

        public bool IsSaveEnabled
        {
            get => this.isSaveEnabled;
            set => SetProperty(ref this.isSaveEnabled, value);
        }


        private void InitializeViewModel()
        {
            InitilizeFeatureParameters();
        }

        protected virtual void SaveChanges(IDialogView dialogView)
        {
            SaveFeatureParameters();
            dialogView.Close();
        }

        protected virtual void CancelChanges(IDialogView dialogView) => dialogView.Close();

        protected abstract void SaveFeatureParameters();

        protected abstract void InitilizeFeatureParameters();
    }
}