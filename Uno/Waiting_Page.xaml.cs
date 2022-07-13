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
using System.Threading;

namespace Uno
{
    /// <summary>
    /// Interaktionslogik für Waiting_Page.xaml
    /// </summary>
    /// 

    public partial class Waiting_Page : Page
    {
        public Waiting_Page()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GameClient.Events.StatusChanged += Events_StatusChanged;
            GameClient.Events.ConnectionCounterChanged += Events_ConnectionCounterChanged;
            GameClient.client.Events.Connected += Events_Connected;
            if (!GameClient.find_server())
            {
                if (GameServer.StartServer())
                    update_status("New Server created");
                GameClient.find_server();
            }
        }

        /// <summary>
        /// Fired when Connection to a server was succesfull
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_Connected(object? sender, SuperSimpleTcp.ConnectionEventArgs e)
        {
            update_status("Connected to server: " + e.IpPort);
        }

        /// <summary>
        /// Fired when the total number of connections to server changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Events_ConnectionCounterChanged(object? sender, GameClient.Events.ConnectionCounterChangedEventArgs e)
        {
            this.update_playercount(e.counter.ToString());
            if (e.counter == Globals.MaxPlayers)
            {
                this.update_status(Globals.MaxPlayers.ToString() + " Players connected. Ready to Start the game.");
                //if(!GameServer.isActive())
                //{
                //}
                await Task.Delay(3000);
                this.Dispatcher.BeginInvoke(new Action(() => { this.NavigationService.Navigate(new Game_Page()); }));
            }
        }

        /// <summary>
        /// Fired when Client status changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Events_StatusChanged(object? sender, GameClient.Events.StatusChangedEventArgs e)
        {
            update_status(e.Status);
        }

        /// <summary>
        /// Fired when Cancel Button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            GameClient.Stop();
            this.NavigationService.Navigate(new Registration_Page());
        }

        /// <summary>
        /// Sets the Connectioncounter-Label to a specific value
        /// </summary>
        /// <param name="number">Number of Clients connected</param>
        private void update_playercount(string number)
        {
            labNumPlayers.Dispatcher.BeginInvoke(new Action(() => { labNumPlayers.Content = number; }));
        }

        /// <summary>
        /// Writes the input text to the status Bar
        /// </summary>
        /// <param name="text">Text to display</param>
        private void update_status(string text)
        {
            txtStatus.Dispatcher.BeginInvoke(new Action(() => {txtStatus.Text = text;}));
        }

    }
}
