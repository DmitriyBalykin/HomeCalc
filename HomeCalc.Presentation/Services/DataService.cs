using HomeCalc.Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Services
{
    public class DataService
    {

        public static bool PerformCalculation(Purchase purchase, CalculationTargetProperty targetProperty)
        {
            bool result = true;
            try
            {
                switch (targetProperty)
                {
                    case CalculationTargetProperty.ItemCost:
                        purchase.ItemCost = purchase.TotalCost / purchase.ItemsNumber;
                        break;
                    case CalculationTargetProperty.ItemsNumber:
                        purchase.ItemsNumber = purchase.TotalCost / purchase.ItemCost;
                        break;
                    case CalculationTargetProperty.TotalCost:
                        purchase.TotalCost = purchase.ItemCost * purchase.ItemsNumber;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public enum CalculationTargetProperty
        { 
            ItemCost,
            ItemsNumber,
            TotalCost
        }
    }
}
