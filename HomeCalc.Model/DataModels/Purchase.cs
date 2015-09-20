using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.DataModels
{
    public class Purchase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double TotalCost { get; set; }
        public double ItemCost { get; set; }
        public double ItemsNumber { get; set; }
        public PurchaseType Type { get; set; }
    }
}
