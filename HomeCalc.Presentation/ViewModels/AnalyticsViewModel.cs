using HomeCalc.ChartsLib.Models;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class AnalyticsViewModel : ViewModel
    {
        public AnalyticsViewModel()
        {
            logger = LogService.GetLogger();
        }

        private IEnumerable<IEnumerable<SeriesDoubleBasedElement>> chartSeries;
        public IEnumerable<IEnumerable<SeriesDoubleBasedElement>> ChartSeries
        {
            get { return chartSeries; }
            set
            {
                if (value != chartSeries)
                {
                    chartSeries = value;
                    OnPropertyChanged(() => ChartSeries);
                }
            }
        }
        
        
    }
}
