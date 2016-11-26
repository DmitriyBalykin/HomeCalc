using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Services.Messages
{
    public class Message
    {
        public MessageType MessageType { get; set; }

        public object Data { get; set; }
    }
}
