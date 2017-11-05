using System;
using System.ServiceModel.Syndication;
using System.Xml;
using myCollections.Data;

namespace myCollections.BL.Services
{
    static class NewsServices
    {
        public static SyndicationItem CheckVersion()
        {
            try
            {


                Version lastVersion = null;
                SyndicationItem objVersion = null;

                XmlReader reader = XmlReader.Create("http://mycollectionsupdates.blogspot.com/feeds/posts/default");
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                if (feed != null)
                {
                    foreach (SyndicationItem item in feed.Items)
                    {
                        lastVersion = new Version(item.Title.Text);
                        objVersion = item;
                        break;
                    }
                }

                reader.Close();

                Version currentVersion = Utils.Util.GetAppVersion();

                if (lastVersion != null && currentVersion != null && lastVersion.CompareTo(currentVersion) > 0)
                    return objVersion;

                return null;
            }
            //FIX 2.7.12.0
            catch (Exception)
            {
                return null;
            }
        }
        public static News GetLastNews(string id)
        {
            int latest = 0;
            if (string.IsNullOrWhiteSpace(id) == false)
                int.TryParse(id, out latest);

            return Utils.Util.GetLatestNews(latest);
        }
    }
}
