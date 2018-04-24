using System.ComponentModel;
using System.Diagnostics;
using InventorShaftGenerator.Commands;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Views;

namespace InventorShaftGenerator.ViewModels
{
    [DebuggerStepThrough]
    public abstract class SectionDimensionsViewModel<TSection> : ViewModelBase, IViewModelWithSection, IDataErrorInfo
        where TSection : ShaftSection
    {
        private TSection section;
        private RelayCommand saveCommand;
        private RelayCommand cancelCommand;
        private bool isSaveEnalbed;

        protected SectionDimensionsViewModel()
        {
            this.ErrorFields.CollectionChanged += (sender, args) => this.IsSaveEnabled = this.ErrorFields.Count == 0;
        }

        protected ObservableSet<string> ErrorFields { get; } = new ObservableSet<string>();

        public TSection Section
        {
            get => this.section;
            set
            {
                SetProperty(ref this.section, value);
                InitializeViewModel();
            }
        }

        ShaftSection IViewModelWithSection.Section
        {
            get => this.section;
            set => this.Section = (TSection) value;
        }

        public bool IsSaveEnabled
        {
            get => this.isSaveEnalbed;
            private set => SetProperty(ref this.isSaveEnalbed, value);
        }

        public RelayCommand SaveCommand =>
            this.saveCommand ?? (this.saveCommand = new RelayCommand(o => SaveChanges((IDialogView) o)));

        public RelayCommand CancelCommand =>
            this.cancelCommand ?? (this.cancelCommand = new RelayCommand(o => CancelChanges((IDialogView) o)));

        private void InitializeViewModel()
        {
            InitializeDimensions();
        }

        private void SaveChanges(IDialogView dialogView)
        {
            SaveDimensions();
            dialogView.Close();
        }

        private void CancelChanges(IDialogView dialogView) => dialogView.Close();

        protected abstract void InitializeDimensions();

        protected abstract void SaveDimensions();

        public abstract string this[string columnName] { get; }

        string IDataErrorInfo.Error => null;
    }
}