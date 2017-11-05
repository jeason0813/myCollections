using System.Collections;
using myCollections.Utils;
using System.Text;
using System.Text.RegularExpressions;
using myCollections.Data;
using System.Collections.ObjectModel;
using System;
using myCollections.Data.SqlLite;
using System.Collections.Generic;
using myCollections.BL.Services;
using myCollections.Properties;
namespace myCollections.BL.Providers
{
    static class AdultBluRayHdDvdServices
    {
        public static Hashtable Parse(string strUrl, bool getPage)
        {
            Hashtable objResuls = new Hashtable();

            try
            {
                string strResults;
                string strTemp;

                if (getPage)
                    strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);
                else
                    strResults = strUrl;

                objResuls.Add("Links", strUrl);
                #region Released
                string strParsing = @"Movie Year: ";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"<br />";

                objResuls.Add("Released", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                #endregion
                #region Studio
                strParsing = @"<a href=";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @">";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</a>";

                objResuls.Add("Studio", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                #endregion
                #region Genres
                strParsing = @"Genres:";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"Director:";
                int intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp == -1)
                {
                    strParsing = @"Starring:";
                    intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                }
                if (intTemp == -1)
                {
                    strParsing = @"E-mail this movie to a friend";
                    intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                }
                string strGenres = strResults.Substring(0, intTemp);
                strParsing = @"<a href=";

                string[] objTables = strGenres.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                List<string> lstGenres = new List<string>();

                foreach (string item in objTables)
                {
                    if (string.IsNullOrEmpty(item.Trim()) == false &&
                        item.Contains(" Our Picks!") == false &&
                        item.Contains(@"adult-"))
                    {
                        strParsing = @">";
                        strTemp = item.Trim().Substring(item.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        lstGenres.Add(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));
                    }
                }
                objResuls.Add("Genre", lstGenres);
                #endregion
                #region Director
                strParsing = @"Director:";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</a>";

                    objResuls.Add("Directed", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                }
                #endregion
                #region Actors
                strParsing = @"Starring:";

                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"<a style";

                    intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTemp > -1)
                    {
                        string strActors = strResults.Substring(0, intTemp);
                        strParsing = @"<a href=";

                        objTables = strActors.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                        List<Artist> lstActors = new List<Artist>();

                        foreach (string item in objTables)
                        {
                            if (string.IsNullOrEmpty(item.Trim()) == false && item.Contains(@"adult-"))
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
                }
                #endregion
                #region Image
                strParsing = @"name=""ProductPic";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"src=""";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"""";
                strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                objResuls.Add("Image", "http://www.adultblurayhddvd.com/" + strTemp);
                #endregion
                #region Background
                objResuls.Add("Background", "http://www.adultblurayhddvd.com/" + strTemp.Replace(@"/medium/", "/large/"));
                #endregion
                #region Description
                strParsing = @"<hr style=""clear:both"" />";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"<";

                if (strResults.IndexOf(strParsing, StringComparison.Ordinal) > -1)
                {
                    strTemp = Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                    if (string.IsNullOrWhiteSpace(strTemp) == false)
                        objResuls.Add("Description", strTemp);
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
                string strUrl = string.Format("http://www.adultblurayhddvd.com/e-pmsearch.aspx?SearchTerm={0}&SearchType=0&Style=Box", strSearch);
                string strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(strResults)) return null;

                if (strResults.IndexOf("No results matching your search", StringComparison.Ordinal) == -1)
                {
                    string strParsing = @"<div>You are at: Search Results</div>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<div>";
                    string[] objTables = Regex.Split(strResults, strParsing);

                    if (objTables.Length > 1)
                        return CreatePartialMatch(strResults);
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
        private static Collection<PartialMatche> CreatePartialMatch(string strHtml)
        {
            string strParsing = @"<div>";
            strHtml = strHtml.Substring(strHtml.IndexOf(strParsing, StringComparison.Ordinal));
            string[] strResults = strHtml.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);

            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in strResults)
            {
                string strTemp = item.Trim();
                strParsing = @"<a href=""";

                if (strTemp.StartsWith(strParsing))
                {
                    PartialMatche objMatche = new PartialMatche();

                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    string strLink = @"http://www.adultblurayhddvd.com/" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                    objMatche.Link = strLink;

                    strParsing = @"title=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.Title = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    strParsing = @"src=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.ImageUrl = @"http://www.adultblurayhddvd.com/" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    lstMatche.Add(objMatche);
                }
            }

            return lstMatche;

        }
    }
}
