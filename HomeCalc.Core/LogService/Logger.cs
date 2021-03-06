﻿using HomeCalc.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.LogService
{
    public class Logger
    {
        NLog.Logger logger;

        public bool SendEmail { get; set; }

        public Logger(string name)
        {
            logger = LogManager.GetLogger(name);
        }
        public void Debug(string message, params object[] args)
        {
#if DEBUG
            logger.Debug(message, args);
#endif
        }
        public void Info(string message, params object[] args)
        {
            logger.Info(message, args);
            Console.WriteLine(string.Format(message, args));
        }
        public void Warn(string message, params object[] args)
        {
            logger.Warn(message, args);
            Console.WriteLine(string.Format("Warning: " + message, args));
        }
        public void Error(string message, params object[] args)
        {
            logger.Error(message, args);
            Console.WriteLine(string.Format("Error: " + message, args));

            if (SendEmail)
            {
                EmailSender.SendLogFile();
            }
        }
    }
}
