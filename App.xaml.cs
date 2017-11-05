using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using myCollections.Utils;
using myCollections.Pages;
using System.Threading.Tasks;

namespace myCollections
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private ResourceDictionary _objLoadedSkinResourceDictionary;
        private ResourceDictionary _objLoadedLanguageResourceDictionary;
        private Mutex _objMutex;
        public string Ip { get; set; }
        public Order CurrentOrder { get; set; }

        public ResourceDictionary LoadedLanguageResourceDictionary
        {
            get { return _objLoadedLanguageResourceDictionary; }
        }

        private Collection<ResourceDictionary> MergedDictionaries
        {
            get { return Resources.MergedDictionaries; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                bool isNewInstance;
                _objMutex = new Mutex(true, "myCollections.Singleton", out isNewInstance);
                if (!isNewInstance)
                    Current.Shutdown();

                Ip = string.Empty;
                CurrentOrder = Order.Name;

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                SplashScreen objScreen = new SplashScreen("Images/Splash2.png");
                objScreen.Show(true);

                foreach (ResourceDictionary item in MergedDictionaries)
                {
                    if (item.Source != null && item.Source.OriginalString.Contains(@"Skins\"))
                        _objLoadedSkinResourceDictionary = item;
                    if (item.Source != null && item.Source.OriginalString.Contains(@"Language\"))
                        _objLoadedLanguageResourceDictionary = item;
                }

                //Used to display date time in correct culture
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), 
                    new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        public void ApplySkin(Uri objSkinDictionaryUri)
        {
            if (string.IsNullOrEmpty(objSkinDictionaryUri.OriginalString) == false)
            {
                ResourceDictionary objNewSkinDictionary = (ResourceDictionary)(LoadComponent(objSkinDictionaryUri));

                if (objNewSkinDictionary != null)
                {
                    MergedDictionaries.Remove(_objLoadedSkinResourceDictionary);
                    _objLoadedSkinResourceDictionary = objNewSkinDictionary;
                    MergedDictionaries.Add(objNewSkinDictionary);
                }
            }
        }
        public void ApplyLanguage(Uri objLanguageDictionaryUri)
        {
            if (string.IsNullOrEmpty(objLanguageDictionaryUri.OriginalString) == false)
            {
                ResourceDictionary objNewLanguageDictionary =
                    (ResourceDictionary)(LoadComponent(objLanguageDictionaryUri));

                if (objNewLanguageDictionary != null)
                {
                    MergedDictionaries.Remove(_objLoadedLanguageResourceDictionary);
                    _objLoadedLanguageResourceDictionary = objNewLanguageDictionary;
                    MergedDictionaries.Add(objNewLanguageDictionary);
                }
            }
        }
        private static void CatchException(Exception ex)
        {
            Task.Factory.StartNew(() => Util.LogException(ex));
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            CatchException(e.Exception);
            e.Handled = true;
        }
    }
}