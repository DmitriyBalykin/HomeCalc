﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DataModels
{
    public class PurchaseModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 PurchaseId { get; set; }
        public string Name { get; set; }
        public Int64 Timestamp { get; set; }
        public double TotalCost { get; set; }
        public double ItemCost { get; set; }
        public double ItemsNumber { get; set; }
        public Int64 TypeId { get; set; }
    }
}
