using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.ChartsLib.Helpers
{
    public class ChartPlotInfo
    {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public double Width
        {
            get
            {
                return MaxX - MinX;
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
                return MaxWidth/Width;
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
        public double LeftLegendWidth { get; set; }
        public double RightLegendWidth { get; set; }

        public double MaxHeight = 600;
        public double MaxWidth = 800;
    }
}
