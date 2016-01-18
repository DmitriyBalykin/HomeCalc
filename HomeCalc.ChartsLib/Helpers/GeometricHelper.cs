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
        public static Point PointToChart(Point point, ChartPlotInfo info)
        {
            return PointToChart(point.X, point.Y, info);
        }
        public static Point PointToChart(double x, double y, ChartPlotInfo info)
        {
            var point = new Point();
            point.X = XCoordToChart(x, info);
            point.Y = YCoordToChart(y, info);
            return point;
        }
        public static double XCoordToChart(double x, ChartPlotInfo info)
        {
            return (x - info.MinX) * info.WidthScale + info.LeftLegendWidth;
        }
        public static double YCoordToChart(double y, ChartPlotInfo info)
        {
            return (info.Height - y) * info.HeightScale + info.HeaderHeight;
        }
    }
}
