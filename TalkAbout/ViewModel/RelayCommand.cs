using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Input;

namespace TalkAbout.ViewModel
{
    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Predicate<T> _canExecute;
        private Action<T> _executeAction;

        public RelayCommand(Action<T> executeAction, Predicate<T> canExecute = null)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }
        

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            bool result = true;
            if (_canExecute != null)
            {
                result = _canExecute((T)parameter);
            }
            return result;
        }

        public void Execute(object parameter)
        {
            _executeAction((T) parameter);
        }
    }
}
