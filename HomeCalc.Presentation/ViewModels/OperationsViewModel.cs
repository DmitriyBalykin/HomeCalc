using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Core.Utilities;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.Models;
using HomeCalc.Presentation.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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

            NewPurchaseTypeEditable = true;

        }
        #region commands
        private bool CanAddType(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewPurchaseType);
        }

        private void AddTypeCommandExecute(object obj)
        {
            Task.Factory.StartNew(async () => 
            {
                if (await StoreService.SavePurchaseType(new PurchaseType { Name = NewPurchaseType }))
                {
                    logger.Info("Purchase type {0} saved", NewPurchaseType);
                    Status.Post("Тип покупки \"{0}\" збережено", NewPurchaseType);
                }
                else
                {
                    logger.Warn("Purchase type {0} not saved", NewPurchaseType);
                    Status.Post("Помилка: тип покупки \"{0}\" не збережено", NewPurchaseType);
                }
            });
            
        }

        private bool CanRenameType(object obj)
        {
            return PurchaseType != null && !string.IsNullOrWhiteSpace(NewPurchaseType);
        }

        private void RenameTypeCommandExecute(object obj)
        {
            NewPurchaseTypeEditable = false;
            Task.Factory.StartNew(async () =>
            {
                if (await StoreService.RenamePurchaseType(PurchaseType, NewPurchaseType))
                {
                    logger.Info("Purchase type {0} renamed", NewPurchaseType);
                    Status.Post("Тип покупки \"{0}\" перейменовано", NewPurchaseType);
                }
                else
                {
                    logger.Warn("Purchase type {0} not renamed", NewPurchaseType);
                    Status.Post("Помилка: тип покупки \"{0}\" не перейменовано", NewPurchaseType);
                }
                NewPurchaseType = string.Empty;
                NewPurchaseTypeEditable = true;
            });
        }

        private bool CanDeleteType(object obj)
        {
            return PurchaseType != null;
        }

        private void DeleteTypeCommandExecute(object obj)
        {
            Task.Factory.StartNew(async () => 
            {
                var typeToDeleteName = PurchaseType.Name;
                if (await StoreService.RemovePurchaseType(PurchaseType))
                {
                    logger.Info("Purchase type {0} removed", typeToDeleteName);
                    Status.Post("Тип покупки \"{0}\" видалено", typeToDeleteName);
                }
                else
                {
                    logger.Warn("Purchase type {0} not removed", typeToDeleteName);
                    Status.Post("Помилка: тип покупки \"{0}\" не видалено", typeToDeleteName);
                }
                NewPurchaseType = string.Empty;
                NewPurchaseTypeEditable = true;
            });
        }

        private bool CanAddSubType(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewPurchaseSubType);
        }

        private void AddSubTypeCommandExecute(object obj)
        {
            Task.Factory.StartNew(async () =>
            {
                if (await StoreService.SavePurchaseSubType(new PurchaseSubType { Name = NewPurchaseSubType }))
                {
                    logger.Info("Purchase sub type {0} saved", NewPurchaseSubType);
                    Status.Post("Підтип покупки \"{0}\" збережено", NewPurchaseSubType);
                }
                else
                {
                    logger.Warn("Purchase Sub type {0} not saved", NewPurchaseSubType);
                    Status.Post("Помилка: підтип покупки \"{0}\" не збережено", NewPurchaseSubType);
                }
            });

        }

        private bool CanRenameSubType(object obj)
        {
            return PurchaseSubType != null && !string.IsNullOrWhiteSpace(NewPurchaseSubType);
        }

        private void RenameSubTypeCommandExecute(object obj)
        {
            NewPurchaseSubTypeEditable = false;
            Task.Factory.StartNew(async () =>
            {
                if (await StoreService.RenamePurchaseSubType(PurchaseSubType, NewPurchaseSubType))
                {
                    logger.Info("Purchase Sub type {0} renamed", NewPurchaseSubType);
                    Status.Post("Підтип покупки \"{0}\" перейменовано", NewPurchaseSubType);
                }
                else
                {
                    logger.Warn("Purchase Sub type {0} not renamed", NewPurchaseSubType);
                    Status.Post("Помилка: підтип покупки \"{0}\" не перейменовано", NewPurchaseSubType);
                }
                NewPurchaseSubType = string.Empty;
                NewPurchaseSubTypeEditable = true;
            });
        }

        private bool CanDeleteSubType(object obj)
        {
            return PurchaseSubType != null;
        }

        private void DeleteSubTypeCommandExecute(object obj)
        {
            Task.Factory.StartNew(async () =>
            {
                var typeToDeleteName = PurchaseSubType.Name;
                if (await StoreService.RemovePurchaseSubType(PurchaseSubType))
                {
                    logger.Info("Purchase Sub type {0} removed", typeToDeleteName);
                    Status.Post("Підтип покупки \"{0}\" видалено", typeToDeleteName);
                }
                else
                {
                    logger.Warn("Purchase Sub type {0} not removed", typeToDeleteName);
                    Status.Post("Помилка: підтип покупки \"{0}\" не видалено", typeToDeleteName);
                }
                NewPurchaseSubType = string.Empty;
                NewPurchaseSubTypeEditable = true;
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
        private string newPurchaseType;
        public string NewPurchaseType
        {
            get
            {
                return newPurchaseType;
            }
            set
            {
                if (newPurchaseType != value)
                {
                    newPurchaseType = value;
                    OnPropertyChanged(() => NewPurchaseType);
                }
            }
        }

        private bool newPurchaseTypeEditable;
        public bool NewPurchaseTypeEditable
        {
            get
            {
                return newPurchaseTypeEditable;
            }
            set
            {
                if (newPurchaseTypeEditable != value)
                {
                    newPurchaseTypeEditable = value;
                    OnPropertyChanged(() => NewPurchaseTypeEditable);
                }
            }
        }

        private PurchaseType purchaseType;
        public PurchaseType PurchaseType
        {
            get
            {
                return purchaseType;
            }
            set
            {
                PurchaseType type;
                if (value != null)
                {
                    type = TypeSelectorItems.Where(e => e.Name == value.Name).FirstOrDefault();
                }
                else
                {
                    type = TypeSelectorItems.FirstOrDefault();
                }
                if (type != purchaseType)
                {
                    purchaseType = type;
                    OnPropertyChanged(() => PurchaseType);
                }
            }
        }

        private string newPurchaseSubType;
        public string NewPurchaseSubType
        {
            get
            {
                return newPurchaseSubType;
            }
            set
            {
                if (newPurchaseSubType != value)
                {
                    newPurchaseSubType = value;
                    OnPropertyChanged(() => NewPurchaseSubType);
                }
            }
        }

        private bool newPurchaseSubTypeEditable;
        public bool NewPurchaseSubTypeEditable
        {
            get
            {
                return newPurchaseSubTypeEditable;
            }
            set
            {
                if (newPurchaseSubTypeEditable != value)
                {
                    newPurchaseSubTypeEditable = value;
                    OnPropertyChanged(() => NewPurchaseSubTypeEditable);
                }
            }
        }

        private PurchaseSubType purchaseSubType;
        public PurchaseSubType PurchaseSubType
        {
            get
            {
                return purchaseSubType;
            }
            set
            {
                PurchaseSubType subType;
                if (value != null)
                {
                    subType = PurchaseSubTypes.Where(e => e.Name == value.Name).FirstOrDefault();
                }
                else
                {
                    subType = PurchaseSubTypes.FirstOrDefault();
                }
                if (subType != purchaseSubType)
                {
                    purchaseSubType = subType;
                    OnPropertyChanged(() => PurchaseSubType);
                }
            }
        }
    }
}
