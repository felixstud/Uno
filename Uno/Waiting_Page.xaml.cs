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
using SuperSimpleTcp;

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

        public SimpleTcpServer server;
        public SimpleTcpClient client;
        public bool host;

        private string IP_Port = "127.0.0.1:8000";
        private uint playercount = 1;

        private bool find_server()          //ToDo
        {
            client = new SimpleTcpClient(IP_Port);
            try
            { client.Connect(); }
            catch
            { return false; }

            if (!client.IsConnected)
                return false;
            client.Events.DataReceived += Events_DataReceived_Client;
            client.Events.Disconnected += Events_Disconnected;
            return true;
        }

        private void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            update_status("Disconnected from Server.");
            this.new_Connection();
        }

        private void Events_DataReceived_Client(object? sender, DataReceivedEventArgs e)
        {
            update_playercount(Encoding.UTF8.GetString(e.Data));
        }

        private bool create_server()
        {
            server = new SimpleTcpServer(IP_Port);

            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived_Server;

            try
            {
                server.Start();
            }
            catch
            {
                update_status("Server could not be started. Please try again.");
                return false;
            }
            update_status("Server started");
            update_playercount(playercount.ToString());
            return true;
        }

        private void Events_DataReceived_Server(object? sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            playercount = (uint)server.GetClients().ToList().Count() + 1;
            update_playercount(playercount.ToString());
            update_status("Player disconnected! Total Players: ");
        }

        private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
            playercount = (uint)server.GetClients().ToList().Count() + 1;
            update_playercount(playercount.ToString());
            update_status("New Player connected! Total Players: ");
            server.Send(e.IpPort, playercount.ToString());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            new_Connection();
        }

        private void new_Connection()
        {
            if (this.find_server())
            {
                update_status("Connected to Server!");
                host = false;
            }
            else
            {
                update_status("No server available. Starting Server...");
                host = this.create_server();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if(server != null)
                if (server.IsListening)
                    server.Stop();
            if(client != null)
                if (client.IsConnected)
                    client.Disconnect();
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
