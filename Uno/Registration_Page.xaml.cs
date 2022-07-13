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
using Uno.Classes;

namespace Uno
{
    /// <summary>
    /// Interaktionslogik für Registration_Page.xaml
    /// </summary>
    public partial class Registration_Page : Page
    {
        public Registration_Page()
        {
            InitializeComponent();
        }

        private void btnToStartScreen_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Start_Screen());
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            GameClient.myName = inputName.Text;
            bool serve = false;
            if (checkbx_Server.IsChecked == true)
                serve = true;

            this.NavigationService.Navigate(new Waiting_Page(serve));
        }
    }
}
