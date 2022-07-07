using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Classes
{
    public static class Globals
    {
        public static int MaxPlayers = 2;
        public static Random randomNumber = new Random();
        static public string ipport = "127.0.0.1:8000";

        static Globals()
        {
        }

    }
}
