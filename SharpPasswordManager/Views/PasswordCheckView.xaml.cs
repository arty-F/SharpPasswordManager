using SharpPasswordManager.BL;
using SharpPasswordManager.Handlers;
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
    /// Interaction logic for PasswordCheckView.xaml
    /// </summary>
    public partial class PasswordCheckView : Window
    {
        public PasswordCheckView()
        {
            InitializeComponent();
            DataContext = new ViewModels.PasswordCheckViewModel(new AppSettingsHandler(), new Autenticator(new Cryptographer()));
        }
    }
}
