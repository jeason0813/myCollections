
using System.Globalization;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Xml;
using myCollections.Utils;
using System.Collections.ObjectModel;
using myCollections.Data;
using myCollections.AmazonWebServices;
using System.Collections.Generic;
using myCollections.Data.SqlLite;
using myCollections.BL.Services;

namespace myCollections.BL.Providers
{
    static class AmazonServices
    {
        const string Aws1 = "AKIAJDMKOP7ZWTKQUP5Q";
        const string Aws2 = "5gKKcebzLmeQxSeMRfXs6ZMU3nKmEiqXPEmy2q6i";
        const string AssociateTag = "mycollectio03-20";

        private class AmazonHeader : MessageHeader
        {
            private readonly string _name;
            private readonly string _value;

            public AmazonHeader(string name, string value)
            {
                _name = name;
                _value = value;
            }

            public override string Name { get { return _name; } }
            public override string Namespace { get { return "http://security.amazonaws.com/doc/2007-01-01/"; } }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter xmlDictionaryWriter, MessageVersion messageVersion)
            {
                xmlDictionaryWriter.WriteString(_value);
            }
        }
        private class AmazonSigningMessageInspector : IClientMessageInspector
        {
            private readonly string _accessKeyId = "";
            private readonly string _secretKey = "";

            public AmazonSigningMessageInspector(string accessKeyId, string secretKey)
            {
                _accessKeyId = accessKeyId;
                _secretKey = secretKey;
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                // prepare the data to sign
                string operation = Regex.Match(request.Headers.Action, "[^/]+$").ToString();
                DateTime now = DateTime.UtcNow;
                string timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string signMe = operation + timestamp;
                byte[] bytesToSign = Encoding.UTF8.GetBytes(signMe);

                // sign the data
                byte[] secretKeyBytes = Encoding.UTF8.GetBytes(_secretKey);
                HMAC hmacSha256 = new HMACSHA256(secretKeyBytes);
                byte[] hashBytes = hmacSha256.ComputeHash(bytesToSign);
                string signature = Convert.ToBase64String(hashBytes);

                // add the signature information to the request headers
                request.Headers.Add(new AmazonHeader("AWSAccessKeyId", _accessKeyId));
                request.Headers.Add(new AmazonHeader("Timestamp", timestamp));
                request.Headers.Add(new AmazonHeader("Signature", signature));

                return null;
            }

            public void AfterReceiveReply(ref Message reply, object correlationState) { }
        }
        private class AmazonSigningEndpointBehavior : IEndpointBehavior
        {
            private readonly string _accessKeyId = string.Empty;
            private readonly string _secretKey = string.Empty;

            public AmazonSigningEndpointBehavior(string accessKeyId, string secretKey)
            {
                _accessKeyId = accessKeyId;
                _secretKey = secretKey;
            }

            public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(new AmazonSigningMessageInspector(_accessKeyId, _secretKey));
            }

