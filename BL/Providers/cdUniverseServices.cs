using System.Collections;
using myCollections.Utils;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.BL.Services;
using myCollections.Properties;

namespace myCollections.BL.Providers
{
    static class CdUniverseServices
    {
        public static Hashtable Parse(string strUrl, bool getPage, string strSearch)
        {
            Hashtable objResuls = new Hashtable();

            try
            {
                string strResults;
                string strTemp;
                string[] objTables;

                if (getPage == true)
                    strResults = Util.GetHtmlPage(strUrl, Encoding.Default, BrowserType.Firefox10, true);
                else
                    strResults = strUrl;

                objResuls.Add("Title", strSearch);
                objResuls.Add("Links", strUrl);

                if (string.IsNullOrEmpty(strResults)) return null;

                #region Image
                string strParsing = @"/images.asp?pid=";
                int intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"""";
                    strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    string imagePage = Util.GetHtmlPage(@"http://www.cduniverse.com/" + strTemp.Replace("amp;", ""), Encoding.Default, BrowserType.Firefox10, true);

                    if (string.IsNullOrWhiteSpace(imagePage) == false)
                    {
                        strParsing = @"<center><p><img src=""";
                        imagePage = imagePage.Substring(imagePage.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"""";


                        objResuls.Add("Image", imagePage.Substring(0, imagePage.IndexOf(strParsing, StringComparison.Ordinal)));
                    }
                }
                #endregion
                #region Genres
                strParsing = @"nobr>Category </nobr>";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"<nobr>Director </nobr>";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin == -1)
                    strParsing = @"<nobr>Starring </nobr>";

                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    string strGenres = strResults.Substring(0, intBegin);

                    strParsing = @"href=";

                    objTables = strGenres.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> lstGenres = new List<string>();

                    foreach (string item in objTables)
                    {
                        strParsing = @">";
                        strTemp = item.Trim().Substring(item.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        strTemp = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                        if (string.IsNullOrWhiteSpace(strTemp) == false)
                            lstGenres.Add(Util.PurgeHtml(strTemp));
                    }
                    objResuls.Add("Genre", lstGenres);
                }

                #endregion
                #region Director
                strParsing = @"<nobr>Director </nobr>";
                int intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @""">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    objResuls.Add("Directed", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                }
                #endregion
                #region Actors
                strParsing = @"<nobr>Starring </nobr>";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"<!---- trimable --->";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    string strActors = strResults.Substring(0, intBegin);
                    strParsing = @"categorylink";

                    objTables = strActors.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                    List<Artist> lstActors = new List<Artist>();

                    foreach (string item in objTables)
                    {
                        if (string.IsNullOrEmpty(item.Trim()) == false &&
                            item.StartsWith(@"' href="))
                        {
                            strParsing = @">";
                            strTemp = item.Trim().Substring(item.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @"<";
                            string name = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                            bool isNew;
                            Artist artist = (ArtistServices.Get(name,out isNew));
                            if (MySettings.FastSearch == false)
                            {
                                string errorMessage;
                                ArtistServices.GetInfoFromWeb(artist, false, Provider.Iafd, out errorMessage, false);
                            }

                            lstActors.Add(artist);
                        }
                    }


                    objResuls.Add("Actors", lstActors);
                }
                #endregion
                #region Description
                strParsing = @"<span itemprop=""description"">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</span>";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                    objResuls.Add("Description", Util.PurgeHtml(strResults.Substring(0, intBegin)));
                #endregion
                #region Publisher
                strParsing = @"<nobr>Studio </nobr>";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @""">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</a></td>";

                    objResuls.Add("Studio", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                }
                #endregion
                #region Released
                strParsing = @"<nobr>Release Date </nobr>";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.InvariantCulture) + strParsing.Length);
                strParsing = @"<td>";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.InvariantCulture) + strParsing.Length);
                strParsing = @"</td>";

                objResuls.Add("Released", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.InvariantCulture)));
                #endregion
                #region Background
                strParsing = @"class='screenshot'";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"src='";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"'";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intBegin > -1)
                        objResuls.Add("Background", strResults.Substring(0, intBegin));
                }
                #endregion
                #region Rating
                strParsing = @"Average Rating:";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"(";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"out";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intBegin > -1)
                    {
                        strTemp = strResults.Substring(0, intBegin);
                        if (int.TryParse(strTemp, out intBegin) == true)
                            objResuls.Add("Rating", intBegin * 4);
                    }
                }
                #endregion
                #region Comments
                strParsing = @"reviewtext";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @">";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"<";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intBegin > -1)
                        objResuls.Add("Comments", Util.PurgeHtml(strResults.Substring(0, intBegin)));
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
                string strUrlMaster = string.Format("http://www.cduniverse.com/sresult.asp?psychicsearch=on&HT_Search=TITLE&HT_Search_Info={0}&style=ice&altsearch=yes", strSearch.Replace(" ", "%20"));
                string strResults = Util.GetHtmlPage(strUrlMaster, Encoding.UTF8, BrowserType.Firefox10, true);

                if (strResults.IndexOf("<title>Adult Video Universe Warning Page</title>", StringComparison.Ordinal) > -1)
                {
                    string strUrl = strResults.Substring(strResults.IndexOf("/warning.asp?Decision=I+Agree", StringComparison.Ordinal));
                    strUrl = strUrl.Substring(0, strUrl.IndexOf(@"""><", StringComparison.Ordinal));
                    Util.GetHtmlPage("http://www.cduniverse.com" + strUrl, Encoding.UTF8, BrowserType.Firefox10, true);
                    strResults = Util.GetHtmlPage(strUrlMaster, Encoding.UTF8, BrowserType.Firefox10, true);
                }

                if (strResults.IndexOf("No Title Matches Found", StringComparison.Ordinal) == -1 && strResults.IndexOf("We could not find any matches for adult title", StringComparison.Ordinal) == -1)
                {
                    const string strParsing = @"<tr valign=top bgcolor=";
                    string[] objTables = Regex.Split(strResults, strParsing);

                    if (objTables.Length > 1)
                        return CreatePartialMatch(objTables);
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
        private static Collection<PartialMatche> CreatePartialMatch(IEnumerable<string> objTables)
        {
            if (objTables == null) return null;

            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in objTables)
            {
                string strTemp = item.Trim();
                string strParsing = @"#";

                if (strTemp.StartsWith(strParsing) == true)
                {
                    PartialMatche objMatche = new PartialMatche();

                    strParsing = @"href=""";

                    int temp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                    if (temp > -1)
                    {
                        strTemp = strTemp.Substring(temp + strParsing.Length);
                        strParsing = @"""";
                        string strLink = @"http://www.cduniverse.com" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                        objMatche.Link = strLink;

                        strParsing = @"src=""";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"""";
                        objMatche.ImageUrl = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                        strParsing = @"alt=""";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"""";
                        objMatche.Title = Util.PurgeHtml(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));

                        lstMatche.Add(objMatche);
                    }
                }
            }

            return lstMatche;

        }
    }
}
