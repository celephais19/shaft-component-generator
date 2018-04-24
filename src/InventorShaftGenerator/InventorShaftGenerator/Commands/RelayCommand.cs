using System;
using System.Diagnostics;
using System.Windows.Input;

namespace InventorShaftGenerator.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> methodToExecute;
        private readonly Predicate<object> canExecuteEvaluator;

        public RelayCommand(Action<object> methodToExecute) : this(methodToExecute, null)
        {
        }

        public RelayCommand(Action<object> methodToExecute, Predicate<object> canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute ?? throw new ArgumentNullException(nameof(methodToExecute));
            this.canExecuteEvaluator = canExecuteEvaluator;
        }

        public bool CanExecute(object parameter) => this.canExecuteEvaluator?.Invoke(parameter) ?? true;

        [DebuggerStepThrough]
        public void Execute(object parameter) => this.methodToExecute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}