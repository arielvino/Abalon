using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abalon
{
    public class Ball
    {
        public Player Player { get; private set; }
        public int X { get; }
        public int Y { get; }
        public bool Active { get; set; } = true;
        public Ball(Player player)
        {
            Player = player;
            X = X;
            Y = Y;
        }
    }
}
