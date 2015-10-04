using HomeCalc.Core.LogService;
using HomeCalc.Core.Presentation;
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
            
        }

        public string NetAddress { get; set; }
        public string NetPort { get; set; }
        public string DBName { get; set; }
        public string DBTable { get; set; }
        public string UserName { get; set; }
        public SecureString SecurePassword {
            get
            {
                return new SecureString();
            }
            set
            {
                if (value != null)
                {
                    
                }
            }
        }
    }
}
