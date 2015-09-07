using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Model.BasicModels;
using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.ViewModels
{
    public class ReadDataViewModel : ViewModel
    {
        public ReadDataViewModel()
        {
            logger = LogService.GetLogger();
            AddCommand("Search", new DelegateCommand(SearchCommandExecute));
            AddCommand("OpenInHTML", new DelegateCommand(OpenInHTMLCommandExecute));
        }

        private void OpenInHTMLCommandExecute(object obj)
        {
            
        }

        private void SearchCommandExecute(object obj)
        {
            
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
