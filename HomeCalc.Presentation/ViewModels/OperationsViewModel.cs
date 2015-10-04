using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Core.Utilities;
using HomeCalc.Presentation.BasicModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
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

        private void ImportDataCommandExecute(object obj)
        {
            FileUtilities.ImportDataFromFile(ExistingPath);
        }
        private bool CanImportData(object obj)
        {
            return FileUtilities.FileExists(ExistingPath);
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
