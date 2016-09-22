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

        public static List<DbItem<ProductTypeModel>> Values
        {
            get
            {
            return new List<DbItem<ProductTypeModel>>(){
                new DbItem<ProductTypeModel>{ Table = "PRODUCTTYPE" , Value = new ProductTypeModel{ Name = "Еда" }},
                new DbItem<ProductTypeModel>{ Table = "PRODUCTTYPE" , Value = new ProductTypeModel{ Name = "Хозяйственные товары" }},
                new DbItem<ProductTypeModel>{ Table = "PRODUCTTYPE" , Value = new ProductTypeModel{ Name = "Автомобиль" }},
                new DbItem<ProductTypeModel>{ Table = "PRODUCTTYPE" , Value = new ProductTypeModel{ Name = "Квартира" }},
                new DbItem<ProductTypeModel>{ Table = "PRODUCTTYPE" , Value = new ProductTypeModel{ Name = "Снаряжение" }},
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
                    SettingName = "ShowProductSubType",
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