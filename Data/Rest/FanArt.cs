using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Navigation;
using Newtonsoft.Json.Linq;

namespace myCollections.Data.Rest
{
    class FanArt
    {
        public string ArtistId { get; set; }
        public string ArtistBackground { get; set; }
        public string ArtistCover { get; set; }
        public string ArtistName { get; set; }
        public string AlbumId { get; set; }
        public string Albumcover { get; set; }
        public string Cdart { get; set; }
        public string HdMusicLogo { get; set; }
        public string MusicLogo { get; set; }

        public static FanArt AlbumToObject(JObject objRest)
        {
            FanArt objItem = null;
            if (objRest != null)
            {
                objItem = new FanArt();
                string artistName = objRest.Properties().First().Name;
                objItem.ArtistName = artistName;
                objItem.ArtistId = (string)objRest[artistName]["mbid_id"];
                objItem.AlbumId=((JObject)objRest[artistName]["albums"]).Properties().First().Name;

                JArray covers = (JArray)objRest[artistName]["albums"][objItem.AlbumId]["albumcover"];
                if (covers.HasValues == true)
                    objItem.Albumcover = (string)covers[0]["url"];

                JArray cdsArt = (JArray)objRest[artistName]["albums"][objItem.AlbumId]["cdart"];
                if (cdsArt.HasValues == true)
                    objItem.Cdart = (string)cdsArt[0]["url"];
            }
            return objItem;
        }
        public static void ArtistToObject(JObject objRest,FanArt objItem)
        {
            if (objRest != null)
            {
                string artistName = objRest.Properties().First().Name;
                objItem.ArtistName = artistName;
                objItem.ArtistId = (string)objRest[artistName]["mbid_id"];
                objItem.AlbumId = ((JObject)objRest[artistName]["albums"]).Properties().First().Name;

                JArray covers = (JArray)objRest[artistName]["albums"][objItem.AlbumId]["albumcover"];
                if (covers.HasValues == true)
                    objItem.Albumcover = (string)covers[0]["url"];

                JArray cdsArt = (JArray)objRest[artistName]["albums"][objItem.AlbumId]["cdart"];
                if (cdsArt.HasValues == true)
                    objItem.Cdart = (string)cdsArt[0]["url"];
            }
        }
        public static FanArt ArtistToObject(JObject objRest, string albumID)
        {
            FanArt objItem = null;
            if (objRest != null)
            {
                objItem=new FanArt();
                string artistName = objRest.Properties().First().Name;
                objItem.AlbumId = albumID;
                objItem.ArtistName = artistName;
                objItem.ArtistId = (string)objRest[artistName]["mbid_id"];

                JArray artistBackground = (JArray)objRest[artistName]["artistbackground"];
                if (artistBackground !=null && artistBackground.HasValues == true)
                    objItem.ArtistBackground = (string)artistBackground[0]["url"];

                JArray artistCover = (JArray)objRest[artistName]["artistthumb"];
                if (artistCover !=null && artistCover.HasValues == true)
                    objItem.ArtistCover = (string)artistCover[0]["url"];

                JArray covers = (JArray)objRest[artistName]["albums"][objItem.AlbumId]["albumcover"];
                if (covers !=null && covers.HasValues == true)
                    objItem.Albumcover = (string)covers[0]["url"];

                JArray cdsArt = (JArray)objRest[artistName]["albums"][objItem.AlbumId]["cdart"];
                if (cdsArt !=null && cdsArt.HasValues == true)
                    objItem.Cdart = (string)cdsArt[0]["url"];

                JArray musiclogo = (JArray)objRest[artistName]["musiclogo"];
                if (musiclogo != null && musiclogo.HasValues == true)
                    objItem.MusicLogo = (string)musiclogo[0]["url"];

                JArray hdmusiclogo = (JArray)objRest[artistName]["hdmusiclogo"];
                if (hdmusiclogo !=null && hdmusiclogo.HasValues == true)
                    objItem.HdMusicLogo = (string)hdmusiclogo[0]["url"];
            }

            return objItem;
        } 

    }
}
