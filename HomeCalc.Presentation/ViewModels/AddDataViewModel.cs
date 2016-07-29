using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCalc.Presentation.Models;
using System.Globalization;
using HomeCalc.Presentation.Services;
using HomeCalc.Core.Helpers;
using HomeCalc.Presentation.Utils;

namespace HomeCalc.Presentation.ViewModels
{
    public class AddDataViewModel : ViewModel
    {
        private const int MINIMAL_SEARCH_LENGTH = 2;

        public AddDataViewModel()
        {
            purchase = new Purchase();

            AddCommand("Save", new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute));
            
            StoreService.HistoryUpdated += UpdatePurchaseHistory;

            Status.Post("Все готово для роботи!");
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (PurchaseType == null && TypeSelectorItems.Count() > 0)
            {
                PurchaseType = TypeSelectorItems.FirstOrDefault();
            }

            actualCalculationTarget = Services.DataService.CalculationTargetProperty.TotalCost;
        }

        #region event handlers
        void UpdatePurchaseHistory(object sender, EventArgs e)
        {
            logger.Debug("Purchase history updated, updating purchase list");
            PurchaseHistoryItemsWrapper = StoreService.PurchaseHistory;
        }

        private void SaveCommandExecute(object obj)
        {
            purchase.Name = purchase.Name.Trim();
            Task.Factory.StartNew(async () => 
            {
                var result = await StoreService.AddPurchase(purchase);
                if (result)
                {
                    logger.Info("Purchase saved");
                    Status.Post("Покупка \"{0}\" збережена", PurchaseName);
                    CleanInputFields();
                }
                else
                {
                    logger.Warn("Purchase not saved");
                    Status.Post("Помилка: покупка \"{0}\" не збережена", PurchaseName);
                }
            });
            
        }
        private bool SaveCommandCanExecute(object obj)
        {
            return
                !string.IsNullOrWhiteSpace(purchase.Name) &&
                !string.IsNullOrWhiteSpace(Count) &&
                !string.IsNullOrWhiteSpace(ItemCost) &&
                !string.IsNullOrWhiteSpace(TotalCost);
        }
        #endregion

        #region helpers

        private void LoadPurchaseTypes()
        {
            Task.Factory.StartNew(async () => 
            {
                TypeSelectorItems = new ObservableCollection<PurchaseType>(await StoreService.LoadPurchaseTypeList().ConfigureAwait(false));
                if (PurchaseType == null && TypeSelectorItems.Count() > 0)
                {
                    PurchaseType = TypeSelectorItems.FirstOrDefault();
                }
            });
        }

        private void CleanInputFields()
        {
            PurchaseName = string.Empty;

            fieldCalculationBlocked = true;
            Count = string.Empty;
            ItemCost = string.Empty;
            TotalCost = string.Empty;
            fieldCalculationBlocked = false;
        }
        private void SearchPurchase()
        {
            if (purchase.Name.Length < MINIMAL_SEARCH_LENGTH)
            {
                return;
            }
            var exactPurchases = StoreService.PurchaseHistory.Where(p => p.Name.Equals(purchase.Name, StringComparison.InvariantCultureIgnoreCase));
            IEnumerable<Purchase> resultList;
            if (exactPurchases.Count() > 0)
            {
                resultList = exactPurchases;

                PurchaseType = resultList.FirstOrDefault().Type;
            }
            else
            {
                resultList = StoreService.PurchaseHistory.Where(p => p.Name.StartsWith(PurchaseName, true, CultureInfo.InvariantCulture));
            }
            PurchaseHistoryItemsWrapper = resultList;
            ShowPurchaseHistory = PurchaseHistoryItems.Count() > 0;
        }
        private void DoCalculations()
        {
            if (fieldCalculationBlocked)
            {
                return;
            }
            try
            {
                fieldCalculationInProgress = true;

                if (purchase != null)
                {
                    purchase.ItemsNumber = String2NumberHelper.ToNumber(Count);
                    purchase.ItemCost = String2NumberHelper.ToNumber(ItemCost);
                    purchase.TotalCost = String2NumberHelper.ToNumber(TotalCost);

                    DataService.PerformCalculation(purchase, actualCalculationTarget);

                    if (actualCalculationTarget == Services.DataService.CalculationTargetProperty.ItemCost)
                    {
                        ItemCost = purchase.ItemCost != 0 ? purchase.ItemCost.ToString() : null;
                    }
                    if (actualCalculationTarget == Services.DataService.CalculationTargetProperty.TotalCost)
                    {
                        TotalCost = purchase.TotalCost != 0 ? purchase.TotalCost.ToString() : null;
                    }
                    if (actualCalculationTarget == Services.DataService.CalculationTargetProperty.ItemsNumber)
                    {
                        Count = purchase.ItemsNumber != 0 ? purchase.ItemsNumber.ToString() : null;
                    }

                    OnPropertyChanged(() => Count);
                    OnPropertyChanged(() => ItemCost);
                    OnPropertyChanged(() => TotalCost);
                }
            }
            finally
            {
                fieldCalculationInProgress = false;
            }
            
        }

