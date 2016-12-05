using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class Purchase : INotifyPropertyChanged
    {
        public Purchase()
        {
            date = DateTime.Now;
        }

        public Purchase(Purchase purchase)
        {
            Id = purchase.Id;
            Name = purchase.Name;
            Date = purchase.Date;
            ItemCost = purchase.ItemCost;
            ItemsNumber = purchase.ItemsNumber;
            TotalCost = purchase.TotalCost;
            Type = purchase.Type;
            SubType = purchase.SubType;
            IsMonthly = purchase.IsMonthly;
            PurchaseRate = purchase.PurchaseRate;
            PurchaseComment = purchase.PurchaseComment;
            StoreName = purchase.StoreName;
            StoreRate = purchase.StoreRate;
            StoreComment = purchase.StoreComment;
            StoreId = purchase.StoreId;
        }
        public long Id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void PublishChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));    
            }
        }
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    PublishChange("Name");
                }
            }
        }
        private DateTime date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                if (value != date)
                {
                    date = value;
                    PublishChange("Date");
                }
            }
        }
        private double totalCost;
        public double TotalCost
        {
            get
            {
                return totalCost;
            }
            set
            {
                if (value != totalCost)
                {
                    totalCost = value;
                    PublishChange("TotalCost");
                }
            }
        }
        private double itemCost;
        public double ItemCost
        {
            get
            {
                return itemCost;
            }
            set
            {
                if (value != itemCost)
                {
                    itemCost = value;
                    PublishChange("ItemCost");
                }
            }
        }
        private double itemsNumber;
        public double ItemsNumber
        {
            get
            {
                return itemsNumber;
            }
            set
            {
                if (value != itemsNumber)
                {
                    itemsNumber = value;
                    PublishChange("ItemsNumber");
                }
            }
        }
        private ProductType type;
        public ProductType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (value != type)
                {
                    type = value;
                    PublishChange("Type");
                }
            }
        }

        private ProductSubType subType;
        public ProductSubType SubType
        {
            get
            {
                return subType;
            }
            set
            {
                if (value != subType)
                {
                    subType = value;
                    PublishChange("SubType");
                }
            }
        }

        public long StoreId { get; set; }
        
        private string storeName;
        public string StoreName
        {
            get
            {
                return storeName;
            }
            set
            {
                if (value != storeName)
                {
                    storeName = value;
                    PublishChange("StoreName");
                }
            }
        }

        private string storeComment;
        public string StoreComment
        {
            get
            {
                return storeComment;
            }
            set
            {
                if (value != storeComment)
                {
                    storeComment = value;
                    PublishChange("StoreComment");
                }
            }
        }

        private int storeRate;
        public int StoreRate
        {
            get
            {
                return storeRate;
            }
            set
            {
                if (value != storeRate)
                {
                    storeRate = value;
                    PublishChange("StoreRate");
                }
            }
        }

        private string purchaseComment;
        public string PurchaseComment
        {
            get
            {
                return purchaseComment;
            }
            set
            {
                if (value != purchaseComment)
                {
                    purchaseComment = value;
                    PublishChange("PurchaseComment");
                }
            }
        }

        private int purchaseRate;
        public int PurchaseRate
        {
            get
            {
                return purchaseRate;
            }
            set
            {
                if (value != purchaseRate)
                {
                    purchaseRate = value;
                    PublishChange("PurchaseRate");
                }
            }
        }

        private bool monthlyPurchase;
        public bool IsMonthly
        {
            get
            {
                return monthlyPurchase;
            }
            set
            {
                if (value != monthlyPurchase)
                {
                    monthlyPurchase = value;
                    PublishChange("IsMonthly");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is Purchase) && ((Purchase)obj).Id == this.Id;
        }
        public bool DeepEquals(object obj)
        {
            var p = obj as Purchase;

            bool result = obj != null;
            result &= p.Date == Date;
            result &= p.Id == Id;
            result &= p.ItemCost == ItemCost;
            result &= p.ItemsNumber == ItemsNumber;
            result &= p.IsMonthly == IsMonthly;
            result &= p.Name == Name;
            result &= p.PurchaseComment == PurchaseComment || (string.IsNullOrEmpty(p.PurchaseComment) && string.IsNullOrEmpty(PurchaseComment));
            result &= p.PurchaseRate == PurchaseRate;
            result &= p.StoreName == StoreName || (string.IsNullOrEmpty(p.StoreName) && string.IsNullOrEmpty(StoreName));
            result &= (p.SubType == null && SubType ==null) || p.SubType.Equals(SubType);
            result &= p.TotalCost == TotalCost;
            result &= p.Type.Equals(Type);

            return result;
        }
        public override int GetHashCode()
        {
            return (int)this.Id;
        }
    }
}
