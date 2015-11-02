﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core
{
    public class StatusService
    {
        private static StatusService instance;

        private string status;
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        public event EventHandler<ProgressUpdatedEventArgs> ProgressUpdated;

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

        public void ResetProgressBar()
        {
            throw new NotImplementedException();
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
