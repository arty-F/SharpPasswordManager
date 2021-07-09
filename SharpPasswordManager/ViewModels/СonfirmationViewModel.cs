using SharpPasswordManager.Helpers;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.ViewModels
{
    public class СonfirmationViewModel : INotifyPropertyChanged
    {
        public bool Result { get; set; } = false;

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public СonfirmationViewModel(string message)
        {
            Message = message;
        }

        private ICommand okCmd;
        public ICommand OkCmd
        {
            get
            {
                return okCmd ?? (okCmd = new CommandHelper(SetResultTrue, () => true));
            }
        }
        private void SetResultTrue()
        {
            Result = true;
            CloseWindow();
        }

        private ICommand cancelCmd;
        public ICommand CancelCmd
        {
            get
            {
                return cancelCmd ?? (cancelCmd = new CommandHelper(CloseWindow, () => true));
            }
        }
        private void CloseWindow()
        {
            foreach (Window item in Application.Current.Windows)
                if (item.DataContext == this)
                    item.Close();
        }

        #region Property changing
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }
        #endregion
    }
}
