using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using myCollections.Utils;
using Newtonsoft.Json.Linq;

namespace myCollections.Data.Rest
{
    class Musicbrainz
    {
        public string Id { get; set; }
        public string ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string AlbumReleased { get; set; }
        public List<string> AlbumTracks { get; set; }
        public int AlbumDuration { get; set; }
        public string Studio { get; set; }
        public string Barcode { get; set; }
        public string GroupID { get; set; }

        public static Collection<PartialMatche> ToPartialMatche(XElement objRest)
        {
            XNamespace naXNamespace = @"http://musicbrainz.org/ns/mmd-2.0#";

            if (objRest == null) return null;

            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();

            var query = from item in objRest.Descendants(naXNamespace + "release")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();

                objItem.Link = Util.GetAttributValue(node, "id");
                query = from item in objRest.Descendants(naXNamespace + "artist")
                        select item;

                string artist = null;
                XElement[] artistElements = query.ToArray();
                if (artistElements.Any())
                    artist = artistElements[0].Element(naXNamespace + "name").Value;

                if (string.IsNullOrWhiteSpace(artist) == false)
                    objItem.Title = node.Element(naXNamespace + "title").Value + " - " + artist;
                else
                    objItem.Title = node.Element(naXNamespace + "title").Value;

                if (lstResults.Contains(objItem, new PartialMatchComparer()) == false)
                    lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Musicbrainz ToObject(JObject objRest)
        {
            Musicbrainz objItem = null;
            if (objRest != null)
            {
                objItem = new Musicbrainz();
                objItem.Id = (string)objRest["id"];
                objItem.AlbumName = (string)objRest["title"];
                objItem.AlbumReleased = (string)objRest["date"];
                objItem.Barcode = (string)objRest["barcode"];

                if (objRest["release-group"] != null)
                    objItem.GroupID = (string)objRest["release-group"]["id"];

                JArray artist = (JArray)objRest["artist-credit"];
                if (artist.HasValues == true)
                {
                    objItem.ArtistName = (string)artist[0]["artist"]["name"];
                    objItem.ArtistId = (string)artist[0]["artist"]["id"];
                }

                JArray media = (JArray)objRest["media"];
                if (media.HasValues)
                {
                    objItem.AlbumTracks = new List<string>();
                    JToken tracks = media[0]["tracks"];
                    foreach (JObject track in tracks)
                    {
                        int duration;
                        if (track["title"] != null)
                            objItem.AlbumTracks.Add((string)track["title"]);

                        if (int.TryParse((string)track["length"], out duration) == true)
                            objItem.AlbumDuration += duration/1000;
                    }

                    JToken studios = objRest["label-info"];
                    if (studios.HasValues)
                        objItem.Studio = (string)studios[0]["label"]["name"];

                }


            }
            return objItem;
        }

    }
}
