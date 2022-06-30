using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Classes
{
    /*
     Only standard Cards, no specials or exceptions
     */
    internal class Card
    {
        public uint number;
        public string color;

        public Card(uint number, string color)
        {
            this.number = number;
            this.color = color;
        }
    }

}
