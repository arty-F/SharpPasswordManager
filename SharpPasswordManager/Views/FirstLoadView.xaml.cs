using SharpPasswordManager.BL;
using SharpPasswordManager.Handlers;
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
            DataContext = new ViewModels.FirstLoadViewModel(new AppSettingsHandler(), new Cryptographer());
        }
    }
}
