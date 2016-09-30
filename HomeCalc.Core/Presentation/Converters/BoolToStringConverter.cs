using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace HomeCalc.Core.Presentation.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var values = parameter.ToString().Split('/');
            if (values.Length != 2)
            {
                throw new ArgumentException("Incorrect converter parameter");
            }
            var isTrue = (bool)value;
            return isTrue ? values[0] : values[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
