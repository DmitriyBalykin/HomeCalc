using HomeCalc.Core;
using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    public class ReportingCancellationTokenSource : CancellationTokenSource
    {
        StatusService status;
        Logger logger;
        string processDescription;
        public ReportingCancellationTokenSource(string description = null) : base()
        {
            logger = LogService.GetLogger();
            status = StatusService.GetInstance();
            processDescription = description ?? string.Empty;
        }

        public void Cancel(string stateDescription) 
        {
            base.Cancel();
            string message = string.Format("Процес \"{0}\" було перервано через помилку \"{1}\"", processDescription, stateDescription);
            status.StopProgress();
            status.Post(message);
            logger.Error(message);
        }
    }
}
