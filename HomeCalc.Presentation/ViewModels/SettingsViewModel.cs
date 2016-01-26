using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
using HomeCalc.Model.DataModels;
using HomeCalc.Presentation.BasicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.ViewModels
{
    public class SettingsViewModel : ViewModel
    {
        public SettingsViewModel()
        {
            logger = LogService.GetLogger();
            AddCommand("Save", new DelegateCommand(SaveCommandExecute));
        }

        private void SaveCommandExecute(object obj)
        {
            StoreService.SaveSettings(new SettingsModel
            {
                AutoWindowPosition = AutoWindowPosition,
                AutoWindowSize = AutoWindowSize,
                ClearFieldsOnSave = ClearFieldsOnSave,
                ResetCalculation = ResetCalculation
            });
        }
        private bool autoWindowSize;
        public bool AutoWindowSize
        {
            get { return autoWindowSize; }
            set
            {
                if (value != autoWindowSize)
                {
                    autoWindowSize = value;
                    OnPropertyChanged(() => AutoWindowSize);
                }
            }
        }
        private bool autoWindowPosition;
        public bool AutoWindowPosition
        {
            get { return autoWindowPosition; }
            set
            {
                if (value != autoWindowPosition)
                {
                    autoWindowPosition = value;
                    OnPropertyChanged(() => AutoWindowPosition);
                }
            }
        }
        private bool clearFieldsOnSave;
        public bool ClearFieldsOnSave
        {
            get { return clearFieldsOnSave; }
            set
            {
                if (value != clearFieldsOnSave)
                {
                    clearFieldsOnSave = value;
                    OnPropertyChanged(() => ClearFieldsOnSave);
                }
            }
        }
        private bool resetCalculation;
        public bool ResetCalculation
        {
            get { return resetCalculation; }
            set
            {
                if (value != resetCalculation)
                {
                    resetCalculation = value;
                    OnPropertyChanged(() => ResetCalculation);
                }
            }
        }
    }
}
