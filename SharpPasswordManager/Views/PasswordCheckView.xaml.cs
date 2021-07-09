using SharpPasswordManager.BL.Security;
using SharpPasswordManager.Helpers;
using System.Windows;

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
            DataContext = new ViewModels.PasswordCheckViewModel(new AppSettingsHelper(), new Autenticator(new Cryptographer()));
        }
    }
}
