using HomeCalc.Core.LogService;
using HomeCalc.Core.Services.Messages;
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

        private HashSet<Action<Message>> handlers;

        private MessageDispatcher()
        {
            logger = new Logger(this.GetType().ToString());
            handlers = new HashSet<Action<Message>>();
        }

        public static MessageDispatcher GetInstance()
        {
            if (instance == null)
            {
                lock (monitor)
                {
                    if (instance == null)
                    {
                        instance = new MessageDispatcher();
                    }
                }
            }
            return instance;
        }

        public void AddHandler(Action<Message> handler)
        {
            handlers.Add(handler);
            logger.Info("Added handler");
        }

        public void RemoveHandler(Action<Message> handler)
        {
            handlers.Remove(handler);
        }
        public void Post(Message message)
        {
            logger.Info("Posting message: " + message.MessageType.ToString());

            foreach (var handler in handlers)
            {
                handler(message);
            }
        }

        public void Post(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.SUBTYPES_UPDATED:
                case MessageType.STATUS_CHANGED:
                case MessageType.PROGRESS_UPDATED:
                case MessageType.UPDATES_AVAILABLE:
                    logger.Error("Message of type {0} called without mandatory data", messageType.ToString());
                    break;

                default:
                    break;
            }
            this.Post(new Message { MessageType = messageType });
        }

    }
}
