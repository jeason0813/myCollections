
using System;
using System.Windows;
using System.Windows.Data;

namespace myCollections.Utils 
{

    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    public class NotBooleanToVisbilityConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            if (value == null || !(value is Boolean))
                return Visibility.Visible;

            else if (((Boolean)value))
                return Visibility.Collapsed;

            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
