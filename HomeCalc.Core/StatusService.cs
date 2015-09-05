using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core
{
    public class StatusService
    {
        private StatusService ()
        {
            Status = "Загружено";
        }
        private static StatusService instance;
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        private string status;
        public string Status {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    status = value;
                    if (StatusChanged != null)
                    {
                        StatusChanged.Invoke(null, new StatusChangedEventArgs { Status = status });
                    }
                }
            }
        }
        public static StatusService GetInstance()
        {
            if (instance == null)
            {
                instance = new StatusService();
            }
            return instance;
        }
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public string Status { get; set; }
    }
}
