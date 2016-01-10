using HomeCalc.ChartsLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HomeCalc.ChartsLib.ViewModels
{
    public class AreaPlotViewModel : UserControlViewModel
    {
        //public event EventHandler SeriesUpdated;

        public AreaPlotViewModel()
        {
            HeaderHeight = 0;
            FooterHeight = 0;
            LeftLegendWidth = 0;
            RightLegendWidth = 0;
        }


        

        private int headerHeight;
        public int HeaderHeight
        {
            get { return headerHeight; }
            set
            {
                if (value != headerHeight)
                {
                    headerHeight = value;
                    OnPropertyChanged(() => HeaderHeight);
                }
            }
        }
        private int footerHeight;
        public int FooterHeight
        {
            get { return footerHeight; }
            set
            {
                if (value != footerHeight)
                {
                    footerHeight = value;
                    OnPropertyChanged(() => FooterHeight);
                }
            }
        }
        private int leftLegendWidth;
        public int LeftLegendWidth
        {
            get { return leftLegendWidth; }
            set
            {
                if (value != leftLegendWidth)
                {
                    leftLegendWidth = value;
                    OnPropertyChanged(() => LeftLegendWidth);
                }
            }
        }
        private int rightLegendWidth;
        public int RightLegendWidth
        {
            get { return rightLegendWidth; }
            set
            {
                if (value != rightLegendWidth)
                {
                    rightLegendWidth = value;
                    OnPropertyChanged(() => RightLegendWidth);
                }
            }
        }
    }
}
