
using System.Web;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
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
    static class JeuxVideoComServices
    {
        private static string GetPlateform(string url)
        {
            if (url.Contains(@"/pc/"))
                return "PC";
            else if (url.Contains(@"/playstation-3-ps3/"))
                return "PS3";
            else if (url.Contains(@"/xbox-360/"))
                return "XBox 360";
            else if (url.Contains(@"/wii/"))
                return "Wii";
            else if (url.Contains(@"/playstation-2-ps2/"))
                return "PS2";
            else if (url.Contains(@"/nintendo-ds/"))
                return "Nitnedo DS";
            else if (url.Contains(@"/nintendo-3ds/"))
                return "Nitnedo 3DS";
            else if (url.Contains(@"/playstation-portable-psp/"))
                return "PSP";
            else if (url.Contains(@"/web/"))
                return "Web";
            else if (url.Contains(@"/iphone-ipod/"))
                return "iPhone";
            else if (url.Contains(@"/ipad/"))
                return "iPad";
            else if (url.Contains(@"/xbox/"))
                return "Xbox";
            else if (url.Contains(@"/gamecube-ngc/"))
                return "GameCube";
            else if (url.Contains(@"/gameboy-g-boy/"))
                return "GameBoy";
            else if (url.Contains(@"/gameboy-advance-gba/"))
                return "GBA";
            else if (url.Contains(@"/playstation-ps1/"))
                return "PSX";
            else if (url.Contains(@"/nintendo-64-n64/"))
                return "N64";
            else if (url.Contains(@"/super-nintendo-snes/"))
                return "SNes";
            else if (url.Contains(@"/megadrive/"))
                return "MegaDrive";
            else if (url.Contains(@"/megadrive-32x/"))
                return "MegaDrive 32";
            else if (url.Contains(@"/mega-cd/"))
                return "Mega CD";
            else if (url.Contains(@"/saturn/"))
                return "Saturn";
            else if (url.Contains(@"/nes/"))
                return "Nes";
            else if (url.Contains(@"/master-system/"))
                return "MasterSystem";
            else if (url.Contains(@"/game-gear-g-gear/"))
                return "GameGear";
            else if (url.Contains(@"/amiga/"))
                return "Amiga";
            else if (url.Contains(@"/n-gage-ngage/"))
                return "Ngage";
            else if (url.Contains(@"/3do/"))
                return "3Do";
            else if (url.Contains(@"/gizmondo/"))
                return "Gizmondo";
            else if (url.Contains(@"/dreamcast-dcast/"))
                return "Dreamcast";
            else
                return string.Empty;
        }
        public static Hashtable Parse(string strUrl, string strSearch)
        {
            Hashtable objResults = new Hashtable();

            try
            {
                string strResults = Util.GetHtmlPage(strUrl, Encoding.UTF8);

                if (strResults == null) return null;

                objResults.Add("Title", strSearch);
                objResults.Add("Links", strUrl);

                #region Description
                string strParsing = @"Descriptif :";
                int intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp1 > -1)
                {
                    strResults = strResults.Substring(intTmp1 + strParsing.Length);
                    strParsing = @"</li>";
                    objResults.Add("Description", Util.PurgeHtml(HttpUtility.HtmlDecode(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim())));
                }
                #endregion
                #region Editor
                strParsing = @"Editeur :";
                intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp1 > -1)
                {
                    strResults = strResults.Substring(intTmp1 + strParsing.Length);
                    strParsing = @""">";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</a>";
                    objResults.Add("Editor", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                }
                else
                {
                    strParsing = @"<strong>D&amp;eacute;veloppeur :</strong>";
                    intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp1 > -1)
                    {
                        strResults = strResults.Substring(intTmp1 + strParsing.Length);
                        strParsing = @""">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"</a>";
                        objResults.Add("Editor", strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim());
                    }
                }
                #endregion
                #region Types
                strParsing = @"<strong>Type :</strong>";
                intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp1 > -1)
                {
                    strResults = strResults.Substring(intTmp1 + strParsing.Length);
                    strParsing = @"</li>";
                    string types = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                    if (types.Contains(@""">"))
                    {
                        string[] lstTypes = Regex.Split(types, @""">");
                        List<Genre> gamesTypes = new List<Genre>();
                        foreach (string lstType in lstTypes)
                        {
                            if (lstType.StartsWith("<") == false)
                            {
                                strParsing = @"<";
                                string item = lstType.Substring(0, lstType.IndexOf(strParsing, StringComparison.Ordinal));
                                Genre gametype = GenreServices.GetGenre(item,EntityType.Games);
                                gamesTypes.Add(gametype);
                            }
                        }

                        objResults.Add("Types", gamesTypes);
                    }
                    else
                        objResults.Add("Types", new List<string> { types });
                }
                #endregion
                #region Released Date
                strParsing = @"<strong>Sortie France :</strong>";
                intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp1 > -1)
                {
                    strResults = strResults.Substring(intTmp1 + strParsing.Length);
                    strParsing = @"</li>";

                    intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    strParsing = @"<br";
                    int intTmp2 = strResults.IndexOf(strParsing, StringComparison.Ordinal);

                    if (intTmp2 == -1 || intTmp2 > intTmp1)
                        objResults.Add("Released", strResults.Substring(0, intTmp1).Trim());
                    else
                        objResults.Add("Released", strResults.Substring(0, intTmp2).Trim());
                }
                #endregion
                strParsing = @"note_redac";
                intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp1 > -1)
                {
                    strResults = strResults.Substring(intTmp1 + strParsing.Length);
                    strParsing = @":";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";

                    string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                    if (string.IsNullOrEmpty(strTemp) == false)
                    {
                        strTemp = strTemp.Trim().Split('/')[0];
                        if (Util.IsNumeric(strTemp) == true)
                        {
                            int intTmp = Convert.ToInt32(strTemp, CultureInfo.InvariantCulture);
                            objResults.Add("Rating", intTmp);
                        }
                    }

                    strParsing = @"<li>";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"</li>";

                    objResults.Add("Comments", Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.InvariantCultureIgnoreCase)).Trim()));
                }
                if (objResults.ContainsKey("Rating") == false)
                {
                    strParsing = @"Note moyenne des Lecteurs :";
                    intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp1 > -1)
                    {
                        strResults = strResults.Substring(intTmp1 + strParsing.Length);
                        strParsing = @"<";

                        string strTemp = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                        if (string.IsNullOrEmpty(strTemp) == false)
                        {
                            strTemp = strTemp.Trim().Split('/')[0];
                            if (Util.IsNumeric(strTemp) == true)
                            {
                                int intTmp = Convert.ToInt32(strTemp, CultureInfo.InvariantCulture);
                                objResults.Add("Rating", intTmp);
                            }
                        }
                    }
                }
                if (objResults.ContainsKey("Comments") == false)
                {
                    strParsing = @"Dernier avis :";
                    intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                    if (intTmp1 > -1)
                    {
                        strResults = strResults.Substring(intTmp1 + strParsing.Length);
                        strParsing = @">";
                        strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"<";

                        objResults.Add("Comments", Util.PurgeHtml(strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.InvariantCultureIgnoreCase)).Trim()));
                    }
                }
                #region Image
                strParsing = @"li class=""recto_verso"">";
                intTmp1 = strResults.IndexOf(strParsing, StringComparison.Ordinal);
                if (intTmp1 > -1)
                {
                    strResults = strResults.Substring(intTmp1 + strParsing.Length);
                    strParsing = @"<a href=""";
                    strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    strResults = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                    strParsing = @"affpic.htm?";

                    if (strResults.Contains(strParsing))
                        strResults = strResults.Replace(strParsing, "");

                    if (strResults.Contains(".htm") == false)
                        objResults.Add("Image", strResults);
                    else
                    {
                        string pageimage = Util.GetHtmlPage(strResults, Encoding.GetEncoding("iso-8859-1"));
                        strParsing = strResults;
                        pageimage = pageimage.Substring(pageimage.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"src='";
                        pageimage = pageimage.Substring(pageimage.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                        strParsing = @"'";
                        pageimage = pageimage.Substring(0, pageimage.IndexOf(strParsing, StringComparison.Ordinal)).Trim();
                        objResults.Add("Image", pageimage);
                    }
                }
                #endregion
                objResults.Add("Platform", GetPlateform(strUrl));

                return objResults;

            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                return null;
            }
        }

        public static Collection<PartialMatche> Search(string strSearch, string plateform)
        {
            try
            {
                string strUrl;
                if (string.IsNullOrWhiteSpace(plateform))
                    strUrl = string.Format(@"http://www.jeuxvideo.com/recherche/jeux/{0}.htm", strSearch);
                else
                    strUrl = string.Format(@"http://www.jeuxvideo.com/recherche/jeux/{0}/{1}.htm", strSearch, plateform);

                string strResults = Util.GetHtmlPage(strUrl, Encoding.Default);

                if (string.IsNullOrWhiteSpace(strResults)) return null;

                string strParsing = @"Désolé, aucun résultat trouvé !";
                if (strResults.Contains(strParsing)) return null;

                strParsing = @"liste_liens on";
                strResults = strResults.Substring(strResults.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                strParsing = @"<script";
                strResults = strResults.Substring(0, strResults.IndexOf(strParsing, StringComparison.Ordinal));
                strParsing = @"<li>";

                string[] objTables = Regex.Split(strResults, strParsing);

                if (objTables.Length > 1)
                    return CreatePartialMatch(objTables);
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
                string strParsing = @"<a href=""h";

                if (strTemp.StartsWith(strParsing) == true)
                {
                    PartialMatche objMatche = new PartialMatche();

                    strParsing = @"href=""";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"""";
                    string strLink = strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal));
                    objMatche.Link = strLink;

                    strParsing = @""">";
                    strTemp = strTemp.Substring(strTemp.IndexOf(strParsing, StringComparison.Ordinal) + strParsing.Length);
                    strParsing = @"<";
                    objMatche.Title = Util.PurgeHtml(strTemp.Substring(0, strTemp.IndexOf(strParsing, StringComparison.Ordinal)));

                    string plateform = GetPlateform(strLink);
                    if (string.IsNullOrWhiteSpace(plateform) == false)
                        objMatche.Title += " - " + plateform;

                    lstMatche.Add(objMatche);
                }
            }

            return lstMatche;

        }
    }
}
