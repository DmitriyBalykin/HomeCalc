using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HomeCalc.Core.Presentation.Converters
{
    public class UpdateStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool localBoolValue = (bool)value;

            var inputColors = parameter as Brush[];

            Color defaultColor = Color.FromRgb(0xD2, 0xEC, 0xF3);

            if (inputColors == null || inputColors.Length == 0)
            {
                return defaultColor;
            }
            return localBoolValue ? inputColors[0] : inputColors[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
