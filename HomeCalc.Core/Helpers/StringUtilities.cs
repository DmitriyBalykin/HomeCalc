using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Helpers
{
    public static class StringUtilities
    {
        public static string TrimEnd(this string str, string toTrim)
        {
            if (str.EndsWith(toTrim, true, CultureInfo.InvariantCulture))
            {
                str = str.Substring(0, str.Length - toTrim.Length - 1);
            }
            return str;
        }
    }
}
