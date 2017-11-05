using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Linq;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using Newtonsoft.Json.Linq;

namespace myCollections.BL.Providers
{
    class MusicbrainzServices
    {
        public static Collection<PartialMatche> Search(string album, string artistName)
        {
            Uri strUrl = new Uri(string.Format(@"http://www.musicbrainz.org/ws/2/release/?query=release:%22{0}%22%20AND%20artist:%22{1}%22", album, artistName));
            XElement restResponse = null;
            string rest = Util.GetRest(strUrl);

            if (string.IsNullOrWhiteSpace(rest) == false)
                restResponse = XElement.Parse(rest);

            if (restResponse == null)
                return null;
            else
                return Musicbrainz.ToPartialMatche(restResponse);
        }
        public static Hashtable Parse(string id)
        {
            Hashtable objResuls = new Hashtable();
            try
            {
                Uri strUrl = new Uri(string.Format(@"http://musicbrainz.org/ws/2/release/{0}?inc=artist-credits+labels+discids+recordings+release-groups&fmt=json", id));
                string results = Util.GetRest(strUrl);
                //FIX 2.7.12.0
                if (string.IsNullOrWhiteSpace(results) == false)
                {
                    JObject restResponse = JObject.Parse(results);
                    Musicbrainz music = Musicbrainz.ToObject(restResponse);

                    #region FanArt
                    if (string.IsNullOrWhiteSpace(music.GroupID) == false && string.IsNullOrWhiteSpace(music.ArtistId)==false)
                    {
                        FanArt images = FanArtServices.ParseArtist(music.ArtistId, music.GroupID);
                        if (images != null)
                        {
                            if (string.IsNullOrWhiteSpace(images.Albumcover)==false)
                                objResuls.Add("Image",images.Albumcover);

                            if (string.IsNullOrWhiteSpace(images.ArtistName) == false)
                            {
                                Artist artist=new Artist();
                                artist.FulleName = images.ArtistName;
                                artist.Picture = Util.GetImage(images.ArtistCover);

                                objResuls.Add("Artist", artist);

                            }

                            if (string.IsNullOrWhiteSpace(images.ArtistBackground) == false)
                                objResuls.Add("Background", images.ArtistBackground);

                            if (string.IsNullOrWhiteSpace(images.Cdart) == false)
                                objResuls.Add("Cdart", images.Cdart);
                        }

                    }
                    #endregion
                    objResuls.Add("Links", strUrl);
                    objResuls.Add("Title", music.AlbumName);

                    #region Album

                    objResuls.Add("Album", music.AlbumName);

                    #endregion

                    #region Editor

                    if (music.Studio != null)
                        objResuls.Add("Editor", music.Studio);

                    #endregion
                    #region ReleaseDate
                    if (string.IsNullOrWhiteSpace(music.AlbumReleased) == false)
                    {
                        DateTime date;
                        if (DateTime.TryParse(music.AlbumReleased, new CultureInfo("en-US"), DateTimeStyles.None,
                            out date))
                            objResuls.Add("Released", date);
                    }
                    #endregion
                    #region Runtime

                    if (music.AlbumDuration > 0)
                        objResuls.Add("Runtime", music.AlbumDuration);

                    #endregion

                    #region Tracks

                    objResuls.Add("Tracks", music.AlbumTracks);

                    #endregion
                    #region BarCode

                    objResuls.Add("BarCode", music.Barcode);

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


    }
}
