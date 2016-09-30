using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Core.Utilities;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.Models;
using HomeCalc.Presentation.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class OperationsViewModel : ViewModel
    {
        public OperationsViewModel()
        {
            //AddCommand("SelectPath", new DelegateCommand(SelectPathCommandExecute));
            //AddCommand("ImportData", new DelegateCommand(ImportDataCommandExecute, CanImportData));
            AddCommand("AddType", new DelegateCommand(AddTypeCommandExecute, CanAddType));
            AddCommand("RenameType", new DelegateCommand(RenameTypeCommandExecute, CanRenameType));
            AddCommand("DeleteType", new DelegateCommand(DeleteTypeCommandExecute, CanDeleteType));
            AddCommand("AddSubType", new DelegateCommand(AddSubTypeCommandExecute, CanAddSubType));
            AddCommand("RenameSubType", new DelegateCommand(RenameSubTypeCommandExecute, CanRenameSubType));
            AddCommand("DeleteSubType", new DelegateCommand(DeleteSubTypeCommandExecute, CanDeleteSubType));

            MsgDispatcher.AddHandler(HandleMessage);
            StoreService.SubTypesUpdated += StoreService_SubTypesUpdated;

            NewProductTypeEditable = true;

        }

        private void HandleMessage(string msg)
        {
            switch (msg)
            {
                case "productSubTypesLoaded":
                    ProductSubTypeSelectable = true;
                    break;
                case "productSubTypesUnLoaded":
                    ProductSubTypeSelectable = false;
                    break;
                default:
                    break;
            }
        }
        #region commands
        private bool CanAddType(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewProductType);
        }

        private void AddTypeCommandExecute(object obj)
        {
            Task.Factory.StartNew(async () => 
            {
                if (await StoreService.SaveProductType(new ProductType { Name = NewProductType }) > 0)
                {
                    logger.Info("Purchase type {0} saved", NewProductType);
                    Status.Post("Тип покупки \"{0}\" збережено", NewProductType);
                }
                else
                {
                    logger.Warn("Purchase type {0} not saved", NewProductType);
                    Status.Post("Помилка: тип покупки \"{0}\" не збережено", NewProductType);
                }
            });
            
        }

        private bool CanRenameType(object obj)
        {
            return ProductType != null && !string.IsNullOrWhiteSpace(NewProductType);
        }

        private void RenameTypeCommandExecute(object obj)
        {
            NewProductTypeEditable = false;
            Task.Factory.StartNew(async () =>
            {
                if (await StoreService.RenameProductType(ProductType, NewProductType) > 0)
                {
                    logger.Info("Purchase type {0} renamed", NewProductType);
                    Status.Post("Тип покупки \"{0}\" перейменовано", NewProductType);
                }
                else
                {
                    logger.Warn("Purchase type {0} not renamed", NewProductType);
                    Status.Post("Помилка: тип покупки \"{0}\" не перейменовано", NewProductType);
                }
                NewProductType = string.Empty;
                NewProductTypeEditable = true;
            });
        }

        private bool CanDeleteType(object obj)
        {
            return ProductType != null;
        }

        private void DeleteTypeCommandExecute(object obj)
        {
            Task.Factory.StartNew(async () => 
            {
                var typeToDeleteName = ProductType.Name;
                if (await StoreService.RemoveProductType(ProductType))
                {
                    logger.Info("Purchase type {0} removed", typeToDeleteName);
                    Status.Post("Тип покупки \"{0}\" видалено", typeToDeleteName);
                }
                else
                {
                    logger.Warn("Purchase type {0} not removed", typeToDeleteName);
                    Status.Post("Помилка: тип покупки \"{0}\" не видалено", typeToDeleteName);
                }
                NewProductType = string.Empty;
                NewProductTypeEditable = true;
            });
        }

        private bool CanAddSubType(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewProductSubType);
        }

        private void AddSubTypeCommandExecute(object obj)
        {
            Task.Factory.StartNew(async () =>
            {
                if (await StoreService.SaveProductSubType(new ProductSubType { Name = NewProductSubType, TypeId = ProductType.Id }) > 0)
                {
                    logger.Info("Purchase sub type {0} saved", NewProductSubType);
                    Status.Post("Підтип покупки \"{0}\" збережено", NewProductSubType);
                }
                else
                {
                    logger.Warn("Purchase Sub type {0} not saved", NewProductSubType);
                    Status.Post("Помилка: підтип покупки \"{0}\" не збережено", NewProductSubType);
                }
            });

        }

        private bool CanRenameSubType(object obj)
        {
            return ProductSubType != null && !string.IsNullOrWhiteSpace(NewProductSubType);
        }

        private void RenameSubTypeCommandExecute(object obj)
        {
            NewProductSubTypeEditable = false;
            Task.Factory.StartNew(async () =>
            {
                ProductSubType.Name = NewProductSubType;
                if (await StoreService.SaveProductSubType(ProductSubType) > 0)
                {
                    logger.Info("Product Sub type {0} renamed", NewProductSubType);
                    Status.Post("Підтип покупки \"{0}\" перейменовано", NewProductSubType);
                }
                else
                {
                    logger.Warn("Product Sub type {0} not renamed", NewProductSubType);
                    Status.Post("Помилка: підтип покупки \"{0}\" не перейменовано", NewProductSubType);
                }
                NewProductSubType = string.Empty;
                NewProductSubTypeEditable = true;
            });
        }

        private bool CanDeleteSubType(object obj)
        {
            return ProductSubType != null;
        }

        private void DeleteSubTypeCommandExecute(object obj)
        {
            Task.Factory.StartNew(async () =>
            {
                var typeToDeleteName = ProductSubType.Name;
                if (await StoreService.RemoveProductSubType(ProductSubType))
                {
                    logger.Info("Purchase Sub type {0} removed", typeToDeleteName);
                    Status.Post("Підтип покупки \"{0}\" видалено", typeToDeleteName);
                }
                else
                {
                    logger.Warn("Purchase Sub type {0} not removed", typeToDeleteName);
                    Status.Post("Помилка: підтип покупки \"{0}\" не видалено", typeToDeleteName);
                }
                NewProductSubType = string.Empty;
                NewProductSubTypeEditable = true;
            });
        }

        private void SelectPathCommandExecute(object obj)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select existing installation path";
            dlg.IsFolderPicker = true;
            dlg.EnsurePathExists = true;
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ExistingPath = dlg.FileName;
            }
        }

        //private async void ImportDataCommandExecute(object obj)
        //{
        //    Status.StartProgress();
        //    await Migrator.MigrateFromCsv(ExistingPath, DataMigrationStatusUpdated).ContinueWith(t => DataMigrationFinished(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        //}

        #endregion
        void StoreService_SubTypesUpdated(object sender, EventArgs e)
        {
            LoadSubTypes(ProductType.Id);
        }

        private void DataMigrationStatusUpdated(MigrationResultArgs e)
        {
            Status.UpdateProgress(e.Percent);
            Status.Post("Виконується міграцію даних. Записів зчитано: {0}", e.Processed);
        }
        private void DataMigrationFinished(MigrationResult result)
        {
            Status.StopProgress();
            Status.Post("Міграцію даних виконано. Записів перенесено: {0}, помилок: {1}", result.SucceededLines, result.FailedLines);
        }
        private bool CanImportData(object obj)
        {
            if (ExistingPath == null)
            {
                return false;
            }
            return FileUtilities.FileExists(Path.Combine(ExistingPath, FileUtilities.HOMEEX_DATA_FILE));
        }

        private string existingPath;
        public string ExistingPath
        {
            get
            {
                return existingPath;
            }
            set
            {
                if (existingPath != value)
                {
                    existingPath = value;
                    OnPropertyChanged(() => ExistingPath);
                }
            }
        }
        private string newProductType;
        public string NewProductType
        {
            get
            {
                return newProductType;
            }
            set
            {
                if (newProductType != value)
                {
                    newProductType = value;
                    OnPropertyChanged(() => NewProductType);
                }
            }
        }

        private bool newProductTypeEditable;
        public bool NewProductTypeEditable
        {
            get
            {
                return newProductTypeEditable;
            }
            set
            {
                if (newProductTypeEditable != value)
                {
                    newProductTypeEditable = value;
                    OnPropertyChanged(() => NewProductTypeEditable);
                }
            }
        }

        private ProductType productType;
        public ProductType ProductType
        {
            get
            {
                return productType;
            }
            set
            {
                ProductType type;
                if (value != null)
                {
                    type = TypeSelectorItems.Where(e => e.Name == value.Name).FirstOrDefault();
                    NewProductSubTypeEditable = true;
                }
                else
                {
                    type = TypeSelectorItems.FirstOrDefault();
                    NewProductSubTypeEditable = false;
                }
                if (type != productType)
                {
                    productType = type;
                    LoadSubTypes(type.Id);
                    OnPropertyChanged(() => ProductType);
                }
            }
        }

        private string newProductSubType;
        public string NewProductSubType
        {
            get
            {
                return newProductSubType;
            }
            set
            {
                if (newProductSubType != value)
                {
                    newProductSubType = value;
                    OnPropertyChanged(() => NewProductSubType);
                }
            }
        }

        private bool newProductSubTypeEditable;
        public bool NewProductSubTypeEditable
        {
            get
            {
                return newProductSubTypeEditable;
            }
            set
            {
                if (newProductSubTypeEditable != value)
                {
                    newProductSubTypeEditable = value;
                    OnPropertyChanged(() => NewProductSubTypeEditable);
                }
            }
        }

        private bool productSubTypeSelectable;
        public bool ProductSubTypeSelectable
        {
            get
            {
                return productSubTypeSelectable;
            }
            set
            {
                if (productSubTypeSelectable != value)
                {
                    productSubTypeSelectable = value;
                    OnPropertyChanged(() => ProductSubTypeSelectable);
                }
            }
        }

        private ProductSubType purchaseSubType;
        public ProductSubType ProductSubType
        {
            get
            {
                return purchaseSubType;
            }
            set
            {
                ProductSubType subType;
                if (value != null)
                {
                    subType = ProductSubTypes.Where(e => e.Name == value.Name).FirstOrDefault();
                }
                else
                {
                    subType = ProductSubTypes.FirstOrDefault();
                }
                if (subType != purchaseSubType)
                {
                    purchaseSubType = subType;
                    OnPropertyChanged(() => ProductSubType);
                }
            }
        }
    }
}
