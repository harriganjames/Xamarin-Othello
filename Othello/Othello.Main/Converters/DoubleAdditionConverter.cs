using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Othello.Main.Enum;

namespace Othello.Main.Converters
{
    public class DoubleAdditionConverter : IValueConverter
    {
        const int Reverse = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = (double)value;
            double p = 0;
            if (parameter.GetType() == typeof(string))
                double.TryParse((string)parameter, out p);
            else
                p = (double)parameter;

            return d + p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
