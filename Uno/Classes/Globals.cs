using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Classes
{
    public static class Globals
    {
        public static int players { get; }
        public static Networking network = new Networking();
        public static List<Player> Players;
    }
}
