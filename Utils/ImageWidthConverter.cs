﻿using System;
using System.Windows.Data;

namespace myCollections.Utils
{
    [ValueConversion(typeof(double), typeof(double))]
    [ValueConversion(typeof(double), typeof(double))]
    class ImageWidthConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == null)
                return 100;
            else if ((double)value <= 0)
                return 100;
            else
            {
                double Height = (double)value ;
                return Height / 1.33;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
