using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Providers
{
    static class SugarVodServices
    {
        public static Hashtable Parse(string strUrl, bool getPage, string strSearch)
        {
            Hashtable objResuls = new Hashtable();

            try
            {
                string strResults;
                string temp;

                if (getPage)
                    strResults = Util.GetHtmlPage(strUrl, Encoding.Default);
                else
                    strResults = strUrl;

                objResuls.Add("Title", strSearch);
                objResuls.Add("Links", strUrl);
                #region Studio
                string strParsing = @"<SPAN CLASS=""details_title_studio_text"">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</A>";

                objResuls.Add("Studio", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                #endregion
                #region Image
                strParsing = @"<a id=""productimg_1""";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"<img src=""";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @""" name=";

                objResuls.Add("Image", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                #endregion
                #region Rating
                strParsing = @"Avg Rating";
                int intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"</B>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"stars";
                    temp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    double rating;
                    if (double.TryParse(temp, out rating) == true)
                        objResuls.Add("Rating", (double?)(rating * 4));
                    else
                    {
                        temp = temp.Replace(".", ",");
                        if (double.TryParse(temp, out rating) == true)
                            objResuls.Add("Rating", (double?)(rating * 4));
                    }
                }
                #endregion
                #region Background
                strParsing = @"javascript:goLoginPage";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);

                    strParsing = @"SRC=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";

                    temp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    if (string.IsNullOrWhiteSpace(temp) == false)
                        objResuls.Add("Background", temp.Replace("_sm", "_hu"));
                }
                #endregion
                #region Description
                strParsing = @"Synopsis:</B></TD><TD ALIGN=""left"">";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"</TD>";

                    objResuls.Add("Description", Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal))));
                }
                #endregion
                #region RunTime
                strParsing = @"<B>Running Time:</B></TD><TD ALIGN=""left"">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</TD>";
                string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                strParsing = @"minutes";
                if (strTemp.IndexOf(strParsing, StringComparison.Ordinal) > -1)
                    objResuls.Add("Runtime", strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));
                #endregion
                #region Released
                strParsing = @"<B>Added on:</B></TD><TD ALIGN=""left"">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</TD>";
                temp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                temp = temp.Replace("th, ", " ");
                objResuls.Add("Released", temp);
                #endregion
                #region Genre
                strParsing = @"Categories:";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"Directed By:";

                string strGenres = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                strParsing = @"<A HREF=""";
                string[] objTables = strGenres.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                List<string> lstGenres = new List<string>();

                foreach (string item in objTables)
                {
                    if (string.IsNullOrEmpty(item.Trim()) == false)
                    {
                        strParsing = @">";
                        strTemp = item.Trim().Substring(item.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        string genre = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                        if (string.IsNullOrWhiteSpace(genre) == false)
                            lstGenres.Add(genre);
                    }
                }
                objResuls.Add("Genre", lstGenres);

                #endregion
                #region Director
                strParsing = @"<B>Directed By:</B></TD><TD ALIGN=""left"">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</TD>";

                objResuls.Add("Directed", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                #endregion
                #region Actors
                strParsing = @"Starring:</B></TD><TD ALIGN=""left""><A HREF=""";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</A></TD></TR></TABLE>";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    string[] strActors = strResults.Substring(0, intTemp).Split(new[] { @"http:" }, StringSplitOptions.RemoveEmptyEntries);
                    List<Artist> lstActors = new List<Artist>();
                    Parallel.ForEach(strActors, item =>
                    {
                        strTemp = item;
                        if (strTemp.StartsWith(@"//"))
                        {
                            strParsing = ">";
                            strTemp =
                                strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) +
                                                  strParsing.Length);
                            strParsing = "<";
                            intTemp = strTemp.IndexOf(strParsing, StringComparison.CurrentCultureIgnoreCase);
                            if (intTemp > -1)
                            {
                                string name = strTemp.Substring(0, intTemp);
                                bool isNew;
                                Artist artist = (ArtistServices.Get(name, out isNew));
                                if (MySettings.FastSearch == false)
                                {
                                    string errorMessage;
                                    ArtistServices.GetInfoFromWeb(artist, false, Provider.Iafd, out errorMessage, false);
                                }

                                lstActors.Add(artist);
                            }

                        }
                    });
                    if (lstActors.Count > 0)
                        objResuls.Add("Actors", lstActors);
                }

                #endregion
                return objResuls;
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
                strSearch = strSearch.Replace(" ", "+");
                //Fix Since 2.6.0.0
                string strUrl = string.Format("http://www.sugarvod.com/browse/vod/?search=true&where=title&query={0}", strSearch);
                string strResults = Util.GetHtmlPage(strUrl, Encoding.Default);

                if (strResults.Contains("no items matched") == false)
                {
                    //Fix 2.8.10.0
                    const string strParsing = @"id=""box_cover";
                    string[] objTables = Regex.Split(strResults, strParsing);

                    if (objTables.Length > 1)
                        return CreatePartialMatch(objTables);
                    //Fix Since 2.6.7.0
                    else if (strResults.Contains(@"<SPAN CLASS=""details_title_studio_text"">"))
                    {
                        Collection<PartialMatche> collection=new Collection<PartialMatche>();
                        PartialMatche partialMatche=new PartialMatche();
                        partialMatche.Link = strUrl;
                        partialMatche.Title = strSearch;
                        collection.Add(partialMatche);
                        return collection;
                    }
                    else
                        return null;
                   
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Util.LogException(ex,strSearch);
                return null;
            }
        }
        private static Collection<PartialMatche> CreatePartialMatch(IEnumerable<string> strResults)
        {
            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in strResults)
            {
                string strTemp = item.Trim();
               string strParsing = @"_";

                if (strTemp.StartsWith(strParsing))
                {
                    PartialMatche objMatche = new PartialMatche();

                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"href=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.Link = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    strParsing = @"SRC=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.ImageUrl = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    strParsing = @"ALT=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.Title = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    lstMatche.Add(objMatche);
                }
            }

            return lstMatche;

        }
    }
}
