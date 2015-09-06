using HomeCalc.Model.BasicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.ViewModels
{
    public class SettingsModel : ViewModel
    {
        public SettingsModel()
        {
            //Add command Save
            //Add command Cancel
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
