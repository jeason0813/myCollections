using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.BL.Providers
{
    static class IafdServices
    {
        public static Hashtable SearchPortrait(string strSearch, bool usePartialMatche)
        {

            try
            {
                string strUrlMaster = string.Format("http://www.iafd.com/results.asp?searchtype=name&searchstring={0}", strSearch.Replace(" ", "+"));
                string strResults = Util.GetHtmlPage(strUrlMaster, Encoding.UTF8, BrowserType.Firefox4);

                if (string.IsNullOrWhiteSpace(strResults) == false)
                {
                    if (strResults.IndexOf(">None Found.<", StringComparison.Ordinal) == -1)
                    {
                        string strParsing = @"<img style=""margin-right:";
                        string[] objTables = Regex.Split(strResults, strParsing);

                        if (usePartialMatche && objTables.Length > 2)
                            return ShowPartialMatch(objTables);
                        else
                        {
                            if (objTables.Length > 1)
                            {
                                strResults = objTables[1];
                                strParsing = @"href=""";
                                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                                strParsing = @"""";
                                string strLink = @"http://www.iafd.com/" + strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                                return Parse(strLink);
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
        private static Hashtable ShowPartialMatch(IEnumerable<string> objTables)
        {
            if (objTables == null) return null;

            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in objTables)
            {
                string strTemp = item.Trim();
                string strParsing = @"0.5em";

                if (strTemp.StartsWith(strParsing) == true)
                {
                    PartialMatche objMatche = new PartialMatche();

                    strParsing = @"src=""";

                    int temp = strTemp.IndexOf(strParsing, StringComparison.Ordinal);
                    if (temp > -1)
                    {
                        strTemp = strTemp.Substring(temp + strParsing.Length);
                        strParsing = @"""";
                        objMatche.ImageUrl = @"http://www.iafd.com" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                        strParsing = @"href=""";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"""";
                        string strLink = @"http://www.iafd.com/" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                        objMatche.Link = strLink;

                        strParsing = @">";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        objMatche.Title = Util.PurgeHtml(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));

                        lstMatche.Add(objMatche);
                    }
                }
            }

            partialMatch objWindow = new partialMatch(lstMatche);
            objWindow.Title = "www.iafd.com" + " - " + objWindow.Title;
            objWindow.ShowDialog();

            if (string.IsNullOrEmpty(objWindow.SelectedLink) == false)
                return Parse(objWindow.SelectedLink);
            else
                return null;

        }
        private static Hashtable Parse(string strUrl)
        {
            Hashtable objResults = new Hashtable();

            try
            {
                string strResults = Util.GetHtmlPage(strUrl, Encoding.Default, BrowserType.Firefox4);

                #region Image
                string strParsing = @"id=""headshot""";
                int intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"src=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (strTemp.StartsWith(@"http://"))
                        objResults.Add("Image", strTemp);
                    else
                        objResults.Add("Image", @"http://www.iafd.com" + strTemp);
                }
                #endregion
                #region Aka
                strParsing = @"Performer AKA";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Aka", strTemp);
                }
                #endregion
                #region Birthday
                strParsing = @"Birthday";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                    {
                        strParsing = @">";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intBegin > -1)
                        {
                            strTemp = strTemp.Substring(0, intBegin);

                            if (string.IsNullOrWhiteSpace(strTemp) == false)
                            {
                                DateTime birthday;
                                if (DateTime.TryParse(strTemp, out birthday) == true)
                                    objResults.Add("Birthday", birthday);
                            }
                        }
                    }
                }
                #endregion
                #region Astrology
                strParsing = @"Astrology";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                    {
                        strParsing = @">";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        strTemp = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                        objResults.Add("Astrology", strTemp);
                    }
                }
                #endregion
                #region Birthplace
                strParsing = @"Birthplace";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Birthplace", strTemp);
                }
                #endregion
                #region Years Active
                strParsing = @"Years Active";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("YearsActive", strTemp);
                }
                #endregion
                #region Ethnicity
                strParsing = @"Ethnicity";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Ethnicity", strTemp);
                }
                #endregion
                #region Nationality/Heritage
                strParsing = @"Nationality/Heritage";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Nationality/Heritage", strTemp);
                }
                #endregion
                #region Hair Color
                strParsing = @"Hair Color";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Hair Color", strTemp);
                }
                #endregion
                #region Measurements
                strParsing = @"Measurements";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Measurements", Util.BraConverter(strTemp.Split('-')[0]));
                }
                #endregion
                #region Height
                strParsing = @"Height";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Height", strTemp);
                }
                #endregion
                #region Weight
                strParsing = @"Weight";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Weight", strTemp);
                }
                #endregion
                #region Tattoos
                strParsing = @"Tattoos";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Tattoos", strTemp);
                }
                #endregion
                #region Piercings
                strParsing = @"Piercings";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "No data")
                        objResults.Add("Piercings", strTemp);
                }
                #endregion
                #region Comments
                strParsing = @"Comments";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<span";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp != "There are no comments for this performer.")
                        objResults.Add("Comments", Util.PurgeHtml(strTemp));
                }
                #endregion
                #region Website
                strParsing = @"Website";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<td>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</td>";
                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));

                    if (string.IsNullOrWhiteSpace(strTemp) == false && strTemp.Contains("No data") == false)
                    {
                        strParsing = @">";
                        strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";
                        strTemp = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                        objResults.Add("Website", strTemp);
                    }
                }
                #endregion
                #region Credits
                strParsing = @"id=""personal""";
                intBegin = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                if (intBegin > -1)
                {
                    strResults = strResults.Substring(intBegin);
                    strParsing = @"<tr";

                    string[] rows = Regex.Split(strResults, strParsing);

                    List<ArtistCredits> credits = new List<ArtistCredits>();
                    foreach (string row in rows)
                    {
                        if (row.StartsWith(" class="))
                        {
                            strParsing = "href=";
                            string temp = row.Substring(row.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = ">";
                            temp = temp.Substring(temp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = "<";
                            string title = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal));

                            strParsing = "<i>";
                            string notes = string.Empty;
                            int index = temp.IndexOf(strParsing, StringComparison.Ordinal);
                            if (index > -1)
                            {
                                temp = temp.Substring(index + strParsing.Length);
                                strParsing = "</i>";
                                notes = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal));
                            }

                            strParsing = "<td>";
                            temp = temp.Substring(temp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = "</td>";
                            string year = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal));

                            strParsing = @"href=""/reviewpage";
                            index = temp.IndexOf(strParsing, StringComparison.Ordinal);
                            if (index > -1)
                                temp = temp.Substring(index + strParsing.Length);

                            strParsing = @"href=""";
                            index = temp.IndexOf(strParsing, StringComparison.Ordinal);

                            string buyLink = string.Empty;

                            if (index > -1)
                            {
                                temp = temp.Substring(index + strParsing.Length);
                                strParsing = @"""";
                                buyLink = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal));
                            }

                            ArtistCredits movie = new ArtistCredits();
                            movie.Title = title;

                            if (string.IsNullOrWhiteSpace(notes) == false)
                                movie.Notes = notes;

                            movie.EntityType = EntityType.XXX;

                            int relaeseDate;
                            if (int.TryParse(year, out relaeseDate) == true)
                                movie.ReleaseDate = new DateTime(relaeseDate, 1, 1);

                            if (string.IsNullOrWhiteSpace(buyLink) == false)
                                movie.BuyLink = @"http://www.iafd.com" + buyLink;

                            credits.Add(movie);
                        }
                    }

                    objResults.Add("Credits", credits);
                }
                #endregion

                return objResults;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, strUrl);
                return null;
            }
        }
    }
}
