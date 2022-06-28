using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Classes
{
    public static class Globals
    {
        public static Networking network = new Networking();
        public static List<Player> Players;
        public static string myName;

        static Globals()
        {
            Players = new List<Player>();
        }

        public static void addPlayer(string name, string IpPort)
        {
            Players.Add(new Player(name, IpPort));
        }

        public static void removePlayer(string IpPort)
        {
            foreach (Player player in Players)
            {
                if(player.ip_port == IpPort)
                {
                    Players.Remove(player);
                    return;
                }
            }
        }
    }
}
