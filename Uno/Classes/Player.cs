using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Classes
{
    public class Player
    {

        public Player(string name, string ip_port)
        {
            Name = name;
            this.ip_port = ip_port;
            CardStack = new CardStack();
        }
        public Player() : this("?", "?") { }

        
        public string Name { get; set; }
        public string ip_port { get; set; }
        public CardStack CardStack;
    }
}
