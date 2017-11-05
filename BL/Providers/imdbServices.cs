using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using myCollections.BL.Services;
using myCollections.Properties;
namespace myCollections.BL.Providers
{
    static class ImdbServices
    {
        public static Hashtable Parse(string strUrl, bool getPage, string strSearch)
        {
            Hashtable objResults = new Hashtable();

            try
            {
                string strResults;

                if (getPage)
                {
                    strResults = Util.GetHtmlPage(strUrl, Encoding.Default, BrowserType.Firefox4);
                    objResults.Add("Links", strUrl);
                }
                else
                    strResults = strUrl;

                if (string.IsNullOrWhiteSpace(strResults) == false)
                {
                    string strParsing;
                    if (strSearch.Contains("("))
                    {
                        strParsing = @"(";
                        strSearch = strSearch.Substring(0, strSearch.IndexOf(strParsing, StringComparison.Ordinal));
                        objResults.Add("Title", strSearch);
                    }

                    #region Image
                    strParsing = @"img_primary";
                    int intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    string strTemp;
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @"<img";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"src=""";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"""";
                        strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                        if (strTemp.Contains("name.png") == false)
                            objResults.Add("Image", strTemp);
                    }
                    #endregion
                    #region Rated
                    strParsing = @"contentRating";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @"content=""";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"""";
                        string rated = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                        if (string.IsNullOrWhiteSpace(rated) == false)
                            objResults.Add("Rated", rated);
                    }
                    #endregion
                    #region IMDB
                    strParsing = @"ratingValue"">";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @"</";
                        intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intTmp > -1)
                        {
                            string strRating = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                            decimal dclTemp;

                            if (decimal.TryParse(strRating, out dclTemp) == false)
                                decimal.TryParse(strRating.Replace(".", ","), out dclTemp);

                            if (dclTemp > 0)
                                objResults.Add("Rating", (long)(dclTemp * 2));
                        }
                    }
                    #endregion
                    #region Directed
                    strParsing = @"Director:</h4>";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @"itemprop=""name"">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"</";

                        string director = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                        if (string.IsNullOrWhiteSpace(director) == false)
                        {
                            List<Artist> directors = new List<Artist>();
                            bool isNew;
                            directors.Add(ArtistServices.Get(director, out isNew));

                            objResults.Add("Director", directors);
                        }
                    }
                    #endregion
                    #region Actors
                    strParsing = @"cast_list";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.InvariantCultureIgnoreCase);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @"</table>";
                        intTmp = strResults.IndexOf(strParsing, StringComparison.CurrentCultureIgnoreCase);
                        if (intTmp > -1)
                        {
                            string[] strActors = strResults.Substring(0, intTmp).Split(new[] { @"<tr" }, StringSplitOptions.RemoveEmptyEntries);
                            List<Artist> lstActors = new List<Artist>();
                            foreach (string item in strActors)
                            {
                                strTemp = item;
                                string image = string.Empty;
                                strParsing = @"loadlate=""";
                                intTmp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                                if (intTmp > -1)
                                {
                                    strTemp = strTemp.Substring(intTmp + strParsing.Length);
                                    strParsing = @"""";
                                    image = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                                }

                                strParsing = @"itemprop=""name"">";
                                intTmp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                                if (intTmp > -1)
                                {
                                    strTemp = strTemp.Substring(intTmp + strParsing.Length);
                                    strParsing = @"</";
                                    intTmp = strTemp.IndexOf(strParsing, StringComparison.CurrentCultureIgnoreCase);
                                    if (intTmp > -1)
                                    {
                                        string fullname = strTemp.Substring(0, intTmp);
                                        bool isNew;
                                        Artist objArtist = ArtistServices.Get(fullname, out isNew);

                                        if (string.IsNullOrWhiteSpace(image) == false && MySettings.FastSearch == false)
                                            if (image.Contains(@"noname.png") == false)
                                                objArtist.Picture = Util.GetImage(image);

                                        lstActors.Add(objArtist);
                                    }
                                }
                            }
                            if (lstActors.Count > 0)
                                objResults.Add("Actors", lstActors);
                        }
                    }
                    #endregion
                    #region Description
                    strParsing = @"Storyline";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.InvariantCultureIgnoreCase);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        int tmp2 = strResults.IndexOf(@"Plot Keywords:", StringComparison.Ordinal);
                        strParsing = @"<p>";
                        intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intTmp > -1 && (intTmp < tmp2 || tmp2 == -1))
                        {
                            strResults = strResults.Substring(intTmp + strParsing.Length);
                            strParsing = @"</p>";
                            intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                            strParsing = @"<em";
                            tmp2 = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                            if (tmp2 > -1 && tmp2 < intTmp)
                                intTmp = tmp2;

                            if (strResults.Substring(0, intTmp).Contains("full synopsis") == false)
                                objResults.Add("Description", Util.PurgeHtml(strResults.Substring(0, intTmp)));
                        }

                    }
                    #endregion
                    #region Genre
                    strParsing = @"Genres:";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);

                        int intPosDiv = strResults.IndexOf("</div>", StringComparison.Ordinal);
                        int intPosClass = strResults.IndexOf("</a> <a class", StringComparison.Ordinal);

                        if (intPosDiv < intPosClass || intPosClass == -1)
                            strParsing = @"</div>";
                        else
                            strParsing = @" <a class=";

                        string strGenre = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                        strParsing = @"<a href=""";
                        string[] strGenres = strGenre.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                        List<string> lstGenres = new List<string>();

                        foreach (string item in strGenres)
                        {
                            if (string.IsNullOrEmpty(item.Trim()) == false && item.StartsWith(@"/"))
                            {
                                strParsing = @">";
                                strTemp = item.Trim();
                                intTmp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                                if (intTmp > -1)
                                {
                                    strTemp = strTemp.Substring(intTmp + strParsing.Length);
                                    strParsing = @"<";
                                    string genre = strTemp.Substring(0,strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                                    genre = Util.PurgeHtml(genre);
                                    lstGenres.Add(genre);
                                }
                            }

                        }
                        objResults.Add("Types", lstGenres);
                    }
                    #endregion
                    #region Released
                    strParsing = @"Release Date:</h4>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"(";

                    string relased = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    if (string.IsNullOrWhiteSpace(relased) == false)
                    {
                        DateTime date;
                        if (DateTime.TryParse(relased.Trim(), out date) == true)
                            objResults.Add("Released", date);
                    }
                    #endregion
                    #region Studio
                    strParsing = @"Production Co:";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @"itemprop=""name"">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";

                        string studio = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                        if (string.IsNullOrWhiteSpace(studio) == false)
                            objResults.Add("Studio", studio);
                    }
                    #endregion
                    #region Runtime
                    strParsing = @"Runtime:";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @""">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";

                        string runtime = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                        if (string.IsNullOrWhiteSpace(runtime) == false)
                        {
                            runtime = runtime.Replace("min", "");
                            int intvalue;
                            if (int.TryParse(runtime, out intvalue) == true)
                                objResults.Add("Runtime", intvalue);
                        }
                    }
                    #endregion
                    #region Comments
                    if (getPage)
                    {
                        strResults = Util.GetHtmlPage(strUrl + "usercomments?filter=love&spoiler=hide", Encoding.Default, BrowserType.Firefox4);
                        if (string.IsNullOrWhiteSpace(strResults) == false)
                        {
                            strParsing = @"reviewBody"">";
                            intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                            if (intTmp > -1)
                            {

                                strResults = strResults.Substring(intTmp + strParsing.Length);

                                intTmp = strResults.IndexOf("</p>", StringComparison.Ordinal);

                                objResults.Add("Comments", Util.PurgeHtml(strResults.Substring(0, intTmp)));
                            }
                        }
                    }
                    #endregion
                }

                return objResults;

            }
            catch (Exception ex)
            {
                Util.LogException(ex, strUrl);
                return null;

            }
        }
        public static Collection<PartialMatche> Search(string strSearch)
        {
            try
            {
                string strUrl = string.Format("http://www.imdb.com/find?s=tt&q={0}", strSearch.Replace(" ", "+"));
                string strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);

                if (string.IsNullOrEmpty(strResults)) return null;

                if (strResults.IndexOf("No Matches.", StringComparison.Ordinal) == -1)
                {
                    if (strResults.IndexOf(@"</a>Titles</h3>", StringComparison.Ordinal) > -1)
                    {
                        string strParsing = @"findList";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"findResult";
                        string[] objTables = Regex.Split(strResults, strParsing);
                        return CreatePartialMatch(objTables);
                    }
                    else
                        return null;
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
        private static Collection<PartialMatche> CreatePartialMatch(IEnumerable<string> rows)
        {

            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();
            int maxRow = 0;

            foreach (string item in rows)
            {
                string strTemp = item.Trim();

                if (strTemp.StartsWith(@"odd"">") || strTemp.StartsWith(@"even"">"))
                {
                    PartialMatche objMatche = new PartialMatche();

                    string strParsing = @"href=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    string strLink = @"http://www.imdb.com" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                    objMatche.Link = strLink;

                    strParsing = @"src=""";
                    int intTmp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strTemp = strTemp.Substring(intTmp + strParsing.Length);
                        strParsing = @"""";
                        string image = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                        image = image.Replace("SY30_SX23_.", "SX80_SY158.");
                        objMatche.ImageUrl = image;
                    }

                    strParsing = @"href";
                    intTmp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @">";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"</a";
                        intTmp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);

                        if (intTmp > -1)
                        {
                            string title = strTemp.Substring(0, intTmp);
                            objMatche.Title = Util.PurgeHtml(title);

                            lstMatche.Add(objMatche);
                            maxRow++;
                            if (maxRow > 10) break;
                        }
                    }
                }
            }
            return lstMatche;
        }
        public static Collection<PartialMatche> SearchSeries2(string strSearch)
        {
            try
            {
                string strUrl = string.Format("http://www.imdb.com/search/title?title={0}&title_type=tv_series", strSearch.Replace(" ", "+"));
                string strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);

                if (string.IsNullOrEmpty(strResults)) return null;

                if (strResults.IndexOf("No results.", StringComparison.Ordinal) == -1)
                {
                    const string strParsing = @"<td class=""number"">";
                    string[] objTables = Regex.Split(strResults, strParsing);

                    if (objTables.Length > 1)
                        return CreatePartialMatchSeries(objTables);
                    else
                        return null;
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
        private static Collection<PartialMatche> CreatePartialMatchSeries(IEnumerable<string> objTables)
        {
            if (objTables == null) return null;

            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in objTables)
            {
                string strTemp = item.Trim();
                string strParsing = @"<!DOCTYPE html";

                if (strTemp.StartsWith(strParsing) == false)
                {
                    PartialMatche objMatche = new PartialMatche();

                    strParsing = @"href=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    string strLink = @"http://www.imdb.com" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                    objMatche.Link = strLink;

                    strParsing = @"title=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.Title = Util.PurgeHtml(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));

                    strParsing = @"src=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.ImageUrl = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    lstMatche.Add(objMatche);
                }
            }
            return lstMatche;

        }
    }
}
