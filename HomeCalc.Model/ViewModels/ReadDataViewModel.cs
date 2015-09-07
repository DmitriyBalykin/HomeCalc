using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Data.Models;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class ReadDataViewModel : ViewModel
    {
        public ReadDataViewModel()
        {
            logger = LogService.GetLogger();
            AddCommand("Search", new DelegateCommand(SearchCommandExecute));
            AddCommand("OpenInHTML", new DelegateCommand(OpenInHTMLCommandExecute, CanOpenInHTML));
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
            
        }
        public bool SearchByDate { get; set; }
        public bool SearchByName { get; set; }
        public bool SearchByType { get; set; }
        public bool SearchByCost { get; set; }

        public DateTime SearchFromDate { get; set; }
        public DateTime SearchToDate { get; set; }

        public string PurchaseName { get; set; }
        public IObservable<PurchaseType> PurchaseTypesList { get; set; }
        public IObservable<PurchaseModel> SearchResultList { get; set; }
        public string CostStart { get; set; }
        public string CostEnd { get; set; }
        public bool SearchSucceded { get; set; }
    }
}
