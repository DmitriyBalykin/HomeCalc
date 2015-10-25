using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class SearchRequest
    {
        public string NameFilter { get; set; }
        public PurchaseType MyProperty { get; set; }

        public PurchaseType Type { get; set; }

        public double CostStart { get; set; }

        public double CostEnd { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public bool SearchByName { get; set; }

        public bool SearchByType { get; set; }

        public bool SearchByDate { get; set; }

        public bool SearchByCost { get; set; }
    }
}
