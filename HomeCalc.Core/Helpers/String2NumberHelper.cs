using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Helpers
{
    public static class String2NumberHelper
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
            bool parsed = double.TryParse(str, out result);
            return parsed && !double.IsNaN(result) && !double.IsInfinity(result);
        }
        public static string GetCorrected(string str, int precision = 6, bool inverse = false)
        {
            if (str == null)
            {
                return null;
            }
            int correcterValue = precision + 1;

            char correctComa, incorrectComa;
            if (inverse)
            {
                // used for DataGrid control
                correctComa = INCORRECT_COMA;
                incorrectComa = CORRECT_COMA;
            }
            else
            {
                correctComa = CORRECT_COMA;
                incorrectComa = INCORRECT_COMA;
            }

            str = str.Replace(incorrectComa, correctComa);
            int dividerPlace = str.IndexOf(correctComa);
            if (dividerPlace > -1 && (dividerPlace + correcterValue) < str.Length)
            {
                str = str.Substring(0, (dividerPlace + correcterValue));
            }

            str = str.Replace(incorrectComa, correctComa);
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
