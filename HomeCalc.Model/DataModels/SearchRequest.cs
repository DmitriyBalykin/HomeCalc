using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class SearchRequestModel
    {
        public bool SearchByName { get; set; }
        public string Name { get; set; }
        public bool SearchById { get; set; }
        public long PurchaseId { get; set; }

        public bool SearchByType { get; set; }
        public int TypeId { get; set; }

        public bool SearchBySubType { get; set; }
        public int SubTypeId { get; set; }

        public bool SearchByDate { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public bool SearchByCost { get; set; }
        public double CostStart { get; set; }
        public double CostEnd { get; set; }

        public bool SearchByMonthly { get; set; }
        public bool IsMonthly { get; set; }

        public bool SearchByRate { get; set; }
        public int RateStart { get; set; }
        public int RateEnd { get; set; }

        public bool SearchByCommentId { get; set; }
        public long CommentId { get; set; }
        public string Comment { get; set; }

        public bool SearchByStoreId { get; set; }
        public long StoreId { get; set; }
        public string StoreName { get; set; }
        public int StoreRateStart { get; set; }
        public int StoreRateEnd { get; set; }
        public string StoreComment { get; set; }
        
        public enum Requests
        {
            Empty
        }
    }
}
