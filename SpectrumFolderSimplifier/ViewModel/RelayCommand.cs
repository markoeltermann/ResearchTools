using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpectrumFolderSimplifier.ViewModel
{
    public class RelayCommand : ICommand
    {

        private Action<object> executeAction;

        public RelayCommand(Action<object> executeAction)
        {
            this.executeAction = executeAction;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
    }
}
