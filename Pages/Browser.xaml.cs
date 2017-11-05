using System;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading.Tasks;
using myCollections.Utils;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser
    {
        private readonly string _url;
        public Browser(string strUrl)
        {
            
            InitializeComponent();
            _url = strUrl;
            LocalBrowser.Navigated += LocalBrowser_Navigated;
            Task.Factory.StartNew(() => Util.NotifyEvent("Navigate :" + strUrl));
        }

        void LocalBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            SetSilent(LocalBrowser, true); // make it silent
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LocalBrowser.Navigate(new Uri(_url, UriKind.RelativeOrAbsolute));
        }

        private static void SetSilent(WebBrowser browser, bool silent)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid iidIWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid iidIWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref iidIWebBrowserApp, ref iidIWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
                }
            }
        }

        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }


    }
}