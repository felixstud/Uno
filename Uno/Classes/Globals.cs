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
        public static int MaxPlayers = 4;
        public static Random randomNumber = new Random();

        static Globals()
        {
            Players = new List<Player>();
        }

        public static string addPlayer(string name, string IpPort)
        {
            name = CheckDuplicateNames(name);
            Players.Add(new Player(name, IpPort));
            return name;
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
        private static string CheckDuplicateNames(string name)
        {
            foreach(Player iter in Players)
            {
                foreach(Player player in Players)
                {
                    if (string.Equals(player.Name, name))
                        name += "!";
                }
            }
            return name;
        }
    }
}
