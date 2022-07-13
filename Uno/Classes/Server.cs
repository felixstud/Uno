﻿using System;
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

        private static void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            string msg = Encoding.UTF8.GetString(e.Data);
            RxMsg m = new RxMsg(msg, e.IpPort);
            //m.readMessage();
            Thread ReadData = new Thread(new ThreadStart(m.readMessage));
            ReadData.Start();
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
            //serverBroadcast("!midcard!" + MiddleStack.Cards.Last().number.ToString() + MiddleStack.Cards.Last().color.ToString());
        }//ToDo
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
        public static bool isActive()
        {
            if (server != null)
                if (server.IsListening)
                    return true;
            return false;
        }

        private class RxMsg
        {
            public string msg;
            public string ipport;

            public RxMsg(string message, string ipport)
            {
                this.msg = message;
                this.ipport = ipport;
            }

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
                    if(!checkMovePossibility(c.number, c.color))
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
                    serverBroadcast("!numCard!" + AllPlayers[activePlayer].CardStack.getCounter().ToString() + "." + AllPlayers[activePlayer].Name);
                    await Task.Delay(100);
                    if (AllPlayers[activePlayer].CardStack.getCounter() <= 0)
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
            public string addPlayer(string name, string IpPort)
            {
                name = CheckDuplicateNames(name);
                AllPlayers.Add(new Player(name, IpPort));
                return name;
            }
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
            private bool checkMovePossibility(int number, int color)
            {
                if (MiddleStack.Cards.Last().number == number) 
                    return true;
                else if (MiddleStack.Cards.Last().color == color)
                    return true;
                else
                    return false;
            }
        }
    }
}
