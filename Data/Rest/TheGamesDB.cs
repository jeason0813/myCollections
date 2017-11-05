using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Linq;
using myCollections.Utils;

namespace myCollections.Data.Rest
{
    internal class TheGamesDb
    {
        public string Background { get; private set; }
        public string Cover { get; private set; }
        public string Description { get; private set; }
        public string Id { get; private set; }
        public string Link { get; private set; }
        public string Platform { get; private set; }
        public string Rated { get; private set; }
        public string Rating { get; private set; }
        public DateTime? Released { get; private set; }
        public string ScreenShot { get; private set; }
        public string Studio { get; private set; }
        public string Title { get; private set; }
        public List<string> Types { get; private set; }

        public static Collection<PartialMatche> GamesToPartialMatche(XElement objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            var query = from item in objRest.Descendants("Game")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();

                objItem.Title = Util.GetElementValue(objTemp, "GameTitle");
                objItem.Link = Util.GetElementValue(objTemp, "id");

                string plateform = Util.GetElementValue(objTemp, "Platform");

                objItem.Title = objItem.Title + " - " + plateform;

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static TheGamesDb GamesToObject(XElement objRest)
        {
            if (objRest == null) return null;

            TheGamesDb objItem = null;
            var query = from item in objRest.Descendants("Game")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objItem = new TheGamesDb();

                if (objTemp != null)
                {
                    objItem.Id = Util.GetElementValue(objTemp, "id");
                    objItem.Link = string.Format(@"http://thegamesdb.net/game/{0}/", objItem.Id);
                    objItem.Title = Util.GetElementValue(objTemp, "GameTitle");
                    objItem.Platform = Util.GetElementValue(objTemp, "Platform");
                    objItem.Description = Util.GetElementValue(objTemp, "Overview");

                    string relased = Util.GetElementValue(objTemp, "ReleaseDate");
                    if (string.IsNullOrWhiteSpace(relased) == false)
                    {
                        DateTime date;
                        if (DateTime.TryParse(relased, out date) == true)
                            objItem.Released = date;
                    }

                    objItem.Rated = Util.GetElementValue(objTemp, "ESRB");

                    var genres = from item in objTemp.Descendants("genre")
                                 select item.Value;

                    objItem.Types = new List<string>(genres.ToList());
                    objItem.Studio = Util.GetElementValue(objTemp, "Publisher");
                    objItem.Rating = Util.GetElementValue(objTemp, "Rating");

                    var covers = from item in objTemp.Descendants("boxart")
                                    let attribute1 = item.Attribute("side")
                                    where attribute1 != null && attribute1.Value == "front" 
                                    select item.Value;

                    var background = from item in objTemp.Descendants("fanart")
                                     let attribute1 = item.Element("original")
                                     select attribute1.Value;

                    var screenshot = from item in objTemp.Descendants("screenshot")
                                     let attribute1 = item.Element("original")
                                     select attribute1.Value;

                    string[] images = covers.ToArray();
                    if (images.Length > 0)
                        objItem.Cover=images[0];

                    images = background.ToArray();
                    if (images.Length > 0)
                        objItem.Background = images[0];

                    images = screenshot.ToArray();
                    if (images.Length > 0)
                        objItem.ScreenShot = images[0];
                    
                }
            }

            return objItem;
        }


    }
}
