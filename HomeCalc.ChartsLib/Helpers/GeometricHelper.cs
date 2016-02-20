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
            if (info.DaysWidth == 1)
            {
                return info.MaxWidth / 2;
            }
            return (x - info.MinX).Days * info.WidthScale + info.LeftMarginWidth + ChartPlotInfo.CHART_START_END_INDENTATION;
        }
        public static double XCoordToChart(double x, ChartPlotInfo info)
        {
            return x + info.LeftMarginWidth + ChartPlotInfo.CHART_START_END_INDENTATION;
        }
        public static double YCoordToChart(double y, ChartPlotInfo info)
        {
            return (info.PhysHeight - y) * info.HeightScale + info.HeaderHeight;
        }
    }
}
