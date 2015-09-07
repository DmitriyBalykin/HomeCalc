using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace HomeCalc.Core.Presentation.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool localBoolValue = (bool)value;

            if (parameter != null)
            {
                bool shouldInverseVisibility;
                if (Boolean.TryParse(parameter.ToString(), out shouldInverseVisibility))
                {
                    localBoolValue = !localBoolValue;
                }
            }
            return localBoolValue ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
