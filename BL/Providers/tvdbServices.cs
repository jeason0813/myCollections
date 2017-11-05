using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Providers
{
    static class TvdbServices
    {
        public static Hashtable Parse(string strId, LanguageType languageType)
        {

            Hashtable objResults = new Hashtable();
            try
            {
                Uri strUrl = new Uri(string.Format(@"http://www.thetvdb.com/api/{0}/series/{1}/{2}.xml", "48CBD9BD208348FB", strId, languageType.ToString().ToLower()));
                //Fix since v2.6.0.0
                string ressult = Util.GetRest(strUrl);

                if (string.IsNullOrWhiteSpace(ressult) == false)
                {
                    XElement restResponse = XElement.Parse(ressult);
                    TVDB objTvdb = TVDB.SerieToObject(restResponse);
                    if (objTvdb != null)
                    {
                        List<Artist> objCast = GetTvdbArtist(strId, languageType);
                        if (objCast != null)
                            objResults.Add("Actors", objCast);

                        objResults.Add("Background", objTvdb.FanArt);

                        if (objTvdb.Genres != null)
                            objResults.Add("Types", objTvdb.Genres);

                        objResults.Add("Description", objTvdb.Overview);

                        if (objTvdb.Poster != null)
                            objResults.Add("Image", objTvdb.Poster);
                        else if (objTvdb.Banner != null)
                            objResults.Add("Image", objTvdb.Banner);

                        if (objTvdb.Rating != null)
                            objResults.Add("Rating", objTvdb.Rating);

                        objResults.Add("Runtime", objTvdb.Runtime);
                        objResults.Add("Title", objTvdb.SeriesName);
                        objResults.Add("Released", objTvdb.Released);
                        objResults.Add("Editor", objTvdb.Studio);
                        objResults.Add("Links", string.Format(@"http://thetvdb.com/?tab=series&id={0}", objTvdb.Id));
                    }
                }

                return objResults;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, strId);
                return null;
            }
        }
        private static List<Artist> GetTvdbArtist(string strId, LanguageType languageType)
        {
            try
            {
                Uri strUrl = new Uri(string.Format(@"http://www.thetvdb.com/api/{0}/series/{1}/actors.xml", "48CBD9BD208348FB", strId));
                //Fix since v2.6.0.0
                string ressult = Util.GetRest(strUrl, false);

                if (string.IsNullOrWhiteSpace(ressult) == false)
                {
                    XElement restResponse = XElement.Parse(ressult);

                    if (MySettings.FastSearch == false)
                        if (MySettings.EnableAlloCineSeries == true && MySettings.EnableTMDBMovies == false)
                            return
                                TVDB.CastToArtist(restResponse)
                                    .Select(item => AlloCineServices.SearchPortrait(item.FulleName, false, languageType))
                                    .ToList();
                        else if (MySettings.EnableTMDBMovies == true)
                        {
                            //Fix Since version 2.5.5.0
                            List<Artist> artistList = new List<Artist>();
                            IEnumerable<Artist> artists = TVDB.CastToArtist(restResponse);

                            Parallel.ForEach(artists, artist =>
                            {
                                Artist newArtist = TheMovieDbServices.SearchPortrait(artist.FulleName, false, languageType);
                                if (newArtist != null)
                                    artistList.Add(newArtist);
                            });

                            return artistList;
                        }
                        else
                            return TVDB.CastToArtist(restResponse).ToList();
                    else
                        return TVDB.CastToArtist(restResponse).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                return null;
            }
        }

        public static Collection<PartialMatche> Search(string strSearch, LanguageType language)
        {
            if (string.IsNullOrEmpty(strSearch) == false)
            {
                XElement restResponse = null;
                Uri strUrl = new Uri(string.Format(@"http://www.thetvdb.com/api/GetSeries.php?seriesname={0}&language={1}", Util.EncodeSearch(strSearch), language.ToString().ToLower()));
                string rest = Util.GetRest(strUrl);
                //Fix since version 2.5.5.0
                if (string.IsNullOrWhiteSpace(rest) == false)
                    restResponse = XElement.Parse(rest);

                if (restResponse == null)
                    return null;
                else
                    return TVDB.SerieToPartialMatche(restResponse);
            }
            else
                return null;
        }
    }
}
