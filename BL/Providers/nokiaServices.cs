using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using Newtonsoft.Json.Linq;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Properties;
using myCollections.Utils;

namespace myCollections.BL.Providers
{
    static class NokiaServices
    {
        private const string AppId = "IaRNpRyM0CNREAyCDmby";
        private const string AppToken = "5F3vwPsr-TAbPYfflJeb_w";

        public static Collection<PartialMatche> Search(string strAlbum)
        {
            Uri strUrl = new Uri(string.Format(@"http://api.ent.nokia.com/1.x/us/?domain=music&app_id={0}&app_code={1}&q={2}&category={3}&itemsperpage={4}", AppId, AppToken, Util.EncodeSearch(strAlbum), "album", 10));
            string result = Util.GetRest(strUrl);
            if (string.IsNullOrWhiteSpace(result) == false)
            {
                JObject restResponse = JObject.Parse(result);
                if (restResponse != null)
                    return NokiaMusic.ToPartialMatche(restResponse);
                else
                    return null;
            }
            else
                return null;
        }
        public static Hashtable Parse(string id)
        {
            Hashtable objResuls = new Hashtable();
            try
            {
                Uri strUrl = new Uri(string.Format(@"http://api.ent.nokia.com/1.x/us/products/{0}?domain=music&app_id={1}&app_code={2}", id, AppId, AppToken));
                string results = Util.GetRest(strUrl);
                //FIX 2.7.12.0
                if (string.IsNullOrWhiteSpace(results) == false)
                {
                    JObject restResponse = JObject.Parse(results);
                    NokiaMusic nokiaMusic = NokiaMusic.ToObject(restResponse);

                    objResuls.Add("Links", nokiaMusic.AlbumUrl);
                    objResuls.Add("Title", nokiaMusic.AlbumName);

                    #region Album

                    objResuls.Add("Album", nokiaMusic.AlbumName);

                    #endregion

                    #region Artist

                    Artist artist = ParseArtist(nokiaMusic.ArtistName, nokiaMusic.ArtistId);
                    if (artist != null)
                        objResuls.Add("Artist", artist);

                    #endregion

                    #region Editor

                    if (nokiaMusic.AlbumStudio != null)
                        objResuls.Add("Editor", nokiaMusic.AlbumStudio);

                    #endregion

                    #region Image

                    if (string.IsNullOrEmpty(nokiaMusic.AlbumImage) == false)
                        objResuls.Add("Image", nokiaMusic.AlbumImage);

                    #endregion

                    #region ReleaseDate

                    if (string.IsNullOrWhiteSpace(nokiaMusic.AlbumReleased) == false)
                    {
                        DateTime date;
                        if (DateTime.TryParse(nokiaMusic.AlbumReleased, new CultureInfo("en-US"), DateTimeStyles.None,
                            out date))
                            objResuls.Add("Released", date);
                    }

                    #endregion

                    #region Runtime

                    if (nokiaMusic.AlbumDuration > 0)
                        objResuls.Add("Runtime", nokiaMusic.AlbumDuration);

                    #endregion

                    #region Tracks

                    objResuls.Add("Tracks", nokiaMusic.AlbumTracks);

                    #endregion

                    #region Types

                    if (nokiaMusic.AlbumTypes != null)
                        objResuls.Add("Types", nokiaMusic.AlbumTypes);

                    #endregion
                }
                return objResuls;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, id);
                return null;
            }
        }
        private static Artist ParseArtist(string artistName, string id)
        {
            try
            {
                bool isNew;
                Artist artist = ArtistServices.Get(artistName, out isNew);

                if (MySettings.FastSearch == true)
                    return artist;

                Uri strUrl = new Uri(string.Format(@"http://api.ent.nokia.com/1.x/us/creators/images/320x320/random/?domain=music&app_id={1}&app_code={2}&id={0}", id, AppId, AppToken));
                byte[] image = Util.GetImage(strUrl.ToString());

                if (image != null)
                    if (artist.Picture == null || artist.Picture.LongLength < image.LongLength)
                        artist.Picture = image;

                strUrl = new Uri(string.Format(@"http://api.ent.nokia.com/1.x/us/creators/{0}/products/?domain=music&app_id={1}&app_code={2}&orderby={3}", id, AppId, AppToken, "releasedate"));

                JObject restResponse = JObject.Parse(Util.GetRest(strUrl));

                NokiaMusic.ToArtist(restResponse, artist);

                return artist;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                return null;
            }
        }


    }
}
