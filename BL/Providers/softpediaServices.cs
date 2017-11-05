using myCollections.Data;
using myCollections.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace myCollections.BL.Providers
{
    static class SoftpediaServices
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

                #region Image
                string strParsing = @"screenshots"">";
                int intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"src=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    objResults.Add("Image", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)));
                }
                #endregion
                #region Rating
                strParsing = @"id=""rater__upd"">";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"(";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    strResults = strResults.Substring(intTmp + strParsing.Length);

                    strParsing = "/";
                    string strRating = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    float floatRating;

                    strRating = strRating.Replace('.', ',');

                    if (float.TryParse(strRating, out floatRating) == true)
                        objResults.Add("Rating", floatRating * 4);
                }
                #endregion
                #region Description
                strParsing = @"<div class=""desch2"">";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"<p>";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                        strResults = strResults.Substring(intTmp);
                    strParsing = "</p>";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                    objResults.Add("Description", Util.PurgeHtml(strResults.Substring(0, intTmp)));
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
        public static Collection<PartialMatche> Search(string strSearch)
        {
            try
            {
                const string strUrl = "http://www.softpedia.com/dyn-search.php";
                NameValueCollection objValues = new NameValueCollection();

                objValues.Add("search_term", strSearch);

                string strResults = Util.GetHtmlPage(strUrl, objValues);

                if (strResults.IndexOf(strSearch, StringComparison.CurrentCultureIgnoreCase) > -1
                    && strResults.IndexOf("No results were found for your search term and conditions", StringComparison.Ordinal) == -1)
                {
                    string strParsing = @"<strong>REGULAR SEARCH RESULTS</strong>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<h2><a href=""";
                    string[] objTables = Regex.Split(strResults, strParsing);

                    if (objTables.Count() > 1)
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
        private static Collection<PartialMatche> CreatePartialMatch(IEnumerable<string> strResults)
        {

            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in strResults)
            {
                string strTemp = item.Trim();
                string strParsing = @"""";

                if (strTemp.StartsWith(@"http:"))
                {
                    PartialMatche objMatche = new PartialMatche();

                    objMatche.Link = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));

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
