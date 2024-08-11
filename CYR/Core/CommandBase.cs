using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CYR.Core
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        abstract public bool CanExecute(object? parameter);

        abstract public void Execute(object? parameter);
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
