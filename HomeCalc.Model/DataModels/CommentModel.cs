using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DataModels
{
    public class CommentModel
    {
        public long Id { get; set; }
        public long PurchaseId { get; set; }
        public long StoreId { get; set; }
        public string Text { get; set; }
        public int Rate { get; set; }
    }
}
