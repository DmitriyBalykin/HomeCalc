using HomeCalc.ChartsLib.Models;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class AnalyticsViewModel : ReadDataViewModel
    {
        public AnalyticsViewModel()
            :base()
        {
            logger = LogService.GetLogger();
            AddCommand("ShowData", new DelegateCommand(ShowDataCommandExecuted));
        }

        private void ShowDataCommandExecuted(object obj)
        {
            var searchRequest = new SearchRequest
            {
                //NameFilter = purchaseName,
                Type = PurchaseType,
                DateStart = SearchFromDate,
                DateEnd = SearchToDate,
                SearchByName = false,
                SearchByType = false,
                SearchByDate = true,
                SearchByCost = false,
            };

            var chartData = StoreService.LoadPurchaseList(searchRequest)
                //.GroupBy()
                .Select(p => new SeriesDateBasedElement { Argument = p.Date, Value = p.TotalCost }).ToList();

            ChartSeries = new List<IEnumerable<SeriesDateBasedElement>> { chartData };
        }
        #region properties
        private IEnumerable<IEnumerable<SeriesDateBasedElement>> chartSeries;
        public IEnumerable<IEnumerable<SeriesDateBasedElement>> ChartSeries
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
        #endregion
    }
}
