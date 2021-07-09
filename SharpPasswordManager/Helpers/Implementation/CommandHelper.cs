using System;
using System.Windows.Input;

namespace SharpPasswordManager.Helpers
{
    /// <summary>
    /// Can handle two types of commands, without parameter and with one parameter.
    /// </summary>
    public class CommandHelper : ICommand
    {
        private Action action;
        private Action<object> actionParam;
        private Func<bool> canExecute;
        private Func<object, bool> canExecuteParam;
        private bool withParam;

        public CommandHelper(Action action, Func<bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
            withParam = false;
        }

        public CommandHelper(Action<object> action, Func<object, bool> canExecute)
        {
            actionParam = action;
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
