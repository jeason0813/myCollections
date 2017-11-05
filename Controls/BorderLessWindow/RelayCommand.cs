using System;
using System.Windows.Input;

namespace myCollections.Controls
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;
        public RelayCommand(Action<object> action)
        {
            _action = action;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
