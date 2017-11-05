using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using myCollections.Data;

namespace myCollections.Utils
{
    class ToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tooltip = string.Empty;
            ThumbItem item = value as ThumbItem;
            if (item != null)
            {
                Order currentOrder = ((App)Application.Current).CurrentOrder;
                switch (currentOrder)
                {
                    case Order.Name:
                        tooltip = item.Name;
                        break;
                    case Order.Added:
                        tooltip = string.Format(@"{0} - {1}", item.Name, item.Added.ToShortDateString());
                        break;
                    case Order.Runtime:
                        int? time = item.Runtime;
                        if (time == null)
                            return item.Name;

                        string runtime;

                        TimeSpan ts = TimeSpan.FromSeconds((int)time);
                        if (ts.Hours <= 0)
                            runtime = String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
                        else
                            runtime = String.Format("{0}:{1}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);

                        tooltip = string.Format(@"{0} - {1}", item.Name, runtime);
                        break;
                    case Order.Media:
                        tooltip = string.Format(@"{0} - {1}", item.Name, item.Media);
                        break;
                    case Order.MyRating:
                        tooltip = string.Format(@"{0} - {1}", item.Name, item.MyRating*4);
                        break;
                    case Order.Artist:
                        tooltip = string.Format(@"{0} - {1}", item.Name, item.Artist);
                        break;
                    case Order.Type:
                        tooltip = string.Format(@"{0} - {1}", item.Name, item.Type);
                        break;
                    case Order.PublicRating:
                        if (item.PublicRating != null)
                            tooltip = string.Format(@"{0} - {1}", item.Name, item.PublicRating *4);
                        else
                            return item.Name;
                        break;
                    case Order.NumId:
                        if (item.NumId != null)
                            tooltip = string.Format(@"{0} - {1}", item.Name, item.NumId);
                        else
                            return item.Name;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return tooltip;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack");
        }
    }
}

