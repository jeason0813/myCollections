using System.Collections;
using myCollections.Utils;
using System;
using System.Collections.ObjectModel;
using myCollections.Data;
using System.Collections.Generic;
using myCollections.Data.SqlLite;
using System.Text;
using System.Text.RegularExpressions;
using myCollections.BL.Services;
using myCollections.Properties;
namespace myCollections.BL.Providers
{
    static class DorcelServices
    {
        public static Hashtable Parse(string strUrl, bool getPage, string strSearch)
        {
            Hashtable objResuls = new Hashtable();

            try
            {
                string strResults;

                if (getPage == true)
                    strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);
                else
                    strResults = strUrl;

                objResuls.Add("Title", strSearch);
                objResuls.Add("Links", strUrl);

                if (string.IsNullOrEmpty(strResults)) return null;
                #region Image
                string strParsing = @"more-views";
                int intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"href=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";

                    objResuls.Add("Image", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                }
                #endregion
                #region Background
                strParsing = @"href=""";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"""";

                    objResuls.Add("Background", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                }
                #endregion
                #region Director
                strParsing = @"Réalisateur :";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"dd>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    objResuls.Add("Directed", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                #region Studio
                strParsing = @"Marque :";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"dd>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    objResuls.Add("Studio", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                #region Released
                strParsing = @"Année :";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"dd>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.InvariantCulture)).Trim();
                    int date;
                    if (int.TryParse(strTemp, out date) == true)
                        objResuls.Add("Released", new DateTime(date, 1, 1));
                }
                #endregion
                #region Actors
                strParsing = @"Actrice :";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"</dd>";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intBegin > -1)
                    {
                        string strActors = strResults.Substring(0, intBegin);
                        strParsing = @"dd>";
                        strActors = strActors.Substring(strActors.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @",";

                        string[] objTables = strActors.Split(new [] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                        List<Artist> lstActors = new List<Artist>();

                        foreach (string item in objTables)
                        {
                            if (string.IsNullOrEmpty(item.Trim()) == false)
                            {
                                string name = item.Trim();
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
                }
                #endregion
                #region Runtime
                strParsing = @"Durée :";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"dd>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    objResuls.Add("Runtime", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                #region Description
                strParsing = @"Description :";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"<p>";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</p>";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                    objResuls.Add("Description", Util.PurgeHtml(strResults.Substring(0, intBegin)));
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
                string strUrl = string.Format(@"http://www.dorcelstore.com/fr/catalogsearch/result/?q={0}", strSearch.Replace(" ", "+"));
                string strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8, BrowserType.Firefox4);

                if (strResults == null) return null;

                if (strResults.IndexOf("Aucun produit disponible", StringComparison.Ordinal) == -1)
                {
                    const string strParsing = @"product-image"">";
                    string[] objTables = Regex.Split(strResults, strParsing);

                    if (objTables.Length > 2)
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
                string strParsing = @"<img";

                if (strTemp.StartsWith(strParsing))
                {
                    PartialMatche objMatche = new PartialMatche();

                    strParsing = @"src=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.ImageUrl = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    strParsing = @"href=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    string strLink = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                    objMatche.Link = strLink;

                    strParsing = @"title=""";

                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.Title = Util.PurgeHtml(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));

                    lstMatche.Add(objMatche);
                }
            }

            return lstMatche;

        }
    }
}
