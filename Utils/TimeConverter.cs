using System;
using System.Globalization;
using System.Windows.Data;

namespace myCollections.Utils
{
    class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            TimeSpan ts = TimeSpan.FromSeconds((int)value);
            if (ts.Hours <= 0)
                return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            else
                return String.Format("{0}:{1}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string time = value as string;
            if (string.IsNullOrWhiteSpace(time))
                return null;
            else
                return value;
        }
    }
}
