using System.Windows;
using System.Windows.Input;

namespace SharpPasswordManager.Views
{
    /// <summary>
    /// Interaction logic for DataValidateView.xaml
    /// </summary>
    public partial class DataValidateView : Window
    {
        public DataValidateView()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (passwordBox.Password != null)
                passwordBlock.Text = passwordBox.Password;

            passwordBox.Visibility = Visibility.Hidden;
            passwordBlock.Visibility = Visibility.Visible;
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            passwordBlock.Visibility = Visibility.Hidden;
            passwordBox.Visibility = Visibility.Visible;
        }
    }
}
