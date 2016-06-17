using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Services
{
    public class StatusService
    {
        private static StatusService instance;

        private string status;
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;
        public event EventHandler ProgressStarted;
        public event EventHandler ProgressStopped;

        public void Post(string msg, params object[] args)
        {
            status = string.Format(msg, args);
            if (StatusChanged != null)
            {
                StatusChanged.Invoke(null, new StatusChangedEventArgs { Status = status });
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

        public void StartProgress()
        {
            UpdateProgress(0);
            
            if (ProgressStarted != null)
            {
                ProgressStarted(null, EventArgs.Empty);
            }
        }
        public void StopProgress()
        {
            if (ProgressStopped != null)
            {
                ProgressStopped(null, EventArgs.Empty);
            }
        }

        public void UpdateProgress(int progress)
        {
            
            if (ProgressUpdated != null)
            {
                ProgressUpdated(null, new ProgressUpdatedEventArgs { Progress = progress });
            }
        }

        public string GetStatus()
        {
            return status;
        }
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public string Status { get; set; }
    }
    public class ProgressUpdatedEventArgs : EventArgs
    {
        public int Progress { get; set; }
    }
}
