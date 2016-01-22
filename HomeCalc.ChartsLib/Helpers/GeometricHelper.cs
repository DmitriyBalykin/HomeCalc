using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomeCalc.ChartsLib.Helpers
{
    public static class GeometricHelper
    {
        public static Point PointToChart(DateTime x, double y, ChartPlotInfo info)
        {
            var point = new Point();
            point.X = DateToChart(x, info);
            point.Y = YCoordToChart(y, info);
            return point;
        }
        public static double DateToChart(DateTime x, ChartPlotInfo info)
        {
            return (x - info.MinX).Days * info.WidthScale + info.LeftMarginWidth;
        }
        public static double XCoordToChart(double x, ChartPlotInfo info)
        {
            return x + info.LeftMarginWidth;
        }
        public static double YCoordToChart(double y, ChartPlotInfo info)
        {
            return (info.PhysHeight - y) * info.HeightScale + info.HeaderHeight;
        }
    }
}
