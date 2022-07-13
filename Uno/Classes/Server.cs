using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;
using Uno.Classes;
using System.Threading;
using System.Text.Json;

namespace Uno.Classes
{
    static class GameServer
    {
        static private SimpleTcpServer server;
        static private CardStack AllCards;
        static private CardStack MiddleStack;
        static private List<Player> AllPlayers = new List<Player>();
        static private int activePlayer = 0;
        
        /// <summary>
        /// starts a new server with global definded IP and Port
        /// </summary>
        /// <returns>
        /// true if start was successfull
        /// </returns>
        static public bool StartServer()
        {
            server = new SimpleTcpServer(Globals.ipport);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
            server.Keepalive.EnableTcpKeepAlives = true;
            server.Keepalive.TcpKeepAliveInterval = 5;      // seconds to wait before sending subsequent keepalive
            server.Keepalive.TcpKeepAliveTime = 5;          // seconds to wait before sending a keepalive
            server.Keepalive.TcpKeepAliveRetryCount = 5;    // number of failed keepalive probes before terminating connection

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
        /// <summary>
        /// Event which is trigerred, when the server received new Data from client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            string msg = Encoding.UTF8.GetString(e.Data);
            RxMsg m = new RxMsg(msg, e.IpPort);
            //m.readMessage();
            Thread ReadData = new Thread(new ThreadStart(m.readMessage));
            ReadData.Start();
        }
        /// <summary>
        /// Event which is trigerred, when a new Client disconnected from server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            serverBroadcast("!counter!" + AllPlayers.Count().ToString());
            removePlayer(e.IpPort);
            server.DisconnectClient(e.IpPort);
        }
        /// <summary>
        /// Event which is triggerred, when a new client connected to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
            server.Send(e.IpPort, "?name?");
        }

        /// <summary>
        /// Method to send a message to all connected clients
        /// </summary>
        /// <param name="msg">
        /// message string
        /// </param>
        public static void serverBroadcast(string msg)
        {
            if (server == null)
                return;
            IEnumerable<string> clients = server.GetClients();
            foreach (string client in clients)
            {
                try
                {
                    if (server.IsConnected(client))
                        server.Send(client, msg);
                }
                catch
                {

                }
            }
        }
        /// <summary>
        /// Stops the server and resets all parameters
        /// </summary>
        public static void Stop()
        {
            if (server != null)
                if (server.IsListening)
                    server.Stop();
            server = null;
            AllPlayers.Clear();
            AllCards.Cards.Clear();
        }
        /// <summary>
        /// Generates all necessary components to start a new game
        /// </summary>
        public static void StartGame()
        {
            AllCards = new CardStack();
            AllCards.createAllCards();
            MiddleStack = new CardStack();
            MiddleStack.AddCard(AllCards.getRandomCard());
        }
        /// <summary>
        /// Removes a Player from AllPlayer-List
        /// </summary>
        /// <param name="IpPort">
        /// IP & Port string of the player who wants to be removed
        /// </param>
        private static void removePlayer(string IpPort)
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
        /// <summary>
        /// Check if server is active
        /// </summary>
        /// <returns>
        /// True if server is running and listening
        /// </returns>
        public static bool isActive()
        {
            if (server != null)
                if (server.IsListening)
                    return true;
            return false;
        }

        /// <summary>
        /// Class to read and process incomning Data
        /// </summary>
        private class RxMsg
        {
            public string msg;
            public string ipport;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="message">Received Message</param>
            /// <param name="ipport">Sender IPPort string</param>
            public RxMsg(string message, string ipport)
            {
                this.msg = message;
                this.ipport = ipport;
            }
            /// <summary>
            /// Reads and processes incoming message strings
            /// </summary>
            public async void readMessage()
            {
                if (msg.Contains("!name!"))
                {
                    string p_name = addPlayer(msg.Remove(0, 6), ipport);
                    if (!p_name.Equals(msg.Remove(0, 6)))
                        await server.SendAsync(ipport, "!name!" + p_name);
                    await Task.Delay(500);
                    serverBroadcast("!counter!" + AllPlayers.Count().ToString());
                    await Task.Delay(500);
                }
                else if (msg.Contains("?EnemyNames?"))
                {
                    int from = 0;
                    for (int i = 0; i < Globals.MaxPlayers; i++)
                    {
                        if (ipport.Equals(AllPlayers[i].ip_port))
                            from = i;
                    }
                    for (int i = 0; i < (Globals.MaxPlayers - 1); i++)
                    {
                        int k = (i + from + 1) % Globals.MaxPlayers;
                        await server.SendAsync(ipport, "!Enemyname!" + i.ToString() + AllPlayers[k].Name + AllPlayers[k].CardStack.Cards.Count().ToString());
                        await Task.Delay(500);
                    }
                }
                else if(msg.Contains("?card?"))
                {
                    Player from = new Player();
                    foreach (Player P in AllPlayers)
                    {
                        if (P.ip_port.Equals(ipport))
                            from = P;
                    }
                    int num;
                    if (msg.Equals("?card?"))
                        num = 1;
                    else
                        num = msg.Remove(0, 6)[0] - 48;
                    for(int i = 0; i < num; i++)
                    {
                        if (AllCards.Count() <= 0)
                            ShuffleMidStack2AllCards();
                        Card c = AllCards.getRandomCard();
                        from.CardStack.AddCard(c);
                        string m = "!card!" + c.number.ToString() + c.color.ToString();
                        await server.SendAsync(ipport, m);
                        await Task.Delay(300);
                    }
                    if(num == 1)
                    {
                        activePlayer++;
                        if (activePlayer >= Globals.MaxPlayers)
                            activePlayer = 0;
                        serverBroadcast("!move!" + AllPlayers[activePlayer].Name);
                    }
                }
                else if(msg.Contains("!card!"))
                {
                    Card c = new Card(msg[6] - 48, msg[7] - 48);
                    if(!checkMovePossibility(c))
                    {
                        serverBroadcast("!move!" + AllPlayers[activePlayer].Name);
                        return;
                    }
                    if(AllPlayers[activePlayer].CardStack.RemoveCard(c) == null)
                    {
                        serverBroadcast("!move!" + AllPlayers[activePlayer].Name);
                        return;
                    }
                    MiddleStack.AddCard(c);
                    await Task.Delay(100);
                    serverBroadcast("!midcard!" + msg.Remove(0, 6));
                    await Task.Delay(100);
                    serverBroadcast("!numCard!" + AllPlayers[activePlayer].CardStack.Count().ToString() + "." + AllPlayers[activePlayer].Name);
                    await Task.Delay(100);
                    if (AllPlayers[activePlayer].CardStack.Count() <= 0)
                    {
                        serverBroadcast("!win!" + AllPlayers[activePlayer].Name);
                        return;
                    }
                    activePlayer++;
                    if (activePlayer >= Globals.MaxPlayers)
                        activePlayer = 0;
                    serverBroadcast("!move!" + AllPlayers[activePlayer].Name);
                    await Task.Delay(100);
                    server.Send(ipport, "!remcard!" + msg.Remove(0, 6));
                }
                else if(msg.Contains("?midcard?"))
                {
                    Card c = MiddleStack.Cards.Last();
                    string m = "!midcard!" + c.number.ToString() + c.color.ToString();
                    await server.SendAsync(ipport, m);
                }
                else if (msg.Contains("?move?"))
                {
                    serverBroadcast("!move!" + AllPlayers[activePlayer].Name);
                }
            }
            /// <summary>
            /// Adds a Player to AllPlayer List
            /// </summary>
            /// <param name="name"> Name if the Player</param>
            /// <param name="IpPort">IP and Port string of the Player</param>
            /// <returns>Changed Name (Only changed, if the name already exists)</returns>
            public string addPlayer(string name, string IpPort)
            {
                name = CheckDuplicateNames(name);
                AllPlayers.Add(new Player(name, IpPort));
                return name;
            }
            /// <summary>
            /// Remove Player from AllPlayers-List
            /// </summary>
            /// <param name="IpPort">IPPort string from Player</param>
            public void removePlayer(string IpPort)
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
            /// <summary>
            /// Check if the name already exists in AllPlayers-List
            /// </summary>
            /// <param name="name">Name to be checked</param>
            /// <returns>Name after checking (Changed if duplicate exists)</returns>
            private string CheckDuplicateNames(string name)
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
            /// <summary>
            /// Checks if the Card Combination of parameter and last MidStack-Card is allowed by rules.
            /// Only Color an Number are getting checked
            /// </summary>
            /// <param name="number">Number of the Card</param>
            /// <param name="color">Color of the card</param>
            /// <returns>True if move is allowed</returns>
            private bool checkMovePossibility(Card c)
            {
                if (MiddleStack.Cards.Last().number == c.number || MiddleStack.Cards.Last().color == c.color) 
                    return true;
                return false;
            }
            /// <summary>
            /// Randomly adds all cards of the MiddleStack to AllCards-List, except for the last in MiddleStack
            /// </summary>
            private void ShuffleMidStack2AllCards()
            {
                for(int i = 0; i < MiddleStack.Count() - 2; i++)
                {
                    int rnd = Globals.randomNumber.Next(0, MiddleStack.Count() - 2);
                    AllCards.AddCard(MiddleStack.Cards[rnd]);
                    MiddleStack.Cards.RemoveAt(rnd);
                }
            }
        }
    }
}