        #endregion

        #region properties
        private DateTime dateToStore = DateTime.Now;
        public DateTime DateToStore {
            get
            {
                return purchase.Date;
            }
            set
            {
                if (value != purchase.Date)
                {
                    purchase.Date = value;
                    OnPropertyChanged(() => DateToStore);
                }
            }
        }
        private IEnumerable<Purchase> PurchaseHistoryItemsWrapper
        {
            set
            {
                var distincted = value.OrderByDescending(p => p.Date).Distinct(new PurchaseForHistoryComparer()).Take(10);
                PurchaseHistoryItems = new ObservableCollection<Purchase>(distincted);
            }
        }
        private ObservableCollection<Purchase> purchaseHistoryItems;
        public ObservableCollection<Purchase> PurchaseHistoryItems
        {
            get
            {
                return purchaseHistoryItems;
            }
            set
            {
                if (purchaseHistoryItems != value)
                {
                    purchaseHistoryItems = value;
                    OnPropertyChanged(() => PurchaseHistoryItems);
                }
            }
        }

        private DataService.CalculationTargetProperty actualCalculationTarget;

        private Purchase purchase;
        public string PurchaseName
        {
            get
            {
                return purchase.Name;
            }
            set
            {
                if (value != purchase.Name)
                {
                    purchase.Name = value;
                    OnPropertyChanged(() => PurchaseName);
                    SearchPurchase();
                }
            }
        }

        private bool fieldCalculationInProgress = false;
        private bool fieldCalculationBlocked = false;
        private string count;
        public string Count
        {
            get { return count; }
            set
            {
                if ((count != value && String2NumberHelper.IsNumber(value)) || value == string.Empty)
                {
                    count = String2NumberHelper.GetCorrected(value);
                    if (!fieldCalculationInProgress)
                    {
                        DoCalculations();
                    }
                    if (fieldCalculationBlocked)
                    {
                        OnPropertyChanged(() => Count);
                    }
                }
            }
        }

        string itemCost;
        public string ItemCost
        {
            get { return itemCost; }
            set
            {
                if ((itemCost != value && String2NumberHelper.IsNumber(value)) || value == string.Empty)
                {
                    itemCost = String2NumberHelper.GetCorrected(value, 2);
                    if (!fieldCalculationInProgress)
                    {
                        DoCalculations();
                    }
                    if (fieldCalculationBlocked)
                    {
                        OnPropertyChanged(() => ItemCost);
                    }
                }
            }
        }

        string totalCost;
        public string TotalCost
        {
            get { return totalCost; }
            set
            {
                if ((totalCost != value && String2NumberHelper.IsNumber(value)) || value == string.Empty)
                {
                    totalCost = String2NumberHelper.GetCorrected(value, 2);
                    if (!fieldCalculationInProgress)
                    {
                        DoCalculations();
                    }
                    if (fieldCalculationBlocked)
                    {
                        OnPropertyChanged(() => TotalCost);
                    }
                }
            }
        }
        public PurchaseType PurchaseType
        {
            get
            {
                return purchase.Type; 
            }
            set
            {
                var type = TypeSelectorItems.Where(e => e.Name == value.Name).FirstOrDefault();
                if (type != purchase.Type)
                {
                    purchase.Type = type;
                    OnPropertyChanged(() => PurchaseType);
                }
            }
        }

