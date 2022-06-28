using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;

namespace Uno.Classes
{
    public class Networking
    {
        private SimpleTcpServer server;
        private SimpleTcpClient client;

        private string IP_Port;

        private int connectionCounter = 0;

        public Networking(string ip_port)
        {
            this.IP_Port = ip_port;
        }

        public Networking() : this("127.0.0.1:8000") { }

        public void new_Connection()
        {
            if (!this.find_server())
                this.create_server();
        }

        private bool find_server()
        {
            client = new SimpleTcpClient(this.IP_Port);
            try
            { client.Connect(); }
            catch
            { return false; }

            if (!client.IsConnected)
                return false;
            client.Events.DataReceived += Events_DataReceived_Client;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.Connected += Events_Connected;
            return true;
        }

        private void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            StatusChanged(this, new StatusChangedEventArgs("Disconnected from Server. Please try again."));
        }
        private void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            StatusChanged(this, new StatusChangedEventArgs("Connected to Server: " + e.IpPort));
        }

        private void Events_DataReceived_Client(object? sender, DataReceivedEventArgs e)
        {
            string msg = Encoding.UTF8.GetString(e.Data);
            if (msg.Contains("!counter!"))
            {
                ConnectionCounterChanged(this, new ConnectionCounterChangedEventArgs(Int32.Parse(msg.Remove(0,9))));
            }
            else if (msg.Contains("?name?"))
            {
                client.Send("!name!" + Globals.myName);
                StatusChanged(this, new StatusChangedEventArgs("Connected to Server: " + e.IpPort));
            }
        }

        private bool create_server()
        {
            server = new SimpleTcpServer(this.IP_Port);

            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived_Server;

            try
            {
                server.Start();
            }
            catch
            {
                StatusChanged(this, new StatusChangedEventArgs("Error starting Server. Please try again."));
                return false;
            }
            connectionCounter++;
            ConnectionCounterChanged(this, new ConnectionCounterChangedEventArgs(connectionCounter));
            StatusChanged(this, new StatusChangedEventArgs("Server started successfully"));
            return true;
        }

        private void Events_DataReceived_Server(object? sender, DataReceivedEventArgs e)            //ToDo
        {
            string msg = Encoding.UTF8.GetString(e.Data);
            if (msg.Contains("!name!"))
            {
                Globals.addPlayer(msg.Remove(0, 6), e.IpPort);
                //Globals.Players.Add(new Player(e.IpPort.ToString(), msg.Remove(0,6)));
                StatusChanged(this, new StatusChangedEventArgs("New Players name: " + msg.Remove(0, 6))); 
                serverBroadcast("!counter!" + connectionCounter.ToString());
            }
        }

        private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            connectionCounter--;
            ConnectionCounterChanged(this, new ConnectionCounterChangedEventArgs(connectionCounter));
            StatusChanged(this, new StatusChangedEventArgs("Client Disconnected"));
            serverBroadcast("!counter!" + connectionCounter.ToString());
        }

        private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
            server.Send(e.IpPort, "?name?");
            connectionCounter++;
            ConnectionCounterChanged(this, new ConnectionCounterChangedEventArgs(connectionCounter));
            StatusChanged(this, new StatusChangedEventArgs("New Client Connected"));
        }

        public void Stop()
        {
            if (server != null)
                if (server.IsListening)
                    server.Stop();
            if (client != null)
                if (client.IsConnected)
                    client.Disconnect();
            return;
        }

        public bool getRole()
        {
            if (server == null)
                return true;
            return false;
        }

        public bool getConnectionStatus()
        {
            if (server != null)
                return this.server.IsListening ? true : false;
            return this.client.IsConnected ? true : false;
        }

        //Custom Events
        public delegate void ConnectionCounterChangedEventHandler(object sender, ConnectionCounterChangedEventArgs e);
        public event ConnectionCounterChangedEventHandler ConnectionCounterChanged;
        protected virtual void OnConnectionCounterChanged(ConnectionCounterChangedEventArgs e)
        {
            ConnectionCounterChangedEventHandler handler = ConnectionCounterChanged;
            handler?.Invoke(this, e);
        }

        public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
        public event StatusChangedEventHandler StatusChanged;
        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler handler = StatusChanged;
            handler?.Invoke(this, e);
        }

        private void serverBroadcast(string msg)
        {
            IEnumerable<string> clients = server.GetClients();
            foreach (string client in clients) server.Send(client, msg);
        }

    }

    public class ConnectionCounterChangedEventArgs : EventArgs
    {
        public ConnectionCounterChangedEventArgs(int counter)
        {
            this.counter = counter;
        }
        public int counter { get; }

    }

    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(string txt)
        {
            this.Status = txt;
        }

        public string Status { get; }
    }
}
