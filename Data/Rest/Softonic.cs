using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using myCollections.Utils;
using Newtonsoft.Json.Linq;

namespace myCollections.Data.Rest
{
    internal class Softonic
    {
        private const string Key = "a86bd70ec041e0ad46649cfe4150608e";

        public string Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }
        public string ReleasedDate { get; set; }
        public string Link { get; set; }
        public string EditorLink { get; set; }
        public string Rating { get; set; }
        public string Editor { get; set; }
        public List<string> Types { get; set; }


        public static Collection<Softonic> AppsToCollection(XElement objRest)
        {
            if (objRest == null) return null;

            Collection<Softonic> lstResults = new Collection<Softonic>();
            var query = from item in objRest.Descendants("program")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                Softonic objItem = new Softonic();

                objItem.Id = Util.GetAttributValue(objTemp, "id");
                objItem.Title = Util.GetElementValue(objTemp, "name");
                objItem.Image = Util.GetElementValue(objTemp, "url_thumbnail");

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Collection<PartialMatche> AppsToPartialMatche(XElement objRest)
        {
            if (objRest == null) return null;

            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            var query = from item in objRest.Descendants("program")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();

                objItem.Link = Util.GetAttributValue(objTemp, "id");
                objItem.Title = Util.GetElementValue(objTemp, "name");
                objItem.ImageUrl = Util.GetElementValue(objTemp, "url_thumbnail");

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Collection<PartialMatche> AppsToPartialMatche(JObject objRest)
        {
            if (objRest == null) return null;
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            JArray nodes = (JArray)objRest["_embedded"]["program"];

            if (nodes != null)
            {
                foreach (JObject node in nodes)
                {
                    PartialMatche objItem = new PartialMatche();

                    objItem.Link = (string)node["program_id"];
                    objItem.Title = (string)node["title"];
                    lstResults.Add(objItem);
                }
            }
            return lstResults;
        }

        public static Softonic AppsToObject(XElement objRest)
        {
            Softonic objItem = null;
            var query = from item in objRest.Descendants("program")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objItem = new Softonic();

                objItem.Id = Util.GetElementValue(objTemp, "id_program");
                objItem.Title = Util.GetElementValue(objTemp, "title");
                objItem.Description = Util.GetElementValue(objTemp, "review");

                if (string.IsNullOrWhiteSpace(objItem.Description))
                    objItem.Description = Util.GetElementValue(objTemp, "short_description");

                if (objTemp != null)
                {
                    var langue = from item in objTemp.Descendants("language")
                                 let xElement = item.Element("name")
                                 where xElement != null
                                 select xElement.Value;

                    Collection<string> lstLangue = new Collection<string>(langue.ToArray());
                    if (lstLangue.Count > 0)
                    {
                        objItem.Language = lstLangue[0];
                    }
                }

                objItem.Version = Util.GetElementValue(objTemp, "version");
                objItem.ReleasedDate = Util.GetElementValue(objTemp, "date_updated");

                if (string.IsNullOrWhiteSpace(objItem.ReleasedDate))
                    objItem.ReleasedDate = Util.GetElementValue(objTemp, "date_added");

                objItem.Image = Util.GetElementValue(objTemp, "img");
                objItem.Link = Util.GetElementValue(objTemp, "url");
                objItem.Editor = Util.GetElementValue(objTemp, "author");
                objItem.EditorLink = Util.GetElementValue(objTemp, "url_author_more");
                objItem.Rating = Util.GetElementValue(objTemp, "rating_softonic");

                if (string.IsNullOrWhiteSpace(objItem.Rating))
                    objItem.Rating = Util.GetElementValue(objTemp, "rating_user");

                if (string.IsNullOrWhiteSpace(objItem.Rating) == false)
                {
                    int rating;

                    if (int.TryParse(objItem.Rating, out rating))
                        objItem.Rating = (rating * 2).ToString(CultureInfo.InvariantCulture);
                }

                if (objTemp != null)
                {
                    var types = from item in objTemp.Descendants("section")
                                let xElement = item.Element("name")
                                where xElement != null
                                select xElement.Value;

                    objItem.Types = new List<string>(types.ToArray());
                }
            }

            return objItem;
        }
        public static Softonic AppsToObject(JToken objRest)
        {
            Softonic objItem = null;
            if (objRest != null)
            {
                objItem = new Softonic();

                if (objRest["_embedded"] != null)
                {
                    JArray programsArray = (JArray)objRest["_embedded"]["program"];
                    objRest = programsArray[0];
                }

                objItem.Id = (string)objRest["program_id"];
                objItem.Title = (string)objRest["title"];
                #region Description

                string review = (string)objRest["_links"]["review"]["href"];

                if (string.IsNullOrWhiteSpace(review) == false)
                {
                    string response = Util.GetRest(new Uri(string.Format("{0}?key={1}", review, Key)));

                    if (string.IsNullOrWhiteSpace(response) == false)
                    {
                        JObject restResponse = JObject.Parse(response);
                        if (restResponse != null)
                        {
                            objItem.Description = (string)restResponse["long_desc"];
                            if (string.IsNullOrWhiteSpace(objItem.Description))
                                objItem.Description = (string)restResponse["title"];
                        }
                        else
                            objItem.Description = (string)objRest["short_description"];
                    }
                }
                else
                    objItem.Description = (string)objRest["short_description"];
                #endregion
                JArray languageArray = (JArray)objRest["languages"];

                //Fix since 2.6.0.0
                if (languageArray != null && languageArray.Count > 0)
                    objItem.Language = (string)languageArray[0]["name"];

                objItem.Version = (string)objRest["version"];

                objItem.ReleasedDate = (string)objRest["date_updated"];
                if (string.IsNullOrWhiteSpace(objItem.ReleasedDate))
                    objItem.ReleasedDate = (string)objRest["date_added"];

                #region Image
                string images = (string)objRest["_links"]["screenshots"]["href"];
                if (string.IsNullOrWhiteSpace(images) == false)
                {
                    string response = Util.GetRest(new Uri(string.Format("{0}?key={1}", images, Key)));

                    if (string.IsNullOrWhiteSpace(response) == false)
                    {
                        JObject restResponse = JObject.Parse(response);
                        if (restResponse != null && restResponse["_embedded"] != null)
                        {
                            JArray imageArray = (JArray)restResponse["_embedded"]["screenshot"];
                            if (imageArray.Count > 0)
                                objItem.Image = (string)imageArray[0]["full_size"];
                        }
                    }
                }
                #endregion

                objItem.Link = (string)objRest["url"];
                objItem.Editor = (string)objRest["author"];
                #region Rating
                objItem.Rating = (string)objRest["rating_softonic"];

                if (string.IsNullOrWhiteSpace(objItem.Rating) == false)
                {
                    int rating;

                    if (int.TryParse(objItem.Rating, out rating))
                        objItem.Rating = (rating * 2).ToString(CultureInfo.InvariantCulture);
                }
                #endregion
                #region Types
                JArray typesArray = (JArray)objRest["section_path"];
                if (typesArray != null && typesArray.Count > 0)
                {
                    objItem.Types=new List<string>();
                    foreach (JToken type in typesArray)
                        if ((string)type["id_section"] != "2")
                            objItem.Types.Add((string)type["name"]);
                }
                #endregion
            }

            return objItem;
        }

    }
}

