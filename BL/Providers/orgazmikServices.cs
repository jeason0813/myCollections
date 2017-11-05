using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Properties;
using myCollections.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace myCollections.BL.Providers
{
    static class OrgazmikServices
    {
        public static Hashtable Parse(string strUrl, bool getPage, string strSearch)
        {

            Hashtable objResuls = new Hashtable();

            try
            {
                string strResults;

                if (getPage == true)
                    strResults = Util.GetHtmlPage(strUrl, Encoding.Default);
                else
                    strResults = strUrl;

                string[] objTables;

                objResuls.Add("Title", strSearch);
                objResuls.Add("Links", strUrl);
                #region Publisher
                string strParsing = @"Studio / Publisher:  ";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.InvariantCulture) + strParsing.Length);
                strParsing = @">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.InvariantCulture) + strParsing.Length);
                strParsing = @"<";

                objResuls.Add("Studio", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.InvariantCulture)));
                #endregion
                #region Image
                strParsing = @"id=""COVER:";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"src=""";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"""";

                objResuls.Add("Image", "http://e.orgazmik.com" + strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                #endregion
                #region Directed
                strParsing = @"<strong>Director</strong>";
                int intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"#183D74"">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";

                    objResuls.Add("Directed", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                }
                #endregion
                #region Actors
                strParsing = @"<strong>Actors</strong>";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"#183D74"">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strActors = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    strParsing = @",";

                    objTables = strActors.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                    List<Artist> lstActors = new List<Artist>();

                    foreach (string item in objTables)
                    {
                        if (string.IsNullOrEmpty(item.Trim()) == false)
                        {
                            bool isNew;
                            Artist actor = ArtistServices.Get(item.Trim(),out isNew);

                            if (MySettings.FastSearch == false)
                            {
                                string errorMessage;
                                ArtistServices.GetInfoFromWeb(actor, false, Provider.Iafd, out errorMessage, false);
                            }

                            lstActors.Add(actor);
                        }
                    }

                    objResuls.Add("Actors", lstActors);
                }
                #endregion
                #region Genres
                strParsing = @"<strong>Categories</strong>";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"#183D74"">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</td>";

                string strGenres = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                strParsing = @"<a href=";

                objTables = strGenres.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                List<string> lstGenres = new List<string>();

                foreach (string item in objTables)
                {
                    if (string.IsNullOrEmpty(item.Trim()) == false &&
                        item.StartsWith(@"""/film"))
                    {
                        strParsing = @">";
                        string strTemp = item.Trim().Substring(item.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        lstGenres.Add(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));
                    }
                }
                objResuls.Add("Genre", lstGenres);
                #endregion
                #region Released
                strParsing = @"Catalog Date: ";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);

                objResuls.Add("Released", strResults.Substring(0, 10));
                #endregion
                #region RunTime
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strParsing = @"<strong>Length:</strong>";
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"Min.";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                        objResuls.Add("Runtime", strResults.Substring(0, intTmp).Trim());
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
                const string strUrl = "http://e.orgazmik.com/film/list/dvd/SRCH.htm";
                NameValueCollection objValues = new NameValueCollection();

                objValues.Add("SRCHTYP", "TITLE");
                objValues.Add("SRCHSTR", strSearch);

                string strResults = Util.GetHtmlPage(strUrl, objValues);

                if (strResults.IndexOf(strSearch, StringComparison.CurrentCultureIgnoreCase) > -1 &&
                    strResults.IndexOf("No entries found", StringComparison.Ordinal) == -1)
                {
                    const string strParsing = "<!-- Start liste element-->";
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
            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in objTables)
            {
                string strTemp = item.Trim();
                string strParsing = @"<table";

                if (strTemp.StartsWith(strParsing))
                {
                    PartialMatche objMatche = new PartialMatche();
                    strParsing = @"<a href=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    string strLink = @"http://e.orgazmik.com" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                    objMatche.Link = strLink;

                    strParsing = @"<img src=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.ImageUrl = @"http://e.orgazmik.com" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    strParsing = @"alt=""";
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
