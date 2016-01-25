using HomeCalc.ChartsLib.Models;
using HomeCalc.Core.LogService;
using HomeCalc.Core.Helpers;
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

            IntervalList = AggregationInterval.GetList();
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
                .GroupBy(x =>
                    {
                        return CutTimeTo(x.Date, SelectedInterval);
                    })
                .Select(g => new SeriesDateBasedElement { Argument = g.Key, Value = g.Sum(p => p.TotalCost) }).ToList();

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

        public IEnumerable<AggregationInterval> IntervalList
        {
            get;
            private set;
        }

        private AggregationInterval selectedInterval;

        public AggregationInterval SelectedInterval
        {
            get
            {
                return selectedInterval;
            }
            set
            {
                if (selectedInterval != value)
                {
                    selectedInterval = value;
                    OnPropertyChanged(() => SelectedInterval);
                }
            }
        }
        
        #endregion

        #region Private

        private DateTime CutTimeTo(DateTime dt, AggregationInterval intl)
        {
            switch (intl.Value)
            {
                case AggregationIntervalValue.Day:
                    return new DateTime(dt.Year, dt.Month, dt.Day);
                case AggregationIntervalValue.Week:
                    return dt.Round(TimeSpan.FromDays(7));
                case AggregationIntervalValue.Month:
                    return new DateTime(dt.Year, dt.Month, 1);
                case AggregationIntervalValue.Quarter:
                    return new DateTime(dt.Year, ((int)(Math.Ceiling((double)dt.Month / 3) * 3 - 2)), 1);
                case AggregationIntervalValue.Year:
                    return new DateTime(dt.Year, 1, 1);
            }
            return new DateTime();
        }

        #endregion
    }
}
