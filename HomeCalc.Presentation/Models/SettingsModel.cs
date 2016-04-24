using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class SettingsModel
    {
        public int SettingId { get; set; }
        public string SettingName { get; set; }
        public string SettingStringValue { get; set; }
        public bool SettingBoolValue { get; set; }
    }
}
