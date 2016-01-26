using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Core.Utilities;
using HomeCalc.Presentation.BasicModels;
using HomeCalc.Presentation.Models;
using HomeCalc.Presentation.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
            logger = LogService.GetLogger();

            AddCommand("SelectPath", new DelegateCommand(SelectPathCommandExecute));
            AddCommand("ImportData", new DelegateCommand(ImportDataCommandExecute, CanImportData));
            AddCommand("AddType", new DelegateCommand(AddTypeCommandExecute, CanAddType));
        }

        private bool CanAddType(object obj)
        {
            return !string.IsNullOrEmpty(NewPurchaseType);
        }

        private void AddTypeCommandExecute(object obj)
        {
            if (StoreService.SavePurchaseType(new PurchaseType { Name = NewPurchaseType }))
            {
                logger.Info("Purchase type saved");
                Status.Post("Тип покупки \"{0}\" збережено", NewPurchaseType);
            }
            else
            {
                logger.Warn("Purchase type not saved");
                Status.Post("Помилка: тип покупки \"{0}\" не збережено", NewPurchaseType);
            }
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

        private async void ImportDataCommandExecute(object obj)
        {
            Status.StartProgress();
            await Migrator.MigrateFromCsv(ExistingPath, DataMigrationStatusUpdated).ContinueWith(t => DataMigrationFinished(t.Result));
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
    }
}
