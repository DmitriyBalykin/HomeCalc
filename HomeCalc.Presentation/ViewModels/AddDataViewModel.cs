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
            MsgDispatcher.AddHandler(HandleMessage);
            purchase = new Purchase();

            AddCommand("Save", new DelegateCommand(SaveCommandExecute, SaveCommandCanExecute));

            Status.Post("Все готово для роботи!");

        }

        protected override void Initialize()
        {
            base.Initialize();

            if (ProductType == null && TypeSelectorItems.Count() > 0)
            {
                ProductType = TypeSelectorItems.FirstOrDefault();
            }

            actualCalculationTarget = Services.DataService.CalculationTargetProperty.TotalCost;
        }

        #region event handlers
        private void HandleMessage(string message)
        {
            switch (message)
            {
                case "historyUpdated":
                    logger.Debug("Purchase history updated, updating purchase list");
                    PurchaseHistoryItemsWrapper = StoreService.PurchaseHistory;
                    break;
                default:
                    break;
            }
        }
        private void SaveCommandExecute(object obj)
        {
            purchase.Name = purchase.Name.Trim();
            Task.Factory.StartNew(async () => 
            {
                var result = await StoreService.AddPurchase(purchase);
                if (result != null)
                {
                    logger.Info("Purchase saved");
                    Status.Post("Покупка \"{0}\" збережена", purchase.Name);
                    CleanInputFields();
                }
                else
                {
                    logger.Warn("Purchase not saved");
                    Status.Post("Помилка: покупка \"{0}\" не збережена", purchase.Name);
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

        private void LoadProductTypes()
        {
            Task.Factory.StartNew(async () => 
            {
                TypeSelectorItems = new ObservableCollection<ProductType>(await StoreService.LoadProductTypeList().ConfigureAwait(false));
                if (ProductType == null && TypeSelectorItems.Count() > 0)
                {
                    ProductType = TypeSelectorItems.FirstOrDefault();
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

                var selectedPurchase = resultList.FirstOrDefault();
                ProductType = selectedPurchase.Type;
                MonthlyPurchase = selectedPurchase.IsMonthly;
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

        public bool MonthlyPurchase
        {
            get { return purchase.IsMonthly; }
            set
            {
                if (purchase.IsMonthly != value)
                {
                    purchase.IsMonthly = value;
                    OnPropertyChanged(() => MonthlyPurchase);
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
        public ProductType ProductType
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
                    OnPropertyChanged(() => ProductType);

                    LoadSubTypes(type.Id);
                }
            }
        }

        public ProductSubType ProductSubType
        {
            get
            {
                return purchase.SubType;
            }
            set
            {
                var type = ProductSubTypes.Where(e => e.Name == value.Name).FirstOrDefault();
                if (type != purchase.SubType)
                {
                    purchase.SubType = type;
                    OnPropertyChanged(() => ProductSubType);
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

        public double PurchaseRate
        {
            get { return purchase.PurchaseRate; }
            set
            {
                if (purchase.PurchaseRate != value)
                {
                    purchase.PurchaseRate = (int)value;
                    OnPropertyChanged(() => PurchaseRate);
                }
            }
        }

        public string PurchaseComment
        {
            get { return purchase.PurchaseComment; }
            set
            {
                if (purchase.PurchaseComment != value)
                {
                    purchase.PurchaseComment = value;
                    OnPropertyChanged(() => PurchaseComment);
                }
            }
        }

        public string StoreName
        {
            get { return purchase.StoreName; }
            set
            {
                if (purchase.StoreName != value)
                {
                    purchase.StoreName = value;
                    OnPropertyChanged(() => StoreName);
                }
            }
        }

        public double StoreRate
        {
            get { return purchase.StoreRate; }
            set
            {
                if (purchase.StoreRate != value)
                {
                    purchase.StoreRate = (int)value;
                    OnPropertyChanged(() => StoreRate);
                }
            }
        }

        public string StoreComment
        {
            get { return purchase.StoreComment; }
            set
            {
                if (purchase.StoreComment != value)
                {
                    purchase.StoreComment = value;
                    OnPropertyChanged(() => StoreComment);
                }
            }
        }

        #endregion
    }
}
