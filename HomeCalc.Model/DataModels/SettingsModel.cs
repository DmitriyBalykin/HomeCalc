using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Model.DataModels
{
    public class SettingsModel
    {
        public int SettingsProfileId { get; set; }
        public int AutoWindowSize { get; set; }
        public int AutoWindowPosition { get; set; }
        public int ClearFieldsOnSave { get; set; }
        public int ResetCalculation { get; set; }
        public string BackupPath { get; set; }
    }
}
