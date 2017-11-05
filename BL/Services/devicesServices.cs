
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System;
namespace myCollections.BL.Services
{
    class DevicesServices
    {
        public static MediaPlayerDevices GetDune()
        {
            MediaPlayerDevices mediaPlayerDevices = new MediaPlayerDevices();
            mediaPlayerDevices.Name = SupportedDevice.Dune;
            mediaPlayerDevices.BGFileName = "background.aai";
            mediaPlayerDevices.BGHeight = 1080;
            mediaPlayerDevices.BGWidth = 1920;
            mediaPlayerDevices.BGRatio = 1.5;

            mediaPlayerDevices.FolderFileName = "cover.aai";
            mediaPlayerDevices.FolderWidth = 220;
            mediaPlayerDevices.FolderHeight = 320;


            mediaPlayerDevices.XmlFileName = "dune_folder.txt";

            return mediaPlayerDevices;
        }
        public static MediaPlayerDevices GetMyMovies()
        {
            MediaPlayerDevices mediaPlayerDevices = new MediaPlayerDevices();
            mediaPlayerDevices.Name = SupportedDevice.MyMovies;
            mediaPlayerDevices.BGFileName = string.Empty;
            mediaPlayerDevices.BGHeight = 0;
            mediaPlayerDevices.BGWidth = 0;
            mediaPlayerDevices.BGRatio = 1;

            mediaPlayerDevices.FolderFileName = "folder.jpg";
            mediaPlayerDevices.FolderWidth = 300;
            mediaPlayerDevices.FolderHeight = 427;

            mediaPlayerDevices.Folder2FileName = "folder.original.jpg";
            mediaPlayerDevices.Folder2Width = 800;
            mediaPlayerDevices.Folder2Height = 1138;

            mediaPlayerDevices.XmlFileName = "mymovies.xml";

            return mediaPlayerDevices;
        }
        public static MediaPlayerDevices GetTvix()
        {
            MediaPlayerDevices mediaPlayerDevices = new MediaPlayerDevices();
            mediaPlayerDevices.Name = SupportedDevice.Tvix;
            mediaPlayerDevices.BGFileName = "tvix.jpg";
            mediaPlayerDevices.BGHeight = 720;
            mediaPlayerDevices.BGWidth = 1280;
            mediaPlayerDevices.BGRatio = 1;

            mediaPlayerDevices.FolderFileName = "folder.jpg";
            mediaPlayerDevices.FolderWidth = 138;
            mediaPlayerDevices.FolderHeight = 186;
            return mediaPlayerDevices;
        }

        public static MediaPlayerDevices GetWindowsMediaCenter()
        {
            MediaPlayerDevices mediaPlayerDevices = new MediaPlayerDevices();
            mediaPlayerDevices.Name = SupportedDevice.WindowsMediaCenter;

            mediaPlayerDevices.BGFileName = "backdrop.jpg";
            mediaPlayerDevices.BGHeight = 1080;
            mediaPlayerDevices.BGWidth = 1920;
            mediaPlayerDevices.BGRatio = 1;

            mediaPlayerDevices.FolderFileName = "folder.jpg";
            mediaPlayerDevices.FolderWidth = 300;
            mediaPlayerDevices.FolderHeight = 450;

            mediaPlayerDevices.Folder2FileName = "folder.original.jpg";
            mediaPlayerDevices.Folder2Width = 1000;
            mediaPlayerDevices.Folder2Height = 1500;

            mediaPlayerDevices.XmlFileName = ".dvdid.xml";

            return mediaPlayerDevices;
        }

        public static MediaPlayerDevices GetMede8Er()
        {
            MediaPlayerDevices mediaPlayerDevices = new MediaPlayerDevices();
            mediaPlayerDevices.Name = SupportedDevice.Mede8er;
            mediaPlayerDevices.BGFileName = "about.jpg";
            mediaPlayerDevices.BGHeight = 1080;
            mediaPlayerDevices.BGWidth = 1920;
            mediaPlayerDevices.BGRatio = 1.5;

            mediaPlayerDevices.FolderFileName = "folder.jpg";
            mediaPlayerDevices.FolderWidth = 147;
            mediaPlayerDevices.FolderHeight = 200;
            return mediaPlayerDevices;
        }

        public static MediaPlayerDevices GetWdhdtv()
        {
            MediaPlayerDevices mediaPlayerDevices = new MediaPlayerDevices();
            mediaPlayerDevices.Name = SupportedDevice.WDHDTV;
            mediaPlayerDevices.BGFileName = "background.jpg";
            mediaPlayerDevices.BGHeight = 720;
            mediaPlayerDevices.BGWidth = 1280;
            mediaPlayerDevices.BGRatio = 1;
            mediaPlayerDevices.FolderFileName = "folder.jpg";
            mediaPlayerDevices.FolderWidth = 147;
            mediaPlayerDevices.FolderHeight = 200;
            return mediaPlayerDevices;
        }

        public static MediaPlayerDevices GetXbmc()
        {
            MediaPlayerDevices mediaPlayerDevices = new MediaPlayerDevices();
            mediaPlayerDevices.BGRatio = 1;
            mediaPlayerDevices.Name = SupportedDevice.XBMC;
            mediaPlayerDevices.XmlFileName = ".nfo";

            mediaPlayerDevices.BGFileName = "fanart.jpg";
            mediaPlayerDevices.BGHeight = 731;
            mediaPlayerDevices.BGWidth = 1300;

            mediaPlayerDevices.FolderWidth = 1000;
            mediaPlayerDevices.FolderHeight = 1500;


            return mediaPlayerDevices;
        }

        public static MediaPlayerDevices GetDevice()
        {
            SupportedDevice supportedDevice = (SupportedDevice)Enum.Parse(typeof(SupportedDevice), MySettings.Device);
            return ReturnDevice(supportedDevice);
        }

        private static MediaPlayerDevices ReturnDevice(SupportedDevice supportedDevice)
        {
            switch (supportedDevice)
            {
                case SupportedDevice.Dune:
                       return GetDune();
                case SupportedDevice.Mede8er:
                       return GetMede8Er();
                case SupportedDevice.Tvix:
                        return GetTvix();
                case SupportedDevice.WindowsMediaCenter:
                        return GetWindowsMediaCenter();
                case SupportedDevice.MyMovies:
                        return GetMyMovies();
                case SupportedDevice.WDHDTV:
                        return GetWdhdtv();
                case SupportedDevice.XBMC:
                        return GetXbmc();
            }
            return null;
        }

        public static MediaPlayerDevices GetDevice(string device)
        {
            SupportedDevice supportedDevice;
            if (string.IsNullOrWhiteSpace(device) == false)
                supportedDevice = (SupportedDevice)Enum.Parse(typeof(SupportedDevice), device);
            else
                supportedDevice = (SupportedDevice)Enum.Parse(typeof(SupportedDevice), MySettings.Device);
            return ReturnDevice(supportedDevice);
        }
    }
}
