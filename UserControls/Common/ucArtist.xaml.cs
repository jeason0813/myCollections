using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.UserControls.Common
{
    public partial class UcArtist
    {
        public string ItemsId { get; set; }
        private readonly EntityType _entityType;
        private readonly Artist _artist;
        private readonly string _job;

        public static readonly RoutedEvent CmdDeleteClickEvent = EventManager.RegisterRoutedEvent("cmdDeleteClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcArtist));
        public static readonly RoutedEvent CmdChangeClickEvent = EventManager.RegisterRoutedEvent("cmdChangeClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcArtist));
        public static readonly RoutedEvent CmdUpdateClickEvent = EventManager.RegisterRoutedEvent("cmdUpdateClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcArtist));
        public static readonly RoutedEvent CmdRefreshClickEvent = EventManager.RegisterRoutedEvent("cmdRefreshClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcArtist));
        #region Constructor

        public UcArtist(EntityType entityType, Artist artist, string job)
        {
            InitializeComponent();
            _entityType = entityType;
            _artist = artist;
            _job = job;
            Bind();
        }

        #endregion
        private void Bind()
        {
            BitmapImage objImage = new BitmapImage();
            objImage.BeginInit();
            objImage.UriSource = new Uri("pack://application:,,,/myCollections;component/Images/elvis64.png");
            objImage.DecodePixelHeight = 50;
            objImage.EndInit();
            objImage.Freeze();

            BitmapImage director = new BitmapImage();
            director.BeginInit();
            director.UriSource = new Uri("pack://application:,,,/myCollections;component/Images/director.png");
            director.DecodePixelHeight = 50;
            director.EndInit();
            director.Freeze();

            if (_artist.Picture != null)
            {
                Image picture = new Image();
                picture.Height = 300;
                picture.Source = Util.CreateImage(_artist.Picture);
                imgPicture.Source = picture.Source;
                ToolTipService.SetToolTip(imgPicture, picture);
            }
            else
                imgPicture.Source = objImage;

            txtName.Text = _artist.FirstName + " " + _artist.LastName;
            ItemsId = _artist.Id;

            if (_job == "Director")
                imgDirector.Source = director;
        }
        private void cmdDetails_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            Task.Factory.StartNew(() => Util.NotifyEvent("ArtistDetail:" + name));
            bool? dialogResults = new ArtistDetail(_artist, _entityType).ShowDialog();
            if (dialogResults == true)
            {
                RoutedEventArgs args = new RoutedEventArgs(CmdRefreshClickEvent);
                RaiseEvent(args);
            }
        }
        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdDeleteClickEvent);
            RaiseEvent(args);

        }
        private void cmdChange_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdChangeClickEvent);
            RaiseEvent(args);
        }

        private void cmdUpdate_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(CmdUpdateClickEvent);
            RaiseEvent(args);
        }
    }
}