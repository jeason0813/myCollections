//using System;
//using System.Collections;
//using System.Collections.ObjectModel;
//using System.Xml.Linq;
//using myCollections.Data;
//using myCollections.Data.Rest;
//using myCollections.Utils;

//namespace myCollections.BL.Providers
//{
//    static class YahooMusicServices
//    {
//        public static Hashtable Parse(string strId)
//        {

//            Hashtable objResuls = new Hashtable();
//            try
//            {
//                Uri strUrl = new Uri(string.Format(@"http://us.music.yahooapis.com/release/v1/item/{0}?appid={1}&format=xml&response=artists,main,tracks,categories,reviews", strId, "rKWgcDTV34FgmBxEryix0U4fMzTq_RVKzvZJjRvQk05fx8xmxUNfd8ChC0zn.7lgiQzx7AbnrUNDohm1aUpL7wBuOA--"));
//                XElement restResponse =  XElement.Parse(Util.GetRest(strUrl));
//                YahooMusic objYahooMusic = YahooMusic.albumToObject(restResponse);

//                if (objYahooMusic != null)
//                {

//                    objResuls.Add("Artist", objYahooMusic.artistName);
//                    objResuls.Add("BarCode", objYahooMusic.albumBarcode);
//                    objResuls.Add("Editor", objYahooMusic.studio);
//                    objResuls.Add("Links", objYahooMusic.albumUrl);
//                    objResuls.Add("Rating", objYahooMusic.albumRating);
//                    objResuls.Add("Released", objYahooMusic.albumReleased);
//                    objResuls.Add("Runtime", objYahooMusic.albumDuration);
//                    objResuls.Add("Tracks", objYahooMusic.albumTracks);

//                    if (objYahooMusic.albumTypes != null)
//                        objResuls.Add("Types", objYahooMusic.albumTypes);

//                    if (string.IsNullOrWhiteSpace(objYahooMusic.trackId) == false)
//                    {
//                        strUrl = new Uri(string.Format(@"http://us.music.yahooapis.com/track/v1/item/{0}?appid={1}&format=xml", objYahooMusic.trackId, "rKWgcDTV34FgmBxEryix0U4fMzTq_RVKzvZJjRvQk05fx8xmxUNfd8ChC0zn.7lgiQzx7AbnrUNDohm1aUpL7wBuOA--"));
//                        restResponse =  XElement.Parse(Util.GetRest(strUrl));
//                        objYahooMusic = YahooMusic.videoToObject(restResponse);
//                        if (objYahooMusic != null)
//                            if (objYahooMusic.albumImage != null)
//                                objResuls.Add("Image", objYahooMusic.albumImage);
//                    }
//                }

//                return objResuls;
//            }
//            catch (Exception ex)
//            {
//                Util.LogException(ex, strId);
//                return null;
//            }
//        }
//        public static Collection<PartialMatche> Search2(string strTrack)
//        {
//            Uri strUrl = new Uri(string.Format(@"http://us.music.yahooapis.com/release/v1/list/search/all/{0}?appid={1}&format=xml&response=artists&count=50", strTrack, "rKWgcDTV34FgmBxEryix0U4fMzTq_RVKzvZJjRvQk05fx8xmxUNfd8ChC0zn.7lgiQzx7AbnrUNDohm1aUpL7wBuOA--"));
//            XElement restResponse =  XElement.Parse(Util.GetRest(strUrl));
//            if (restResponse == null) return null;

//            return YahooMusic.albumToPartialMatche(restResponse);

//        }
//    }
//}
