using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Properties;
using myCollections.Utils;

namespace myCollections.BL.Providers
{
    static class TheMovieDbServices
    {
        const string RootUrl = @"http://api.themoviedb.org/3/";
        //const string rootUrl = @"http://private-21cd-themoviedb.apiary.io/3/";



        //private static void GetConfig()
        //{
        //    Uri strUrl = new Uri(string.Format(RootUrl + "configuration?api_key={0}", "ec7abdb7bea26002ff70d02cb5b11224"));
        //    JObject restResponse = JObject.Parse(Util.GetRest(strUrl, false, true));

        //}

        public static Artist SearchPortrait(string strSearch, bool usePartialMatch, LanguageType language)
        {
            Collection<TMDB> lstResults = new Collection<TMDB>();
            if (string.IsNullOrEmpty(strSearch) == false)
            {
                Uri strUrl = new Uri(string.Format(RootUrl + "search/person?api_key={0}&query={1}", "ec7abdb7bea26002ff70d02cb5b11224", strSearch));
                JObject restResponse = JObject.Parse(Util.GetRest(strUrl, false, true));
                lstResults = TMDB.ArtistToCollection(restResponse);
            }


            if (usePartialMatch == true && lstResults.Count > 1)
                return ShowPartialMatchArtist(lstResults);
            else
            {
                if (lstResults.Count > 0)
                    return GetArtist(lstResults[0].Id);
                else
                    return null;
            }
        }
        public static Hashtable Parse(string strId, LanguageType language)
        {
            Hashtable objResults = new Hashtable();
            try
            {
                Uri strUrl = new Uri(string.Format(RootUrl + "movie/{2}?api_key={1}&language={0}&append_to_response={3}", language.ToString().ToLower(), "ec7abdb7bea26002ff70d02cb5b11224", strId, "casts,rating"));
                string response = Util.GetRest(strUrl, false, true);

                //Fix since version 2.6.0.0
                if (string.IsNullOrWhiteSpace(response))
                    return null;

                JObject restResponse = JObject.Parse(response);
                TMDB objTmdb = TMDB.MovieToObject(restResponse);
                if (objTmdb != null)
                {
                    if (objTmdb.Cast != null && MySettings.FastSearch == false)
                    {
                        //FIX 2.8.9.0
                        ParallelQuery<Artist> artists = objTmdb.Cast.AsParallel().Select(item => GetArtist(item));
                        if (artists != null && artists.Any())
                            objResults.Add("Actors", artists.ToList());
                    }
                    objResults.Add("Country", objTmdb.Country);

                    if (!string.IsNullOrWhiteSpace(objTmdb.MovieDescription))
                        objResults.Add("Description", objTmdb.MovieDescription);

                    if (string.IsNullOrEmpty(objTmdb.Director) == false && MySettings.FastSearch == false)
                    {
                        List<Artist> directors = new List<Artist>();
                        Artist director = GetArtist(objTmdb.Director);
                        directors.Add(director);

                        objResults.Add("Director", directors);
                    }

                    objResults.Add("Types", objTmdb.Genres);

                    if (!string.IsNullOrWhiteSpace(objTmdb.PosterOriginal))
                        objResults.Add("Image", objTmdb.PosterOriginal);

                    objResults.Add("Background", objTmdb.BackdropOriginal);
                    if (string.IsNullOrWhiteSpace(objTmdb.Url) == false)
                        objResults.Add("Links", objTmdb.Url);

                    if (string.IsNullOrWhiteSpace(objTmdb.Rated) == false)
                        objResults.Add("Rated", objTmdb.Rated);

                    if (!string.IsNullOrWhiteSpace(objTmdb.Tagline))
                        objResults.Add("Comments", objTmdb.Tagline);

                    if (objTmdb.Rating != null)
                    {
                        decimal dclTemp;
                        if (decimal.TryParse(objTmdb.Rating.Replace(".", ","), out dclTemp))
                            if (dclTemp > 0)
                                objResults.Add("Rating", dclTemp * 2);
                    }

                    if (objTmdb.Released.HasValue)
                        objResults.Add("Released", objTmdb.Released.Value);

                    if (objTmdb.Runtime.HasValue)
                        objResults.Add("Runtime", objTmdb.Runtime.Value);

                    if (!string.IsNullOrWhiteSpace(objTmdb.Studio))
                        objResults.Add("Studio", objTmdb.Studio);

                    objResults.Add("Title", objTmdb.Title);

                }

                return objResults;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, strId);
                return null;
            }
        }
        private static Artist ShowPartialMatchArtist(IEnumerable<TMDB> lstItems)
        {
            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();
            foreach (TMDB item in lstItems)
            {
                PartialMatche objMatche = new PartialMatche();

                objMatche.Link = item.Id;
                objMatche.ImageUrl = item.PosterThumb;
                objMatche.Title = Util.PurgeHtml(item.Title);

                lstMatche.Add(objMatche);
            }


            partialMatch objWindow = new partialMatch(lstMatche, Provider.Tvdb);
            objWindow.Title = "www.themoviedb.org" + " - " + objWindow.Title;
            objWindow.ShowDialog();

            if (string.IsNullOrEmpty(objWindow.SelectedLink) == false)
                return GetArtist(objWindow.SelectedLink);

            return null;

        }
        private static Artist GetArtist(string strId)
        {
            try
            {
                Uri strUrl = new Uri(string.Format(RootUrl + "person/{1}?api_key={0}&append_to_response={2}", "ec7abdb7bea26002ff70d02cb5b11224", strId, "credits"));
                string response = Util.GetRest(strUrl, false, true);

                //Fix since version 2.6
                if (string.IsNullOrWhiteSpace(response))
                    return null;

                JObject restResponse = JObject.Parse(response);

                return TMDB.CastToArtist(restResponse);

            }
            catch (Exception ex)
            {
                Util.LogException(ex, "Artist id :" + strId);
                return null;
            }
        }

        public static Collection<PartialMatche> Search(string strSearch, LanguageType language)
        {

            if (string.IsNullOrEmpty(strSearch) == false)
            {
                Uri strUrl = new Uri(string.Format(RootUrl + "search/movie?api_key={1}&query={2}&language={0}", language.ToString().ToLower(), "ec7abdb7bea26002ff70d02cb5b11224", strSearch));
                string response = Util.GetRest(strUrl, false, true);

                //Fix since version 2.6
                if (string.IsNullOrWhiteSpace(response))
                    return null;

                JObject restResponse = JObject.Parse(response);

                return TMDB.MovieToPartialMatche(restResponse);
            }

            else
                return null;
        }
    }
}
