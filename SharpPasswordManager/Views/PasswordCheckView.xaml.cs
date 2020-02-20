using SharpPasswordManager.BL;
using SharpPasswordManager.Handlers;
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
            DataContext = new ViewModels.PasswordCheckViewModel(new AppSettingsHandler(), new Autenticator(new Cryptographer()));
        }
    }
}
