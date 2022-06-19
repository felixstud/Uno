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
using SuperSimpleTcp;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            SimpleTcpServer server = new SimpleTcpServer("127.0.0.1:8000");

            server.Keepalive.EnableTcpKeepAlives = true;
            server.Keepalive.TcpKeepAliveInterval = 5;      // seconds to wait before sending subsequent keepalive
            server.Keepalive.TcpKeepAliveTime = 5;          // seconds to wait before sending a keepalive
            server.Keepalive.TcpKeepAliveRetryCount = 5;    // number of failed keepalive probes before terminating connection


            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;

            server.Start();
            txtStatus.Text += "Server started..."; 
        }

        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            txtStatus.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtStatus.Text += "Data received: ";
                txtStatus.Text += Encoding.UTF8.GetString(e.Data);
            }));
        }

        private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            txtStatus.Dispatcher.BeginInvoke(() =>
            {
                txtStatus.Text += "Client disconnected IP: ";
                txtStatus.Text += e.IpPort;
            });

        }

        private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
            txtStatus.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtStatus.Text += "Client connected IP:";
                txtStatus.Text += e.IpPort;
            }));
        }
    }
}
