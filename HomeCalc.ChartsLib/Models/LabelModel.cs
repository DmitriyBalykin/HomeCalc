using HomeCalc.ChartsLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.ChartsLib.Models
{
    public class LabelModel
    {
        private ChartPlotInfo info;
        public LabelModel(ChartPlotInfo info)
        {
            this.info = info;
        }

        public string Text { get; set; }
        public double XPix { get; set; }
        public double YPix { get; set; }
        private DateTime xphys;
        public DateTime? XPhys
        {
            get
            {
                return xphys;
            }
            set
            {
                if (value != null)
                {
                    xphys = value ?? new DateTime();
                    XPix = GeometricHelper.DateToChart(xphys, info);
                }
                
            }
        }
        private double yphys;
        public double? YPhys
        {
            get
            {
                return yphys;
            }
            set
            {
                if (value != null)
                {
                    yphys = value ?? 0;
                    YPix = GeometricHelper.YCoordToChart(yphys, info);
                }
            }
        }
    }
}
