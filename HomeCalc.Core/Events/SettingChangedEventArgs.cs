using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Events
{
    public class SettingChangedEventArgs : EventArgs
    {
        public string SettingName { get; set; }
    }
}
