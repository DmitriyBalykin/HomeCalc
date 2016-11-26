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
                str = str.Substring(0, str.Length - toTrim.Length);
            }
            return str;
        }

        public static string EscapeStringForDatabase(string p)
        {
            if (p == null)
            {
                return null;
            }
            var result = p.Replace("'", "''");
            return result;
        }
    }
}
