using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
