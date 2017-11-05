using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Providers
{
    class GraceNoteServices
    {

        private const string ClientId = "15765760-E98EC4696F3BED8ED111656CB58B41F9";
        private static string _userId = string.Empty;

        public static Hashtable Parse(string id,GraceNoteLanguage language)
        {
            Hashtable objResuls = new Hashtable();
            try
            {
                //FIX 2.7.12.0
                if (string.IsNullOrWhiteSpace(id) == true)
                    return objResuls;

                Uri strUrl = new Uri(string.Format(@"https://c{0}.web.cddbp.net/webapi/xml/1.0/", ClientId));

                if (string.IsNullOrWhiteSpace(_userId))
                    _userId = Register();

                if (string.IsNullOrWhiteSpace(_userId) == false)
                {
                    string query = string.Format(@"<QUERIES>
                                                    <AUTH>
                                                        <CLIENT>{0}</CLIENT>
                                                        <USER>{1}</USER>
                                                    </AUTH>
                                                    <LANG>{2}</LANG>
                                                    <QUERY CMD=""ALBUM_FETCH"">
                                                        <GN_ID>{3}</GN_ID>
                                                        <OPTION>
                                                            <PARAMETER>SELECT_EXTENDED</PARAMETER>
                                                            <VALUE>COVER,ARTIST_IMAGE,CONTENT,ARTIST_BIOGRAPHY,REVIEW,ARTIST_OET,MOOD,TEMPO</VALUE>
                                                        </OPTION>
                                                        <OPTION>
                                                            <PARAMETER>COVER_SIZE</PARAMETER>
                                                            <VALUE>LARGE</VALUE>
                                                      </OPTION>
                                                    </QUERY>
                                                 </QUERIES>", ClientId, _userId, language, id);


                    XElement restResponse = XElement.Parse(Util.PostRequest(strUrl, query));

                    GraceNote graceNote = GraceNote.AlbumToObject(restResponse);
                    //FIX 2.8.9.0
                    if (graceNote != null)
                    {
                        objResuls.Add("Title", graceNote.AlbumName);

                        #region Album

                        objResuls.Add("Album", graceNote.AlbumName);

                        #endregion

                        #region Artist

                        Artist artist = ParseArtist(graceNote.ArtistName, graceNote.ArtistUrl, graceNote.ArtistImage);
                        if (artist != null)
                            objResuls.Add("Artist", artist);

                        #endregion

                        #region Description

                        if (string.IsNullOrWhiteSpace(graceNote.AlbumDescription) == false)
                            objResuls.Add("Description", graceNote.AlbumDescription);

                        #endregion

                        #region Image

                        if (string.IsNullOrWhiteSpace(graceNote.AlbumImage) == false)
                            objResuls.Add("Image", graceNote.AlbumImage);

                        #endregion

                        #region Tracks

                        objResuls.Add("Tracks", graceNote.AlbumTracks);

                        #endregion

                        #region Types

                        if (graceNote.AlbumTypes != null)
                            objResuls.Add("Types", graceNote.AlbumTypes);

                        #endregion
                    }
                }
                return objResuls;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, id);
                return null;
            }
        }


        public static Collection<PartialMatche> Search(string strAlbum, GraceNoteLanguage language, string artist)
        {
            Uri strUrl = new Uri(string.Format(@"https://c{0}.web.cddbp.net/webapi/xml/1.0/", ClientId));

            if (string.IsNullOrWhiteSpace(_userId))
                _userId = Register();

            if (string.IsNullOrWhiteSpace(_userId) == false)
            {
                string query = string.Format(@"<QUERIES>
                                             <AUTH>
                                                <CLIENT>{0}</CLIENT>
                                                <USER>{1}</USER>
                                            </AUTH>
                                            <LANG>{2}</LANG>
                                            <QUERY CMD=""ALBUM_SEARCH"">
                                                <TEXT TYPE=""ALBUM_TITLE"">{3}</TEXT>", ClientId, _userId, language, strAlbum);

                if (string.IsNullOrWhiteSpace(artist) == false)
                    query += string.Format(@"<TEXT TYPE=""ARTIST"">{0}</TEXT>", artist);

                query += @"    </QUERY>
                          </QUERIES>";


                XElement restResponse = XElement.Parse(Util.PostRequest(strUrl, query));

                return GraceNote.AlbumToPartialMatch(restResponse);
            }
            return null;
        }
        private static Artist ParseArtist(string artistName,string artistUrl, string artistImage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(artistName)) 
                    return null;

                bool isNew;
                Artist artist = ArtistServices.Get(artistName,out isNew);
                
                if (artist == null) 
                    artist = new Artist();

                if (string.IsNullOrWhiteSpace(artistUrl) == false)
                {
                    Uri strUrl = new Uri(artistUrl);

                    string description = Util.GetRest(strUrl);

                    if (string.IsNullOrWhiteSpace(description) == false)
                        artist.Bio = description;
                }

                artist.FulleName = artistName;

                if (string.IsNullOrWhiteSpace(artistImage) == false)
                {
                    byte[] image = Util.GetImage(artistImage);

                    if (image != null)
                        if (artist.Picture == null || artist.Picture.LongLength < image.LongLength)
                            artist.Picture = image;
                }

                return artist;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                return null;
            }
        }

        private static string Register()
        {
            Uri strUrl = new Uri(string.Format(@"https://c{0}.web.cddbp.net/webapi/xml/1.0/", ClientId));
            string query = string.Format(@"<QUERIES>
                                             <QUERY CMD=""REGISTER"">
                                                <CLIENT>{0}</CLIENT>
                                              </QUERY>
                                        </QUERIES>", ClientId);
            string result = Util.PostRequest(strUrl, query);
            if (string.IsNullOrWhiteSpace(result) == false)
            {
                XElement restResponse = XElement.Parse(result);
                return GraceNote.GetClientId(restResponse);
            }
            else
                return null;
        }

    }
}
