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
using System.ComponentModel;
using HomeCalc.Presentation.Services;
using System.Windows.Forms;
using HomeCalc.Core.Helpers;

namespace HomeCalc.Presentation.ViewModels
{
    public class ReadDataViewModel : ViewModel
    {
        public ReadDataViewModel()
        {
            AddCommand("Search", new DelegateCommand(SearchCommandExecute));

            AddCommand("Calculate", new DelegateCommand(CalculateCommandExecuted));
            AddCommand("CancelCalculate", new DelegateCommand(CancelCalculateCommandExecuted));

            SearchFromDate = DateTime.Now.AddMonths(-1);
            SearchToDate = DateTime.Now;
        }

        void SearchResultList_ListChanged(object sender, ListChangedEventArgs e)
        {
            Purchase purchase;
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    purchase = searchResultList.ElementAt(e.NewIndex);
                    var referencePurchase = SearchResultListBackup.ElementAt(e.NewIndex);
                    if (!RecalculatePurchase(purchase, referencePurchase))
	                {
                        Task.Factory.StartNew(async () => await StoreService.UpdatePurchase(purchase));
	                }
                    break;
                case ListChangedType.ItemDeleted:
                    purchase = SearchResultListBackup.ElementAt(e.NewIndex);
                    var result = MessageBox.Show(
                        string.Format("Видалити запис \"{0}\"?", purchase.Name),
                        "Видалення запису",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);
                    Task.Factory.StartNew(async () => 
                    {
                        if (result == DialogResult.OK && await StoreService.RemovePurchase(purchase.Id))
                        {
                        BackupSearchList(SearchResultList);
                        Status.Post("Покупка \"{0}\" видалена", purchase.Name);
                    }
                    });
                    
                    break;
                default:
                    break;
            }
            
        }
        private Purchase editingPurchase;
        private bool RecalculatePurchase(Purchase purchase, Purchase referencePurchase)
        {
            if (ShowDataCalcPopup)
            {
                return false;
            }
            bool calculationNeeded = false;
            if (purchase.ItemCost != referencePurchase.ItemCost)
            {
                ShowCalcItemCost = false;
                calculationNeeded = true;
            }
            else if (purchase.ItemsNumber != referencePurchase.ItemsNumber)
            {
                ShowCalcItemNumber = false;
                calculationNeeded = true;
            }
            else if (purchase.TotalCost != referencePurchase.TotalCost)
            {
                ShowCalcTotalCost = false;
                calculationNeeded = true;
            }
            editingPurchase = purchase;
            ShowDataCalcPopup = calculationNeeded;

            return calculationNeeded;
        }
        private void CancelCalculateCommandExecuted(object obj)
        {
            ShowDataCalcPopup = false;
            SearchResultList = new BindingList<Purchase>(SearchResultListBackup);
        }

        private void CalculateCommandExecuted(object obj)
        {
            if (DataService.PerformCalculation(editingPurchase, actualCalculationTarget))
            {
                OnPropertyChanged(() => SearchResultList);
                BackupSearchList(SearchResultList);
                //SearchResultList = new BindingList<Purchase>(SearchResultListBackup);
                Task.Factory.StartNew(async () => 
                {
                    if (await StoreService.UpdatePurchase(editingPurchase))
                    {
                        Status.Post("Зміни до покупки \"{0}\" збрежені", editingPurchase.Name);
                    }
                });
                
            }
            else
            {
                SearchResultList = new BindingList<Purchase>(SearchResultListBackup);
            }
            ShowDataCalcPopup = false;
        }
        private bool CanOpenInHTML(object obj)
        {
            return SearchSucceded;
        }

        private void SearchCommandExecute(object obj)
        {
            var searchRequest = new SearchRequestModel
            {
                Name = purchaseName,
                TypeId = PurchaseType != null ?PurchaseType.TypeId : -1,
                CostStart = costStart,
                CostEnd = costEnd,
                DateStart = searchFromDate,
                DateEnd = searchToDate,
                SearchByName = searchByName,
                SearchByType = searchByType,
                SearchByDate = searchByDate,
                SearchByCost = searchByCost,
            };

            Task.Factory.StartNew(async () => 
            {
                List<Purchase> results = await StoreService.LoadPurchaseList(searchRequest).ConfigureAwait(false);
                results = results.OrderBy(p => p.Date).ToList();
                TotalCount = results.Sum(p => p.ItemsNumber).ToString();
                TotalCost = results.Sum(p => p.TotalCost).ToString();
                BackupSearchList(results);
                SearchResultList = new BindingList<Purchase>(results);
                Status.Post("Пошук завершено, знайдено {0} записів", searchResultList.Count);
            });
            
        }

