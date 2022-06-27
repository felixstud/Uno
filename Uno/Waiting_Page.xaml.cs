using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaktionslogik für Waiting_Page.xaml
    /// </summary>
    /// 
    /*ToDo:
    - Playercounter works instable
    - Transferring Names to some kind of database
     */
    public partial class Waiting_Page : Page
    {
        public Waiting_Page()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Globals.network.ConnectionCounterChanged += Network_ConnectionCounterChanged;
            Globals.network.StatusChanged += Network_StatusChanged;
            Globals.network.new_Connection();
        }

        private void Network_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            update_status(e.Status);
        }

        private void Network_ConnectionCounterChanged(object sender, ConnectionCounterChangedEventArgs e)
        {
            this.update_playercount(e.counter.ToString());
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {   
            Globals.network.Stop();
            this.NavigationService.Navigate(new Registration_Page());
        }

        private void update_playercount(string number)
        {
            labNumPlayers.Dispatcher.BeginInvoke(new Action(() => { labNumPlayers.Content = number; }));
        }

        private void update_status(string text)
        {
            txtStatus.Dispatcher.BeginInvoke(new Action(() => {txtStatus.Text = text;}));
        }
    }
}
