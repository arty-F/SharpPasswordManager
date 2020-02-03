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
    /// Interaction logic for FirstLoadView.xaml
    /// </summary>
    public partial class FirstLoadView : Window
    {
        public FirstLoadView()
        {
            InitializeComponent();
            DataContext = new ViewModels.FirstLoadViewModel();
        }
    }
}
