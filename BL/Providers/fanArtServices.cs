using System;
using System.Collections;
using System.Globalization;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using Newtonsoft.Json.Linq;

namespace myCollections.BL.Providers
{
    class FanArtServices
    {
        private const string key = "f936c953fba4c0ea8b0836d40e8809e1";

        public static Hashtable ParseAlbum(string id)
        {
            Hashtable objResuls = new Hashtable();
            try
            {
                Uri strUrl = new Uri(string.Format(@"http://api.fanart.tv/webservice/album/{0}/{1}/{2}/{3}/{4}/{5}/", key, id, "JSON", "all", 1, 1));
                string results = Util.GetRest(strUrl);
                if (string.IsNullOrWhiteSpace(results) == false)
                {
                    JObject restResponse = JObject.Parse(results);
                    FanArt album = FanArt.AlbumToObject(restResponse);
                    if (string.IsNullOrWhiteSpace(album.ArtistId) == false)
                       ParseArtist(album);

                    //   if (string.IsNullOrWhiteSpace(musicbrainz.GroupID) == false)
                }
                return objResuls;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, id);
                return null;
            }
        }
        public static Hashtable ParseArtist(string id)
        {
            Hashtable objResuls = new Hashtable();
            try
            {
                Uri strUrl = new Uri(string.Format(@"http://api.fanart.tv/webservice/album/{0}/{1}/{2}/{3}/{4}/{5}/", key, id, "JSON", "all", 1, 1));
                string results = Util.GetRest(strUrl);
                if (string.IsNullOrWhiteSpace(results) == false)
                {
                    JObject restResponse = JObject.Parse(results);
                    FanArt album = FanArt.AlbumToObject(restResponse);
                    if (string.IsNullOrWhiteSpace(album.ArtistId) == false)
                        ParseArtist(album);

                    //   if (string.IsNullOrWhiteSpace(musicbrainz.GroupID) == false)
                }
                return objResuls;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, id);
                return null;
            }
        }
        public static FanArt ParseArtist(string id, string albumId)
        {
            FanArt fanArt = null;
            try
            {
                Uri strUrl = new Uri(string.Format(@"http://api.fanart.tv/webservice/artist/{0}/{1}/{2}/{3}/{4}/{5}/", key, id, "JSON", "all", 1, 1));
                string results = Util.GetRest(strUrl);
                if (string.IsNullOrWhiteSpace(results) == false)
                {
                    JObject restResponse = JObject.Parse(results);
                    fanArt = FanArt.ArtistToObject(restResponse, albumId);
                }
                return fanArt;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, id);
                return null;
            }
        }

        public static void ParseArtist(FanArt fanArt)
        {
            try
            {
                Uri strUrl = new Uri(string.Format(@"http://api.fanart.tv/webservice/artist/{0}/{1}/{2}/{3}/{4}/{5}/", key, fanArt.ArtistId, "JSON", "all", 1, 1));
                string results = Util.GetRest(strUrl);
                if (string.IsNullOrWhiteSpace(results) == false)
                {
                    JObject restResponse = JObject.Parse(results);
                    FanArt.ArtistToObject(restResponse,fanArt);
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }

    }
}
