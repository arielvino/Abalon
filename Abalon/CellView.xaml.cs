using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Abalon
{
    /// <summary>
    /// Interaction logic for Cell.xaml
    /// </summary>
    public partial class CellView : UserControl
    {
        //Properties
        BoardView Board { get; set; }
        public int X { get; }
        public int Y { get; }
        public CellModel Model { get; set; }
        public Brush Border
        {
            get
            {
                return ellipse.Stroke;
            }
            set
            {
                ellipse.Stroke = value;
            }
        }
        public Brush Fill
        {
            get
            {
                return ellipse.Fill;
            }
            set
            {
                ellipse.Fill = value;
            }
        }
        public Brush CurrentMainBorder { get; protected set; } = ViewSetting.Borders.Regular;

        //Constructors
        public CellView(BoardView board, int x, int y)
        {
            InitializeComponent();
            Board = board;
            X = x;
            Y = y;
            Grid.SetRow(this, Y);
            Grid.SetColumn(this, X);
            Grid.SetColumnSpan(this, 2);
            Board.grid.Children.Add(this);
            Model = Board.Model[x, y];
            Model.TargetChanged += OnTargetChanged;
            Model.BallChanged += OnBallChanged;
            CellModel.SelectionChanged += OnSelectionChanged;
            DataContext = Model;
            OnBallChanged();
        }

        //Methods
        private void OnSelectionChanged()
        {
            if (Model.IsSelected)
                CurrentMainBorder = ViewSetting.Borders.Selected;
            else
                CurrentMainBorder = ViewSetting.Borders.Regular;
            Border = CurrentMainBorder;
        }
        private void OnBallChanged()
        {
            if (Model.Ball != null)
            {
                Fill = Model.Ball.Player.Color;
            }
            else
            {
                Fill = ViewSetting.Backgrounds.Regular;
            }
        }
        void OnTargetChanged()
        {
            switch (Model.Target)
            {
                case Target.None:
                    Border = CurrentMainBorder;
                    break;
                case Target.Move:
                    Border = ViewSetting.Borders.MoveTarget;
                    break;
                case Target.Kill:
                    Border = ViewSetting.Borders.KillTarget;
                    break;
            }
        }

        //InterfaceMethods
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Model.Ball != null || Model.Target != Target.None)
                Border = ViewSetting.Borders.MouseHover;
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            OnTargetChanged();
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Model.OnClick();
        }
    }
}