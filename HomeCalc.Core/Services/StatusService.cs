using HomeCalc.Core.Services.Messages;
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

        MessageDispatcher MsgDispatcher;

        private StatusService()
        {
            MsgDispatcher = MessageDispatcher.GetInstance();
        }

        public void Post(string msg, params object[] args)
        {
            status = string.Format(msg, args);

            MsgDispatcher.Post(new Message { MessageType = MessageType.STATUS_CHANGED, Data = status });
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

            MsgDispatcher.Post(MessageType.PROGRESS_STARTED);
        }
        public void StopProgress()
        {
            MsgDispatcher.Post(MessageType.PROGRESS_FINISHED);
        }

        public void UpdateProgress(int progress)
        {
            MsgDispatcher.Post(new Message { MessageType = MessageType.PROGRESS_UPDATED, Data = progress });
        }

        public string GetStatus()
        {
            return status;
        }
    }
}
