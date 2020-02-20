using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpPasswordManager.Views
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataView : UserControl
    {
        public DataView()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in ((sender as Button).Parent as Grid).Children)
            {
                if ((item as UIElement).Uid == "value")
                    (item as TextBlock).Visibility = Visibility.Visible;
                if ((item as UIElement).Uid == "mask")
                    (item as TextBlock).Visibility = Visibility.Hidden;
            }
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in ((sender as Button).Parent as Grid).Children)
            {
                if ((item as UIElement).Uid == "value")
                    (item as TextBlock).Visibility = Visibility.Hidden;
                if ((item as UIElement).Uid == "mask")
                    (item as TextBlock).Visibility = Visibility.Visible;
            }
        }
    }
}
