using SharpPasswordManager.BL.Security;
using SharpPasswordManager.Helpers;
using System.Windows;

namespace SharpPasswordManager.Views
{
    /// <summary>
    /// Interaction logic for FirstLoadView.xaml
    /// </summary>
    public partial class FirstLoadView : Window
    {
        public FirstLoadView()
        {
            InitializeComponent();
            DataContext = new ViewModels.FirstLoadViewModel(new AppSettingsHelper(), new Cryptographer());
        }
    }
}
