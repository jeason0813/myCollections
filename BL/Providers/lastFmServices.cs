using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Utils;
namespace myCollections.BL.Providers
{
    static class LastFmServices
    {
        public static Hashtable Parse(string id, string title, string artistName)
        {
            Hashtable objResuls = new Hashtable();
            try
            {
                Uri strUrl;
                if (string.IsNullOrWhiteSpace(id) == false)
                    strUrl = new Uri(string.Format(@"http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key=7e1cb1060b6150e4fbb60e119250f54c&mbid={0}", id));
                else
                    strUrl = new Uri(string.Format(@"http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key=7e1cb1060b6150e4fbb60e119250f54c&album={0}&artist={1}&autocorrect=1", title, artistName));

                XElement restResponse = XElement.Parse(Util.GetRest(strUrl));

                LastFm objLastFm = LastFm.AlbumToObject(restResponse);

                objResuls.Add("Links", objLastFm.AlbumUrl);
                objResuls.Add("Title", objLastFm.AlbumName);

                #region Album
                objResuls.Add("Album", objLastFm.AlbumName);
                #endregion
                #region Artist
                Artist artist = ParseArtist(objLastFm.ArtistName);
                if (artist != null)
                    objResuls.Add("Artist", artist);
                #endregion
                #region Description
                if (string.IsNullOrEmpty(objLastFm.AlbumDescription) == false)
                    objResuls.Add("Description", objLastFm.AlbumDescription);
                else if (string.IsNullOrEmpty(objLastFm.AlbumContent) == false)
                    objResuls.Add("Description", objLastFm.AlbumContent);
                else if (string.IsNullOrEmpty(objLastFm.ArtistContent) == false)
                    objResuls.Add("Description", objLastFm.ArtistContent);
                else if (string.IsNullOrEmpty(objLastFm.ArtistDescription) == false)
                    objResuls.Add("Description", objLastFm.ArtistDescription);
                #endregion
                #region Image
                //if (string.IsNullOrEmpty(objLastFM.albumMegaImage) == false)
                //    objResuls.Add("Image", objLastFM.albumMegaImage);
                //else if (string.IsNullOrEmpty(objLastFM.albumExtraLargeImage) == false)
                //    objResuls.Add("Image", objLastFM.albumExtraLargeImage);
                //else 
                if (string.IsNullOrEmpty(objLastFm.AlbumLargeImage) == false)
                    objResuls.Add("Image", objLastFm.AlbumLargeImage);
                else
                    objResuls.Add("Image", objLastFm.AlbumSmallImage);
                #endregion
                #region ReleaseDate
                objResuls.Add("Released", objLastFm.AlbumReleased);
                #endregion
                #region Runtime
                if (objLastFm.AlbumDuration > 0)
                    objResuls.Add("Runtime", objLastFm.AlbumDuration);
                #endregion
                #region Tracks
                objResuls.Add("Tracks", objLastFm.AlbumTracks);
                #endregion
                #region Types
                if (objLastFm.AlbumTypes != null)
                    objResuls.Add("Types", objLastFm.AlbumTypes);
                #endregion

                return objResuls;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, id);
                return null;
            }
        }

        private static Artist ParseArtist(string artistName)
        {
            try
            {
                bool isNew;
                Artist artist = ArtistServices.Get(artistName,out isNew);
                if (artist == null) artist = new Artist();

                Uri strUrl = new Uri(string.Format(@"http://ws.audioscrobbler.com/2.0/?method=artist.getinfo&api_key=7e1cb1060b6150e4fbb60e119250f54c&artist={0}&autocorrect=1", artistName));
                XElement restResponse = XElement.Parse(Util.GetRest(strUrl));

                LastFm objLastFm = LastFm.ArtistToObject(restResponse);

                artist.Bio = objLastFm.ArtistContent;
                artist.FulleName = objLastFm.ArtistName;

                if (objLastFm.ArtistExtraLargeImage != null)
                    artist.Picture = Util.GetImage(objLastFm.ArtistLargeImage);

                artist.WebSite = objLastFm.ArtistUrl;

                strUrl = new Uri(string.Format(@"http://ws.audioscrobbler.com/2.0/?method=artist.getTopAlbums&api_key=7e1cb1060b6150e4fbb60e119250f54c&mbid={0}&limit=500", objLastFm.ArtistId));
                restResponse = XElement.Parse(Util.GetRest(strUrl));
                IEnumerable<LastFm> credits = LastFm.AlbumToCollection(restResponse);

                if (credits != null  && artist.ArtistCredits != null)
                {
                    foreach (LastFm item in credits)
                    {
                        if (artist.ArtistCredits.Any(x => x.Title.Trim().ToUpper() == item.AlbumName.Trim().ToUpper()) == false)
                        {
                            ArtistCredits artistcredit = new ArtistCredits();
                            artistcredit.ArtistId = artist.Id;
                            artistcredit.BuyLink = item.AlbumUrl;
                            artistcredit.Title = item.AlbumName;
                            artistcredit.EntityType = EntityType.Music;

                            artist.ArtistCredits.Add(artistcredit);
                        }
                    }
                }
                return artist;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                return null;
            }
        }
        public static Collection<PartialMatche> Search(string strAlbum)
        {
            Uri strUrl = new Uri(string.Format(@"http://ws.audioscrobbler.com/2.0/?method=album.search&album={0}&api_key=7e1cb1060b6150e4fbb60e119250f54c", Util.EncodeSearch(strAlbum)));
            XElement restResponse = XElement.Parse(Util.GetRest(strUrl));
            return LastFm.AlbumToPartialMatch(restResponse);
            
        }
    }
}
