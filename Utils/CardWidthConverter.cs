using System;
using System.Windows.Data;
namespace myCollections.Utils
{
    [ValueConversion(typeof(double), typeof(double))]
    [ValueConversion(typeof(double), typeof(double))]
    class CardWidthConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null)
                return 350;
            else if ((double)value <= 0)
                return 350;
            else
            {
                double Width = (double)value;
                return Width * 3.5;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