        public PurchaseSubType PurchaseSubType
        {
            get
            {
                return purchase.SubType;
            }
            set
            {
                var type = PurchaseSubTypes.Where(e => e.Name == value.Name).FirstOrDefault();
                if (type != purchase.SubType)
                {
                    purchase.SubType = type;
                    OnPropertyChanged(() => PurchaseSubType);
                }
            }
        }
        
        private bool calcItemCost = false;
        public bool IsCalcItemCost
        {
            get
            {
                return calcItemCost;
            }
            set
            {
                if (calcItemCost != value)
                {
                    calcItemCost = value;
                    OnPropertyChanged(() => IsCalcItemCost);
                }
                if (value)
                {
                    IsCalcItemsCount = false;
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.ItemCost;
                }
                setTotalCalc();
            }
        }
        private bool calcItemsCount = false;
        public bool IsCalcItemsCount
        {
            get
            {
                return calcItemsCount;
            }
            set
            {
                if (calcItemsCount != value)
                {
                    calcItemsCount = value;
                    OnPropertyChanged(() => IsCalcItemsCount);
                }
                if (value)
                {
                    IsCalcItemCost = false;
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.ItemsNumber;
                }
                setTotalCalc();
            }
        }
        private void setTotalCalc()
        {
            IsCalcByTotal = !(calcItemsCount || calcItemCost);
        }
        private bool calcTotalCost = true;
        public bool IsCalcByTotal
        {
            get
            {
                return !calcTotalCost;
            }
            set
            {
                if (value != calcTotalCost)
                {
                    calcTotalCost = value;
                    OnPropertyChanged(() => IsCalcByTotal);
                }
                if (value)
                {
                    actualCalculationTarget = Services.DataService.CalculationTargetProperty.TotalCost;
                }
            }
        }

        private bool showPurchaseHistory;
        public bool ShowPurchaseHistory
        {
            get
            {
                return showPurchaseHistory;
            }
            set
            {
                if (value != showPurchaseHistory)
                {
                    showPurchaseHistory = value;
                    OnPropertyChanged(() => ShowPurchaseHistory);
                }
            }
        }

        public bool ShowRatingPanel
        {
            get
            {
                return ShowPurchaseSubType || ShowPurchaseRate || ShowPurchaseComment || ShowStoreRate || ShowStoreName;
            }
        }

        double purchaseRate;
        public double PurchaseRate
        {
            get { return purchaseRate; }
            set
            {
                if (purchaseRate != value)
                {
                    purchaseRate = value;
                    OnPropertyChanged(() => PurchaseRate);
                }
            }
        }

        string purchaseComment = string.Empty;
        public string PurchaseComment
        {
            get { return purchaseComment; }
            set
            {
                if (purchaseComment != value)
                {
                    purchaseComment = value;
                    OnPropertyChanged(() => PurchaseComment);
                }
            }
        }

        string storeName;
        public string StoreName
        {
            get { return storeName; }
            set
            {
                if (storeName != value)
                {
                    storeName = value;
                    OnPropertyChanged(() => StoreName);
                }
            }
        }

        double storeRate;
        public double StoreRate
        {
            get { return storeRate; }
            set
            {
                if (storeRate != value)
                {
                    storeRate = value;
                    OnPropertyChanged(() => StoreRate);
                }
            }
        }

        string storeComment = string.Empty;
        public string StoreComment
        {
            get { return storeComment; }
            set
            {
                if (storeComment != value)
                {
                    storeComment = value;
                    OnPropertyChanged(() => StoreComment);
                }
            }
        }

        #endregion
    }
}
