using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Othello.Main.Enum;

namespace Othello.Main.Converters
{
    public class CellStateToColorConverter : IValueConverter
    {
        const int Reverse = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (CellStateEnum)value;
            var color = Color.Transparent;

            if (state == CellStateEnum.Black)
                color = Color.Black;
            else if (state == CellStateEnum.White)
                color = Color.White;

            if (parameter as string == "reverse" && color != Color.Transparent)
                color = color == Color.Black ? Color.White : Color.Black;

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
