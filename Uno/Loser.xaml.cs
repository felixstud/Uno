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
    /// Interaktionslogik für Loser.xaml
    /// </summary>
    public partial class Loser : Page
    {
        private string name;
        public Loser(string name)
        {
            this.name = name;
            InitializeComponent();
        }
        public Loser() : this("unknown") {}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lbl_WinnerName.Dispatcher.BeginInvoke(new Action(() =>
            {
                lbl_WinnerName.Content = this.name + " won the game!";
            }));
        }
    }
}
