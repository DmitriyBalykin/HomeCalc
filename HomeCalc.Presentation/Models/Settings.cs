using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class Settings
    {
        public int SettingsProdileId { get; set; }
        public bool AutoWindowSize { get; set; }
        public bool AutoWindowPosition { get; set; }
        public bool ClearFieldsOnSave { get; set; }
        public bool ResetCalculation { get; set; }
        public string BackupPath { get; set; }
    }
}
