using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpPasswordManager.Handlers
{
    /// <summary>
    /// Can handle two types of commands, without parameter and with one parameter.
    /// </summary>
    public class CommandHandler : ICommand
    {
        private Action action;
        private Action<object> actionParam;
        private Func<bool> canExecute;
        private Func<object, bool> canExecuteParam;
        private bool withParam;

        public CommandHandler(Action action, Func<bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
            withParam = false;
        }

        public CommandHandler(Action<object> actions, Func<object, bool> canExecute)
        {
            actionParam = actions;
            canExecuteParam = canExecute;
            withParam = true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return withParam ? canExecuteParam.Invoke(parameter) : canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            if (withParam)
                actionParam(parameter);
            else
                action();
        }
    }
}
