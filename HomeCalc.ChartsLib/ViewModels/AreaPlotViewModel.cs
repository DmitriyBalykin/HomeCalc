using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.ChartsLib.ViewModels
{
    public class AreaPlotViewModel : UserControlViewModel
    {
        public AreaPlotViewModel()
        {
            HeaderHeight = 0;
            FooterHeight = 0;
            LeftLegendWidth = 0;
            RightLegendWidth = 0;
        }
        public int HeaderHeight { get; set; }
        public int FooterHeight { get; set; }
        public int LeftLegendWidth { get; set; }
        public int RightLegendWidth { get; set; }
    }
}
