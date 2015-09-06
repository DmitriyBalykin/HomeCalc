using HomeCalc.Model.BasicModels;
using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.ViewModels
{
    public class AddDataModel : ViewModel
    {
        public AddDataModel()
        {
            //Add command CalcCount
            //Add command CalcItemCount
            //Add command Save
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
    }
}
