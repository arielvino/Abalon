using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Abalon
{
    public class BoardModel
    {
        //Indexers
        public CellModel this[int x, int y]
        {
            get
            {
                return Cells[x, y];
            }
        }

        //Properties
        public CellModel[,] Cells { get; set; } = new CellModel[18, 9];
        public List<Player> Players { get; private set; } = new List<Player>(2);
        public Player CurrentTurn { get; set; }

        //Constructors
        public BoardModel()
        {
            for (int x = 0; x < 18; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if ((x + y) % 2 == 0)
                        if (x + y >= 4 && x + y <= 20 && x - y <= 12 && x - y >= -4)
                        {
                            Cells[x, y] = new CellModel(this, x, y);
                        }
                }
            }
            Players.Add(new Player { Color = Brushes.White });
            Players.Add(new Player { Color = Brushes.Black });
            for (int i = 0; i < 2; i++)
            {
                for (int b = 0; b < 5; b++)
                {
                    Ball ball = new Ball(Players[i]);
                    Players[i].Pieces.Add(ball);
                    Cells[4 + b * 2, getRow(i, 0)].Ball = ball;
                }
                for (int b = 0; b < 6; b++)
                {
                    Ball ball = new Ball(Players[i]);
                    Players[i].Pieces.Add(ball);
                    Cells[3 + b * 2, getRow(i, 1)].Ball = ball;
                }
                for (int b = 0; b < 3; b++)
                {
                    Ball piece = new Ball(Players[i]);
                    Players[i].Pieces.Add(piece);
                    Cells[6 + b * 2, getRow(i, 2)].Ball = piece;
                }
            }
            CurrentTurn = Players[0];

            int getRow(int player, int row)
            {
                if (player == 0) return row;
                else return 8 - row;
            }
        }

        //Methods
        public void ChangeTurn()
        {
            if (CurrentTurn == Players[0])
                CurrentTurn = Players[1];
            else CurrentTurn = Players[0];
        }
    }
}