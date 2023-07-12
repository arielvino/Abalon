using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Abalon
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class BoardView : UserControl
    {
        public CellView this[int x, int y]
        {
            get
            {
                return Cells[x, y];
            }
        }

        //Properties
        public BoardModel Model { get; set; } = new BoardModel();
        public CellView[,] Cells { get; set; } = new CellView[18, 9];

        //Constructors
        public BoardView()
        {
            InitializeComponent();
            for (int x = 0; x < 18; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if ((x + y) % 2 == 0)
                        if (x + y >= 4 && x + y <= 20 && x - y <= 12 && x - y >= -4)
                        {
                            CellView cellView = new CellView(this, x, y);
                            Cells[x, y] = cellView;
                            cellView.DataContext = Model[x,y];
                        }
                }
            }
        }
    }
}