using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using myCollections.Data;
using myCollections.Data.Rest;
using myCollections.Utils;
using Newtonsoft.Json.Linq;

namespace myCollections.BL.Providers
{
    static class SoftTonicServices
    {
        private const string Key = "a86bd70ec041e0ad46649cfe4150608e";
        public static Hashtable Parse(string strId)
        {

            Hashtable objResults = new Hashtable();
            //Uri strUrl = new Uri(string.Format(@"http://api.softonic.com/{0}/programs.{1}?program_id={2}&key={3}", "en", "json", strId, Key));
            Uri strUrl = new Uri(string.Format(@"http://api.softonic.com/{0}/programs/{2}.{1}?key={3}", "en", "json", strId, Key));
            try
            {
                //Fix Since 2.6.0.0
                string response = Util.GetRest(strUrl);

                if (string.IsNullOrWhiteSpace(response) == false)
                {
                    JObject restResponse = JObject.Parse(response);

                    Softonic objSoftonic = Softonic.AppsToObject(restResponse);
                    if (objSoftonic != null)
                    {
                        objResults.Add("Description", Util.PurgeHtml(objSoftonic.Description));
                        objResults.Add("EditorLink", objSoftonic.EditorLink);
                        objResults.Add("Image", objSoftonic.Image);

                        if (string.IsNullOrWhiteSpace(objSoftonic.Language) == false)
                            objResults.Add("Language", objSoftonic.Language);

                        if (string.IsNullOrWhiteSpace(objSoftonic.Link) == false)
                            objResults.Add("Links", objSoftonic.Link);

                        objResults.Add("Rating", objSoftonic.Rating);

                        if (string.IsNullOrWhiteSpace(objSoftonic.ReleasedDate) == false)
                            objResults.Add("Released", objSoftonic.ReleasedDate);

                        objResults.Add("Title", objSoftonic.Title);

                        if (string.IsNullOrWhiteSpace(objSoftonic.Version) == false)
                            objResults.Add("Version", objSoftonic.Version);
                       
                        objResults.Add("Types", objSoftonic.Types);

                        if (string.IsNullOrWhiteSpace(objSoftonic.Editor) == false)
                            objResults.Add("Editor", objSoftonic.Editor);

                        return objResults;
                    }
                    else return null;
                }
                else return null;
            }
            catch (Exception ex)
            {
                Util.LogException(ex, strUrl.ToString());
                return null;
            }
        }
        public static Collection<PartialMatche> Search(string search)
        {
            if (string.IsNullOrEmpty(search) == false)
            {
                Uri strUrl = new Uri(string.Format(@"http://api.softonic.com/{0}/programs/search.{1}?query={2}&platform_id={3}&key={4}",
                                                    "en", "json", search, "2", Key));

                string response = Util.GetRest(strUrl);
                if (string.IsNullOrWhiteSpace(response) == false)
                {
                    JObject restResponse = JObject.Parse(response);
                    if (restResponse != null)
                        return Softonic.AppsToPartialMatche(restResponse);
                    else
                        return null;
                }
                else return null;
            }
            else
                return null;
        }
    }
}
