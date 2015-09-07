using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class AddDataViewModel : ViewModel
    {
        public AddDataViewModel()
        {
            logger = LogService.GetLogger();
            AddCommand("Save", new DelegateCommand(SaveCommandExecute));
        }

        private void SaveCommandExecute(object obj)
        {
            //throw new NotImplementedException();
        }
        private DateTime dateToStore = DateTime.Now;
        public DateTime DateToStore {
            get
            {
                return dateToStore;
            }
            set
            {
                if (value != dateToStore)
                {
                    dateToStore = value;
                }
            }
        }

        public IObservable<PurchaseType> TypeSelectorItems { get; set; }

        public string PurchaseName { get; set; }
        public string Count { get; set; }
        public string ItemCount { get; set; }
        public string TotalCost { get; set; }

        private bool calcTotalCost = true;
        private bool calcItemCost = false;
        public bool CalcItemCost
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
                    OnPropertyChanged(() => CalcItemCost);
                }
                if (value)
                {
                    CalcItemsCount = false;
                    calcTotalCost = false;
                }
                else
                {
                    setTotalCalc();
                }
            }
        }
        private bool calcItemsCount = false;
        public bool CalcItemsCount
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
                    OnPropertyChanged(() => CalcItemsCount);
                }
                if (value)
                {
                    CalcItemCost = false;
                    calcTotalCost = false;
                }
                else
                {
                    setTotalCalc();
                }
            }
        }
        private void setTotalCalc()
        {
            if (!calcItemsCount && !calcItemCost)
            {
                calcTotalCost = true;
            }
        }
    }
}
