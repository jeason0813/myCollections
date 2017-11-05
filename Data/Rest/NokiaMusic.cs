using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Data.Rest
{
    internal class NokiaMusic
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
        public string AlbumImage { get; set; }
        public string AlbumDescription { get; set; }
        public string AlbumContent { get; set; }
        public string AlbumReleased { get; set; }
        public List<string> AlbumTracks { get; set; }
        public List<string> AlbumTypes { get; set; }
        public int AlbumDuration { get; set; }
        public string AlbumStudio { get; set; }

        public static Collection<PartialMatche> ToPartialMatche(JObject objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            JArray nodes = (JArray)objRest["items"];

            if (nodes != null)
            {
                foreach (JObject node in nodes)
                {
                    PartialMatche objItem = new PartialMatche();

                    objItem.Link = (string)node["id"];

                    if (node["creators"]["performers"] != null)
                        objItem.Title = (string)node["name"] + " - " + (string)node["creators"]["performers"][0]["name"];
                    else
                        objItem.Title = (string)node["name"];

                    if (node["thumbnails"] != null)
                        objItem.ImageUrl = (string)node["thumbnails"]["100x100"];

                    lstResults.Add(objItem);
                }
            }
            return lstResults;

        }
        public static NokiaMusic ToObject(JObject objRest)
        {
            NokiaMusic objItem = null;
            if (objRest != null)
            {
                objItem = new NokiaMusic();
                objItem.AlbumStudio = (string)objRest["label"];
                objItem.AlbumName = (string)objRest["name"];
                objItem.AlbumImage = (string)objRest["thumbnails"]["320x320"];
                if (objRest["creators"]["performers"] != null)
                {
                    objItem.ArtistName = (string)objRest["creators"]["performers"][0]["name"];
                    objItem.ArtistId = (string)objRest["creators"]["performers"][0]["id"];
                }
                objItem.AlbumUrl = (string)objRest["storeuri"];

                JArray tracks = (JArray)objRest["tracks"];

                objItem.AlbumTracks = new List<string>();
                foreach (JObject track in tracks)
                {
                    int duration;
                    if (track["name"] != null)
                        objItem.AlbumTracks.Add((string)track["name"]);

                    if (int.TryParse((string)track["duration"], out duration) == true)
                        objItem.AlbumDuration += duration;
                }

                if (objRest["genres"] != null)
                {
                    JArray genre = (JArray)objRest["genres"];
                    objItem.AlbumTypes = new List<string>();

                    foreach (JObject item in genre)
                        objItem.AlbumTypes.Add((string)item["name"]);
                }

                if (objRest["streetreleasedate"] != null)
                    objItem.AlbumReleased = (string)objRest["streetreleasedate"];
            }
            return objItem;
        }
        public static Artist ToArtist(JObject objRest, Artist objItem)
        {
            if (objRest != null)
            {

                JArray albums = (JArray)objRest["items"];
                foreach (JObject album in albums)
                {
                    if ((string)album["category"]["id"] == "Album")
                    {
                        ArtistCredits credits = new ArtistCredits();

                        credits.Title = (string)album["name"];
                        credits.BuyLink = (string)album["storeuri"];
                        credits.EntityType = EntityType.Music;

                        if (string.IsNullOrWhiteSpace(credits.Title) == false && string.IsNullOrWhiteSpace(objItem.FulleName) == false)
                            if (Dal.GetInstance.GetArtistCredit(credits.Title, objItem.FulleName) == null)
                                objItem.ArtistCredits.Add(credits);
                    }
                }
            }
            return objItem;
        }
    }
}
