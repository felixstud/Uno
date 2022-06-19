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

namespace Client
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

        SimpleTcpClient client;

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            client = new SimpleTcpClient("127.0.0.1:8000");
            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;

            client.Connect();
        }

        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            txtStatus.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtStatus.Text += "New Data received: ";
                txtStatus.Text += e.Data.ToString();
            }));
        }

        private void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            txtStatus.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtStatus.Text += "Disonnected from Server";
            }
            ));
        }

        private void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            txtStatus.Text += "Connected to Server IP: ";
            txtStatus.Text += e.IpPort;
            client.Send(txtName.Text);
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            client.Send(txtData.Text.ToString());
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            client.Disconnect();
        }
    }
}
