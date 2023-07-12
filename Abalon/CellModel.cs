using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Abalon
{
    public class CellModel
    {
        //Fields
        Ball ball;
        Target asTargetMode = Target.None;

        //Properties
        public BoardModel Board { get; }
        public int X { get; }
        public int Y { get; }
        public Ball Ball
        {
            get { return ball; }
            set
            {
                ball = value;
                BallChanged();
            }
        }
        public Target Target
        {
            get { return asTargetMode; }
            set
            {
                asTargetMode = value;
                TargetChanged();
            }
        }
        public bool IsSelected { get; private set; }

        //Constructors
        public CellModel(BoardModel board, int x, int y)
        {
            Board = board;
            X = x;
            Y = y;
            BallChanged += () => { };
        }


        //Functions
        CellModel GetCell(Directions direction, int step) => GetCell((int)direction, step);
        CellModel GetCell(int direction, int step)
        {
            switch (direction)
            {
                case 0:
                    try
                    {
                        return Board[X + step, Y - step];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return null;
                    }
                case 1:
                    try
                    {
                        return Board[X + step * 2, Y];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return null;
                    }
                case 2:
                    try
                    {
                        return Board[X + step, Y + step];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return null;
                    }
                case 3:
                    try
                    {
                        return Board[X - step, Y + step];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return null;
                    }
                case 4:
                    try
                    {
                        return Board[X - step * 2, Y];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return null;
                    }
                case 5:
                    try
                    {
                        return Board[X - step, Y - step];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }
        int GetDistance(CellModel cell)
        {
            if (Y == cell.Y)
                return Math.Abs(X - cell.X) / 2;
            return Math.Abs(X - cell.X);
        }
        Directions GetDirection(CellModel cell)
        {
            if (X > cell.X)
            {
                if (Y == cell.Y) return Directions.Left;
                if (Y > cell.Y) return Directions.UpLeft;
                if (Y < cell.Y) return Directions.DownLeft;
            }
            if (X < cell.X)
            {
                if (Y == cell.Y) return Directions.Right;
                if (Y > cell.Y) return Directions.UpRight;
                if (Y < cell.Y) return Directions.DownRight;
            }
            if (X == cell.X && Y != cell.Y) throw new Exception("The cells arn't in a row.");
            throw new Exception("You tried to equal the same cell.");
        }
        public override string ToString()
        {
            string ball = "Null";
            if (Ball != null)
            {
                if (Ball.Player.Color == Brushes.Black) ball = "Black";
                if (Ball.Player.Color == Brushes.White) ball = "White";
            }
            return $"({X},{Y}) {ball}";
        }

        //Methods
        public void OnClick()
        {
            //Move:
            if (Target == Target.Move)
            {
                if (SelectedCells.Count > 1)
                {
                    try
                    {
                        if ((int)GetDirection(SelectedCells[0]) % 3 == (int)SelectedCells[1].GetDirection(SelectedCells[0]) % 3)
                            forward();

                        else
                            aside();
                    }
                    catch
                    {
                        aside();
                    }

                }
                else forward();
                ///<summary>
                ///תנועה לאורך השורה
                ///</summary>
                void forward()
                {
                    Directions direction = GetDirection(SelectedCells[0]);
                    CellModel cell = this;
                    while (true)//כל כדור
                    {
                        bool end = true;
                        foreach (CellModel item in SelectedCells)
                        {
                            try
                            {
                                if (cell.GetDirection(item) == direction)
                                    end = false;
                            }
                            catch { }
                        }
                        if (end)//אם זה הכדור האחרון שיש להזיז
                        {
                            cell.Ball = null;//רוקן את הגומה
                            break;
                        }
                        else
                        {
                            cell.Ball = cell.GetCell(direction, 1).Ball;//מקם כאן את הכדור של הגומה הבאה
                        }
                        cell = cell.GetCell(direction, 1);
                    }
                }
                void aside()
                {
                    Directions direction = SelectedCells[SelectedCells.Count - 1].GetDirection(this);
                    foreach (CellModel cell in SelectedCells)
                    {
                        cell.GetCell(direction, 1).Ball = cell.Ball;
                        cell.Ball = null;
                    }
                }
                Board.ChangeTurn();
                ClearTargets();
                ClearSelected();
                return;
            }
            if (Target == Target.Kill)
            {
                Directions direction = GetDirection(SelectedCells[0]);
                CellModel cell = this;
                Ball.Active = false;
                while (true)//כל כדור
                {
                    bool end = true;
                    foreach (CellModel item in SelectedCells)
                    {
                        try
                        {
                            if (cell.GetDirection(item) == direction)
                                end = false;
                        }
                        catch { }
                    }
                    if (end)//אם זה הכדור האחרון שיש להזיז
                    {
                        cell.Ball = null;//רוקן את הגומה
                        break;
                    }
                    else
                    {
                        cell.Ball = cell.GetCell(direction, 1).Ball;//מקם כאן את הכדור של הגומה הבאה
                    }
                    cell = cell.GetCell(direction, 1);
                }
                Board.CurrentTurn.FalledBalls++;
                Board.ChangeTurn();
                ClearTargets();
                ClearSelected();
                return;
            }
            ClearTargets();
            //Selection:
            if (SelectedCells.Contains(this))
            {
                IsSelected = false;
                SelectedCells.Remove(this);
                SelectionChanged();
                if (SelectedCells.Count == 2)
                    if (SelectedCells[0].GetDistance(SelectedCells[1]) == 2)
                    {
                        ClearSelected();
                        IsSelected = true;
                        SelectedCells.Add(this);
                        SelectionChanged();
                    }
                goto MarkTargets;
            }
            if (Ball != null)
            {
                if (Ball.Player == Board.CurrentTurn)
                {
                    if (SelectedCells.Count > 2) ClearSelected();
                    if (SelectedCells.Count == 1 && GetDistance(SelectedCells[0]) != 1)
                    {
                        ClearSelected();
                    }
                    if (SelectedCells.Count == 2)
                        if (GetDistance(SelectedCells[0]) != 1 && GetDistance(SelectedCells[1]) != 1
                                || ((int)GetDirection(SelectedCells[0]) % 3 != (int)GetDirection(SelectedCells[1]) % 3 || GetDistance(SelectedCells[0]) + GetDistance(SelectedCells[1]) != 2
                            && GetDirection(SelectedCells[0]) != GetDirection(SelectedCells[1]) || GetDistance(SelectedCells[0]) + GetDistance(SelectedCells[1]) != 3))
                        {
                            ClearSelected();
                        }
                    IsSelected = true;
                    SelectedCells.Add(this);
                    SelectionChanged();
                }
            }
        MarkTargets:
            if (Ball != null)
                if (Ball.Player == Board.CurrentTurn)
                {
                    //כדור אחד סומן
                    if (SelectedCells.Count == 1)
                    {
                        for (int d = 0; d < 6; d++)//לכל כיוון
                        {
                            int me = 0, enemy = 0;
                            for (int m = 1; m <= 2; m++)//כדורים שלי
                            {
                                CellModel cell = GetCell(d, m);
                                if (cell == null) break;
                                if (cell.Ball == null) break;
                                if (cell.Ball.Player == Ball.Player)
                                {
                                    me++;
                                }
                                else break;
                            }
                            for (int en = 1; en <= me; en++)//כדורים של היריב
                            {
                                CellModel cell = GetCell(d, me + en);
                                if (cell == null) break;
                                if (cell.Ball == null) break;
                                if (cell.Ball.Player != Ball.Player)
                                {
                                    enemy++;
                                }
                                else break;
                            }
                            CellModel target = GetCell(d, me + enemy + 1);
                            if (target == null)//אם המטרה מחוץ ללוח
                            {
                                target = GetCell(d, me + enemy);
                                if (target.Ball.Player != Ball.Player)
                                {
                                    TargetCells.Add(target);
                                    target.Target = Target.Kill;
                                }
                            }
                            else//אם המטרה בתוך הלוח
                            if (target.Ball == null)
                            {
                                TargetCells.Add(target);
                                target.Target = Target.Move;
                            }
                        }
                    }
                    //יותר מאחד
                    if (SelectedCells.Count > 1)
                    {
                        //Aside:
                        foreach (Directions direction in Enum.GetValues(typeof(Directions)))
                        {
                            bool empty = true;
                            foreach (CellModel cell in SelectedCells)
                            {
                                if (cell.GetCell(direction, 1) == null)
                                {
                                    empty = false;
                                    break;
                                }
                                if (cell.GetCell(direction, 1).Ball != null)
                                {
                                    empty = false;
                                    break;
                                }
                            }
                            if (empty)
                            {
                                CellModel target = SelectedCells[SelectedCells.Count - 1].GetCell(direction, 1);
                                TargetCells.Add(target);
                                target.Target = Target.Move;
                            }
                        }
                        //Forward:
                        CellModel start = SelectedCells[0];
                        Directions myDirection = start.GetDirection(SelectedCells[1]);
                        int me = 0, enemy = 0;
                        for (int m = 1; m <= 2; m++)//כדורים שלי
                        {
                            CellModel cell = start.GetCell(myDirection, m);
                            if (cell == null) break;
                            if (cell.Ball == null) break;
                            if (cell.Ball.Player == Ball.Player)
                            {
                                me++;
                            }
                            else break;
                        }
                        for (int en = 1; en <= me; en++)//כדורים של היריב
                        {
                            CellModel cell = start.GetCell(myDirection, me + en);
                            if (cell == null) break;
                            if (cell.Ball == null) break;
                            if (cell.Ball.Player != Ball.Player)
                            {
                                enemy++;
                            }
                            else break;
                        }
                        CellModel myTarget = start.GetCell(myDirection, me + enemy + 1);
                        if (myTarget == null)//אם המטרה מחוץ ללוח
                        {
                            myTarget = start.GetCell(myDirection, me + enemy);
                            if (myTarget.Ball.Player != Ball.Player)
                            {
                                TargetCells.Add(myTarget);
                                myTarget.Target = Target.Kill;
                            }
                        }
                        else//אם המטרה בתוך הלוח
                        if (myTarget.Ball == null)
                        {
                            TargetCells.Add(myTarget);
                            myTarget.Target = Target.Move;
                        }
                        //Backward
                        start = SelectedCells[SelectedCells.Count -1];
                        myDirection = start.GetDirection(SelectedCells[0]);
                        me = 0;
                        enemy = 0;
                        for (int m = 1; m <= 2; m++)//כדורים שלי
                        {
                            CellModel cell = start.GetCell(myDirection, m);
                            if (cell == null) break;
                            if (cell.Ball == null) break;
                            if (cell.Ball.Player == Ball.Player)
                            {
                                me++;
                            }
                            else break;
                        }
                        for (int en = 1; en <= me; en++)//כדורים של היריב
                        {
                            CellModel cell = start.GetCell(myDirection, me + en);
                            if (cell == null) break;
                            if (cell.Ball == null) break;
                            if (cell.Ball.Player != Ball.Player)
                            {
                                enemy++;
                            }
                            else break;
                        }
                        myTarget = start.GetCell(myDirection, me + enemy + 1);
                        if (myTarget == null)//אם המטרה מחוץ ללוח
                        {
                            myTarget = start.GetCell(myDirection, me + enemy);
                            if (myTarget.Ball.Player != Ball.Player)
                            {
                                TargetCells.Add(myTarget);
                                myTarget.Target = Target.Kill;
                            }
                        }
                        else//אם המטרה בתוך הלוח
                        if (myTarget.Ball == null)
                        {
                            TargetCells.Add(myTarget);
                            myTarget.Target = Target.Move;
                        }
                    }
                }
        }

        //Statics Things
        static List<CellModel> SelectedCells { get; set; } = new List<CellModel>();
        static List<CellModel> TargetCells { get; set; } = new List<CellModel>();

        static void ClearTargets()
        {
            foreach (CellModel cell in TargetCells)
            {
                cell.Target = Target.None;
            }
            TargetCells.Clear();
        }
        static void ClearSelected()
        {
            foreach (CellModel cell in SelectedCells)
            {
                cell.IsSelected = false;
            }
            SelectedCells.Clear();
            SelectionChanged();
        }

        //Events
        public event Action TargetChanged;
        public event Action BallChanged;
        public static event Action SelectionChanged;
    }
}