using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System.Collections.Generic;

namespace myCollections.Data.Rest
{
    internal class TVDB
    {
        public string Id { get; set; }
        public string Language { get; set; }
        public string SeriesName { get; set; }
        public string Banner { get; set; }
        public string Poster { get; set; }
        public string FanArt { get; set; }
        public string Overview { get; set; }
        public string ImdbID { get; set; }
        public double? Rating { get; set; }
        public string Runtime { get; set; }
        public string Status { get; set; }
        public string Released { get; set; }
        public string Studio { get; set; }
        public List<string> Genres { get; set; }


        public static Collection<TVDB> SerieToCollection(XElement objRest)
        {
            if (objRest == null) return null;

            Collection<TVDB> lstResults = new Collection<TVDB>();
            var query = from item in objRest.Descendants("Series")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                TVDB objItem = new TVDB();

                objItem.SeriesName = Util.GetElementValue(objTemp, "SeriesName");
                objItem.Id = Util.GetElementValue(objTemp, "seriesid");

                string banner = Util.GetElementValue(objTemp, "banner");
                if (!string.IsNullOrEmpty(banner))
                    objItem.Banner = "http://www.thetvdb.com/banners/" + banner;

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static TVDB SerieToObject(XElement objRest)
        {
            if (objRest == null) return null;

            TVDB objItem = null;
            var query = from item in objRest.Descendants("Series")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objItem = new TVDB();

                objItem.Id = Util.GetElementValue(objTemp, "id");

                string banner = Util.GetElementValue(objTemp, "banner");
                if (!string.IsNullOrEmpty(banner))
                    objItem.Banner = "http://www.thetvdb.com/banners/" + banner;

                string fanArt = Util.GetElementValue(objTemp, "fanart");
                if (!string.IsNullOrEmpty(fanArt))
                    objItem.FanArt = "http://www.thetvdb.com/banners/" + fanArt;

                string genres = Util.GetElementValue(objTemp, "Genre");
                if (!string.IsNullOrEmpty(genres))
                    objItem.Genres = new List<string>(genres.Split('|').ToArray());

                objItem.ImdbID = Util.GetElementValue(objTemp, "IMDB_ID");
                objItem.Language = Util.GetElementValue(objTemp, "Language");
                objItem.Overview = Util.GetElementValue(objTemp, "Overview");
                objItem.Released = Util.GetElementValue(objTemp, "FirstAired");
                objItem.Studio = Util.GetElementValue(objTemp, "Network");

                string poster = Util.GetElementValue(objTemp, "poster");
                if (!string.IsNullOrEmpty(poster))
                    objItem.Poster = "http://www.thetvdb.com/banners/" + poster;

                string strrating = Util.GetElementValue(objTemp, "Rating"); 
                double value;
                CultureInfo cultureInfo = new CultureInfo("en-US", true);
                if (double.TryParse(strrating, NumberStyles.Any, cultureInfo.NumberFormat, out value) == true)
                    objItem.Rating = (long)(value * 2);
                else if (double.TryParse(strrating.Replace(',', '.'), out value) == true)
                    objItem.Rating = (long)(value * 2);
               
                objItem.Runtime = Util.GetElementValue(objTemp, "Runtime");
                objItem.SeriesName = Util.GetElementValue(objTemp, "SeriesName");
                objItem.Status = Util.GetElementValue(objTemp, "Status");

            }

            return objItem;
        }
        public static IEnumerable<Artist> CastToArtist(XElement objRest)
        {
            if (objRest == null) return null;

            List<Artist> lstResults = new List<Artist>();
            var query = from item in objRest.Descendants("Actor")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                Artist objItem = new Artist();
                XElement objTemp = node;
                string[] name = Util.GetElementValue(objTemp, "Name").Split(' ');
                foreach (string item in name)
                {
                    if (string.IsNullOrEmpty(objItem.FirstName))
                        objItem.FirstName = item;
                    else
                        objItem.LastName += " " + item;
                }

                if (objItem.LastName != null)
                    objItem.LastName = objItem.LastName.Trim();
                else
                    objItem.LastName = string.Empty;

                objItem.FulleName = objItem.FirstName + " " + objItem.LastName;

                string picture = Util.GetElementValue(objTemp, "Image");
                if (!string.IsNullOrEmpty(picture))
                    objItem.Picture = Util.GetImage("http://www.thetvdb.com/banners/" + picture);

                lstResults.Add(objItem);
            }
            return lstResults;
        }

        public static Collection<PartialMatche> SerieToPartialMatche(XElement objRest)
        {
            if (objRest == null) return null;

            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            var query = from item in objRest.Descendants("Series")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();

                objItem.Title = Util.GetElementValue(objTemp, "SeriesName");
                objItem.Link = Util.GetElementValue(objTemp, "seriesid");

                string banner = Util.GetElementValue(objTemp, "banner");
                if (!string.IsNullOrEmpty(banner))
                    objItem.ImageUrl = "http://www.thetvdb.com/banners/" + banner;

                lstResults.Add(objItem);
            }
            return lstResults;
        }
    }
}

