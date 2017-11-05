using System;
using System.Globalization;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using myCollections.Utils;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using myCollections.Data.SqlLite;
using myCollections.Data;
using System.Collections.ObjectModel;
using myCollections.BL.Services;
using myCollections.Properties;

namespace myCollections.BL.Providers
{
    static class HotMoviesServices
    {
        public static Hashtable Parse(string strUrl, bool getPage)
        {

            Hashtable objResuls = new Hashtable();

            try
            {
                string strResults;
                strUrl = strUrl.Replace(@"/fr/", @"/en/");

                if (getPage)
                    strResults = Util.GetHtmlPage(strUrl, Encoding.Default);
                else
                    strResults = strUrl;
                objResuls.Add("Links", strUrl);
                #region Image
                string strParsing = @"onmouseout=""document.getElementById('cover').src='";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"'";

                string temp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                objResuls.Add("Image", temp);
                #endregion
                #region Rating
                strParsing = @"images/stars-";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @".png";

                temp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                temp = temp.Replace("-", ".");

                double rating;
                if (double.TryParse(temp, out rating) == true)
                    objResuls.Add("Rating", rating * 4);
                else
                {
                    temp = temp.Replace(".", ",");
                    if (double.TryParse(temp, out rating) == true)
                        objResuls.Add("Rating", rating * 4);
                }
                #endregion
                #region Studio
                strParsing = @"Studio:";
                  int intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"title=""";
                    strResults =
                        strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) +
                                             strParsing.Length);
                    strParsing = @"""";
                    temp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                    if (string.IsNullOrWhiteSpace(temp) == false)
                        objResuls.Add("Studio", temp);
                }

                #endregion
                #region Director
                strParsing = @"Director:";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"title=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";

                    objResuls.Add("Directed", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                }
                #endregion
                #region RunTime
                strParsing = @"Approximate Running Time:</strong>";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"duration";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    temp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                    if (string.IsNullOrWhiteSpace(temp)==false)
                    {
                        TimeSpan time;
                        if (TimeSpan.TryParse(temp,out time))
                        objResuls.Add("Runtime",time.TotalMinutes);
                    }
                }
                #endregion
                #region Released
                strParsing = @"Released:</strong>";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";

                    temp = Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                    if (string.IsNullOrEmpty(temp) == false)
                    {
                        DateTime date;
                        if (DateTime.TryParseExact(temp, "yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out date))
                            objResuls.Add("Released", date);
                    }
                }
                #endregion
                #region Description
                strParsing = @"class=""video_description""";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {

                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @">";
                    strResults =
                        strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) +
                                             strParsing.Length);
                    strParsing = @"<span id=";

                    objResuls.Add("Description",
                        Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal))));
                }

                #endregion
                #region Genre
                strParsing = @"Categories";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"</div>";
                string strGenres = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                strParsing = @"title=";

                string[] objTables = strGenres.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                List<string> lstGenres = new List<string>();

                foreach (string item in objTables)
                {
                    if (string.IsNullOrEmpty(item.Trim()) == false && item.StartsWith(":") == false)
                    {
                        strParsing = @"->";
                        intTemp = item.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intTemp > -1)
                        {
                            string strTemp = item.Trim().Substring(intTemp + strParsing.Length);
                            strParsing = @"""";
                            lstGenres.Add(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));
                        }
                    }
                }
                objResuls.Add("Genre", lstGenres);
                #endregion
                #region Background
                strParsing = @"<img class=""shot""";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"src=""";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"""";

                temp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                if (string.IsNullOrWhiteSpace(temp) == false)
                    objResuls.Add("Background", temp);
                #endregion
                #region Actors
                strParsing = @"Stars:";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"</div>";

                    intTemp = strResults.IndexOf(strParsing, StringComparison.CurrentCultureIgnoreCase);
                    if (intTemp > -1)
                    {
                        string[] strActors = strResults.Substring(0, intTemp).Split(new string[] { @"<a href=""" }, StringSplitOptions.RemoveEmptyEntries);
                        List<Artist> lstActors = new List<Artist>();
                        Parallel.ForEach(strActors, item =>
                        {
                            if (item.StartsWith(@"http://"))
                            {
                                strParsing = @"title=""";
                                string name =
                                    item.Substring(item.IndexOf(strParsing, StringComparison.Ordinal) +
                                                   strParsing.Length);
                                strParsing = @"""";
                                name = name.Substring(0, name.IndexOf(strParsing, StringComparison.Ordinal));

                                bool isNew;
                                Artist artist = (ArtistServices.Get(name, out isNew));
                                if (MySettings.FastSearch == false)
                                {
                                    string errorMessage;
                                    ArtistServices.GetInfoFromWeb(artist, false, Provider.Iafd, out errorMessage, false);
                                }

                                lstActors.Add(artist);
                            }
                        });
                        if (lstActors.Count > 0)
                            objResuls.Add("Actors", lstActors);
                    }
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
                string strUrl = string.Format("http://hotmovies.com/search.php?CLICK=116257%2C1%2Chm_rs%2C&language=english&title=on&conjunction=and&search_x=search&search_videos=1&words={0}&search_in%5B%5D=", strSearch.Replace(" ", "+"));
                string strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(strResults))
                    return null;

                if (strResults.IndexOf("Il y a 0 correspondances", StringComparison.Ordinal) == -1)
                {

                    const string strParsing = @"div class=""movie_box""";
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
        private static Collection<PartialMatche> CreatePartialMatch( IEnumerable<string> tableStrings)
        {
            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in tableStrings)
            {
                string strTemp = item.Trim();
                string strParsing = @"class=""studio"">";

                if (strTemp.Contains(strParsing))
                {
                    PartialMatche objMatche = new PartialMatche();
                    strParsing = @"<a href=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    string strLink = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                    strLink = strLink.Replace("french", "english");
                    objMatche.Link = strLink;

                    strParsing = @"title=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.Title = Util.PurgeHtml(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));

                    strParsing = @" style=""background: url(";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @")";
                    objMatche.ImageUrl = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    lstMatche.Add(objMatche);
                }
            }

            return lstMatche;

        }
    }
}
