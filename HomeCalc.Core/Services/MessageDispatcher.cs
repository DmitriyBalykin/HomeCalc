using HomeCalc.Core.LogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Services
{
    
    public class MessageDispatcher
    {
        private static object monitor = new object();
        private Logger logger;
        private static MessageDispatcher instance;

        private HashSet<Action<string>> handlers;
        private MessageDispatcher()
        {
            logger = new Logger(this.GetType().ToString());
            handlers = new HashSet<Action<string>>();
        }

        public static MessageDispatcher GetInstance()
        {
            lock (monitor)
            {
                if (instance == null)
                {
                    instance = new MessageDispatcher();
                }
            }
            return instance;
        }

        public void AddHandler(Action<string> handler)
        {
            handlers.Add(handler);
            logger.Info("Added handler");
        }
        public void RemoveHandler(Action<string> handler)
        {
            handlers.Remove(handler);
        }
        public void Post(string message)
        {
            logger.Info("Posting message: "+message);
            foreach (var handler in handlers)
            {
                handler(message);
            }
        }

    }
}
