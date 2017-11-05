using System;
using System.Net;

namespace myCollections.Utils
{
    class CookieAwareWebClient : WebClient
    {

        private readonly CookieContainer m_container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
           
            if (request is HttpWebRequest)
                (request as HttpWebRequest).CookieContainer = m_container;
            return request;
        }
    }
}
