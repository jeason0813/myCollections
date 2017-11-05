using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using myCollections.BL.Services;

namespace myCollections.UserControls.Common
{
  
    public partial class UcCastToolbar
    {
        public static readonly RoutedEvent CmdAddArtistClickEvent = EventManager.RegisterRoutedEvent("cmdAddArtistClick",RoutingStrategy.Bubble, typeof(RoutedEventHandler),typeof(UcCastToolbar));

        public string FullName { get; set; }

        public UcCastToolbar()
        {
            InitializeComponent();
        }

        private void cmdAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cboName.DataContext.GetType() != typeof(List<string>))
                cboName.DataContext = ArtistServices.GetFullNames();

            if (addPanel.Visibility == Visibility.Collapsed)
            {
                addPanel.Visibility = Visibility.Visible;
                cboName.Text = string.Empty;
                imgAdd.Source = new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Delete2.png"));
            }
            else
            {
                addPanel.Visibility = Visibility.Collapsed;
                imgAdd.Source = new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/Add.png"));
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            FullName = cboName.Text;
            RoutedEventArgs args = new RoutedEventArgs(CmdAddArtistClickEvent);
            RaiseEvent(args);
        }
    }
}
