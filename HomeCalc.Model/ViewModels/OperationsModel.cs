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
    public class OperationsModel : ViewModel
    {
        public OperationsModel()
        {
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
            return FileUtilities.FileExists(ExistingPath);
        }

        public string ExistingPath { get; set; }
    }
}
