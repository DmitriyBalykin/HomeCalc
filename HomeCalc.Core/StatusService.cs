using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core
{
    public class StatusService
    {
        private static StatusService instance;
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public void Post(string msg, params object[] args)
        {
            if (StatusChanged != null)
            {
                StatusChanged.Invoke(null, new StatusChangedEventArgs { Status = string.Format(msg, args) });
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
