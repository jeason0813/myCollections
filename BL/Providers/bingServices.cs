using System.Collections;
using System;
using System.Net;
using System.Collections.Generic;
using Bing;
using System.Globalization;
using System.Data.Services.Client;
using System.Linq;
using System.Collections.ObjectModel;
using myCollections.Data;
using myCollections.Pages;
using System.Threading.Tasks;
using myCollections.Utils;
namespace myCollections.BL.Providers
{
    static class BingServices
    {
        const string AppId = "PKGnECxGoQP2lQQ95s530xeQSFeit2S9d8xR1oR6p0s=";

        public static Hashtable SearchPortrait(string strSearch, bool usePartialMatche)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strSearch))
                    return null;

                Task.Factory.StartNew(() => Util.NotifyEvent("Provider: Bing Portrait : " + strSearch));

                BingSearchContainer objService = new BingSearchContainer(new Uri("https://api.datamarket.azure.com/Bing/Search/"));
                objService.Credentials = new NetworkCredential(AppId, AppId);
                DataServiceQuery<ImageResult> imagequery = objService.Image(strSearch, "DisableLocationDetection",
                                                           null, "Off", null, null, @"Style:Photo+Face:Face+Aspect:Tall");

                if (imagequery != null)
                {
                    List<ImageResult> images = imagequery.Execute().ToList();

                    if (usePartialMatche == true)
                        return ShowPartialMatch(images);
                    else if (images.Any())
                    {
                        Hashtable objResults = new Hashtable();
                        objResults.Add("Image", images[0].MediaUrl);
                        return objResults;
                    }
                    else return null;
                }
                else return null;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, strSearch);
                return null;

            }
        }
        private static Hashtable ShowPartialMatch(List<ImageResult> images)
        {
            Collection<PartialMatche> objPartialMatch = new Collection<PartialMatche>();
            Hashtable objResults = new Hashtable();

            if (images == null || images.Any() == false)
                return null;

            foreach (ImageResult objResult in images)
            {
                PartialMatche objItem = new PartialMatche
                {
                    Title = objResult.Title,
                    ImageUrl = objResult.Thumbnail.MediaUrl,
                    Link = objResult.MediaUrl
                };

                objPartialMatch.Add(objItem);
            }

            partialMatch objWindow = new partialMatch(objPartialMatch);
            objWindow.Title = "Bing - " + objWindow.Title;
            objWindow.ShowDialog();
            if (string.IsNullOrEmpty(objWindow.SelectedLink) == false)
            {
                objResults.Add("Image", objWindow.SelectedLink);
                objResults.Add("Title", objWindow.SelectedTitle);
            }

            return objResults;
        }

        public static Collection<PartialMatche> Search(string strSearch)
        {
            if (string.IsNullOrWhiteSpace(strSearch))
                return null;

            BingSearchContainer objService = new BingSearchContainer(new Uri("https://api.datamarket.azure.com/Bing/Search/"));
            objService.Credentials = new NetworkCredential(AppId, AppId);
            DataServiceQuery<ImageResult> imagequery = objService.Image(strSearch, "DisableLocationDetection",
                                                        CultureInfo.CurrentCulture.Name, "Off", null, null, null);

            IEnumerable<ImageResult> imagesList = imagequery.Execute();

            if (imagesList != null)
            {
                List<ImageResult> images = imagesList.ToList();
                return CreatePartialMatch(images);
            }
            else
                return null;

        }
        private static Collection<PartialMatche> CreatePartialMatch(IEnumerable<ImageResult> images)
        {
            Collection<PartialMatche> objPartialMatch = new Collection<PartialMatche>();

            IEnumerable<ImageResult> imageResults = images.ToArray();
            if (images == null || imageResults.Any() == false)
                return null;

            foreach (ImageResult objResult in imageResults)
            {
                PartialMatche objItem = new PartialMatche
                {
                    Title = objResult.Title,
                    ImageUrl = objResult.Thumbnail.MediaUrl,
                    Link = objResult.MediaUrl
                };

                objPartialMatch.Add(objItem);
            }

            return objPartialMatch;

        }
        public static Hashtable Parse(string strUrl)
        {
            Hashtable objResults = new Hashtable();
            if (Uri.IsWellFormedUriString(strUrl, UriKind.RelativeOrAbsolute))
            {
                Uri uri = new Uri(strUrl);
                objResults.Add("Image", strUrl);
                objResults.Add("Links", uri.AbsoluteUri);
            }
            return objResults;
        }
    }
}
