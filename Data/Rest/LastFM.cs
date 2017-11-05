using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using myCollections.Utils;
using System.Collections.Generic;

namespace myCollections.Data.Rest
{
    internal class LastFm
    {
        public string Id { get; set; }

        public string ArtistName { get; set; }
        public string ArtistDescription { get; set; }
        public string ArtistContent { get; set; }
        public string ArtistUrl { get; set; }
        public string ArtistSmallImage { get; set; }
        public string ArtistLargeImage { get; set; }
        public string ArtistExtraLargeImage { get; set; }
        public string ArtistMegaImage { get; set; }
        public string ArtistId { get; set; }

        public string AlbumName { get; set; }
        public string AlbumUrl { get; set; }
        public string AlbumSmallImage { get; set; }
        public string AlbumLargeImage { get; private set; }
        public string AlbumExtraLargeImage { get; set; }
        public string AlbumMegaImage { get; set; }
        public string AlbumDescription { get; set; }
        public string AlbumContent { get; set; }
        public string AlbumReleased { get; set; }
        public List<string> AlbumTracks { get; set; }
        public List<string> AlbumTypes { get; set; }
        public int AlbumDuration { get; set; }

        public static LastFm AlbumToObject(XElement objRest)
        {
            return AlbumToObject(objRest, new LastFm());
        }
        private static LastFm AlbumToObject(XElement objRest, LastFm objResult)
        {
            var query = from item in objRest.Descendants("album")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objResult.AlbumName = Util.GetElementValue(objTemp, "name");
                objResult.ArtistName = Util.GetElementValue(objTemp, "artist");
                objResult.AlbumUrl = Util.GetElementValue(objTemp, "url");
                objResult.AlbumReleased = Util.GetElementValue(objTemp, "releasedate");
                objResult.AlbumSmallImage = Util.GetElementValue(objTemp, "image", "size", "small");
                objResult.AlbumLargeImage = Util.GetElementValue(objTemp, "image", "size", "large");
                objResult.AlbumExtraLargeImage = Util.GetElementValue(objTemp, "image", "size", "extralarge");
                objResult.AlbumMegaImage = Util.GetElementValue(objTemp, "image", "size", "mega");
            }


            query = from item in objRest.Descendants("track")
                    select item;

            nodes = query.ToArray();

            objResult.AlbumDuration = 0;
            objResult.AlbumTracks = new List<string>();

            foreach (XElement item in nodes)
            {
                objResult.AlbumTracks.Add(Util.GetElementValue(item, "name"));
                int duration;
                if (int.TryParse(Util.GetElementValue(item, "duration"), out duration))
                    objResult.AlbumDuration += duration;
            }

            query = from item in objRest.Descendants("tag")
                    select item;

            nodes = query.ToArray();
            objResult.AlbumTypes = new List<string>();
            foreach (XElement item in nodes)
            {
                string strTemp = Util.GetElementValue(item, "name");
                if (IsBadTag(strTemp) == false)
                    if (strTemp.ToUpper().Contains(objResult.AlbumName.ToUpper()) == false)
                        if (strTemp.ToUpper().Contains(objResult.ArtistName.ToUpper()) == false)
                            objResult.AlbumTypes.Add(Util.FirstCharToUpper(strTemp));
            }

            query = from item in objRest.Descendants("wiki")
                    select item;

            nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objResult.AlbumDescription = Util.PurgeHtml(Util.GetElementValue(objTemp, "summary"));
                objResult.AlbumContent = Util.PurgeHtml(Util.GetElementValue(objTemp, "content"));
            }

            return objResult;
        }
        public static IEnumerable<LastFm> AlbumToCollection(XElement objRest)
        {

            if (objRest == null) return null;

            Collection<LastFm> lstResults = new Collection<LastFm>();
            var query = from item in objRest.Descendants("album")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                LastFm objItem = new LastFm();
                objItem.AlbumName = Util.GetElementValue(objTemp, "name");
                objItem.ArtistName = Util.GetElementValue(objTemp, "artist");
                objItem.AlbumUrl = Util.GetElementValue(objTemp, "url");
                objItem.Id = Util.GetElementValue(objTemp, "mbid");
                objItem.AlbumSmallImage = Util.GetElementValue(objTemp, "image", "size", "small");
                objItem.AlbumLargeImage = Util.GetElementValue(objTemp, "image", "size", "large");
                objItem.AlbumExtraLargeImage = Util.GetElementValue(objTemp, "image", "size", "extralarge");
                lstResults.Add(objItem);
            }

            return lstResults;
        }
        public static Collection<PartialMatche> AlbumToPartialMatch(XElement objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            var query = from item in objRest.Descendants("album")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();
                objItem.Title = Util.GetElementValue(objTemp, "name");
                objItem.Artist = Util.GetElementValue(objTemp, "artist");
                objItem.Link = Util.GetElementValue(objTemp, "mbid");
                objItem.ImageUrl = Util.GetElementValue(objTemp, "image", "size", "small");

                if (string.IsNullOrWhiteSpace(objItem.Link) == false)
                    lstResults.Add(objItem);
            }

            return lstResults;
        }
        public static LastFm ArtistToObject(XElement objRest)
        {

            LastFm objResult = new LastFm();
            var query = from item in objRest.Descendants("artist")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objResult.ArtistName = Util.GetElementValue(objTemp, "name");

                objResult.ArtistId = Util.GetElementValue(objTemp, "mbid");
                objResult.ArtistUrl = Util.GetElementValue(objTemp, "url");

                objResult.ArtistSmallImage = Util.GetElementValue(objTemp, "image", "size", "small");
                objResult.ArtistLargeImage = Util.GetElementValue(objTemp, "image", "size", "large");
                objResult.ArtistExtraLargeImage = Util.GetElementValue(objTemp, "image", "size", "extralarge");
                objResult.ArtistMegaImage = Util.GetElementValue(objTemp, "image", "size", "mega");
            }

            query = from item in objRest.Descendants("bio")
                    select item;

            nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objResult.ArtistDescription = Util.PurgeHtml(Util.GetElementValue(objTemp, "summary"));
                objResult.ArtistContent = Util.PurgeHtml(Util.GetElementValue(objTemp, "content"));
            }

            return objResult;
        }

        private static bool IsBadTag(string strTag)
        {
            if (strTag.Contains("album"))
                return true;

            if (strTag.Contains("iam marseille"))
                return true;

            if (strTag.Contains("201"))
                return true;

            if (strTag.Contains("200"))
                return true;

            if (strTag.Contains("199"))
                return true;

            if (strTag.Contains("198"))
                return true;

            if (strTag.Contains("197"))
                return true;

            if (strTag.Contains("196"))
                return true;

            if (strTag.Contains("195"))
                return true;

            if (strTag.Contains("194"))
                return true;

            return false;
        }
    }
}