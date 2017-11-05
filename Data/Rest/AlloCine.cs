using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System.Globalization;
using myCollections.BL.Services;

namespace myCollections.Data.Rest
{
    internal class AlloCine
    {
        public string Rating { get; private set; }
        public long SerieRating { get; private set; }

        public string Title { get; private set; }
        public string OriginalTitle { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string Tagline { get; set; }
        public DateTime? Released { get; set; }
        public int? Runtime { get; set; }
        public string Trailer { get; set; }
        public string Studio { get; set; }
        public string Country { get; set; }
        public string Url { get; set; }
        public bool IsInProduction { get; set; }
        public string NumberOfSeason { get; set; }
        public string NumberOfEpisodes { get; set; }
        public string SeasonEpisodes { get; set; }

        public List<string> Genres { get; set; }
        public List<Artist> Cast { get; set; }
        public List<Artist> Directors { get; set; }

        public string PosterOriginal { get; set; }
        public string BackdropOriginal { get; set; }

        public static AlloCine MovieToObject(XElement objRest)
        {
            AlloCine objItem = null;
            if (objRest != null)
            {
                objItem = new AlloCine();

                objItem.OriginalTitle = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}originalTitle");
                objItem.Country = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}nationality");
                objItem.Id = Util.GetAttributValue(objRest, "code");
                objItem.Description = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}synopsis");
                objItem.Tagline = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}synopsisShort");
                objItem.PosterOriginal = Util.GetAttributValue(objRest, @"{http://www.allocine.net/v6/ns/}poster", "href");
                objItem.Title = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}title");
                objItem.Url = @"http://www.allocine.fr/film/fichefilm_gen_cfilm=" + objItem.Id + ".html";
                objItem.Trailer = Util.GetAttributValue(objRest, @"{http://www.allocine.net/v6/ns/}trailer", "href");

                string runtime = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}runtime");
                int intRunTime;
                if (int.TryParse(runtime, out intRunTime))
                    objItem.Runtime = (int?)new TimeSpan(0, 0, intRunTime).TotalMinutes;

                var released = from item in objRest.Elements()
                               where item.Name.LocalName == "release"
                               select ((XElement)item.FirstNode).Value;

                IEnumerable<string> enumerable = released as IList<string> ?? released.ToList();
                if (enumerable.Any())
                {
                    string relased = enumerable.First();
                    if (string.IsNullOrWhiteSpace(relased) == false)
                    {
                        DateTime date;
                        if (DateTime.TryParse(relased, out date) == true)
                            objItem.Released = date;
                    }
                }

                var cast = from item in objRest.Descendants("{http://www.allocine.net/v6/ns/}castMember")
                           select item;

                XElement[] artists = cast.ToArray();
                objItem.Cast = new List<Artist>();
                objItem.Directors = new List<Artist>();
                foreach (XElement item in artists)
                {

                    string picture = Util.GetAttributValue(item, @"{http://www.allocine.net/v6/ns/}picture", "href");
                    string name = Util.GetElementValue(item, @"{http://www.allocine.net/v6/ns/}person");
                    string job = Util.GetAttributValue(item, @"{http://www.allocine.net/v6/ns/}activity", "code");

                    if (string.IsNullOrWhiteSpace(name) == false)
                    {
                        bool isNew;
                        Artist artist = (ArtistServices.Get(name, out isNew));

                        if (string.IsNullOrWhiteSpace(picture) == false && MySettings.FastSearch == false)
                            artist.Picture = Util.GetImage(picture);

                        if (job == "8002")
                            objItem.Directors.Add(artist);
                        else if (job == "8001")
                            objItem.Cast.Add(artist);
                    }
                }

                var genres = from item in objRest.Descendants("{http://www.allocine.net/v6/ns/}genre")
                             select item.Value;

                objItem.Genres = new List<string>(genres.ToArray());

                var imagesUrl = from item in objRest.Descendants("{http://www.allocine.net/v6/ns/}media")
                                let xElement = item.Element("{http://www.allocine.net/v6/ns/}type")
                                where xElement != null && xElement.Value == "Photo"
                                select item;

                Collection<XElement> images = new Collection<XElement>(imagesUrl.ToArray());
                if (images.Count > 0)
                    objItem.BackdropOriginal = Util.GetAttributValue(images[0], @"{http://www.allocine.net/v6/ns/}thumbnail", "href");

                var studio = from item in objRest.Elements()
                             where item.Name.LocalName == "release"
                             select item;

                Collection<XElement> studios = new Collection<XElement>(studio.ToArray());
                if (studios.Count > 0)
                    objItem.Studio = Util.GetAttributValue(studios[0], @"{http://www.allocine.net/v6/ns/}distributor", "name");

                var rating = from item in objRest.Elements()
                             where item.Name.LocalName == "statistics"
                             select item;

                Collection<XElement> lstRating = new Collection<XElement>(rating.ToArray());
                if (lstRating.Count > 0)
                {
                    string strrating = Util.GetElementValue(lstRating[0], "{http://www.allocine.net/v6/ns/}userRating");
                    double value;
                    CultureInfo cultureInfo = new CultureInfo("en-US", true);
                    if (double.TryParse(strrating, NumberStyles.Any, cultureInfo.NumberFormat, out value) == true)
                        objItem.Rating = value.ToString("F");
                    else if (double.TryParse(strrating.Replace(',', '.'), out value) == true)
                        objItem.Rating = value.ToString(CultureInfo.InvariantCulture);
                    else
                        objItem.Rating = strrating;
                }
            }

            return objItem;
        }
        public static AlloCine MovieToObject(JObject objRest, LanguageType languageType)
        {
            AlloCine objItem = null;
            if (objRest != null)
            {
                //Fix since v2.7.12.0
                if (objRest["movie"] != null)
                {
                    objItem = new AlloCine();

                    if (objRest["movie"]["originalTitle"] != null)
                        objItem.OriginalTitle = (string)objRest["movie"]["originalTitle"];
                    if (objRest["movie"]["nationality"] != null)
                        objItem.Country = (string)objRest["movie"]["nationality"][0]["$"];

                    objItem.Id = (string)objRest["movie"]["code"];
                    objItem.Description = (string)objRest["movie"]["synopsis"];

                    if (objRest["movie"]["synopsisShort"] != null)
                        objItem.Tagline = (string)objRest["movie"]["synopsisShort"];

                    if (objRest["movie"]["poster"] != null)
                        objItem.PosterOriginal = (string)objRest["movie"]["poster"]["href"];

                    objItem.Title = (string)objRest["movie"]["title"];

                    switch (languageType)
                    {
                        case LanguageType.BR:
                        case LanguageType.PT:
                            objItem.Url = @"http://www.adorocinema.com/filmes/filme-" + objItem.Id;
                            break;
                        default:
                            objItem.Url = @"http://www.allocine.fr/film/fichefilm_gen_cfilm=" + objItem.Id + ".html";
                            break;
                    }

                    if (objRest["movie"]["trailer"] != null)
                        objItem.Trailer = (string)objRest["movie"]["trailer"]["href"];

                    string runtime = (string)objRest["movie"]["runtime"];
                    int intRunTime;
                    if (int.TryParse(runtime, out intRunTime))
                        objItem.Runtime = (int?)new TimeSpan(0, 0, intRunTime).TotalMinutes;

                    string relased = string.Empty;
                    if (objRest["movie"]["release"] != null)
                        relased = (string)objRest["movie"]["release"]["releaseDate"];

                    if (string.IsNullOrWhiteSpace(relased) == false)
                    {
                        DateTime date;
                        if (DateTime.TryParse(relased, out date) == true)
                            objItem.Released = date;
                    }

                    JArray cast = (JArray)objRest["movie"]["castMember"];

                    //Fix since 2.6.0.0
                    if (cast != null)
                    {
                        objItem.Cast = new List<Artist>();
                        objItem.Directors = new List<Artist>();

                        foreach (JObject item in cast)
                        {
                            string picture = string.Empty;
                            if (item["picture"] != null)
                                picture = (string)item["picture"]["href"];

                            string name = (string)item["person"]["name"];
                            string job = (string)item["activity"]["code"];

                            if (string.IsNullOrWhiteSpace(name) == false)
                            {
                                bool isNew;
                                Artist artist = (ArtistServices.Get(name, out isNew));

                                if (string.IsNullOrWhiteSpace(picture) == false && MySettings.FastSearch == false)
                                    artist.Picture = Util.GetImage(picture);

                                if (job == "8002")
                                    objItem.Directors.Add(artist);
                                else if (job == "8001")
                                    objItem.Cast.Add(artist);
                            }
                        }
                    }

                    JArray genre = (JArray)objRest["movie"]["genre"];
                    objItem.Genres = new List<string>();

                    foreach (JObject item in genre)
                        objItem.Genres.Add((string)item["$"]);

                    JArray media = (JArray)objRest["movie"]["media"];

                    //Fix Since version 2.5.5.0
                    if (media != null)
                    {
                        foreach (JObject item in media)
                        {
                            if ((string)item["type"]["$"] == "Photo")
                            {
                                objItem.BackdropOriginal = (string)item["thumbnail"]["href"];
                                break;
                            }
                        }
                    }

                    if (objRest["movie"]["release"] != null)
                        if (objRest["movie"]["release"]["distributor"] != null)
                            objItem.Studio = (string)objRest["movie"]["release"]["distributor"]["name"];

                    if (objRest["movie"]["statistics"] != null)
                    {
                        string strrating = (string)objRest["movie"]["statistics"]["userRating"];
                        if (string.IsNullOrWhiteSpace(strrating) == false)
                        {
                            double value;
                            CultureInfo cultureInfo = new CultureInfo("en-US", true);
                            if (double.TryParse(strrating, NumberStyles.Any, cultureInfo.NumberFormat, out value) ==
                                true)
                                objItem.Rating = value.ToString("F");
                            else if (double.TryParse(strrating.Replace(',', '.'), out value) == true)
                                objItem.Rating = value.ToString(CultureInfo.InvariantCulture);
                            else
                                objItem.Rating = strrating;
                        }
                    }
                }
            }

            return objItem;
        }
        public static Collection<PartialMatche> MovieToPartialMatche(XElement objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            var query = from item in objRest.Elements()
                        where item.Name.LocalName == "movie"
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();

                objItem.Link = Util.GetAttributValue(objTemp, "code");

                objItem.Title = Util.GetElementValue(objTemp, @"{http://www.allocine.net/v6/ns/}title");
                if (string.IsNullOrWhiteSpace(objItem.Title))
                    objItem.Title = Util.GetElementValue(objTemp, @"{http://www.allocine.net/v6/ns/}originalTitle");

                objItem.ImageUrl = Util.GetAttributValue(objTemp, @"{http://www.allocine.net/v6/ns/}poster", "href");

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Collection<PartialMatche> MovieToPartialMatche(JObject objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            JArray nodes = (JArray)objRest["feed"]["movie"];

            if (nodes != null)
            {
                foreach (JObject node in nodes)
                {
                    PartialMatche objItem = new PartialMatche();

                    objItem.Link = (string)node["code"];

                    objItem.Title = (string)node["title"];
                    if (string.IsNullOrWhiteSpace(objItem.Title))
                        objItem.Title = (string)node["originalTitle"];

                    if (node["poster"] != null)
                        objItem.ImageUrl = (string)node["poster"]["href"];

                    lstResults.Add(objItem);
                }
            }
            return lstResults;

        }

        public static Collection<PartialMatche> SeriesToPartialMatche(XElement objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            var query = from item in objRest.Elements()
                        where item.Name.LocalName == "tvseries"
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                PartialMatche objItem = new PartialMatche();

                objItem.Link = Util.GetAttributValue(objTemp, "code");

                objItem.Title = Util.GetElementValue(objTemp, @"{http://www.allocine.net/v6/ns/}title");
                if (string.IsNullOrWhiteSpace(objItem.Title))
                    objItem.Title = Util.GetElementValue(objTemp, @"{http://www.allocine.net/v6/ns/}originalTitle");

                objItem.ImageUrl = Util.GetAttributValue(objTemp, @"{http://www.allocine.net/v6/ns/}poster", "href");

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Collection<PartialMatche> SeriesToPartialMatche(JObject objRest)
        {
            Collection<PartialMatche> lstResults = new Collection<PartialMatche>();
            if (objRest["feed"] != null)
            {
                JArray nodes = (JArray)objRest["feed"]["tvseries"];

                if (nodes != null)
                {
                    foreach (JObject node in nodes)
                    {
                        PartialMatche objItem = new PartialMatche();

                        objItem.Link = (string)node["code"];

                        objItem.Title = (string)node["title"];
                        if (string.IsNullOrWhiteSpace(objItem.Title))
                            objItem.Title = (string)node["originalTitle"];

                        if (node["poster"] != null)
                            objItem.ImageUrl = (string)node["poster"]["href"];

                        lstResults.Add(objItem);
                    }
                }
            }
            return lstResults;
        }
        public static AlloCine SerieToObject(XElement objRest, string seasonNumber)
        {
            AlloCine objItem = null;
            if (objRest != null)
            {
                objItem = new AlloCine();
                objItem.Id = Util.GetAttributValue(objRest, "code");
                objItem.OriginalTitle = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}originalTitle");
                objItem.Title = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}title");

                DateTime date;
                if (DateTime.TryParse(Util.GetAttributValue(objRest, @"{http://www.allocine.net/v6/ns/}originalBroadcast", "dateStart"), out date) == true)
                    objItem.Released = date;

                string runtime = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}formatTime");
                int intRunTime;
                if (int.TryParse(runtime, out intRunTime))
                    objItem.Runtime = intRunTime;

                string inProduction = Util.GetAttributValue(objRest, @"{http://www.allocine.net/v6/ns/}productionStatus", "code");
                if (inProduction == "122004")
                    objItem.IsInProduction = true;

                objItem.NumberOfSeason = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}seasonCount");
                objItem.NumberOfEpisodes = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}episodeCount");

                objItem.Country = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}nationalityList", @"{http://www.allocine.net/v6/ns/}nationality");

                var genres = from item in objRest.Descendants("{http://www.allocine.net/v6/ns/}genre")
                             select item.Value;

                objItem.Genres = new List<string>(genres.ToArray());

                objItem.Tagline = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}synopsisShort");
                objItem.Description = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}synopsis");

                var cast = from item in objRest.Descendants("{http://www.allocine.net/v6/ns/}castMember")
                           select item;

                XElement[] artists = cast.ToArray();
                objItem.Cast = new List<Artist>();
                objItem.Directors = new List<Artist>();
                foreach (XElement item in artists)
                {

                    string picture = Util.GetAttributValue(item, @"{http://www.allocine.net/v6/ns/}picture", "href");
                    string name = Util.GetElementValue(item, @"{http://www.allocine.net/v6/ns/}person");
                    string job = Util.GetAttributValue(item, @"{http://www.allocine.net/v6/ns/}activity", "code");

                    if (string.IsNullOrWhiteSpace(name) == false)
                    {
                        bool isNew;
                        Artist artist = (ArtistServices.Get(name, out isNew));

                        if (string.IsNullOrWhiteSpace(picture) == false && MySettings.FastSearch == false)
                            artist.Picture = Util.GetImage(picture);

                        if (job == "8002")
                            objItem.Directors.Add(artist);
                        else if (job == "8001")
                            objItem.Cast.Add(artist);
                    }
                }

                objItem.PosterOriginal = Util.GetAttributValue(objRest, @"{http://www.allocine.net/v6/ns/}poster", "href");

                objItem.Url = @"http://www.allocine.fr/series/ficheserie_gen_cserie=" + objItem.Id + ".html";
                objItem.Trailer = Util.GetAttributValue(objRest, @"{http://www.allocine.net/v6/ns/}trailer", "href");

                var imagesUrl = from item in objRest.Descendants("{http://www.allocine.net/v6/ns/}media")
                                let xElement = item.Element("{http://www.allocine.net/v6/ns/}type")
                                where xElement != null && xElement.Value == "Photo"
                                select item;

                Collection<XElement> images = new Collection<XElement>(imagesUrl.ToArray());
                if (images.Count > 0)
                    objItem.BackdropOriginal = Util.GetAttributValue(images[0], @"{http://www.allocine.net/v6/ns/}thumbnail", "href");

                objItem.Studio = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}originalChannel");

                var rating = from item in objRest.Elements()
                             where item.Name.LocalName == "statistics"
                             select item;

                Collection<XElement> lstRating = new Collection<XElement>(rating.ToArray());
                if (lstRating.Count > 0)
                {
                    string strrating = Util.GetElementValue(lstRating[0], "{http://www.allocine.net/v6/ns/}userRating");
                    double value;
                    CultureInfo cultureInfo = new CultureInfo("en-US", true);
                    if (double.TryParse(strrating, NumberStyles.Any, cultureInfo.NumberFormat, out value) == true)
                        objItem.SerieRating = (long)(value * 4);
                    else if (double.TryParse(strrating.Replace(',', '.'), out value) == true)
                        objItem.SerieRating = (long)(value * 4);
                }

                var episodes = from item in objRest.Descendants("{http://www.allocine.net/v6/ns/}season")
                               let xElement = item.Element("{http://www.allocine.net/v6/ns/}seasonNumber")
                               where xElement != null && xElement.Value == seasonNumber
                               select item;

                Collection<XElement> seasonEp = new Collection<XElement>(episodes.ToArray());
                if (seasonEp.Count > 0)
                    objItem.SeasonEpisodes = Util.GetElementValue(seasonEp[0], @"{http://www.allocine.net/v6/ns/}episodeCount");

            }

            return objItem;
        }
        public static AlloCine SerieToObject(JObject objRest, string seasonNumber, LanguageType languageType)
        {
            AlloCine objItem = null;
            if (objRest != null)
            {
                objItem = new AlloCine();

                objItem.Id = (string)objRest["tvseries"]["code"];
                objItem.OriginalTitle = (string)objRest["tvseries"]["originalTitle"];
                objItem.Title = (string)objRest["tvseries"]["title"];

                string relased = string.Empty;
                if (objRest["tvseries"]["originalBroadcast"]["dateStart"] != null)
                    relased = (string)objRest["tvseries"]["originalBroadcast"]["dateStart"];

                if (string.IsNullOrWhiteSpace(relased) == false)
                {
                    DateTime date;
                    if (DateTime.TryParse(relased, out date) == true)
                        objItem.Released = date;
                }


                objItem.Runtime = (int)objRest["tvseries"]["formatTime"];

                string inProduction = (string)objRest["tvseries"]["productionStatus"]["code"];
                if (inProduction == "122004")
                    objItem.IsInProduction = true;

                objItem.NumberOfSeason = (string)objRest["tvseries"]["seasonCount"];
                objItem.NumberOfEpisodes = (string)objRest["tvseries"]["episodeCount"];

                objItem.Country = (string)objRest["tvseries"]["nationality"][0]["$"];

                JArray genre = (JArray)objRest["tvseries"]["genre"];
                objItem.Genres = new List<string>();

                foreach (JObject item in genre)
                    objItem.Genres.Add((string)item["$"]);

                objItem.Tagline = (string)objRest["tvseries"]["synopsisShort"];
                objItem.Description = (string)objRest["tvseries"]["synopsis"];

                JArray cast = (JArray)objRest["tvseries"]["castMember"];

                objItem.Cast = new List<Artist>();
                objItem.Directors = new List<Artist>();
                foreach (JObject item in cast)
                {
                    string picture = string.Empty;
                    if (item["picture"] != null)
                        picture = (string)item["picture"]["href"];

                    string name = (string)item["person"]["name"];
                    string job = (string)item["activity"]["code"];

                    if (string.IsNullOrWhiteSpace(name) == false)
                    {
                        bool isNew;
                        Artist artist = (ArtistServices.Get(name, out isNew));

                        if (string.IsNullOrWhiteSpace(picture) == false && MySettings.FastSearch == false)
                            artist.Picture = Util.GetImage(picture);

                        if (job == "8002")
                            objItem.Directors.Add(artist);
                        else if (job == "8001")
                            objItem.Cast.Add(artist);
                    }
                }

                //FIX 2.8.0.0
                if (objRest["tvseries"]["poster"] != null)
                    objItem.PosterOriginal = (string)objRest["tvseries"]["poster"]["href"];

                switch (languageType)
                {
                    case LanguageType.BR:
                    case LanguageType.PT:
                        objItem.Url = @"http://www.adorocinema.com/filmes/filme-" + objItem.Id;
                        break;
                    default:
                        objItem.Url = @"http://www.allocine.fr/series/ficheserie_gen_cserie=" + objItem.Id + ".html";
                        break;
                }

                //FIX 2.8.0.0
                if (objRest["tvseries"]["trailer"] != null)
                    objItem.Trailer = (string)objRest["tvseries"]["trailer"]["href"];

                JArray media = (JArray)objRest["tvseries"]["media"];

                //FIX 2.8.9.0
                if (media != null)
                {
                    foreach (JObject item in media)
                    {
                        if ((string)item["type"]["$"] == "Photo")
                        {
                            objItem.BackdropOriginal = (string)item["thumbnail"]["href"];
                            break;
                        }
                    }
                }

                objItem.Studio = (string)objRest["tvseries"]["originalChannel"]["$"];


                string strrating = (string)objRest["tvseries"]["statistics"]["userRating"];
                double value;
                CultureInfo cultureInfo = new CultureInfo("en-US", true);
                if (double.TryParse(strrating, NumberStyles.Any, cultureInfo.NumberFormat, out value) == true)
                    objItem.SerieRating = (long)(value * 4);
                else if (double.TryParse(strrating.Replace(',', '.'), out value) == true)
                    objItem.SerieRating = (long)(value * 4);

            }

            return objItem;
        }

        public static Collection<AlloCine> ArtistToCollection(XElement objRest)
        {
            Collection<AlloCine> lstResults = new Collection<AlloCine>();
            var query = from item in objRest.Elements()
                        where item.Name.LocalName == "person"
                        select item;

            XElement[] nodes = query.ToArray();
            foreach (XElement node in nodes)
            {
                XElement objTemp = node;
                AlloCine objItem = new AlloCine();

                objItem.Title = Util.GetElementValue(objTemp, @"{http://www.allocine.net/v6/ns/}name");
                objItem.Id = Util.GetAttributValue(objTemp, "code");
                objItem.PosterOriginal = Util.GetAttributValue(objTemp, @"{http://www.allocine.net/v6/ns/}picture", "href");

                lstResults.Add(objItem);
            }
            return lstResults;
        }
        public static Collection<AlloCine> ArtistToCollection(JObject objRest)
        {
            Collection<AlloCine> lstResults = new Collection<AlloCine>();
            JArray nodes = (JArray)objRest["feed"]["person"];
            //Fix Since 2.6.7.0
            if (nodes != null)
            {
                foreach (JObject node in nodes)
                {
                    AlloCine objItem = new AlloCine();

                    objItem.Title = (string)node["name"];
                    objItem.Id = (string)node["code"];
                    if (node["picture"] != null)
                        objItem.PosterOriginal = (string)node["picture"]["href"];

                    lstResults.Add(objItem);
                }
            }
            return lstResults;
        }
        public static Artist CastToArtist(XElement objRest)
        {
            if (objRest == null) return null;

            XElement objTemp = objRest;
            Artist objItem = new Artist();

            #region Name
            string firstName = Util.GetAttributValue(objTemp, @"{http://www.allocine.net/v6/ns/}name", "given");
            string lastName = Util.GetAttributValue(objTemp, @"{http://www.allocine.net/v6/ns/}name", "family");

            objItem.FirstName = firstName;
            objItem.LastName = lastName;

            objItem.FulleName = objItem.FirstName + " " + objItem.LastName;
            #endregion
            #region Sex
            string sex = Util.GetElementValue(objRest, @"{http://www.allocine.net/v6/ns/}gender");
            if (sex == "1")
                objItem.Sex = false;
            else
                objItem.Sex = true;
            #endregion
            #region Birthday
            DateTime objDate;
            if (DateTime.TryParse(Util.GetElementValue(objTemp, @"{http://www.allocine.net/v6/ns/}birthDate"), out objDate))
                objItem.BirthDay = objDate;
            #endregion
            #region Picture
            objItem.Picture = Util.GetImage(Util.GetAttributValue(objTemp, @"{http://www.allocine.net/v6/ns/}picture", "href"));
            #endregion
            #region Credits
            var creditsquery = from item in objTemp.Descendants(@"{http://www.allocine.net/v6/ns/}participation")
                               select item;

            XElement[] credits = creditsquery.ToArray();
            foreach (XElement item in credits)
            {

                ArtistCredits movie = new ArtistCredits();

                movie.Title = Util.GetElementValue(item, @"{http://www.allocine.net/v6/ns/}movie", @"{http://www.allocine.net/v6/ns/}title");

                if (string.IsNullOrWhiteSpace(movie.Title) == false)
                {

                    movie.BuyLink = string.Format(@"http://www.allocine.fr/film/fichefilm_gen_cfilm={0}.html", Util.GetAttributValue(item, @"{http://www.allocine.net/v6/ns/}movie", "code"));
                    movie.EntityType = EntityType.Movie;
                    movie.Notes = Util.GetElementValue(item, @"{http://www.allocine.net/v6/ns/}role");

                    var query = from dates in item.Descendants("{http://www.allocine.net/v6/ns/}movie").Elements()
                                where dates.Name.LocalName == "release"
                                select dates;

                    XElement[] releaseTab = query.ToArray();
                    if (releaseTab.Any())
                    {
                        string date = Util.GetElementValue(releaseTab[0], @"{http://www.allocine.net/v6/ns/}releaseDate");
                        DateTime releaseDate;
                        if (DateTime.TryParse(date, CultureInfo.CreateSpecificCulture("fr-FR"), DateTimeStyles.None, out releaseDate) == true)
                            movie.ReleaseDate = releaseDate;
                    }

                    movie.Title = movie.Title.Replace(@"""", " ");

                    if (Dal.GetInstance.GetArtistCredit(movie.Title, objItem.FulleName) == null)
                        objItem.ArtistCredits.Add(movie);
                }
            }
            #endregion
            #region Bio
            objItem.Bio = Util.GetElementValue(objTemp, @"{http://www.allocine.net/v6/ns/}biography");
            #endregion
            #region PlaceBirth
            objItem.PlaceBirth = Util.GetElementValue(objTemp, @"{http://www.allocine.net/v6/ns/}birthPlace");
            #endregion

            return objItem;
        }
        public static Artist CastToArtist(JObject objRest)
        {
            if (objRest == null) return null;

            Artist objItem = new Artist();

            #region Name
            //Fix Since 2.6.7.0
            if (objRest["person"]["name"]["given"] != null)
                objItem.FirstName = (string)objRest["person"]["name"]["given"];

            objItem.LastName = (string)objRest["person"]["name"]["family"];

            objItem.FulleName = (objItem.FirstName + " " + objItem.LastName).Trim();
            #endregion
            #region Sex
            string sex = (string)objRest["person"]["gender"];
            if (sex == "1")
                objItem.Sex = false;
            else
                objItem.Sex = true;
            #endregion
            #region Birthday
            DateTime date;
            if (DateTime.TryParse((string)objRest["person"]["birthDate"], out date) == true)
                objItem.BirthDay = date;
            #endregion
            #region Picture
            if (objRest["person"]["picture"] != null)
                objItem.Picture = Util.GetImage((string)objRest["person"]["picture"]["href"]);
            #endregion
            #region Credits

            JArray credits = (JArray)objRest["person"]["participation"];

            //FIX 2.9.0.0
            if (credits != null)
            {
                Parallel.ForEach(credits, item =>
                {
                    if (item["movie"] != null)
                    {
                        ArtistCredits movie = new ArtistCredits();

                        movie.Title = (string) item["movie"]["title"];

                        if (string.IsNullOrWhiteSpace(movie.Title) == false)
                        {
                            //FIX 2.9.0.0
                            string code = (string) item["movie"]["code"];
                            if (string.IsNullOrWhiteSpace(code) == false)
                                movie.BuyLink =
                                    string.Format(@"http://www.allocine.fr/film/fichefilm_gen_cfilm={0}.html", code);

                            movie.EntityType = EntityType.Movie;
                            movie.Notes = (string) item["movie"]["role"];
                            DateTime dateTime;
                            if (item["movie"]["release"] != null)
                                if (DateTime.TryParse((string) item["movie"]["release"]["releaseDate"], out dateTime) ==
                                    true)
                                    movie.ReleaseDate = dateTime;

                            if (objItem.ArtistCredits == null)
                                objItem.ArtistCredits = new List<ArtistCredits>();

                            movie.Title = movie.Title.Replace(@"""", " ");

                            if (Dal.GetInstance.GetArtistCredit(movie.Title, objItem.FulleName) == null)
                                objItem.ArtistCredits.Add(movie);
                        }
                    }
                });
            }

            #endregion
            #region Bio
            objItem.Bio = (string)objRest["person"]["biographyShort"];
            #endregion
            #region PlaceBirth
            objItem.PlaceBirth = (string)objRest["person"]["birthPlace"];
            #endregion

            return objItem;
        }
    }
}