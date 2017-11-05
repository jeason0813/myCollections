using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace myCollections.Utils
{
    [ValueConversion(typeof(Byte[]), typeof(Byte[]))]
    [ValueConversion(typeof(Byte[]), typeof(BitmapImage))]
    class ByteArrayToBitmapImageConverter : IValueConverter
    {

        public object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
               if (value is Byte[])
                {
                    BitmapImage img = new BitmapImage();

                    using (MemoryStream ms = new MemoryStream((Byte[])value))
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.StreamSource = ms;
                        img.EndInit();
                        img.Freeze();
                    }
                    return img;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}