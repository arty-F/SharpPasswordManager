using SharpPasswordManager.Helpers;
using SharpPasswordManager.Infrastructure.Injector;
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
            DataContext = new ViewModels.FirstLoadViewModel(new AppSettingsHelper(), Injector.Instance);
        }
    }
}
