using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Othello.Main.Converters
{
    public class GameStateToReverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is GameStateEnum))
                return null;
            if (!(parameter is GameStateEnum))
                return null;
            var gs = (GameStateEnum)value;
            var compare = (GameStateEnum)parameter;
            return gs != compare;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
