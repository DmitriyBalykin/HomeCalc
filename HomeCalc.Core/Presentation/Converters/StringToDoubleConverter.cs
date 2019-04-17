using HomeCalc.Core.Helpers;
using System;
using System.Windows.Data;

namespace HomeCalc.Core.Presentation.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = String2NumberHelper.GetCorrected(value.ToString(), 2, true);

            return result;
        }
    }
}
