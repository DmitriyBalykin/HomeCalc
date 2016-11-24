using HomeCalc.Core.LogService;
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
        private static Logger logger = LogService.GetLogger();
        public static bool PerformCalculation(Purchase purchase, CalculationTargetProperty targetProperty)
        {
            logger.SendEmail = SettingsService.GetInstance().GetSetting(SettingsService.SEND_EMAIL_AUTO_KEY).SettingBoolValue;

            bool result = true;
            try
            {
                switch (targetProperty)
                {
                    case CalculationTargetProperty.ItemCost:
                        purchase.ItemCost = purchase.TotalCost / purchase.ItemsNumber;
                        logger.Debug("Calculating item cost");
                        break;
                    case CalculationTargetProperty.ItemsNumber:
                        purchase.ItemsNumber = purchase.TotalCost / purchase.ItemCost;
                        logger.Debug("Calculating items number");
                        break;
                    case CalculationTargetProperty.TotalCost:
                        purchase.TotalCost = purchase.ItemCost * purchase.ItemsNumber;
                        logger.Debug("Calculating total cost");
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
