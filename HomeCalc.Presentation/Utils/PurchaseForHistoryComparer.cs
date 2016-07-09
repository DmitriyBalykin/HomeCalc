using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Utils
{
    public class PurchaseForHistoryComparer : IEqualityComparer<Purchase>
    {
        public int Compare(Purchase x, Purchase y)
        {
            return string.Compare(x.Name, y.Name);
        }

        public bool Equals(Purchase x, Purchase y)
        {
            return string.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Purchase obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
