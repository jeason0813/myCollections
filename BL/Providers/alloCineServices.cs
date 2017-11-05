using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json.Linq;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.BL.Providers
{
    internal static class AlloCineServices
    {
        private const string PartnerAlloCine = "V2luZG93czg";
        private const string SecretKeyAlloCine = "e2b7fd293906435aa5dac4be670e7982";
        private const string UserAgentAllocine = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0; MSAppHost/1.0)";

        const string PartnerFilmStart = "100043982026";
        const string SecretKeyFilmStart = "29d185d98c984a359e6e6f26a0474269";
        private const string UserAgentFilmStart = "Dalvik/1.6.0 (Linux; U; Android 4.2.2; Nexus 4 Build/JDQ39E)";

        private static string _partner;
        private static string _secretKey;
        private static string _userAgent;

        public static Collection<PartialMatche> Search(string strSearch, LanguageType languageType)
        {
            string baseUrl = SetBaseUrl(languageType);

            strSearch = HttpUtility.UrlEncode(strSearch.Trim());
            string sed = DateTime.Now.ToString("yyyyMMdd");
            string tokens = string.Format(@"count={2}&filter={3}&format=json&partner={1}&q={0}&sed={4}", strSearch, _partner, "25", "movie", sed);

            string sig = GetSig(tokens);

            if (string.IsNullOrEmpty(strSearch) == false)
            {
                Uri strUrl = new Uri(string.Format(baseUrl + @"search?count={2}&filter={3}&format=json&partner={1}&q={0}&sed={4}&sig={5}", strSearch, _partner, "25", "movie", sed, sig));

                string response = Util.GetRest(strUrl, true, false, _userAgent);
                if (string.IsNullOrWhiteSpace(response) == false)
                {
                    JObject restResponse = JObject.Parse(response);
                    if (restResponse != null)
                        return AlloCine.MovieToPartialMatche(restResponse);
                    else
                        return null;
                }
                else return null;
            }
            else
                return null;
        }
        public static Collection<PartialMatche> SearchSeries(string strSearch, LanguageType languageType)
        {
            if (string.IsNullOrEmpty(strSearch) == false)
            {
                string baseUrl = SetBaseUrl(languageType);

                strSearch = HttpUtility.UrlEncode(strSearch.Trim());
                string sed = DateTime.Now.ToString("yyyyMMdd");
                string tokens = string.Format(@"count={2}&filter={3}&format=json&partner={1}&q={0}&sed={4}", strSearch, _partner, "25", "tvseries", sed);

                string sig = GetSig(tokens);

                Uri strUrl =
                new Uri(string.Format(baseUrl + @"search?count={2}&filter={3}&format=json&partner={1}&q={0}&sed={4}&sig={5}", strSearch, _partner, "25", "tvseries", sed, sig));

                //Uri strUrl =
                //    new Uri(string.Format(@"http://ma-filmotheque.fr/ma_filmoteque_api/allocine/search/{0}", strSearch));

                string response = Util.GetRest(strUrl, true, false, _userAgent);
                if (string.IsNullOrWhiteSpace(response) == false)
                {
                    JObject restResponse = JObject.Parse(response);

                    if (restResponse != null)
                        return AlloCine.SeriesToPartialMatche(restResponse);
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;
        }
        public static Artist SearchPortrait(string strSearch, bool usePartialMatch, LanguageType languageType)
        {
            string baseUrl = SetBaseUrl(languageType);

            strSearch = HttpUtility.UrlEncode(strSearch.Trim());
            string sed = DateTime.Now.ToString("yyyyMMdd");
            string tokens = string.Format(@"count={2}&filter={3}&format=json&partner={1}&q={0}&sed={4}", strSearch, _partner, "10", "person", sed);

            string sig = GetSig(tokens);

            Collection<AlloCine> lstResults = new Collection<AlloCine>();
            if (string.IsNullOrEmpty(strSearch) == false)
            {
                Uri strUrl =
                    new Uri(string.Format(baseUrl + @"search?count={2}&filter={3}&format=json&partner={1}&q={0}&sed={4}&sig={5}", strSearch, _partner, "10", "person", sed, sig));

                //Uri strUrl =
                //   new Uri(string.Format(@"http://ma-filmotheque.fr/ma_filmoteque_api/allocine/search/{0}", strSearch));

                //Fix Since version 2.6.0.0
                string response = Util.GetRest(strUrl, true, false, _userAgent);
                if (string.IsNullOrWhiteSpace(response) == false)
                {
                    JObject restResponse = JObject.Parse(response);
                    if (restResponse != null)
                        lstResults = AlloCine.ArtistToCollection(restResponse);
                    else
                        return null;
                }
                else
                    return null;

            }
            //Fix Since version 2.6.0.0
            if (lstResults != null)
            {
                if (usePartialMatch == true && lstResults.Count > 1)
                    return ShowPartialMatchArtist(lstResults, languageType);
                else
                {
                    if (lstResults.Count > 0)
                        return GetArtist(lstResults[0].Id, languageType);
                    else
                        return null;
                }
            }
            else
                return null;
        }

        public static Hashtable Parse(string strId, LanguageType languageType)
        {
            Hashtable objResults = new Hashtable();
            try
            {
                string sed = DateTime.Now.ToString("yyyyMMdd");
                string tokens = string.Format(@"code={0}&filter={1}&format=json&partner={2}&profile={3}&sed={4}", strId, "movie", _partner, "large", sed);

                string sig = GetSig(tokens);

                string baseUrl = SetBaseUrl(languageType);
                Uri strUrl =
                    new Uri(string.Format(baseUrl + @"movie?code={0}&filter={1}&format=json&partner={2}&profile={3}&sed={4}&sig={5}", strId, "movie", _partner, "large", sed, sig));

                //Fix since version 2.6.1.0
                string response = Util.GetRest(strUrl, true, false, _userAgent);

                if (string.IsNullOrWhiteSpace(response) == false)
                {
                    JObject restResponse = JObject.Parse(response);

                    AlloCine objAlloCine = AlloCine.MovieToObject(restResponse, languageType);
                    if (objAlloCine != null)
                    {
                        objResults.Add("Background", objAlloCine.BackdropOriginal);

                        if (objAlloCine.Cast != null)
                            objResults.Add("Actors", objAlloCine.Cast);

                        objResults.Add("Country", objAlloCine.Country);
                        objResults.Add("Description", Util.PurgeHtml(objAlloCine.Description));

                        if (objAlloCine.Directors != null)
                            objResults.Add("Director", objAlloCine.Directors);

                        objResults.Add("Types", objAlloCine.Genres);
                        objResults.Add("OriginalTitle", objAlloCine.OriginalTitle);
                        objResults.Add("Image", objAlloCine.PosterOriginal);

                        if (objAlloCine.Rating != null)
                        {
                            decimal dclTemp;
                            if (decimal.TryParse(objAlloCine.Rating.Replace(".", ","), out dclTemp))
                                if (dclTemp > 0)
                                    objResults.Add("AlloCine", ((int)(dclTemp * 4)).ToString(CultureInfo.InvariantCulture));
                        }

                        if (objAlloCine.Released.HasValue)
                            objResults.Add("Released", objAlloCine.Released.Value);

                        if (objAlloCine.Runtime.HasValue)
                            objResults.Add("Runtime", objAlloCine.Runtime.Value);

                        if (!string.IsNullOrEmpty(objAlloCine.Studio))
                            objResults.Add("Studio", objAlloCine.Studio);

                        objResults.Add("Comments", Util.PurgeHtml(objAlloCine.Tagline));
                        objResults.Add("Title", objAlloCine.Title);

                        if (string.IsNullOrWhiteSpace(objAlloCine.Url) == false)
                            objResults.Add("Links", objAlloCine.Url);
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
        public static Hashtable ParseSeries(string id, string seasonNumber, LanguageType languageType)
        {
            Hashtable objResults = new Hashtable();
            try
            {
                string sed = DateTime.Now.ToString("yyyyMMdd");
                string tokens = string.Format(@"code={0}&format=json&partner={1}&profile={2}&sed={3}", id, _partner, "large", sed);

                string sig = GetSig(tokens);

                string baseUrl = SetBaseUrl(languageType);
                Uri strUrl =
                    new Uri(string.Format(baseUrl + @"tvseries?code={0}&format=json&partner={1}&profile={2}&sed={3}&sig={4}", id, _partner, "large", sed, sig));

                //Uri strUrl =
                //    new Uri(string.Format(@"http://ma-filmotheque.fr/ma_filmoteque_api/allocine/tvseries/{0}", id));

                //Fix since version 2.6.1.0
                string response = Util.GetRest(strUrl, true, false, _userAgent);
                if (string.IsNullOrWhiteSpace(response) == false)
                {
                    JObject restResponse = JObject.Parse(response);

                    AlloCine objAlloCine = AlloCine.SerieToObject(restResponse, seasonNumber, languageType);

                    if (objAlloCine != null)
                    {
                        objResults.Add("Background", objAlloCine.BackdropOriginal);

                        if (objAlloCine.Cast != null)
                            objResults.Add("Actors", objAlloCine.Cast);

                        objResults.Add("Country", objAlloCine.Country);
                        objResults.Add("Description", Util.PurgeHtml(objAlloCine.Description));

                        if (objAlloCine.Directors != null)
                            objResults.Add("Director", objAlloCine.Directors);

                        objResults.Add("Types", objAlloCine.Genres);
                        objResults.Add("OriginalTitle", objAlloCine.OriginalTitle);
                        objResults.Add("Image", objAlloCine.PosterOriginal);

                        objResults.Add("Rating", objAlloCine.SerieRating);

                        if (objAlloCine.Released.HasValue)
                            objResults.Add("Released", objAlloCine.Released.Value);

                        if (objAlloCine.Runtime.HasValue)
                            objResults.Add("Runtime", objAlloCine.Runtime.Value);

                        if (!string.IsNullOrEmpty(objAlloCine.Studio))
                            objResults.Add("Editor", objAlloCine.Studio);

                        objResults.Add("Comments", Util.PurgeHtml(objAlloCine.Tagline));
                        objResults.Add("Title", objAlloCine.Title);
                        objResults.Add("Links", objAlloCine.Url);
                        objResults.Add("Episodes", objAlloCine.SeasonEpisodes);
                    }
                }

                return objResults;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, id);
                return null;
            }
        }
        private static Artist GetArtist(string strId, LanguageType languageType)
        {
            try
            {
                string sed = DateTime.Now.ToString("yyyyMMdd");
                string tokens = string.Format(@"code={0}&format=json&partner={1}&profile={2}&sed={3}", strId, _partner, "large", sed);

                string sig = GetSig(tokens);

                string baseUrl = SetBaseUrl(languageType);

                Uri strUrl =
                    new Uri(string.Format(baseUrl + @"person?code={0}&format=json&partner={1}&profile={2}&sed={3}&sig={4}", strId, _partner, "large", sed,sig));

                //Uri strUrl = new Uri(string.Format(@"http://ma-filmotheque.fr/ma_filmoteque_api/allocine/person/{0}", strId));

                //Fix Since version 2.6.0.0
                string response = Util.GetRest(strUrl, true, false, _userAgent);
                if (string.IsNullOrWhiteSpace(response) == false)
                {
                    JObject restResponse = JObject.Parse(response);
                    return AlloCine.CastToArtist(restResponse);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                return null;
            }
        }

        private static Artist ShowPartialMatchArtist(IEnumerable<AlloCine> lstItems, LanguageType languageType)
        {
            var lstMatche = new Collection<PartialMatche>();
            foreach (AlloCine item in lstItems)
            {
                var objMatche = new PartialMatche();

                objMatche.Link = item.Id;
                objMatche.ImageUrl = item.PosterOriginal;
                objMatche.Title = Util.PurgeHtml(item.Title);

                lstMatche.Add(objMatche);
            }


            partialMatch objWindow = new partialMatch(lstMatche, Provider.AlloCine);
            objWindow.ShowDialog();

            if (string.IsNullOrEmpty(objWindow.SelectedLink) == false)
                return GetArtist(objWindow.SelectedLink, languageType);

            return null;
        }

        private static string SetBaseUrl(LanguageType language)
        {
            switch (language)
            {
                case LanguageType.PT:
                case LanguageType.BR:
                    _partner = PartnerAlloCine;
                    _secretKey = SecretKeyAlloCine;
                    _userAgent = UserAgentAllocine;
                    return @"http://api.adorocinema.com/rest/v3/";
                case LanguageType.DE:
                    _partner = PartnerFilmStart;
                    _secretKey = SecretKeyFilmStart;
                    _userAgent = UserAgentFilmStart;
                    return @"http://api.filmstarts.de/rest/v3/";
                case LanguageType.ES:
                    return @"http://api.sensacine.com/rest/v3/";
                case LanguageType.EN:
                    return @"http://api.screenrush.co.uk/rest/v3/";
                case LanguageType.TK:
                    return @"http://api.beyazperde.com/rest/v3/";
                //case LanguageType.RU:
                //    return @"http://api.kinopoisk.ru/rest/v3/";
                //case LanguageType.CN:
                //    return @"http://api.mtime.com/rest/v3/";
                default:
                    _partner = PartnerAlloCine;
                    _secretKey = SecretKeyAlloCine;
                    _userAgent = UserAgentAllocine;
                    return @"http://api.allocine.fr/rest/v3/";

            }
        }
        private static string GetSig(string tokens)
        {
            string toDigest = string.Format("{0}{1}", _secretKey, tokens);
            byte[] toEncode = System.Text.Encoding.ASCII.GetBytes(toDigest);
            SHA1 sha = new SHA1CryptoServiceProvider();
            sha.Initialize();
            string encodedValue = HttpUtility.UrlEncode(Convert.ToBase64String(sha.ComputeHash(toEncode)));
            return Regex.Replace(encodedValue, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
        }
    }
}