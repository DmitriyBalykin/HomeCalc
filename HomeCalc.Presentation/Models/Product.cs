using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsMonthly { get; set; }
        public long TypeId { get; set; }
        public long SubTypeId { get; set; }
    }
}
