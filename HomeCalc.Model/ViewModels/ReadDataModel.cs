using HomeCalc.Model.BasicModels;
using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.ViewModels
{
    public class ReadDataModel : ViewModel
    {
        public ReadDataModel()
        {
            //Add command "Search"
            //Add command "OpenInHTML"
        }
        public bool SearchByDate { get; set; }
        public bool SearchByName { get; set; }
        public bool SearchByType { get; set; }
        public bool SearchByCost { get; set; }

        public DateTime SearchFromDate { get; set; }
        public DateTime SearchToDate { get; set; }

        public string PurchaseName { get; set; }
        public IObservable<PurchaseType> PurchaseTypesList { get; set; }
        public string CostStart { get; set; }
        public string CostEnd { get; set; }
    }
}
