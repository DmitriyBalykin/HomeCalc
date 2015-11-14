using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCalc.Presentation.Models;
using System.Collections.ObjectModel;

namespace HomeCalc.Presentation.ViewModels
{
    public class ReadDataViewModel : ViewModel
    {
        public ReadDataViewModel()
        {
            logger = LogService.GetLogger();
            AddCommand("Search", new DelegateCommand(SearchCommandExecute));
            AddCommand("OpenInHTML", new DelegateCommand(OpenInHTMLCommandExecute, CanOpenInHTML));

            SearchFromDate = DateTime.Now.AddMonths(-1);
            SearchToDate = DateTime.Now;
        }

        private bool CanOpenInHTML(object obj)
        {
            return SearchSucceded;
        }

        private void OpenInHTMLCommandExecute(object obj)
        {
            
        }

        private void SearchCommandExecute(object obj)
        {
            var searchRequest = new SearchRequest
            {
                NameFilter = purchaseName,
                Type = PurchaseType,
                CostStart = costStart,
                CostEnd = costEnd,
                DateStart = searchFromDate,
                DateEnd = searchToDate,
                SearchByName = searchByName,
                SearchByType = searchByType,
                SearchByDate = searchByDate,
                SearchByCost = searchByCost,
            };
            IList<Purchase> results = StoreService.LoadPurchaseList(searchRequest);
            TotalCount = results.Sum(p => p.ItemsNumber).ToString();
            TotalCost = results.Sum(p => p.TotalCost).ToString();
            SearchResultList = new ObservableCollection<Purchase>(results);
            Status.Post("Пошук завершено, знайдено {0} записів", searchResultList.Count);
        }
        public ObservableCollection<PurchaseType> PurchaseTypesList { get; set; }
        public ObservableCollection<Purchase> searchResultList;
        public ObservableCollection<Purchase> SearchResultList
        {
            get
            {
                return searchResultList;
            }
            set
            {
                if (searchResultList != value)
                {
                    searchResultList = value;
                    OnPropertyChanged(() => SearchResultList);
                }
                SearchSucceded = searchResultList.Count > 0;
            }
        }

        private bool searchByDate;
        public bool SearchByDate
        {
            get { return searchByDate; }
            set
            {
                if (searchByDate != value)
                {
                    searchByDate = value;
                    OnPropertyChanged(() => SearchByDate);
                }
            }
        }
        private bool searchByName;
        public bool SearchByName
        {
            get { return searchByName; }
            set
            {
                if (searchByName != value)
                {
                    searchByName = value;
                    OnPropertyChanged(() => SearchByName);
                }
            }
        }
        private bool searchByType;
        public bool SearchByType
        {
            get { return searchByType; }
            set
            {
                if (searchByType != value)
                {
                    searchByType = value;
                    OnPropertyChanged(() => SearchByType);
                }
            }
        }
        private bool searchByCost;
        public bool SearchByCost
        {
            get { return searchByCost; }
            set
            {
                if (searchByCost != value)
                {
                    searchByCost = value;
                    OnPropertyChanged(() => SearchByCost);
                }
            }
        }
        private DateTime searchFromDate;
        public DateTime SearchFromDate
        {
            get { return searchFromDate; }
            set
            {
                SearchByDate = true;
                if (searchFromDate != value &&
                    searchFromDate <= searchToDate)
                {
                    searchFromDate = value;
                    OnPropertyChanged(() => SearchFromDate);
                }
            }
        }
        private DateTime searchToDate;
        public DateTime SearchToDate
        {
            get
            {
                return searchToDate;
            }
            set
            {
                SearchByDate = true;
                if (searchToDate != value &&
                    searchFromDate >= searchToDate)
                {
                    searchToDate = value;
                    OnPropertyChanged(() => SearchToDate);
                }
            }
        }

        private string purchaseName;
        public string PurchaseName
        {
            get
            {
                return purchaseName;
            }
            set
            {
                SearchByName = true;
                if (purchaseName != value)
                {
                    purchaseName = value;
                    OnPropertyChanged(() => PurchaseName);
                }
            }
        }
        public PurchaseType PurchaseType { get; set; }
        
        private double costStart;
        public string CostStart
        {
            get
            {
                return costStart.ToString();
            }
            set
            {
                SearchByCost = true;
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value != CostStart)
                {
                    var str = value.Replace(',', '.');
                    try
                    {
                        costStart = Double.Parse(value);
                    }
                    catch (FormatException)
                    { }
                    OnPropertyChanged(() => CostStart);
                }
            }
        }
        private double costEnd;
        public string CostEnd
        {
            get
            {
                return costEnd.ToString();
            }
            set
            {
                SearchByCost = true;
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value != CostEnd)
                {
                    var str = value.Replace(',', '.');
                    try
                    {
                        costEnd = Double.Parse(value);
                    }
                    catch (FormatException)
                    { }
                    OnPropertyChanged(() => CostEnd);
                }
            }
        }
        private bool searchSucceded;
        public bool SearchSucceded
        {
            get { return searchSucceded; }
            set
            {
                if (searchSucceded != value)
                {
                    searchSucceded = value;
                    OnPropertyChanged(() => SearchSucceded);
                }
            }
        }

        private string totalCount;
        public string TotalCount
        {
            get
            {
                return totalCount;
            }
            set
            {
                if (value != totalCount)
                {
                    totalCount = value;
                    OnPropertyChanged(() => TotalCount);
                }
            }
        }
        private string totalCost;
        public string TotalCost
        {
            get
            {
                return totalCost;
            }
            set
            {
                if (value != totalCost)
                {
                    totalCost = value;
                    OnPropertyChanged(() => TotalCost);
                }
            }
        }
    }
}
