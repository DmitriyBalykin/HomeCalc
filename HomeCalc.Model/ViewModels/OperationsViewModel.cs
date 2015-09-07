using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Core.Utilities;
using HomeCalc.Model.BasicModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.ViewModels
{
    public class OperationsViewModel : ViewModel
    {
        public OperationsViewModel()
        {
            logger = LogService.GetLogger();

            AddCommand("SelectPath", new DelegateCommand(SelectPathCommandExecute));
            AddCommand("ImportData", new DelegateCommand(ImportDataCommandExecute, CanImportData));
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
            bool result = FileUtilities.FileExists(ExistingPath);
            logger.Debug("CanImport result for path: {0} , result: {1}", ExistingPath, result);
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
    }
}
