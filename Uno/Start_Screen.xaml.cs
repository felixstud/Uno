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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Uno
{
    /// <summary>
    /// Interaktionslogik für Start_Screen.xaml
    /// </summary>
    public partial class Start_Screen : Page
    {
        public Start_Screen()
        {
            InitializeComponent();
        }

        private void showRegPage(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Registration_Page());
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
