using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using myCollections.Data;
using myCollections.Utils;

namespace myCollections.BL.Providers
{
    static class FnacServices
    {
        public static Hashtable Parse(string strUrl, bool getPage, string strSearch)
        {
            Hashtable objResults = new Hashtable();

            try
            {
                string strResults;

                if (getPage == true)
                {
                    strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);
                    objResults.Add("Links", strUrl);
                }
                else
                    strResults = strUrl;

                if (string.IsNullOrWhiteSpace(strResults)) return null;

                objResults.Add("Title", strSearch);

                #region Tracks & Duration
                string duration;
                List<string> tracks = GetTracks(strResults, out duration);
                if (tracks != null)
                    objResults.Add("Tracks", tracks);
                if (string.IsNullOrWhiteSpace(duration) == false)
                    objResults.Add("Runtime", duration);
                #endregion
                #region Image
                string strParsing = @"zoom lienNosouligne";

                int intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"href=""";
                    int hRef = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                    strResults = strResults.Substring(hRef + strParsing.Length);
                    strParsing = @"""";

                    string strImage = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();

                    if (strImage.Contains("noscan_100x125.gif") == false)
                        objResults.Add("Image", strImage);


                }
                #endregion
                #region Album
                strParsing = @"<strong class=""titre dispeblock"">";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"</strong>";
                    objResults.Add("AlbumName", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                #region Artist
                strParsing = @"participants dispeblock"">";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    objResults.Add("Artist", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                #region Description
                strParsing = @"laSuite";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"<br />";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";

                    objResults.Add("Description", Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim()));
                }
                #endregion
                #region Rating
                strParsing = @"class=""note""";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"alt=""note ";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @" sur";

                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                        objResults.Add("Rating", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                else
                {
                    strParsing = @"Note moyenne des internautes";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @":";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"/";

                        intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intTmp > -1)
                        {
                            string strRating = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                            double dblRating;
                            if (double.TryParse(strRating, out dblRating))
                                objResults.Add("Rating", (int)dblRating * 4);

                        }
                    }
                }
                #endregion
                #region Comments
                if (objResults.ContainsKey("Description") == false)
                {
                    strParsing = @"Le Mot de l'éditeur";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        strResults = strResults.Substring(intTmp + strParsing.Length);
                        strParsing = @""">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";

                        objResults.Add("Comments", Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim()));
                    }
                    else
                    {
                        strParsing = @"coupDeCoeur pdg_b";
                        intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intTmp > -1)
                        {
                            strResults = strResults.Substring(intTmp + strParsing.Length);
                            strParsing = @"lireLaSuite mrg_v_sm";
                            strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @"<br";
                            strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @"</div>";

                            objResults.Add("Comments", Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim()));
                        }
                    }
                }
                #endregion
                #region Editor
                strParsing = @">Editeur";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"<A HREF";
                    intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp == -1)
                    {
                        strParsing = @"href=";
                        intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    }
                    if (intTmp > -1)
                    {
                        strResults =
                            strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) +
                                                 strParsing.Length);
                        strParsing = @">";
                        strResults =
                            strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) +
                                                 strParsing.Length);
                        strParsing = @"</";

                        objResults.Add("Editor",
                                       strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal))
                                                 .Trim());
                    }
                }
                #endregion
                #region ReleasedDate
                strParsing = @"Date de parution";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(intTmp + strParsing.Length);
                    strParsing = @"<span>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</span>";

                    objResults.Add("Released", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                #endregion
                strParsing = @"Artistes liés</h3>";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @" <div class=""part"">";

                    if (strResults.IndexOf(strParsing, StringComparison.Ordinal) > -1)
                        objResults.Add("LinkedArtist", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }

                #region Types
                strParsing = @"<strong>Voir aussi</strong>";
                intTmp = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp > -1)
                {
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<a href=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</ul>";
                    string strGenre = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                    strParsing = @"<li>";

                    string[] objTables = Regex.Split(strGenre, strParsing);
                    List<string> types = new List<string>();
                    foreach (string item in objTables)
                    {
                        if (IsBadTagFnac(item) == false)
                        {
                            strParsing = @">";
                            strGenre = item.Substring(item.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @"<";
                            strGenre = strGenre.Substring(0, strGenre.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                            if (objResults.ContainsKey("Artist"))
                            {
                                if (strGenre.Contains(objResults["Artist"].ToString()) == false)
                                    types.Add(Util.PurgeHtml(strGenre));
                            }
                            else
                                types.Add(Util.PurgeHtml(strGenre));
                        }
                    }
                    objResults.Add("Types", types);
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
        private static bool IsBadTagFnac(string strTag)
        {
            if (strTag.Contains("Attention Talent"))
                return true;

            if (strTag.Contains("Petits prix"))
                return true;

            if (strTag.Contains("4 CD ou DVD"))
                return true;

            if (strTag.Contains("albums"))
                return true;

            if (strTag.Contains("Coups de coeur"))
                return true;

            if (strTag.Contains("Meilleures"))
                return true;

            if (strTag.Contains("Nouveautés"))
                return true;

            if (strTag.Contains("Haut de page"))
                return true;

            if (strTag.Contains(@"Album de la semaine"))
                return true;

            return false;
        }
        private static List<string> GetTracks(string html, out string length)
        {
            List<string> tracks = new List<string>();
            length = string.Empty;
            double duration = 0;
            string strParsing = @"pistesaudios context";
            int intTmp = html.IndexOf(strParsing, StringComparison.Ordinal);
            if (intTmp > -1)
            {
                html = html.Substring(intTmp + strParsing.Length);
                strParsing = @"text/javascript";
                html = html.Substring(0, html.IndexOf(strParsing, StringComparison.Ordinal)).Trim();


                strParsing = @"numbertract";
                string[] objTables = Regex.Split(html, strParsing);
                foreach (string item in objTables)
                {

                    string temp = item.Trim();
                    strParsing = "titletrack";
                    intTmp = temp.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp > -1)
                    {
                        temp = temp.Substring(intTmp + strParsing.Length);
                        strParsing = ">";
                        temp = temp.Substring(temp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = "<";
                        temp = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                        if (temp.EndsWith("-"))
                            temp = temp.Remove(temp.LastIndexOf('-')).Trim();
                        tracks.Add(Util.PurgeHtml(temp));
                    }
                    else
                    {

                        strParsing = "nametract";
                        intTmp = temp.IndexOf(strParsing, StringComparison.Ordinal);
                        if (intTmp > -1)
                        {
                            temp = temp.Substring(intTmp + strParsing.Length);
                            strParsing = @"title=""";
                            temp = temp.Substring(temp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                            strParsing = @"""";
                            temp = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                            if (temp.EndsWith("-"))
                                temp = temp.Remove(temp.LastIndexOf('-')).Trim();
                            tracks.Add(Util.PurgeHtml(temp));
                            strParsing = @"duration"">";
                            intTmp = temp.IndexOf(strParsing, StringComparison.Ordinal);
                            if (intTmp > -1)
                            {
                                temp = temp.Substring(intTmp + strParsing.Length);
                                strParsing = @"<";
                                temp = temp.Substring(0, temp.IndexOf(strParsing, StringComparison.Ordinal));
                                if (temp.Length == 5)
                                    temp = "00:" + temp;

                                duration += TimeSpan.Parse(temp).TotalSeconds;

                            }
                        }
                    }

                }
            }

            if (duration > 0)
                length = duration.ToString(CultureInfo.InvariantCulture);
            if (tracks.Count > 0)
                return tracks;
            else
                return null;
        }

        public static Collection<PartialMatche> Search(string strAlbum, string strArtist)
        {
            try
            {
                string strUrl = string.Format("http://recherche.fnac.com/advanced/audio.do?isAdvanced=1&title={0}&exactTitle=true&track=&exactTrack=false&author={1}&collection=&label=&format=1000&startMonth=-1&startYear=-1&endMonth=-1&endYear=-1&x=45&y=14", Util.EncodeSearch(strAlbum), Util.EncodeSearch(strArtist));

                string strResults = Util.GetHtmlPage(strUrl, Encoding.Default);

                if (string.IsNullOrWhiteSpace(strResults)) return null;

                if (strResults.Contains(" 0 résultat") == false)
                {
                    const string strParsing = @"oneprd";

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
        private static Collection<PartialMatche> CreatePartialMatch(IEnumerable<string> items)
        {

            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();

            foreach (string item in items)
            {
                string strTemp = item.Trim();
                string parsing = @""">";

                if (strTemp.StartsWith(parsing))
                {
                    PartialMatche objMatche = new PartialMatche();
                    parsing = @"id=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(parsing, StringComparison.Ordinal) + parsing.Length);
                    parsing = @"""";
                    objMatche.Link = @"http://musique.fnac.com/" + strTemp.Substring(0, strTemp.IndexOf(parsing, StringComparison.Ordinal)) + @"/";

                    parsing = @"src=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(parsing, StringComparison.Ordinal) + parsing.Length);
                    parsing = @"""";
                    objMatche.ImageUrl = strTemp.Substring(0, strTemp.IndexOf(parsing, StringComparison.Ordinal));

                    parsing = @"<a href";
                    strTemp = strTemp.Substring(strTemp.IndexOf(parsing, StringComparison.Ordinal) + parsing.Length);
                    parsing = @">";
                    strTemp = strTemp.Substring(strTemp.IndexOf(parsing, StringComparison.Ordinal) + parsing.Length);
                    parsing = @"<";
                    objMatche.Title = strTemp.Substring(0, strTemp.IndexOf(parsing, StringComparison.Ordinal));

                    parsing = @"participants dispeblock"">";
                    strTemp = strTemp.Substring(strTemp.IndexOf(parsing, StringComparison.Ordinal) + parsing.Length);
                    parsing = @">";
                    strTemp = strTemp.Substring(strTemp.IndexOf(parsing, StringComparison.Ordinal) + parsing.Length);
                    parsing = @"<";
                    objMatche.Artist = strTemp.Substring(0, strTemp.IndexOf(parsing, StringComparison.Ordinal));

                    lstMatche.Add(objMatche);
                }
            }
            return lstMatche;

        }
    }
}
