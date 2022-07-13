using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSimpleTcp;
using System.Text.Json;

namespace Uno.Classes
{
    static class GameClient
    {
        public static string myName;
        public static CardStack myCards = new CardStack();
        public static SimpleTcpClient client = new SimpleTcpClient(Globals.getIPPort());

        /// <summary>
        /// Try to find a Server with globally defined Ip & Port
        /// </summary>
        /// <returns>True if a Server was found and is connection was successfull</returns>
        public static bool find_server(string ipport)
        {
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
        /// <summary>
        /// Fired when connection to server got lost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            Events.StatusChangedEvent("Disconnected from Server, please try again!");
        }
        /// <summary>
        /// Fired when Connection to server was successfull
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            Events.StatusChangedEvent("Connected to Server! IP: " + e.IpPort);
        }
        /// <summary>
        /// Fired when new Data was received from server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Events_DataReceived_Client(object? sender, DataReceivedEventArgs e)
        {
            string msg = Encoding.UTF8.GetString(e.Data);
            if(msg.Contains("!counter!"))
            {
                Events.ConnectionCounterChangedEvent(Int32.Parse(msg.Remove(0, 9)));
            }
            else if (msg.Contains("?name?"))
            {
                client.Send("!name!" + myName);
            }
            else if (msg.Contains("!name!"))
            {
                myName = msg.Remove(0, 6);
                Events.StatusChangedEvent("Name changed to: " + myName);
            }
            else if (msg.Contains("!card!"))
            {
                msg = msg.Remove(0, 6);
                Card c = new Card(msg[0] - 48, msg[1] - 48);
                //Card c = JsonSerializer.Deserialize<Card>(msg);
                Events.CardReceivedEvent(e.IpPort, c, false, false);
            }
            else if (msg.Contains("!remcard!"))
            {
                msg = msg.Remove(0, 9);
                Card c = new Card(msg[0] - 48, msg[1] - 48);
                Events.CardReceivedEvent(e.IpPort, c, false, true);
            }
            else if (msg.Contains("!numCard!"))
            {
                msg = msg.Remove(0, 9);
                string num = "";
                while(true)
                {
                    if (msg[0] == '.')
                    {
                        msg = msg.Remove(0, 1);
                        break;
                    }
                    else
                    {
                        num += msg[0] - 48;
                        msg = msg.Remove(0, 1);
                    }
                }
                Events.EnemyPlayerNameReceivedEvent(-1, msg, Int32.Parse(num));
            }
            else if(msg.Contains("!Enemyname!"))
            {
                string name = msg.Remove(0, 12);
                name = name.Remove(name.Length - 1);
                int number = msg.Remove(0, 11)[0] - 48;
                int nCards = msg[msg.Length - 1] - 48;
                Events.EnemyPlayerNameReceivedEvent(number, name, nCards);
            }
            else if(msg.Contains("!midcard!"))
            {
                msg = msg.Remove(0, 9);
                Card c = new Card(msg[0] - 48, msg[1] - 48);
                Events.CardReceivedEvent(e.IpPort, c, true, false);
            }
            else if(msg.Contains("!move!"))
            {
                Events.MoveReceivedEvent(msg.Remove(0, 6));
            }
            else if(msg.Contains("!win!"))
            {
                Events.WinnerReceivedEvent(msg.Remove(0, 5));
            }
        }
        /// <summary>
        /// Stops all connections. Server and Client if active
        /// </summary>
        public static void Stop()
        {
            if (client.IsConnected)
                client.Disconnect();
            myName = null;
            myCards.Cards.Clear();
        }
        /// <summary>
        /// Sends a request to server
        /// </summary>
        /// <param name="data">Message string</param>
        public static void RequestServer(string data)
        {
            client.Send(data);
        }

        /// <summary>
        /// Class for firing custom Events with custom EventArgs
        /// </summary>
        public static class Events
        {
            public static event EventHandler<CardReceivedEventArgs> CardReceived;
            public static event EventHandler<PlayerReceivedEventArgs> PlayerReceived;
            public static event EventHandler<StatusChangedEventArgs> StatusChanged;
            public static event EventHandler<ConnectionCounterChangedEventArgs> ConnectionCounterChanged;
            public static event EventHandler<EnemyNameReceivedEventArgs> EnemyPlayerNameReceived;
            public static event EventHandler<MoveEventArgs> MoveReceived;
            public static event EventHandler<WinEventArgs> WinnerReceived;

            public static void StatusChangedEvent(string state)
            {
                if (StatusChanged != null)
                    StatusChanged(null, new StatusChangedEventArgs(state));
            }
            public static void CardReceivedEvent(string ipport, Card c, bool mid, bool remove)
            {
                if (CardReceived != null)
                    CardReceived(null, new CardReceivedEventArgs(ipport, c, mid, remove));
            }
            public static void PlayerReceivedEvent(string ipport, Player P, int nCards)
            {
                if(PlayerReceived != null)
                    PlayerReceived(null, new PlayerReceivedEventArgs(ipport, P));
            }
            public static void ConnectionCounterChangedEvent(int counter)
            {
                if (ConnectionCounterChanged != null)
                    ConnectionCounterChanged(null, new ConnectionCounterChangedEventArgs(counter));
            }
            public static void EnemyPlayerNameReceivedEvent(int Pnumber, string Pname, int nCards)
            {
                if(EnemyPlayerNameReceived != null)
                    EnemyPlayerNameReceived(null, new EnemyNameReceivedEventArgs(Pnumber, Pname, nCards));
            }
            public static void MoveReceivedEvent(string Name)
            {
                if (MoveReceived != null)
                    MoveReceived(null, new MoveEventArgs(Name));
            }

            public static void WinnerReceivedEvent(string name)
            {
                if(WinnerReceived != null)
                    WinnerReceived(null, new WinEventArgs(name));
            }
            public class CardReceivedEventArgs : EventArgs
            {
                public string IpPort;
                public Card Card;
                public bool midcard = false;
                public bool remove;

                public CardReceivedEventArgs(string ipPort, Card card, bool midcard, bool remove)
                {
                    IpPort = ipPort;
                    Card = card;
                    this.midcard = midcard;
                    this.remove = remove;
                }


            }

            public class PlayerReceivedEventArgs : EventArgs
            {
                public PlayerReceivedEventArgs(string ipport, Player P)
                {
                    Player = P;
                    this.ipport = ipport;
                }
                public Player Player;
                public string ipport;

            }

            public class StatusChangedEventArgs : EventArgs
        {
            public StatusChangedEventArgs(string txt)
            {
                this.Status = txt;
            }

            public string Status { get; }
        }

            public class ConnectionCounterChangedEventArgs : EventArgs
            {
                public int counter;

                public ConnectionCounterChangedEventArgs(int counter)
                {
                    this.counter = counter;
                }
            }

            public class EnemyNameReceivedEventArgs : EventArgs
            {
                public int Playernumber;
                public string PlayerName;
                public int nCards;

                public EnemyNameReceivedEventArgs(int playernumber, string playerName, int nCards)
                {
                    Playernumber = playernumber;
                    PlayerName = playerName;
                    this.nCards = nCards;
                }
            }

            public class MoveEventArgs : EventArgs
            {
                public string Playername;

                public MoveEventArgs(string playername)
                {
                    Playername = playername;
                }
            }

            public class WinEventArgs : EventArgs
            {
                public string name;
                public int points;
                public WinEventArgs(string name, int points)
                {
                    this.name = name;
                    this.points = points;
                }
                public WinEventArgs(string name) : this(name, 0) { }
            }
        }

    }
}