        private void BackupSearchList(IEnumerable<Purchase> list)
        {
            searchResultListBackup = list.Select(p => new Purchase(p)).ToList();
        }
        private List<Purchase> searchResultListBackup;
        private List<Purchase> SearchResultListBackup
        {
            get
            { return new List<Purchase>(searchResultListBackup); }
        }
        public BindingList<Purchase> searchResultList;
        public BindingList<Purchase> SearchResultList
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
                    if (SearchResultList != null)
                    {
                        SearchResultList.ListChanged += SearchResultList_ListChanged;
                    }
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
                    searchFromDate <= value)
                {
                    searchToDate = value;
                    if (searchToDate.Hour == 0 && searchToDate.Minute == 0 && searchToDate.Second == 0)
                    {
                        searchToDate = searchToDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    
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
                if (!string.IsNullOrEmpty(value))
                {
                    SearchByName = true;
                }
                
                if (purchaseName != value)
                {
                    purchaseName = value;
                    OnPropertyChanged(() => PurchaseName);
                }
            }
        }

        private PurchaseType purchaseType;
        public PurchaseType PurchaseType
        {
            get
            {
                return purchaseType;
            }
            set
            {
                SearchByType = true;

                if (purchaseType != value)
                {
                    purchaseType = value;
                    OnPropertyChanged(() => PurchaseType);
                }
            }
        }
        
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
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value != CostStart && String2NumberHelper.IsNumber(value))
                {
                    var str = String2NumberHelper.GetCorrected(value, 2);
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
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value) && value != CostEnd && String2NumberHelper.IsNumber(value))
                {
                    var str = String2NumberHelper.GetCorrected(value, 2);
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
                    totalCount = String2NumberHelper.GetCorrected(value, 2);
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
                    totalCost = String2NumberHelper.GetCorrected(value, 2);
                    OnPropertyChanged(() => TotalCost);
                }
            }
        }

        private bool calcItemCost;
        public bool CalcItemCost
        {
            get { return calcItemCost; }
            set
            {
                calcItemCost = value;
                if (value)
                {
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.ItemCost;
                    CalcItemNumber = false;
                    CalcTotalCost = false;
                }
                OnPropertyChanged(() => CalcItemCost);
            }
        }

        private bool calcItemNumber;
        public bool CalcItemNumber
        {
            get { return calcItemNumber; }
            set
            {
                calcItemNumber = value;
                if (value)
                {
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.ItemsNumber;
                    CalcItemCost = false;
                    CalcTotalCost = false;
                }
                OnPropertyChanged(() => CalcItemNumber);
            }
        }

        private bool calcTotalCost = true;
        public bool CalcTotalCost
        {
            get { return calcTotalCost; }
            set
            {
                calcTotalCost = value;
                if (value)
                {
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.TotalCost;
                    CalcItemNumber = false;
                    CalcItemCost = false;
                }
                OnPropertyChanged(() => CalcTotalCost);
            }
        }

        private bool showCalcItemCost;
        public bool ShowCalcItemCost
        {
            get { return showCalcItemCost; }
            set {
                showCalcItemCost = value;
                OnPropertyChanged(() => ShowCalcItemCost);
                if (!value)
                {
                    ShowCalcItemNumber = true;
                    ShowCalcTotalCost = true;
                }
            }
        }

        private bool showcalcItemNumber;
        public bool ShowCalcItemNumber
        {
            get { return showcalcItemNumber; }
            set {
                showcalcItemNumber = value;
                OnPropertyChanged(() => ShowCalcItemNumber);
                if (!value)
                {
                    ShowCalcItemCost = true;
                    ShowCalcTotalCost = true;
                }
            }
        }

        private bool showcalcTotalCost;
        public bool ShowCalcTotalCost
        {
            get { return showcalcTotalCost; }
            set {
                showcalcTotalCost = value;
                OnPropertyChanged(() => ShowCalcTotalCost);
                if (!value)
                {
                    ShowCalcItemCost = true;
                    ShowCalcItemNumber = true;
                }
            }
        }

        private bool showDataCalcPopup;
        public bool ShowDataCalcPopup
        {
            get { return showDataCalcPopup; }
            set {
                if (value != showDataCalcPopup)
                {
                    showDataCalcPopup = value;
                    OnPropertyChanged(() => ShowDataCalcPopup);
                }
            }
        }
        private DataService.CalculationTargetProperty actualCalculationTarget = DataService.CalculationTargetProperty.TotalCost;
    }
}
