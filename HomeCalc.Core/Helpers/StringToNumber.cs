using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Helpers
{
    public static class StringHelper
    {
        private const char INCORRECT_COMA = '.';
        private const char CORRECT_COMA = ',';
        public static double ToNumber(string str)
        {
            str = Normalize(str);
            if (str == null)
            {
                return 0;
            }
            return double.Parse(str);
        }
        public static bool IsNumber(string str)
        {
            str = Normalize(str);
            if (str == null)
            {
                return false;
            }
            double result;
            return double.TryParse(str, out result);
        }
        public static string GetCorrected(string str)
        {
            str = str.Replace(INCORRECT_COMA, CORRECT_COMA);
            return str;
        }
        private static string Normalize(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            str = GetCorrected(str);
            if (str.Count( c => c == CORRECT_COMA) > 1)
            {
                return null;
            }
            str = str.TrimEnd(CORRECT_COMA);

            return str;
        }
    }
}
