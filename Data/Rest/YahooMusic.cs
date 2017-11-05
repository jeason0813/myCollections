using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using myCollections.Utils;
using System.Collections.Generic;

namespace myCollections.Data.Rest
{
    internal class YahooMusic
    {
        public string artistName { get; private set; }
        private string artistUrl { get; set; }

        public string albumBarcode { get; set; }
        public string albumName { get; set; }
        public string albumUrl { get; set; }
        private string albumFlags { get; set; }
        public string albumImage { get; set; }
        public string albumId { get; set; }
        public string albumRating { get; set; }
        public string albumReleased { get; set; }

        public List<string> albumTracks { get; set; }
        public List<string> albumTypes { get; set; }
        public int albumDuration { get; set; }

        public string trackId { get; set; }
        public string studio { get; set; }

        public static Collection<YahooMusic> albumToCollection(XElement objRest)
        {
            if (objRest == null) return null;

            Collection<YahooMusic> lstResults = new Collection<YahooMusic>();
            var query = from item in objRest.Descendants("Release")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;

                YahooMusic objItem = new YahooMusic();
                objItem.albumId = Util.GetAttributValue(objTemp, "id");
                objItem.albumName = Util.GetAttributValue(objTemp, "title");

                var artist = from item in objTemp.Descendants("Artist")
                             select item;
                XElement[] nodeArtist = artist.ToArray();

                XElement objArtist = nodeArtist[0];
                objItem.artistName = Util.GetAttributValue(objArtist, "name");

                var image = from item in objTemp.Descendants("Image")
                            select item;
                XElement[] nodeImage = image.ToArray();

                if (nodeImage.Any())
                {
                    XElement objImage = nodeImage[nodeImage.Count() - 1];
                    objItem.albumImage = Util.GetAttributValue(objImage, "url");
                }

                lstResults.Add(objItem);

            }

            return lstResults;
        }
        public static Collection<PartialMatche> albumToPartialMatche(XElement objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            var query = from item in objRest.Descendants("Release")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;

                PartialMatche objItem = new PartialMatche();
                objItem.Link = Util.GetAttributValue(objTemp, "id");
                objItem.Title = Util.GetAttributValue(objTemp, "title");

                var artist = from item in objTemp.Descendants("Artist")
                             select item;

                XElement[] nodeArtist = artist.ToArray();

                XElement objArtist = nodeArtist[0];
                objItem.Artist = Util.GetAttributValue(objArtist, "name");

                var image = from item in objTemp.Descendants("Image")
                            select item;
                XElement[] nodeImage = image.ToArray();

                if (nodeImage.Any())
                {
                    XElement objImage = nodeImage[nodeImage.Count() - 1];
                    objItem.ImageUrl = Util.GetAttributValue(objImage, "url");
                }

                lstResults.Add(objItem);

            }

            return lstResults;
        }
        public static YahooMusic albumToObject(XElement objRest)
        {
            if (objRest == null) return null;

            YahooMusic objResults = new YahooMusic();

            var query = from item in objRest.Descendants("Release")
                        select item;

            XElement[] nodes = query.ToArray();
            XElement objTemp;

            if (nodes.Any())
            {
                objTemp = nodes[0];
                objResults.albumName = Util.GetAttributValue(objTemp, "title");
                objResults.albumBarcode = Util.GetAttributValue(objTemp, "UPC");
                objResults.albumReleased = Util.GetAttributValue(objTemp, "releaseDate");
                objResults.albumUrl = Util.GetAttributValue(objTemp, "url");
                objResults.studio = Util.GetAttributValue(objTemp, "label");
                objResults.albumRating = Util.GetAttributValue(objTemp, "rating");
                objResults.albumFlags = Util.GetAttributValue(objTemp, "flags");
            }

            #region Tracks
            query = from item in objRest.Descendants("Track")
                    select item;

            nodes = query.ToArray();
            objResults.albumDuration = 0;
            objResults.albumTracks = new List<string>();

            foreach (XElement item in nodes)
            {
                objResults.albumTracks.Add(Util.GetAttributValue(item, "title"));
                int duration;
                if (int.TryParse(Util.GetAttributValue(item, "duration"), out duration))
                    objResults.albumDuration += duration;

                if (string.IsNullOrWhiteSpace(objResults.trackId))
                    objResults.trackId = Util.GetAttributValue(item, "id");
            }

            #endregion
            query = from item in objRest.Descendants("Artist")
                    select item;

            nodes = query.ToArray();
            if (nodes.Any())
            {
                objTemp = nodes[0];
                objResults.artistName = Util.GetAttributValue(objTemp, "name");
                objResults.artistUrl = Util.GetAttributValue(objTemp, "website");
            }

            query = from item in objRest.Descendants("Category")
                    select item;

            nodes = query.ToArray();
            if (nodes.Length > 0)
            {

                objResults.albumTypes = new List<string>();
                foreach (XElement item in nodes)
                    if (Util.GetAttributValue(item, "type") == "Genre")
                        objResults.albumTypes.Add(Util.GetAttributValue(item, "name"));
            }

            return objResults;
        }
        public static YahooMusic videoToObject(XElement objRest)
        {
            if (objRest == null) return null;

            YahooMusic objResults = new YahooMusic();

            var query = from item in objRest.Descendants("Image")
                        select item;

            XElement[] nodes = query.ToArray();
            int currentSize = 0;
            foreach (XElement item in nodes)
            {
                int size;
                if (int.TryParse(Util.GetAttributValue(item, "size"), out size))
                    if (size > currentSize)
                    {
                        objResults.albumImage = Util.GetAttributValue(item, "url");
                        currentSize = size;
                    }
            }

            return objResults;
        }

    }
}