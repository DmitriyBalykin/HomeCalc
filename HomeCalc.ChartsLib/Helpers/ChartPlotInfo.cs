using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.ChartsLib.Helpers
{
    public class ChartPlotInfo
    {
        public DateTime MinX { get; set; }
        public DateTime MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public int DaysWidth
        {
            get
            {
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

        public double Height
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
                return MaxWidth/DaysWidth;
            }
        }
        public double HeightScale
        {
            get
            {
                return MaxHeight/Height;
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
