using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;
using Uno.Classes;

namespace Uno.Classes
{
    static class GameServer
    {
        static private SimpleTcpServer server;
        static private CardStack AllCards;
        static private CardStack MiddleStack;
        static private List<Player> AllPlayers;

        static public bool StartServer()
        {
            server = new SimpleTcpServer(Globals.ipport);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;

            try
            {
                server.Start();
            }
            catch
            {
                return false;
            }
            StartGame();
            return true;
        }

        private static async void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            string msg = Encoding.UTF8.GetString(e.Data);
            if (msg.Contains("!name!"))
            {
                string p_name = addPlayer(msg.Remove(0, 6), e.IpPort);
                if (!p_name.Equals(msg.Remove(0, 6)))
                    server.Send(e.IpPort, "!name!" + p_name);
                serverBroadcast("!counter!" + AllPlayers.Count().ToString());
            }
            else if(msg.Contains("?EnenyNames?"))
            {
                int from = 0;
                for(int i = 0; i < Globals.MaxPlayers; i++)
                {
                    if (e.IpPort.Equals(AllPlayers[i].ip_port))
                        from = i;
                }
                for(int i = 0; i < Globals.MaxPlayers-1; i++)
                {
                    int k = (i + from + 1) % 4;
                    server.Send(e.IpPort, i.ToString() + AllPlayers[k].Name);
                    await Task.Delay(200);
                }
            }
        }
        private static void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            serverBroadcast("!counter!" + AllPlayers.Count().ToString());
            removePlayer(e.IpPort);
        }
        private static void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
            server.Send(e.IpPort, "?name?");
        }

        public static string addPlayer(string name, string IpPort)
        {
            name = CheckDuplicateNames(name);
            AllPlayers.Add(new Player(name, IpPort));
            return name;
        }
        public static void removePlayer(string IpPort)
        {
            foreach (Player P in AllPlayers)
            {
                if (P.ip_port == IpPort)
                {
                    AllPlayers.Remove(P);
                    return;
                }
            }
        }
        private static string CheckDuplicateNames(string name)
        {
            foreach (Player iter in AllPlayers)
            {
                foreach (Player player in AllPlayers)
                {
                    if (string.Equals(player.Name, name))
                        name += "!";
                }
            }
            return name;
        }
        public static void serverBroadcast(string msg)
        {
            if (server == null)
                return;
            IEnumerable<string> clients = server.GetClients();
            foreach (string client in clients) server.Send(client, msg);
        }
        public static void Stop()
        {
            if (server != null)
                if (server.IsListening)
                    server.Stop();
            server = null;
            AllPlayers.Clear();
            AllCards.Cards.Clear();
        }
        public static void StartGame()
        {
            AllCards = new CardStack();
            AllCards.createAllCards();
            MiddleStack = new CardStack();
            MiddleStack.AddCard(AllCards.getRandomCard());
            //send random Card to Clients
        }//ToDo
    }
}
