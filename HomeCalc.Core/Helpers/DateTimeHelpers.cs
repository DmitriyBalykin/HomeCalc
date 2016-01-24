using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Helpers
{
    public static class DateTimeHelpers
    {
        public static DateTime Round(this DateTime dateTime, TimeSpan roundInterval)
        {
            var delta = dateTime.Ticks % roundInterval.Ticks;

            return new DateTime(dateTime.Ticks - delta, dateTime.Kind);
        }
    }
}
