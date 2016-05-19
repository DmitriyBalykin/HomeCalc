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

            TypeSelectorItems.Insert(0, new PurchaseType { Name = "Не обрано", TypeId = -1 });

            SelectedInterval = IntervalList.FirstOrDefault();

            PurchaseType = TypeSelectorItems.FirstOrDefault();

            TotalCostChart = true;
        }

        private void ShowDataCommandExecuted(object obj)
        {
            var searchRequest = new SearchRequestModel
            {
                Name = PurchaseName,
                TypeId = PurchaseType.TypeId,
                DateStart = SearchFromDate,
                DateEnd = SearchToDate,
                SearchByName = !string.IsNullOrEmpty(PurchaseName),
                SearchByType = PurchaseType.TypeId != -1,
                SearchByDate = true,
                SearchByCost = false,
            };

            var chartData = StoreService.LoadPurchaseList(searchRequest).Result
                .GroupBy(x =>
                    {
                        return CutTimeTo(x.Date, SelectedInterval);
                    })
                .Select(g => GetChartElement(g)).ToList();

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

        //TODO rework to listbox selector

        ChartValueType chartValueType = ChartValueType.TotalCost;

        private bool totalCostChart;
        public bool TotalCostChart
        {
            get
            {
                return totalCostChart;
            }
            set
            {
                if (totalCostChart != value)
                {
                    totalCostChart = value;
                    OnPropertyChanged(() => TotalCostChart);
                }
                if (value)
                {
                    chartValueType = ChartValueType.TotalCost;
                }
            }
        }

        private bool itemCostChart;
        public bool ItemCostChart
        {
            get
            {
                return itemCostChart;
            }
            set
            {
                if (itemCostChart != value)
                {
                    itemCostChart = value;
                    OnPropertyChanged(() => ItemCostChart);
                }
                if (value)
                {
                    chartValueType = ChartValueType.ItemCost;
                }
            }
        }

        private bool numberChart;
        public bool NumberChart
        {
            get
            {
                return numberChart;
            }
            set
            {
                if (numberChart != value)
                {
                    numberChart = value;
                    OnPropertyChanged(() => NumberChart);
                }
                if (value)
                {
                    chartValueType = ChartValueType.Number;
                }
            }
        }

        #endregion

        #region Private
        private SeriesDateBasedElement GetChartElement(IGrouping<DateTime, Purchase> g)
        {
            var seriesElement = new SeriesDateBasedElement{ Argument = g.Key };
            switch (chartValueType)
            {
                case ChartValueType.TotalCost:
                    var list = g.ToList();
                    seriesElement.Value = g.Sum(purchase => purchase.TotalCost);
                    break; 
                case ChartValueType.ItemCost:
                    list = g.ToList();
                    seriesElement.Value = g.Average(purchase => purchase.ItemCost);
                    break;
                case ChartValueType.Number:
                    list = g.ToList();
                    seriesElement.Value = g.Sum(purchase => purchase.ItemsNumber);
                    break;
            }
            return seriesElement;
        }

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

    enum ChartValueType
    {
        TotalCost = 0,
        ItemCost = 1,
        Number = 2
    }
}
