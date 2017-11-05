using myCollections.Data;
using myCollections.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace myCollections.BL.Providers
{
    static class TucowsServices
    {
        public static Hashtable Parse(string strUrl, string strSearch)
        {
            Hashtable objResults = new Hashtable();

            try
            {

                string strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);
                if (string.IsNullOrEmpty(strResults))
                    return null;

                objResults.Add("Title", strSearch);
                objResults.Add("Links", strUrl);

                #region ReleaseDate
                string strParsing = @"Published:</div>";
                int intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);

                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);

                    strParsing = @"<";
                    objResults.Add("Released", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                #region Editor
                strParsing = @"Published by:</div>";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"<a href=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @""">";
                    objResults.Add("EditorLink", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());

                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</a>";
                    objResults.Add("Editor", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                #region Rating
                strParsing = @"Popularity:</div>";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";

                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    if (string.IsNullOrEmpty(strTemp) == false)
                    {
                        strTemp = strTemp.Replace("%", "");
                        if (Util.IsNumeric(strTemp) == true)
                        {
                            intTmp = Convert.ToInt32(strTemp, CultureInfo.InvariantCulture);
                            objResults.Add("Rating", intTmp / 10);
                        }
                    }
                }
                #endregion
                #region Image
                strParsing = @"screenshot.html?id=";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.InvariantCultureIgnoreCase) + strParsing.Length);
                strParsing = @"'";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                    objResults.Add("Image", "http://www-s.tucows.com/new/static/images/shots/windows/" + strResults.Substring(0, intTmp).Trim() + ".jpg");
                #endregion
                #region Description
                strParsing = @"<p class=""description_big"">";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"</p>";
                    objResults.Add("Description", Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim()));
                }
                #endregion
                #region Type
                strParsing = @"<span class=""categ"">";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    string[] strArray = strResults.Split(new[] { strParsing }, StringSplitOptions.RemoveEmptyEntries);
                    Collection<string> lstTypes = new Collection<string>();
                    foreach (string item in strArray)
                    {
                        strParsing = @"</span>";
                        lstTypes.Add(item.Substring(0, item.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                    }
                    objResults.Add("Types", lstTypes);
                }
                #endregion
                return objResults;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                return null;
            }
        }

        public static Collection<PartialMatche> Search(string strSearch)
        {

            try
            {
                string strUrl = string.Format("http://www.tucows.com/search.html?search_scope=win&search_terms={0}&search_type=soft", strSearch.Replace(" ", "+"));
                string strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);

                //FIX since 2.7.11.0
                if (string.IsNullOrWhiteSpace(strResults) == false)
                {
                    if (strResults.IndexOf("There are currently no matching search results.", StringComparison.Ordinal) ==-1)
                    {

                        string strParsing = @"<div class=""search_result_box"" >";
                        int intEnd = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intEnd == -1)
                            strParsing = @"<div class=""search_result_box"">";

                        strResults =
                            strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) +
                                                 strParsing.Length);
                        strParsing = @"search_result_title"">";

                        string[] objTables = Regex.Split(strResults, strParsing);

                        if (objTables.Count() > 1)
                            return CreatePartialMatch(objTables);
                        else
                            return null;
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
                string strParsing = @"<a href=""preview";

                if (strTemp.StartsWith(strParsing))
                {
                    PartialMatche objMatche = new PartialMatche();
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objMatche.Link = @"http://www.tucows.com/preview" + strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    strTemp = strTemp.Replace("<em>", "");
                    strTemp = strTemp.Replace(@"</em>", "");

                    strParsing = @">";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    objMatche.Title = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

                    lstMatche.Add(objMatche);
                }
            }

            return lstMatche;
        }

    }
}
