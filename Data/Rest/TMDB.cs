using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Data.Rest
{
    internal class TMDB
    {
        const string ImageUrl = @"http://d3gtl9l2a4fn1j.cloudfront.net/t/p/";
        const string SmallImage = "w92";
        const string BigImage = "w500";
        const string BackdropSize = "w1280";
        const string ProfileSize = "w185";

        public string BackdropOriginal { get; private set; }
        public List<string> Cast { get; private set; }
        public string Country { get; private set; }
        public string Director { get; private set; }
        public List<string> Genres { get; private set; }
        public string Id { get; private set; }
        public string ImdbId { get; private set; }
        public string MovieDescription { get; private set; }
        public string MovieUrl { get; private set; }
        public string OriginalTitle { get; private set; }
        public string Popularity { get; private set; }
        public string PosterOriginal { get; private set; }
        public string PosterThumb { get; private set; }
        public string Rated { get; private set; }
        public string Rating { get; private set; }
        public DateTime? Released { get; private set; }
        public int? Runtime { get; private set; }
        public string Studio { get; private set; }
        public string Tagline { get; private set; }
        public string Title { get; private set; }
        public string Trailer { get; private set; }
        public string Type { get; private set; }
        public string Url { get; private set; }

        public static TMDB MovieToObject(XElement objRest)
        {
            if (objRest == null) return null;

            TMDB objItem = null;
            var query = from item in objRest.Descendants("movie")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objItem = new TMDB();

                if (objTemp != null)
                {
                    var castId = from item in objTemp.Descendants("person")
                                 let xAttribute = item.Attribute("job")
                                 where xAttribute != null && xAttribute.Value == "Actor"
                                 let attribute = item.Attribute("id")
                                 where attribute != null
                                 select attribute.Value;

                    objItem.Cast = new List<string>(castId.ToList());

                    var country = from item in objTemp.Descendants("country")
                                  let xAttribute = item.Attribute("name")
                                  where xAttribute != null
                                  select xAttribute.Value;

                    IEnumerable<string> country1 = country as IList<string> ?? country.ToList();
                    if (country1.Any())
                        objItem.Country = country1.First();

                    var director = from item in objTemp.Descendants("person")
                                   let xAttribute = item.Attribute("job")
                                   where xAttribute != null && xAttribute.Value == "Director"
                                   let attribute = item.Attribute("id")
                                   where attribute != null
                                   select attribute.Value;

                    IEnumerable<string> director1 = director as IList<string> ?? director.ToList();
                    if (director1.Any())
                        objItem.Director = director1.First();

                    var genres = from item in objTemp.Descendants("category")
                                 let attribute2 = item.Attribute("type")
                                 where attribute2 != null && attribute2.Value == "genre"
                                 let xAttribute3 = item.Attribute("name")
                                 where xAttribute3 != null
                                 select xAttribute3.Value;

                    objItem.Genres = new List<string>(genres.ToList());

                    objItem.Id = Util.GetElementValue(objTemp, "id");
                    objItem.ImdbId = Util.GetElementValue(objTemp, "imdb_id");
                    objItem.MovieDescription = Util.GetElementValue(objTemp, "overview");
                    objItem.MovieUrl = Util.GetElementValue(objTemp, "homepage");
                    objItem.OriginalTitle = Util.GetElementValue(objTemp, "alternative_name");
                    objItem.Popularity = Util.GetElementValue(objTemp, "popularity");
                    objItem.Rated = Util.GetElementValue(objTemp, "certification");
                    objItem.Tagline = Util.GetElementValue(objTemp, "tagline");
                    var imagesUrl = from item in objTemp.Descendants("image")
                                    let attribute3 = item.Attribute("type")
                                    let attribute = item.Attribute("size")
                                    where attribute3 != null && (attribute != null && (attribute3.Value == "poster"
                                                                                       &&
                                                                                       attribute.Value == "original"))
                                    let xAttribute4 = item.Attribute("url")
                                    where xAttribute4 != null
                                    select xAttribute4.Value;

                    string[] images = imagesUrl.ToArray();
                    if (images.Length > 0)
                        objItem.PosterOriginal = images[0];

                    imagesUrl = from item in objTemp.Descendants("image")
                                let attribute1 = item.Attribute("size")
                                let xAttribute1 = item.Attribute("type")
                                where attribute1 != null && (xAttribute1 != null && (xAttribute1.Value == "backdrop"
                                                                                     &&
                                                                                     attribute1.Value == "original"))
                                let xAttribute2 = item.Attribute("url")
                                where xAttribute2 != null
                                select xAttribute2.Value;

                    images = imagesUrl.ToArray();
                    if (images.Length > 0 && images[0] != objItem.PosterOriginal)
                        objItem.BackdropOriginal = images[0];
                    else if (images.Length > 1 && images[1] != objItem.PosterOriginal)
                        objItem.BackdropOriginal = images[1];

                    objItem.Rating = Util.GetElementValue(objTemp, "rating");

                    string relased = Util.GetElementValue(objTemp, "released");
                    if (string.IsNullOrWhiteSpace(relased) == false)
                    {
                        DateTime date;
                        if (DateTime.TryParse(relased, out date) == true)
                            objItem.Released = date;
                    }

                    string runtime = Util.GetElementValue(objTemp, "runtime");
                    if (string.IsNullOrWhiteSpace(runtime) == false)
                    {
                        int intvalue;
                        if (int.TryParse(runtime, out intvalue) == true)
                            objItem.Runtime = intvalue;
                    }

                    var studio = from item in objTemp.Descendants("studio")
                                 let attribute4 = item.Attribute("name")
                                 where attribute4 != null
                                 select attribute4.Value;

                    IEnumerable<string> enumerable = studio as IList<string> ?? studio.ToList();
                    if (enumerable.Any())
                        objItem.Studio = enumerable.First();

                    objItem.Title = Util.GetElementValue(objTemp, "name");
                    objItem.Trailer = Util.GetElementValue(objTemp, "trailer");
                    objItem.Type = Util.GetElementValue(objTemp, "type");
                    objItem.Url = Util.GetElementValue(objTemp, "url");
                }
            }

            return objItem;
        }
        public static TMDB MovieToObject(JObject objRest)
        {
            if (objRest == null) return null;

            TMDB objItem = new TMDB();

            JArray cast = (JArray)objRest["casts"]["cast"];
            objItem.Cast = new List<string>();

            foreach (JObject person in cast.OrderBy(x => x["order"]))
                objItem.Cast.Add((string)person["id"]);

            if (objRest["production_countries"] != null)
                if (objRest["production_countries"].Any())
                    objItem.Country = (string)objRest["production_countries"][0]["name"];

            JArray crew = (JArray)objRest["casts"]["crew"];

            foreach (JObject person in crew)
            {
                if ((string)person["job"] == "Director")
                {
                    objItem.Director = (string)person["id"];
                    break;
                }
            }

            JArray genres = (JArray)objRest["genres"];
            objItem.Genres = new List<string>();
            foreach (JObject genre in genres)
                objItem.Genres.Add((string)genre["name"]);


            objItem.Id = (string)objRest["id"];
            objItem.ImdbId = (string)objRest["imdb_id"];
            objItem.MovieDescription = (string)objRest["overview"];
            objItem.MovieUrl = (string)objRest["homepage"];
            objItem.OriginalTitle = (string)objRest["original_title"];
            objItem.Popularity = (string)objRest["popularity"];
            //        objItem.rated = Util.GetElementValue(objTemp, "certification");
            objItem.Tagline = (string)objRest["tagline"];

            objItem.PosterOriginal = ImageUrl + BigImage + (string)objRest["poster_path"];
            if (string.IsNullOrWhiteSpace((string)objRest["backdrop_path"]) == false)
                objItem.BackdropOriginal = ImageUrl + BackdropSize + (string)objRest["backdrop_path"];

            objItem.Rating = (string)objRest["vote_average"];

            string relased = (string)objRest["release_date"];
            if (string.IsNullOrWhiteSpace(relased) == false)
            {
                DateTime date;
                if (DateTime.TryParse(relased, out date) == true)
                    objItem.Released = date;
            }

            string runtime = (string)objRest["runtime"];
            if (string.IsNullOrWhiteSpace(runtime) == false)
            {
                int intvalue;
                if (int.TryParse(runtime, out intvalue) == true)
                    objItem.Runtime = intvalue;
            }

            if (objRest["production_companies"] != null)
                if (objRest["production_companies"].Any())
                    objItem.Studio = (string)objRest["production_companies"][0]["name"];


            objItem.Title = (string)objRest["title"];
            objItem.Url = string.Format(@"http://www.themoviedb.org/movie/{0}", objItem.Id);

            return objItem;
        }

        public static Collection<PartialMatche> MovieToPartialMatche(XElement objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            var query = from item in objRest.Descendants("movie")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();

                objItem.Title = Util.GetElementValue(objTemp, "name");
                objItem.Link = Util.GetElementValue(objTemp, "id");

                var imagesUrl = from item in objTemp.Descendants("image")
                                let attribute = item.Attribute("type")
                                let xAttribute = item.Attribute("size")
                                where attribute != null && (xAttribute != null && (attribute.Value == "poster"
                                                                                                && xAttribute.Value == "thumb"))
                                let xAttribute1 = item.Attribute("url")
                                where xAttribute1 != null
                                select xAttribute1.Value;

                string[] images = imagesUrl.ToArray();
                if (images.Length > 0)
                    objItem.ImageUrl = images[0];

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Collection<PartialMatche> MovieToPartialMatche(JObject objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            JArray nodes = (JArray)objRest["results"];

            foreach (var node in nodes)
            {
                PartialMatche objItem = new PartialMatche();

                if (node.HasValues)
                {
                    objItem.Link = (string)node["id"];

                    objItem.Title = (string)node["title"];
                    if (string.IsNullOrWhiteSpace(objItem.Title))
                        objItem.Title = (string)node["original_title"];

                    if (node["poster_path"] != null)
                        objItem.ImageUrl = ImageUrl + SmallImage + (string)node["poster_path"];

                    lstResults.Add(objItem);
                }
            }
            return lstResults;
        }

        public static Collection<TMDB> ArtistToCollection(XElement objRest)
        {
            Collection<TMDB> lstResults = new Collection<TMDB>();
            var query = from item in objRest.Descendants("person")
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                TMDB objItem = new TMDB();

                objItem.Title = Util.GetElementValue(objTemp, "name");
                objItem.Id = Util.GetElementValue(objTemp, "id");

                var imagesUrl = from item in objTemp.Descendants("image")
                                let attribute = item.Attribute("type")
                                let xAttribute = item.Attribute("size")
                                where attribute != null && (xAttribute != null && (attribute.Value == "poster"
                                                                                                && xAttribute.Value == "thumb"))
                                let xAttribute1 = item.Attribute("url")
                                where xAttribute1 != null
                                select xAttribute1.Value;

                string[] images = imagesUrl.ToArray();
                if (images.Length > 0)
                    objItem.PosterThumb = images[0];

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Collection<TMDB> ArtistToCollection(JObject objRest)
        {
            Collection<TMDB> lstResults = new Collection<TMDB>();
            JArray nodes = (JArray)objRest["results"];

            foreach (JObject node in nodes)
            {
                TMDB objItem = new TMDB();

                objItem.Title = (string)node["name"];
                objItem.Id = (string)node["id"];

                if (string.IsNullOrWhiteSpace((string)node["poster_path"]) == false)
                    objItem.PosterThumb = ImageUrl + ProfileSize + (string)node["profile_path"];

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Artist CastToArtist(XElement objRest)
        {
            if (objRest == null) return null;

            Artist objItem = null;
            var query = from item in objRest.Descendants("person")
                        select item;

            XElement[] nodes = query.ToArray();
            if (nodes.Length > 0)
            {
                XElement objTemp = nodes[0];
                objItem = new Artist();

                #region Name
                string[] name = Util.GetElementValue(objTemp, "name").Split(' ');
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
                #endregion
                #region Birthday
                DateTime objDate;
                if (DateTime.TryParse(Util.GetElementValue(objTemp, "birthday"), out objDate))
                    objItem.BirthDay = objDate;
                #endregion
                #region Picture
                if (objTemp != null)
                {
                    var imagesUrl = from item in objTemp.Descendants("image")
                                    let attribute = item.Attribute("size")
                                    let xAttribute = item.Attribute("type")
                                    where attribute != null && (xAttribute != null && (xAttribute.Value == "profile"
                                                                                   && attribute.Value == "original"))
                                    let xAttribute1 = item.Attribute("url")
                                    where xAttribute1 != null
                                    select xAttribute1.Value;

                    string[] images = imagesUrl.ToArray();
                    if (images.Length > 0)
                        objItem.Picture = Util.GetImage(images[0]);

                }
                #endregion
                #region Credits

                if (objTemp != null)
                {
                    var creditsquery = from item in objTemp.Descendants("movie")
                                       where item.Attribute("job").Value == "Actor"
                                       select item;

                    XElement[] credits = creditsquery.ToArray();
                    foreach (XElement item in credits)
                    {

                        ArtistCredits movie = new ArtistCredits();

                        movie.Title = Util.GetAttributValue(item, "name");
                        movie.BuyLink = Util.GetAttributValue(item, "url");
                        movie.EntityType = EntityType.Movie;
                        movie.Notes = Util.GetAttributValue(item, "character");

                        DateTime releaseDate;
                        if (DateTime.TryParse(Util.GetAttributValue(item, "release"), out releaseDate) == true)
                            movie.ReleaseDate = releaseDate;

                        movie.Title = movie.Title.Replace(@"""", " ");

                        if (string.IsNullOrWhiteSpace(movie.Title) == false && string.IsNullOrWhiteSpace(objItem.FulleName) == false)
                            if (Dal.GetInstance.GetArtistCredit(movie.Title, objItem.FulleName) == null)
                                objItem.ArtistCredits.Add(movie);

                    }
                }

                #endregion
                #region Bio
                objItem.Bio = Util.GetElementValue(objTemp, "biography");
                #endregion
                #region PlaceBirth
                objItem.PlaceBirth = Util.GetElementValue(objTemp, "birthplace");
                #endregion
            }

            return objItem;
        }
        public static Artist CastToArtist(JObject objRest)
        {
            if (objRest == null) return null;

            Artist objItem = new Artist();
            objItem.ArtistCredits = new List<ArtistCredits>();

            #region Name
            string[] name = ((string)objRest["name"]).Split(' ');
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
            #endregion
            #region Birthday
            DateTime objDate;
            if (DateTime.TryParse((string)objRest["birthday"], out objDate))
                objItem.BirthDay = objDate;
            #endregion
            #region Picture
            if (string.IsNullOrWhiteSpace((string)objRest["profile_path"]) == false)
                objItem.Picture = Util.GetImage(ImageUrl + ProfileSize + (string)objRest["profile_path"]);

            #endregion
            #region Credits

            JArray cast = (JArray)objRest["credits"]["cast"];

            foreach (JObject person in cast)
            {
                ArtistCredits movie = new ArtistCredits();

                movie.Title = (string)person["title"];
                movie.BuyLink = string.Format(@"http://www.themoviedb.org/movie/{0}", (string)person["id"]);
                movie.EntityType = EntityType.Movie;
                movie.Notes = (string)person["character"];

                DateTime releaseDate;
                if (DateTime.TryParse((string)person["release_date"], out releaseDate) == true)
                    movie.ReleaseDate = releaseDate;

                movie.Title = movie.Title.Replace(@"""", " ");

                if (string.IsNullOrWhiteSpace(movie.Title) == false && string.IsNullOrWhiteSpace(objItem.FulleName) == false)
                    if (Dal.GetInstance.GetArtistCredit(movie.Title, objItem.FulleName) == null)
                        objItem.ArtistCredits.Add(movie);

            }

            #endregion
            #region Bio
            objItem.Bio = (string)objRest["biography"];
            #endregion
            #region PlaceBirth
            objItem.PlaceBirth = (string)objRest["place_of_birth"];
            #endregion


            return objItem;
        }
    }
}