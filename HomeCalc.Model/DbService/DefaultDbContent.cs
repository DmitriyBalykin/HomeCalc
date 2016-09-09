using HomeCalc.Model.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DbService
{
    public static class DefaultDbContent
    {

        public static List<DbItem<PurchaseTypeModel>> Values
        {
            get
            {
            return new List<DbItem<PurchaseTypeModel>>(){
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Еда" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Хозяйственные товары" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Автомобиль" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Квартира" }},
                new DbItem<PurchaseTypeModel>{ Table = "PURCHASETYPE" , Value = new PurchaseTypeModel{ Name = "Снаряжение" }},
            };
            }
        }

        public static List<SettingsStorageModel> Settings
        {
            get
            {
                var list = new List<SettingsStorageModel>();
                list.Add(new SettingsStorageModel 
                {
                    ProfileId = 0,
                    SettingId = 0,
                    SettingName = "AutoUpdate",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 1,
                    SettingName = "AutoUpdateCheck",
                    SettingValue = "true"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 2,
                    SettingName = "BackupPath",
                    SettingValue = ""
                });

                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 3,
                    SettingName = "ShowPurchaseSubType",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 4,
                    SettingName = "ShowPurchaseComment",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 5,
                    SettingName = "ShowPurchaseRate",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 6,
                    SettingName = "ShowMonthlyPurchaseRate",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 6,
                    SettingName = "ShowStoreName",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 6,
                    SettingName = "ShowStoreComment",
                    SettingValue = "false"
                });
                list.Add(new SettingsStorageModel
                {
                    ProfileId = 0,
                    SettingId = 7,
                    SettingName = "ShowStoreRate",
                    SettingValue = "false"
                });





                return list;
            }
        }
    }

    public class DbItem<T>
    {
        public string Table { get; set; }
        public T Value { get; set; }
    }
}