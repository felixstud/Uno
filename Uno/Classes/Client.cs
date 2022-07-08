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
        public static SimpleTcpClient client = new SimpleTcpClient(Globals.ipport);

        public static bool find_server()
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
        private static void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            Events.StatusChangedEvent("Disconnected from Server, please try again!");
        }
        private static void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            Events.StatusChangedEvent("Connected to Server! IP: " + e.IpPort);
        }
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
                Events.StatusChangedEvent("Name changed to: " + msg);
            }
            else if (msg.Contains("!card!"))
            {
                msg = msg.Remove(0, 6);
                Card c = new Card(msg[0] - 48, msg[1] - 48);
                //Card c = JsonSerializer.Deserialize<Card>(msg);
                Events.CardReceivedEvent(e.IpPort, c, false);
            }
            else if(msg.Contains("!Enemyname!"))
            {
                string name = msg.Remove(0, 12);
                int number = msg.Remove(0, 11)[0] - 48;
                Events.EnemyPlayerNameReceivedEvent(number, name);
            }
            else if(msg.Contains("!midcard!"))
            {
                msg = msg.Remove(0, 9);
                Card c = new Card(msg[0] - 48, msg[1] - 48);
                Events.CardReceivedEvent(e.IpPort, c, true);
            }
        }
        public static void Stop()
        {
            if (client.IsConnected)
                client.Disconnect();
            myName = null;
            myCards.Cards.Clear();
        }
        public static void RequestServer(string data)
        {
            client.Send(data);
        }

        public static class Events
        {
            public static event EventHandler<CardReceivedEventArgs> CardReceived;
            public static event EventHandler<PlayerReceivedEventArgs> PlayerReceived;
            public static event EventHandler<StatusChangedEventArgs> StatusChanged;
            public static event EventHandler<ConnectionCounterChangedEventArgs> ConnectionCounterChanged;
            public static event EventHandler<EnemyNameReceivedEventArgs> EnemyPlayerNameReceived;

            public static void StatusChangedEvent(string state)
            {
                if (StatusChanged != null)
                    StatusChanged(null, new StatusChangedEventArgs(state));
            }
            public static void CardReceivedEvent(string ipport, Card c, bool mid)
            {
                if (CardReceived != null)
                    CardReceived(null, new CardReceivedEventArgs(ipport, c, mid));
            }
            public static void PlayerReceivedEvent(string ipport, Player P)
            {
                if(PlayerReceived != null)
                    PlayerReceived(null, new PlayerReceivedEventArgs(ipport, P));
            }
            public static void ConnectionCounterChangedEvent(int counter)
            {
                if (ConnectionCounterChanged != null)
                    ConnectionCounterChanged(null, new ConnectionCounterChangedEventArgs(counter));
            }
            public static void EnemyPlayerNameReceivedEvent(int Pnumber, string Pname)
            {
                if(EnemyPlayerNameReceived != null)
                    EnemyPlayerNameReceived(null, new EnemyNameReceivedEventArgs(Pnumber, Pname));
            }
        }

        public class CardReceivedEventArgs : EventArgs
        {
            public string IpPort;
            public Card Card;
            public bool midcard = false;

            public CardReceivedEventArgs(string ipPort, Card card, bool midcard)
            {
                IpPort = ipPort;
                Card = card;
                this.midcard = midcard;
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

            public EnemyNameReceivedEventArgs(int playernumber, string playerName)
            {
                Playernumber = playernumber;
                PlayerName = playerName;
            }
        }
    }
}
