using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace coal_raider
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
                game.Run();
        }
    }
#endif
}
