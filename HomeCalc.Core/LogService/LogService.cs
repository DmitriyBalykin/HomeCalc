using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.LogService
{
    public class LogService
    {
        public static Logger GetLogger([CallerMemberName] string name = null, bool sendEmail = false)
        {
            if (name == null)
            {
                return null;
            }
            return new Logger(name);
        }
    }
}
