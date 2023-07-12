using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Abalon
{
    static class ViewSetting
    {
        public static class Borders
        {
            public static Brush MouseHover { get; } = Brushes.Turquoise;
            public static Brush Regular { get; } = Brushes.Gray;
            public static Brush MoveTarget { get; } = Brushes.Green;
            public static Brush KillTarget { get; } = Brushes.Red;
            public static Brush Selected { get; } = Brushes.Gold;
        }
        public static class Backgrounds
        {
            public static Brush Regular { get; } = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
    }
}