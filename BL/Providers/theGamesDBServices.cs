using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Linq;

namespace myCollections.BL.Providers
{
    class TheGamesDbServices
    {
        private const string Url = @"http://thegamesdb.net/api/";
        public static Collection<PartialMatche> Search(string strSearch, GamesPlateform plateform)
        {
            if (string.IsNullOrEmpty(strSearch) == false)
            {

                Uri strUrl;
                switch (plateform)
                {
                    case GamesPlateform.All:
                        strUrl = new Uri(string.Format(Url + @"/GetGamesList.php?name={0}", strSearch));
                        break;
                    case GamesPlateform.Nds:
                        strUrl =
                            new Uri(string.Format(Url + @"/GetGamesList.php?name={0}&platform={1}", strSearch,
                                "Nintendo DS"));
                        break;
                    default:
                        strUrl = new Uri(string.Format(Url + @"/GetGamesList.php?name={0}", strSearch));
                        break;
                }

                string results = Util.GetRest(strUrl);
                if (string.IsNullOrEmpty(results) == false)
                {
                    XElement restResponse = XElement.Parse(results);
                    return TheGamesDb.GamesToPartialMatche(restResponse);
                }
                else
                    return null;
            }
            else
                return null;
        }
        public static Hashtable Parse(string strId)
        {
            Hashtable objResults = new Hashtable();
            try
            {
                const string imageUrl = @"http://thegamesdb.net/banners/";
                Uri strUrl = new Uri(string.Format(Url + @"/GetGame.php?id={0}", strId));
                string results = Util.GetRest(strUrl);
                
                if (string.IsNullOrWhiteSpace(results)) return null;

                XElement restResponse =  XElement.Parse(results);

                TheGamesDb game = TheGamesDb.GamesToObject(restResponse);
                if (game != null)
                {
                    if (string.IsNullOrWhiteSpace(game.Background) == false)
                        objResults.Add("Background", imageUrl + game.Background);

                    if (string.IsNullOrWhiteSpace(game.Cover) == false)
                        objResults.Add("Image", imageUrl + game.Cover);

                    if (string.IsNullOrWhiteSpace(game.Description) == false)
                        objResults.Add("Description", game.Description);

                    if (string.IsNullOrWhiteSpace(game.Platform) == false)
                        objResults.Add("Platform", game.Platform);

                    if (string.IsNullOrWhiteSpace(game.Rating) == false)
                    {
                        double intTmp;
                        if (double.TryParse(game.Rating, NumberStyles.Any, new CultureInfo("en-US", true), out intTmp))
                            objResults.Add("Rating", intTmp*2);
                    }

                    if (game.Released != null)
                        objResults.Add("Released", game.Released);

                    if (string.IsNullOrWhiteSpace(game.Studio) == false)
                        objResults.Add("Editor", game.Studio);

                    if (string.IsNullOrWhiteSpace(game.Title) == false)
                        objResults.Add("Title", game.Title);

                    if (string.IsNullOrWhiteSpace(game.Link) == false)
                        objResults.Add("Links", game.Link);

                    List<Genre> gamesTypes = new List<Genre>();
                    foreach (string item in game.Types)
                    {
                        Genre gametype = GenreServices.GetGenre(item,EntityType.Games);
                        gamesTypes.Add(gametype);
                    }

                    objResults.Add("Types", gamesTypes);
                }

                return objResults;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, strId);
                return null;
            }
        }

    }
}
