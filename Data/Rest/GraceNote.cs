
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using myCollections.Utils;

namespace myCollections.Data.Rest
{
    internal class GraceNote
    {
        public string Id { get; set; }

        public string ArtistName { get; set; }
        public string ArtistDescription { get; set; }
        public string ArtistContent { get; set; }
        public string ArtistUrl { get; set; }
        public string ArtistImage { get; set; }
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

        public static string GetClientId(XElement objRest)
        {
            string clientId = string.Empty;
            var query = from item in objRest.Descendants("USER")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                return objTemp.Value;

            }

            return clientId;
        }

        public static Collection<PartialMatche> AlbumToPartialMatch(XElement objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            List<string> lstTitles = new List<string>();
            var query = from item in objRest.Descendants("ALBUM")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();
                objItem.Title = Util.GetElementValue(objTemp, "TITLE");
                objItem.Artist = Util.GetElementValue(objTemp, "ARTIST");
                objItem.Link = Util.GetElementValue(objTemp, "GN_ID");
                objItem.ImageUrl = Util.GetElementValue(objTemp, "URL");

                if (lstTitles.Contains(objItem.Title + objItem.Artist) == false)
                {
                    if (string.IsNullOrWhiteSpace(objItem.Link) == false)
                    {
                        lstTitles.Add(objItem.Title + objItem.Artist);
                        lstResults.Add(objItem);
                    }
                }
            }

            return lstResults;
        }
        public static GraceNote AlbumToObject(XElement objRest)
        {
            var query = from item in objRest.Descendants("ALBUM")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                GraceNote graceNote = new GraceNote();
                graceNote.AlbumTypes = new List<string>();

                graceNote.ArtistName = Util.GetElementValue(objTemp, "ARTIST");
                graceNote.AlbumName = Util.GetElementValue(objTemp, "TITLE");
                graceNote.AlbumTypes.Add(Util.GetElementValue(objTemp, "GENRE"));

                graceNote.AlbumImage = Util.GetElementValue(objTemp, "URL", "TYPE", "COVERART");
                graceNote.ArtistUrl = Util.GetElementValue(objTemp, "URL", "TYPE", "ARTIST_BIOGRAPHY");
                graceNote.ArtistImage = Util.GetElementValue(objTemp, "URL", "TYPE", "ARTIST_IMAGE");

                string url = Util.GetElementValue(objTemp, "URL", "TYPE", "REVIEW");
                if (string.IsNullOrWhiteSpace(url) == false)
                    graceNote.AlbumDescription = Util.GetRest(new Uri(url));

                query = from item in objRest.Descendants("TRACK")
                        select item;

                nodes = query.ToArray();

                if (nodes.Any())
                {
                    graceNote.AlbumTracks=new List<string>();
                    foreach (XElement item in nodes)
                        graceNote.AlbumTracks.Add(Util.GetElementValue(item, "TITLE"));
                }


                return graceNote;
            }

           return null;
        }


    }
}
