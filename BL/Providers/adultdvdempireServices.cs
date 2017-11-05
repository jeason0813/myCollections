
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Properties;
using myCollections.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace myCollections.BL.Providers
{
    static class AdultdvdempireServices
    {
        public static Hashtable Parse(string strUrl, bool getPage, string strSearch)
        {
            Hashtable objResuls = new Hashtable();

            try
            {
                string strResults;
                string strTemp;
                string coverlink = string.Empty;

                if (getPage == true)
                    strResults = Util.GetHtmlPage(strUrl, Encoding.Default);
                else
                    strResults = strUrl;

                objResuls.Add("Title", strSearch);
                objResuls.Add("Links", strUrl);

                if (string.IsNullOrEmpty(strResults)) return null;

                #region Rating
                string strParsing = @"img_Rating";
                int intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"Item_StudioProductionRating";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = " out";
                    intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTemp > -1)
                    {
                        string rating = strResults.Substring(0, intTemp).Trim();
                        decimal decRating;
                        if (decimal.TryParse(rating, out decRating) == true)
                        {
                            decRating = decRating * 4;
                            objResuls.Add("Rating", (int)decRating);
                        }
                        else
                        {
                            rating = rating.Replace('.', ',');
                            if (decimal.TryParse(rating, out decRating) == true)
                            {
                                decRating = decRating * 4;
                                objResuls.Add("Rating", (int)decRating);
                            }
                        }
                    }
                }
                #endregion
                #region CoverUrl
                strParsing = @"Boxcover";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"href=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    coverlink = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                }

                #endregion
                #region Description
                strParsing = @"Section Synopsis";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"Tagline"">";
                int intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"</div";
                    objResuls.Add("Description", Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal))));
                }
                else
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"</span>";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"<";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intBegin > -1)
                        objResuls.Add("Description", Util.PurgeHtml(strResults.Substring(0, intBegin)));
                }
                #endregion
                #region Runtime
                strParsing = @"Length</strong>";
                intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTemp > -1)
                {
                    strResults = strResults.Substring(intTemp + strParsing.Length);
                    strParsing = @"<";
                    strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    strParsing = "hrs.";
                    intTemp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    int total = 0;
                    if (intTemp > -1)
                    {
                        string strHour = strTemp.Substring(0, intTemp).Trim();
                        int hour;
                        if (int.TryParse(strHour, out hour) == true)
                            total = hour * 60;

                        string strMin = strTemp.Substring(intTemp + strParsing.Length, strTemp.IndexOf("mins", StringComparison.Ordinal) - (intTemp + strParsing.Length)).Trim();
                        int min;
                        if (int.TryParse(strMin, out min) == true)
                            total += min;

                        if (total > 0)
                            objResuls.Add("Runtime", total);
                    }
                }
                #endregion
                #region Released
                strParsing = @"Released</strong>";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.InvariantCulture) + strParsing.Length);
                strParsing = @"<";
                strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.InvariantCulture)).Trim();
                DateTime date;
                if (DateTime.TryParse(strTemp, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out date) == true)
                    objResuls.Add("Released", date);
                #endregion
                #region Studio
                strParsing = @"Studio</strong>";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @""">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";

                    objResuls.Add("Studio", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                #region Actors
                strParsing = @"Section Cast";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"<li>";
                    string[] objTables = Regex.Split(strResults, strParsing);
                    List<Artist> lstActors = new List<Artist>();

                   foreach(string item in objTables)
                   {
                       if (string.IsNullOrEmpty(item.Trim()) == false && item.StartsWith(@"<a href")==true && item.Contains("pornstars.html"))
                       {
                           strParsing = @"href=""";
                           strTemp = item.Trim().Substring(item.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                           strParsing = @"""";
                           string url = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                           strParsing = @">";
                           strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                           strParsing = @"<";
                           string name = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                           if (string.IsNullOrWhiteSpace(name) == false)
                           {
                               bool isNew;
                               Artist artist = (ArtistServices.Get(name, out isNew));

                               lstActors.Add(artist);

                               if (MySettings.FastSearch == false)
                               {
                                   if (url.Contains("http:") == false)
                                       url = "http://www.adultdvdempire.com" + url;

                                   ArtistServices.Fill(ParsePortrait(url), artist, name);
                               }
                           }
                       }
                   }

                    objResuls.Add("Actors", lstActors);
                }
                #endregion
                #region Genres
                strParsing = @"Section Categories";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {

                    strResults = strResults.Substring(intBegin + strParsing.Length);
                    strParsing = @"href=";

                    string[] objTables = strResults.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> lstGenres = new List<string>();

                    foreach (string item in objTables)
                    {
                        if (item.Contains(@"/category/") && item.StartsWith("http:"))
                        {
                            strParsing = @">";
                            strTemp = item.Trim().Substring(item.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @"<";
                            strTemp = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                            if (string.IsNullOrWhiteSpace(strTemp) == false)
                                lstGenres.Add(Util.PurgeHtml(strTemp));
                        }
                    }
                    objResuls.Add("Genre", lstGenres);
                }
                #endregion
                #region Image

                if (string.IsNullOrWhiteSpace(coverlink) == false)
                    objResuls.Add("Image", coverlink);

                #endregion
                #region Background
                if (string.IsNullOrWhiteSpace(coverlink) == false)
                {
                    coverlink = coverlink.Replace("h.jpg", "bh.jpg");
                    objResuls.Add("Background", coverlink);
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
        public static Hashtable SearchPortrait(string strSearch, bool usePartialMatche)
        {
            try
            {
                string strUrlMaster = string.Format("http://www.adultdvdempire.com/performer/search?q={0}", strSearch.Replace(" ", "+"));
                string strResults = Util.GetHtmlPage(strUrlMaster, Encoding.UTF8, BrowserType.Firefox4);

                if (string.IsNullOrWhiteSpace(strResults) == false)
                {
                    if (strResults.IndexOf("Having Trouble Finding Something", StringComparison.Ordinal) == -1)
                    {
                        string strParsing = @"class=""performer""";
                        string[] objTables = Regex.Split(strResults, strParsing);

                        if (usePartialMatche && objTables.Length > 2)
                            return ShowPartialMatchPortrait(objTables);
                        else
                        {
                            if (objTables.Length > 1)
                            {
                                strResults = objTables[1];
                                strParsing = @"href=""";
                                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                                strParsing = @"""";
                                string strLink = @"http://www.adultdvdempire.com" + strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                                return ParsePortrait(strLink+"?pageSize=100");
                            }
                            else
                                return null;
                        }
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
        private static Hashtable ShowPartialMatchPortrait(IEnumerable<string> objTables)
        {
            if (objTables == null) return null;

            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();
            Hashtable objResults = new Hashtable();

            foreach (string item in objTables)
            {
                string strTemp = item.Trim();
                string strParsing = @""">";

                if (strTemp.StartsWith(strParsing) == true)
                {
                    PartialMatche objMatche = new PartialMatche();

                    strParsing = @"Filmography";
                    int temp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                    if (temp > -1)
                    {

                        strTemp = strTemp.Substring(0, temp);

                        strParsing = @"href=""";

                        temp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                        if (temp > -1)
                        {
                            strTemp = strTemp.Substring(temp + strParsing.Length);
                            strParsing = @""">";

                            string strLink = @"http://www.adultdvdempire.com" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                            objMatche.Link = strLink;

                            strParsing = @"src=""";
                            temp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                            if (temp > -1)
                            {
                                strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                                strParsing = @"""";
                                objMatche.ImageUrl = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                            }

                            strParsing = @"ListItem_ItemTitle"">";
                            strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @""">";
                            strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @"<";
                            objMatche.Title = Util.PurgeHtml(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));

                            lstMatche.Add(objMatche);
                        }
                    }
                }
            }

            partialMatch objWindow = new partialMatch(lstMatche);
            objWindow.Title = "www.adultdvdempire.com" + " - " + objWindow.Title;
            objWindow.ShowDialog();

            if (string.IsNullOrEmpty(objWindow.SelectedLink) == false)
                return ParsePortrait(objWindow.SelectedLink);

            return objResults;

        }
        private static Hashtable ParsePortrait(string strUrl)
        {
            Hashtable objResults = new Hashtable();

            try
            {
                string strResults = Util.GetHtmlPage(strUrl, Encoding.Default, BrowserType.Firefox4);

                if (strResults == null) return null;

                #region Image
                string strParsing = @"Headshot";
                int intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"src='";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"'";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    objResults.Add("Image", strTemp);
                }
                #endregion
                #region Weight
                strParsing = @"Weight";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                    {
                        strTemp = strTemp.Replace("lbs.", "").Trim();
                        objResults.Add("Weight", strTemp);
                    }
                }
                #endregion
                #region Height
                strParsing = @"Height";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Height", strTemp.Trim());
                }
                #endregion
                #region Eye Color
                strParsing = @"Eye Color";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Eye Color", strTemp.Trim());
                }
                #endregion
                #region Measurements
                strParsing = @"Meas.";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Measurements", Util.BraConverter(strTemp.Split('-')[0]));
                }
                #endregion        
                #region Hair Color
                strParsing = @"Hair Color";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Hair Color", strTemp);
                }
                #endregion
                #region Ethnicity
                strParsing = @"Ethnicity";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Ethnicity", strTemp);
                }
                #endregion
                #region Credits
                strParsing = @"class=""rank""";
                string[] objTables = Regex.Split(strResults, strParsing);

                List<ArtistCredits> credits = new List<ArtistCredits>();
                foreach (string item in objTables)
                {
                    string temp = item.Trim();
                    strParsing = @">";

                    if (temp.StartsWith(strParsing) == true)
                    {
                        strParsing = @"href=""";

                        intBegin = temp.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intBegin > -1)
                        {
                            temp = temp.Substring(intBegin + strParsing.Length);
                            strParsing = @".html";

                            string buyLink = string.Format(@"http://www.adultdvdempire.com{0}?partner_id={1}", temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal)+strParsing.Length).Trim(), "86947268");

                            strParsing = @"title=""";
                            temp = temp.Substring(temp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @"""";
                            string title = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal));

                            strParsing = "- Released ";
                            temp = temp.Substring(temp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = " ";
                            string year = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal)).Trim();

                            ArtistCredits movie = new ArtistCredits();
                            movie.Title = Util.PurgeHtml(title);

                            movie.EntityType = EntityType.XXX;

                            DateTime relaeseDate;
                            if (DateTime.TryParse(year, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out relaeseDate) == true)
                                movie.ReleaseDate = relaeseDate;

                            if (string.IsNullOrWhiteSpace(buyLink) == false)
                                movie.BuyLink = buyLink;

                            credits.Add(movie);
                        }
                    }
                }
                if (credits.Count > 0)
                    objResults.Add("Credits", credits);
                #endregion
                #region Bio

                strParsing = @"class=""Bio""";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"href='";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"'";
                    string strTemp = @"http://www.adultdvdempire.com" +
                                     strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    strResults = Util.GetHtmlPage(strTemp, Encoding.Default, BrowserType.Firefox4);
                }
                if (strResults != null)
                {
                    #region Aka

                    strParsing = @"Alias:";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                    if (intBegin > -1)
                    {
                        strResults = strResults.Substring(intBegin);
                        strParsing = @">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();

                        if (string.IsNullOrWhiteSpace(strTemp.Trim())==false)
                        objResults.Add("Aka", strTemp);
                    }
                    #endregion
                    #region Birthplace
                    strParsing = @"Birthplace:";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                    if (intBegin > -1)
                    {
                        strResults = strResults.Substring(intBegin);
                        strParsing = @">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();

                        if (string.IsNullOrWhiteSpace(strTemp.Trim()) == false)
                            objResults.Add("Birthplace", strTemp);
                    }
                    #endregion
                    #region Birthday
                    strParsing = @"Born On:";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                    if (intBegin > -1)
                    {
                        strResults = strResults.Substring(intBegin);
                        strParsing = @">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();

                        if (string.IsNullOrWhiteSpace(strTemp) == false)
                        {
                            DateTime birthday;
                            if (DateTime.TryParse(strTemp, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out birthday) == true)
                                if (birthday.Year < DateTime.Now.Year)
                                    objResults.Add("Birthday", birthday);
                        }
                    }

                    #endregion
                    #region Comments
                    strParsing = @"<p>";
                    intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                    if (intBegin > -1)
                    {
                        strResults = strResults.Substring(intBegin + strParsing.Length);
                        strParsing = @"</div";
                        string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                        if (string.IsNullOrWhiteSpace(strTemp.Trim()) == false)
                            objResults.Add("Comments", Util.PurgeHtml(strTemp));
                    }
                    #endregion
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
                string strUrlMaster = string.Format("http://www.adultdvdempire.com/dvd/search?q={0}", strSearch.Replace(" ", "+"));
                string strResults = Util.GetHtmlPage(strUrlMaster, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(strResults)) return null;

                if (strResults.IndexOf("Having Trouble Finding Something?", StringComparison.Ordinal) == -1)
                {
                    string strParsing = "ListView";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"div class=""item";
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
                string strParsing = @"container";

                if (strTemp.Contains(strParsing) == true)
                {
                    PartialMatche objMatche = new PartialMatche();

                    strParsing = @"href=";

                    int temp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                    if (temp > -1)
                    {
                        strTemp = strTemp.Substring(temp + strParsing.Length);
                        strParsing = @"/";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);

                        string strLink = string.Format(@"http://www.adultdvdempire.com/itempage.aspx?item_id={0}&partner_id={1}", strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)), "86947268");
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
            }

            return lstMatche;

        }
    }
}
