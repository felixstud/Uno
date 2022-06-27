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
            client.Events.Disconnected += Events_ClientDisconnected;
            client.Events.Connected += Events_ClientConnected;
            return true;
        }

        private void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            StatusChanged(this, new StatusChangedEventArgs("Disconnected from Server. Please try again."));
        }

        private void Events_DataReceived_Client(object? sender, DataReceivedEventArgs e)            //ToDo
        {
            //Get Connected Players
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
            //Get Names of Connected Player
            //Save Names in Array
            //Change Name if it's a duplicate of already connected Players
            throw new NotImplementedException();
        }

        private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            connectionCounter--;
            ConnectionCounterChanged(this, new ConnectionCounterChangedEventArgs(connectionCounter));
            StatusChanged(this, new StatusChangedEventArgs("Client Disconnected"));
        }

        private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
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
