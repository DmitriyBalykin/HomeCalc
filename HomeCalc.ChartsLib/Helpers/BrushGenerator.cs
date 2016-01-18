using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HomeCalc.ChartsLib.Helpers
{
    public static class BrushGenerator
    {
        static List<Brush> seriaBrushes = new List<Brush> { Brushes.Green, Brushes.HotPink, Brushes.Magenta, Brushes.RoyalBlue, Brushes.Salmon };
        private static int current = 0;
        public static Brush GetBrush()
        {
            current = (current == seriaBrushes.Count - 1) ? 0 : current + 1;
            return seriaBrushes[current];
        }
    }
}
