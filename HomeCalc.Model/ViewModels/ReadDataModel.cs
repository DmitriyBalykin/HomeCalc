using HomeCalc.Model.BasicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.ViewModels
{
    public class ReadDataModel : ViewModel
    {
        public bool SearchByDate { get; set; }
        public bool SearchByName { get; set; }
        public bool SearchByType { get; set; }
        public bool SearchByCost { get; set; }
    }
}
