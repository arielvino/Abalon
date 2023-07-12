using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Abalon
{
    public class Player
    {
        public List<Ball> Pieces { get; set; }
        public Brush Color { get; set; }
        public int FalledBalls { get; set; }
        public Player()
        {
            Pieces = new List<Ball>();
            for (int i = 0; i < 14; i++)
            {
                Pieces.Add(new Ball(this) { });
            }
        }
    }
}
