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
        { }
        public Purchase(Purchase purchase)
        {
            Id = purchase.Id;
            Name = purchase.Name;
            Date = new DateTime(purchase.Date.Ticks);
            ItemCost = purchase.ItemCost;
            ItemsNumber = purchase.ItemsNumber;
            TotalCost = purchase.TotalCost;
            Type = purchase.Type;
        }
        public int Id { get; set; }

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
        private PurchaseType type;
        public PurchaseType Type
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

        public override bool Equals(object obj)
        {
            return (obj is Purchase) && ((Purchase)obj).Id == this.Id;
        }
        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}
