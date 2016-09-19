using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TalkAbout.ViewModel
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Predicate<object> _canExecute;
        private Action _execute;

        public Command(Action action, Predicate<object> canExecute = null)
        {
            _canExecute = canExecute;
            _execute = action;
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
                result = _canExecute(parameter);
            }
            return result;
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
