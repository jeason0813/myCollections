using System;
using System.Windows.Data;

namespace myCollections.Utils
{
    [ValueConversion(typeof(double), typeof(double))]
    [ValueConversion(typeof(double), typeof(double))]
    class DetailsPaneHeightConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {

            return (double)value - 15;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