            public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher) { }
            public void Validate(ServiceEndpoint serviceEndpoint) { }
            public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters) { }
        }

        public static Hashtable Parse(string asin, AmazonCountry country, bool isBarcode, AmazonIndex index, string barcodeType)
        {
            Hashtable objResults = new Hashtable();

            try
            {
                ItemLookupRequest request = new ItemLookupRequest();

                if (isBarcode)
                {
                    switch (barcodeType)
                    {
                        case "EAN_8":
                        case "EAN_13":
                            request.IdType = ItemLookupRequestIdType.EAN;
                            break;
                        case "UPC_A":
                        case "UPC_E":
                        case "UPC_EAN_EXTENSION":
                            request.IdType = ItemLookupRequestIdType.UPC;
                            break;
                        default:
                            request.IdType = ItemLookupRequestIdType.EAN;
                            break;
                    }
                    request.SearchIndex = index.ToString();
                }
                else
                    request.IdType = ItemLookupRequestIdType.ASIN;

                request.IdTypeSpecified = true;
                request.ItemId = new [] { asin };


                request.ResponseGroup = new [] {"EditorialReview", 
                                                      "Images", "ItemAttributes", "Large","Tracks"};

                ItemLookup itemLookup = new ItemLookup();
                itemLookup.AWSAccessKeyId = Aws1;
                itemLookup.AssociateTag = AssociateTag;
                itemLookup.Request = new [] { request };

                AWSECommerceServicePortTypeClient client =
                        new AWSECommerceServicePortTypeClient(
                             new BasicHttpBinding("AWSECommerceServiceBinding"),
                            new EndpointAddress(string.Format("https://webservices.amazon.{0}/onca/soap?Service=AWSECommerceService", country.ToString())));

                // add authentication to the ECS client
                client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(Aws1, Aws2));

                ItemLookupResponse response = client.ItemLookup(itemLookup);

                if (response.Items.GetLength(0) > 0)
                    if (response.Items[0].Item != null)
                        if (response.Items[0].Item.GetLength(0) > 0)
                        {
                            Item item = response.Items[0].Item[0];
                            #region Actor

                            bool isNew;
                            if (item.ItemAttributes.Actor != null)
                            {
                                List<Artist> actors = item.ItemAttributes.Actor.Select(actor => ArtistServices.Get(actor, out isNew)).ToList();
                                objResults.Add("Actors", actors);
                            }
                            #endregion
                            #region Album
                            if (string.IsNullOrWhiteSpace(item.ItemAttributes.Title) == false)
                                objResults.Add("Album", item.ItemAttributes.Title);
                            #endregion
                            #region AlternateVersions
                            if (item.AlternateVersions != null)
                                if (item.AlternateVersions.GetLength(0) > 0)
                                    objResults.Add("OriginalTitle", item.AlternateVersions[0].Title);
                            #endregion
                            #region Artist
                            if (item.ItemAttributes.Artist != null)
                                if (item.ItemAttributes.Artist.GetLength(0) > 0)
                                    if (MySettings.FastSearch == false && index == AmazonIndex.Music)
                                    {
                                        Artist artist = ArtistServices.Get(item.ItemAttributes.Artist[0],out isNew);

                                        GetArtistCredits(artist, index, country, 1);
                                        objResults.Add("Artist", artist);
                                    }
                                    else
                                        objResults.Add("Artist", ArtistServices.Get(item.ItemAttributes.Artist[0], out isNew));
                            #endregion
                            #region AspectRatio
                            if (string.IsNullOrWhiteSpace(item.ItemAttributes.AspectRatio) == false)
                                objResults.Add("DisplayAspectRatio", MovieServices.GetAspectRatio(item.ItemAttributes.AspectRatio));
                            #endregion
                            #region Author

                            if (item.ItemAttributes.Author != null)
                                if (item.ItemAttributes.Author.GetLength(0) > 0)
                                {
                                    if (MySettings.FastSearch == false && index == AmazonIndex.Books)
                                    {
                                        //Fix since 2.6.7.0
                                        Artist artist = ArtistServices.Get(item.ItemAttributes.Author[0],out isNew);
                                        GetArtistCredits(artist, index, country, 1);
                                        objResults.Add("Author", artist);
                                    }
                                    else
                                        objResults.Add("Author", ArtistServices.Get(item.ItemAttributes.Author[0], out isNew));
                                }
                            #endregion
                            #region Background
                            if (item.ImageSets != null && item.LargeImage != null)
                                if (item.ImageSets.GetLength(0) > 0 && item.ImageSets[0].LargeImage.URL != item.LargeImage.URL)
                                    objResults.Add("Background", item.ImageSets[0].LargeImage.URL);
                                else if (item.ImageSets.GetLength(0) > 1 && item.ImageSets[1].LargeImage.URL != item.LargeImage.URL)
                                    objResults.Add("Background", item.ImageSets[1].LargeImage.URL);
                            #endregion
                            #region BarCode
                            if (string.IsNullOrWhiteSpace(item.ItemAttributes.EAN) == false)
                                objResults.Add("BarCode", item.ItemAttributes.EAN);
                            else if (string.IsNullOrWhiteSpace(item.ItemAttributes.UPC) == false)
                                objResults.Add("BarCode", item.ItemAttributes.UPC);
                            else if (string.IsNullOrWhiteSpace(item.ASIN) == false)
                                objResults.Add("BarCode", item.ASIN);
                            #endregion
                            #region Comments
                            if (item.CustomerReviews != null)
                                if (item.CustomerReviews.HasReviews == true)
                                    objResults.Add("Comments", Util.PurgeHtml(GetReview(item.CustomerReviews.IFrameURL, country)));
                            #endregion
                            #region Description
                            if (item.EditorialReviews != null)
                            {
                                string description = string.Empty;
                                foreach (EditorialReview edito in item.EditorialReviews)
                                {
                                    if (edito.Content.Length > description.Length && edito.IsLinkSuppressed == false)
                                    {
                                        description = edito.Content;
                                        break;
                                    }
                                }
                                objResults.Add("Description", Util.PurgeHtml(description));
                            }
                            #endregion
                            #region Director
                            if (item.ItemAttributes.Director != null)
                            {
                                List<Artist> directors = item.ItemAttributes.Director.Select(director => ArtistServices.Get(director,out isNew)).ToList();
                                objResults.Add("Director", directors);
                            }
                            #endregion
                            #region Editor
                            if (string.IsNullOrEmpty(item.ItemAttributes.Publisher) == false)
                                objResults.Add("Editor", item.ItemAttributes.Publisher);

                            if (objResults.ContainsKey("Editor") == false)
                            {
                                if (string.IsNullOrEmpty(item.ItemAttributes.Label) == false)
                                    objResults.Add("Editor", item.ItemAttributes.Label);
                            }

                            if (objResults.ContainsKey("Editor") == false)
                            {
                                if (string.IsNullOrEmpty(item.ItemAttributes.Manufacturer) == false)
                                    objResults.Add("Editor", item.ItemAttributes.Manufacturer);
                            }

                            if (objResults.ContainsKey("Editor") == false)
                            {
                                if (string.IsNullOrEmpty(item.ItemAttributes.Studio) == false)
                                    objResults.Add("Editor", item.ItemAttributes.Studio);
                            }

                            #endregion
                            #region Format
                            if (item.ItemAttributes.Format != null)
                                if (item.ItemAttributes.Format.GetLength(0) > 0)
                                    objResults.Add("Format", item.ItemAttributes.Format[0]);
                            #endregion
                            #region Feature
                            if (item.ItemAttributes.Feature != null)
                                if (item.ItemAttributes.Feature.GetLength(0) > 0)
                                    if (objResults.ContainsKey("Description") == false)
                                        objResults.Add("Description", item.ItemAttributes.Feature[0]);
                                    else if (objResults.ContainsKey("Comments") == false)
                                        objResults.Add("Comments", item.ItemAttributes.Feature[0]);
                            #endregion
                            #region Image
                            if (item.LargeImage != null)
                                objResults.Add("Image", item.LargeImage.URL);
                            #endregion
                            #region ISBN
                            if (string.IsNullOrWhiteSpace(item.ItemAttributes.ISBN) == false)
                                objResults.Add("ISBN", item.ItemAttributes.ISBN);
                            #endregion
                            #region Language
                            if (item.ItemAttributes.Languages != null)
                                if (item.ItemAttributes.Languages.GetLength(0) > 0)
                                    objResults.Add("Language", item.ItemAttributes.Languages[0].Name);
                            #endregion
                            #region Links
                            if (string.IsNullOrEmpty(item.DetailPageURL) == false)
                                objResults.Add("Links", item.DetailPageURL);
                            #endregion
                            #region Pages
                            if (string.IsNullOrEmpty(item.ItemAttributes.NumberOfPages) == false)
                                objResults.Add("Pages", item.ItemAttributes.NumberOfPages);
                            #endregion
                            #region Platform
                            if (item.ItemAttributes.Platform != null)
                                if (item.ItemAttributes.Platform.GetLength(0) > 0)
                                    objResults.Add("Platform", item.ItemAttributes.Platform[0]);
                            #endregion
                            #region ProductGroup
                            if (string.IsNullOrEmpty(item.ItemAttributes.ProductGroup) == false)
                                objResults.Add("ProductGroup", item.ItemAttributes.ProductGroup);
                            #endregion
                            #region ProductTypeName
                            if (string.IsNullOrEmpty(item.ItemAttributes.ProductTypeName) == false)
                                objResults.Add("ProductTypeName", item.ItemAttributes.ProductTypeName);
                            #endregion
                            #region Price
                            if (item.ItemAttributes.ListPrice != null)
                                objResults.Add("Price", item.ItemAttributes.ListPrice.Amount);
                            #endregion
                            #region Rated
                            if (string.IsNullOrEmpty(item.ItemAttributes.AudienceRating) == false)
                            {
                                if (item.ItemAttributes.AudienceRating.Contains("PG-13"))
                                    objResults.Add("Rated", "PG-13");
                                else if (item.ItemAttributes.AudienceRating.Contains("NC-17"))
                                    objResults.Add("Rated", "NC-17");
                                else if (item.ItemAttributes.AudienceRating.Contains("PG"))
                                    objResults.Add("Rated", "PG");
                                else if (item.ItemAttributes.AudienceRating.Contains("R"))
                                    objResults.Add("Rated", "R");
                                else if (item.ItemAttributes.AudienceRating.Contains("G") || item.ItemAttributes.AudienceRating.Contains("Tous publics"))
                                    objResults.Add("Rated", "G");
                            }
                            #endregion
                            #region Rating
                            if (objResults.ContainsKey("Rating") == false)
                            {
                                if (item.CustomerReviews != null)
                                    if (item.CustomerReviews.HasReviews == true)
                                        objResults.Add("Rating", GetRating(item.CustomerReviews.IFrameURL));
                            }
                            #endregion
                            #region Released
                            if (string.IsNullOrWhiteSpace(item.ItemAttributes.ReleaseDate) == false)
                            {
                                DateTime date;
                                if (DateTime.TryParse(item.ItemAttributes.ReleaseDate, out date) == true)
                                    objResults.Add("Released", date);
                            }
                            else if (string.IsNullOrWhiteSpace(item.ItemAttributes.PublicationDate) == false)
                            {
                                DateTime date;
                                if (DateTime.TryParse(item.ItemAttributes.PublicationDate, out date) == true)
                                    objResults.Add("Released", date);
                            }
                            #endregion
                            #region RunTime
                            if (item.ItemAttributes.RunningTime != null)
                                objResults.Add("Runtime", (int?)item.ItemAttributes.RunningTime.Value);
                            #endregion
                            #region Studio
                            if (string.IsNullOrWhiteSpace(item.ItemAttributes.Studio) == false)
                                objResults.Add("Studio", item.ItemAttributes.Studio);
                            #endregion
                            #region Title
                            if (string.IsNullOrWhiteSpace(item.ItemAttributes.Title) == false)
                                objResults.Add("Title", item.ItemAttributes.Title);
                            #endregion
                            #region Tracks
                            if (item.Tracks != null)
                                if (item.Tracks[0].Track.GetLength(0) > 0)
                                {
                                    List<string> tracks = item.Tracks[0].Track.Select(node => node.Value).ToList();
                                    objResults.Add("Tracks", tracks);
                                }
                            #endregion
                            #region Types
                            if (item.BrowseNodes != null)
                                if (item.BrowseNodes.BrowseNode.GetLength(0) > 0)
                                {
                                    List<string> types = new List<string>();
                                    foreach (BrowseNode node in item.BrowseNodes.BrowseNode)
                                    {
                                        if (node.Ancestors.GetLength(0) > 0)
                                            if (node.Ancestors[0].Name == "Genres" ||
                                                node.Ancestors[0].Name == "Styles" ||
                                                node.Ancestors[0].Name == "Nintendo DS" ||
                                                node.Ancestors[0].Name == "Categories" ||
                                                node.Ancestors[0].Name == "Categorías" ||
                                                node.Ancestors[0].Name == "Thèmes" ||
                                                node.Ancestors[0].Name == "Juegos" ||
                                                node.Ancestors[0].Name == "Genre (theme_browse-bin)" ||
                                                node.Ancestors[0].Name == "Les collections" ||
                                                node.Ancestors[0].Name == "Literature & Fiction" ||
                                                node.Ancestors[0].BrowseNodeId == "301138" ||
                                                node.Ancestors[0].BrowseNodeId == "301134" ||
                                                (node.Ancestors[0].BrowseNodeId == "425527031" && node.BrowseNodeId != "2486268031") ||
                                                node.Ancestors[0].BrowseNodeId == "466276" ||
                                                node.Ancestors[0].BrowseNodeId == "302068" ||
                                                node.Ancestors[0].BrowseNodeId == "924340031" ||
                                                node.Ancestors[0].BrowseNodeId == "301132" ||
                                                node.Ancestors[0].BrowseNodeId == "2" ||
                                                node.Ancestors[0].BrowseNodeId == "2396" ||
                                                node.Ancestors[0].Name == "Literatura y ficción")
                                                types.Add(node.Name);
                                            else if (node.Ancestors[0].IsCategoryRoot == false && node.Ancestors[0].Ancestors != null)
                                                if (node.Ancestors[0].Ancestors.GetLength(0) > 0)
                                                    if (node.Ancestors[0].Ancestors[0].Name == "Genres" ||
                                                        node.Ancestors[0].Name == "Nintendo DS" ||
                                                        node.Ancestors[0].Name == "Styles" ||
                                                        node.Ancestors[0].Name == "Categories" ||
                                                        node.Ancestors[0].Name == "Categorías" ||
                                                        node.Ancestors[0].Name == "Les collections" ||
                                                        node.Ancestors[0].Name == "Juegos" ||
                                                        node.Ancestors[0].Name == "Genre (theme_browse-bin)" ||
                                                        node.Ancestors[0].Name == "Literature & Fiction" ||
                                                        node.Ancestors[0].Name == "Literatura y ficción")
                                                        types.Add(node.Name);
                                    }
                                    objResults.Add("Types", types);
                                }
                            #endregion
                        }
                return objResults;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, asin);
                return null;
            }
        }
        private static string GetReview(string url, AmazonCountry country)
        {
            Encoding encoding = Encoding.UTF8;
            if (country == AmazonCountry.fr)
                encoding = Encoding.Default;

            string strResults = Util.GetHtmlPage(url, encoding);
            string parsing = "crIFrameReviewList";

            if (string.IsNullOrWhiteSpace(strResults)) return null;

            strResults = strResults.Substring(strResults.IndexOf(parsing, StringComparison.Ordinal) + parsing.Length);
            parsing = @"<div style=""margin-left:0.5em;"">";
            string[] objTables = Regex.Split(strResults, parsing);
            foreach (string row in objTables)
            {
                if (row.Trim().StartsWith(@"<div"))
                {
                    parsing = @"<div style=""padding-top: 10px; clear: both; width: 100%;"">";
                    string review = row.Substring(0, row.IndexOf(parsing, StringComparison.Ordinal)).Trim();
                    parsing = @"</div>";
                    review = review.Substring(review.LastIndexOf(parsing, StringComparison.Ordinal) + parsing.Length).Trim();
                    if (string.IsNullOrWhiteSpace(review) == false)
                        return Util.PurgeHtml(review);
                }
            }
            return null;
        }
        private static double? GetRating(string url)
        {
            string strResults = Util.GetHtmlPage(url, Encoding.UTF8);
            string parsing = "stars-";

            if (string.IsNullOrWhiteSpace(strResults)) return 0;

            strResults = strResults.Substring(strResults.IndexOf(parsing, StringComparison.Ordinal) + parsing.Length);
            parsing = ".";
            strResults = strResults.Substring(0, strResults.IndexOf(parsing, StringComparison.Ordinal));
            strResults = strResults.Replace('-', '.');
            double rating;
            if (double.TryParse(strResults, out rating) == false)
            {
                strResults = strResults.Replace('.', ',');
                if (double.TryParse(strResults, out rating) == true)
                    return rating * 4;

            }
            else
                return rating * 4;

            return null;
        }

        public static Collection<PartialMatche> Search(string search, string artist, AmazonIndex index, AmazonCountry country, AmazonBrowserNode node)
        {

            ItemSearch service = new ItemSearch();
            ItemSearchRequest request = new ItemSearchRequest();

            request.Title = search.Trim();
            request.SearchIndex = index.ToString();

            if (string.IsNullOrWhiteSpace(artist) == false)
                if (index == AmazonIndex.Books)
                    request.Author = artist;
                else
                    request.Artist = artist;

            request.Sort = "salesrank";

            if (node != AmazonBrowserNode.None)
                request.BrowseNode = Enum.Format(typeof(AmazonBrowserNode), node, "d");
            switch (index)
            {
                case AmazonIndex.VideoGames:
                    request.ResponseGroup = new [] { "Medium", "Images" };
                    break;
                default:
                    request.ResponseGroup = new [] { "Small", "Images" };
                    break;
            }

            service.Request = new [] { request };
            service.AWSAccessKeyId = Aws1;
            service.AssociateTag = AssociateTag;

            AWSECommerceServicePortTypeClient client =
                new AWSECommerceServicePortTypeClient(
                     new BasicHttpBinding("AWSECommerceServiceBinding"),
                     new EndpointAddress(string.Format("https://webservices.amazon.{0}/onca/soap?Service=AWSECommerceService", country.ToString())));

            // add authentication to the ECS client
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(Aws1, Aws2));
            ItemSearchResponse response = client.ItemSearch(service);
            if (response.Items.GetLength(0) > 0)
            {
                if (response.Items[0].Item != null)
                    return CreatePartialMatch(response.Items[0].Item, index);
                else
                    return null;
            }
            else
                return null;
        }
        private static Collection<PartialMatche> CreatePartialMatch(IEnumerable<Item> items, AmazonIndex index)
        {
            Collection<PartialMatche> lstMatche = new Collection<PartialMatche>();
            int i = 0;

            foreach (Item item in items)
            {
                PartialMatche objMatche = new PartialMatche();
                string infos = string.Empty;

                objMatche.Link = item.ASIN;

                if (item.SmallImage != null)
                    objMatche.ImageUrl = item.SmallImage.URL;

                switch (index)
                {
                    case AmazonIndex.Books:
                        if (item.ItemAttributes.Author != null)
                        {
                            foreach (string info in item.ItemAttributes.Author)
                            {
                                i++;
                                infos += info;
                                if (i < item.ItemAttributes.Author.GetLength(0))
                                    infos += "; ";
                            }
                        }
                        break;
                    case AmazonIndex.Music:
                        if (item.ItemAttributes.Artist != null)
                        {
                            foreach (string info in item.ItemAttributes.Artist)
                            {
                                i++;
                                infos += info;
                                if (i < item.ItemAttributes.Artist.GetLength(0))
                                    infos += "; ";
                            }
                        }
                        break;
                    case AmazonIndex.DVD:
                    case AmazonIndex.Classical:
                    case AmazonIndex.MusicTracks:
                    case AmazonIndex.Software:
                        break;
                    case AmazonIndex.VideoGames:
                        if (item.ItemAttributes.Platform != null)
                        {
                            foreach (string info in item.ItemAttributes.Platform)
                            {
                                i++;
                                infos += info;
                                if (i < item.ItemAttributes.Platform.GetLength(0))
                                    infos += "; ";
                            }
                        }
                        break;
                }


                objMatche.Title = item.ItemAttributes.Title;

                if (string.IsNullOrWhiteSpace(infos) == false)
                    objMatche.Title += " - " + infos;

                lstMatche.Add(objMatche);
            }
            return lstMatche;

        }

        private static void GetArtistCredits(Artist artist, AmazonIndex index, AmazonCountry country, int pageIndex)
        {
            ItemSearch itemsearch = new ItemSearch();
            ItemSearchRequest request = new ItemSearchRequest();
            request.SearchIndex = index.ToString();
            request.ItemPage = pageIndex.ToString(CultureInfo.InvariantCulture);
            EntityType entityType=EntityType.Music;

            switch (index)
            {
                case AmazonIndex.Books:
                    request.Author = artist.FulleName;
                    entityType=EntityType.Books;
                    break;
                case AmazonIndex.Music:
                    request.Artist = artist.FulleName;
                    entityType=EntityType.Music;
                    break;
            }

            request.ResponseGroup = new [] { "Small" };
            itemsearch.Request = new [] { request };
            itemsearch.AWSAccessKeyId = Aws1;
            itemsearch.AssociateTag = AssociateTag;

            AWSECommerceServicePortTypeClient client =
                new AWSECommerceServicePortTypeClient(
                     new BasicHttpBinding("AWSECommerceServiceBinding"),
                     new EndpointAddress(string.Format("https://webservices.amazon.{0}/onca/soap?Service=AWSECommerceService", country.ToString())));

            // add authentication to the ECS client
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(Aws1, Aws2));
            try
            {

            
            ItemSearchResponse response = client.ItemSearch(itemsearch);
            if (response.Items.GetLength(0) > 0)
            {
                if (response.Items[0].Item != null)
                {
                    foreach (Item item in response.Items[0].Item)
                    {
                        if (artist.ArtistCredits.Any(c => c.Title.ToUpperInvariant() == item.ItemAttributes.Title.ToUpperInvariant()) == false)
                        {
                            ArtistCredits credit = new ArtistCredits();
                            credit.Title = item.ItemAttributes.Title;
                            #region Released
                            if (string.IsNullOrWhiteSpace(item.ItemAttributes.ReleaseDate) == false)
                            {
                                DateTime date;
                                if (DateTime.TryParse(item.ItemAttributes.ReleaseDate, out date) == true)
                                    credit.ReleaseDate = date;
                            }
                            else if (string.IsNullOrWhiteSpace(item.ItemAttributes.PublicationDate) == false)
                            {
                                DateTime date;
                                if (DateTime.TryParse(item.ItemAttributes.PublicationDate, out date) == true)
                                    credit.ReleaseDate = date;
                            }
                            #endregion
                            credit.EntityType = entityType;
                            // credit.BuyLink = item.DetailPageURL;

                            //  credit.BuyLink = string.Format(@"http://www.amazon.{0}/gp/aws/cart/add.html?ASIN.1={1}&AWSAccessKeyId={2}&AssociateTag={3}",
                            //                              country.ToString(), item.ASIN, Aws1,AssociateTag);

                            credit.BuyLink = string.Format(@"http://www.amazon.{0}/exec/obidos/ASIN/{1}/{2}/",country, item.ASIN, AssociateTag);

                            #region Description
                            if (item.EditorialReviews != null)
                            {
                                string description = string.Empty;
                                foreach (EditorialReview edito in item.EditorialReviews)
                                {
                                    if (edito.Content.Length > description.Length && edito.IsLinkSuppressed == false)
                                    {
                                        description = edito.Content;
                                        break;
                                    }
                                }
                                credit.Notes = Util.PurgeHtml(description);
                            }
                            #endregion

                            artist.ArtistCredits.Add(credit);
                        }
                    }

                    int totalpage;
                    if (int.TryParse(response.Items[0].TotalPages, out totalpage))
                    {
                        if (pageIndex < totalpage)
                        {
                            pageIndex++;
                            GetArtistCredits(artist, index, country, pageIndex);
                        }
                    }
                }
            }
            }
            catch (ServerTooBusyException)
            {
            }
        }

    }
}
