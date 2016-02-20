using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.ChartsLib.Helpers
{
    public class ChartPlotInfo
    {
        public const double CHART_START_END_INDENTATION = 20;

        public DateTime MinX { get; set; }
        public DateTime MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
        public TimeSpan XStep { get; set; }
        public double YStep { get; set; }

        public int DaysWidth
        {
            get
            {
                if (MaxX != null && MaxX != DateTime.MinValue && MaxX == MinX)
                {
                    return 1;
                }
                return (MaxX - MinX).Days;
            }
        }
        public double Width
        {
            get
            {
                return DaysWidth * WidthScale;
            }
        }

        public double PhysHeight
        {
            get
            {
                return MaxY - MinY;
            }
        }
        public double WidthScale
        {
            get
            {
                return (MaxWidth - 2*CHART_START_END_INDENTATION) / DaysWidth;
            }
        }
        public double HeightScale
        {
            get
            {
                return MaxHeight/PhysHeight;
            }
        }

        public double HeaderHeight { get; set; }
        public double FooterHeight { get; set; }
        public double LeftMarginWidth { get; set; }
        public double RightMarginWidth { get; set; }

        public double MaxHeight = 600;
        public double MaxWidth = 1000;
    }
}
