using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Imports
{
    class ImportNds : IProgressOperation, IDisposable
    {
        private readonly XNode[] _selectedItems;
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private string _message;

        public ImportNds(XElement[] nodes)
        {
            _current = 1;
            _selectedItems = nodes;
        }
        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;


        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork_XML;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        public int Total
        {
            get { return _total; }
            private set
            {
                _total = value;
                OnProgressTotalChanged(EventArgs.Empty);
            }
        }

        public int AddedItem
        {
            get { return _intAddedItem; }
        }
        public int NotAddedItem
        {
            get { return _intNotAddedItem; }
        }
        public int Current
        {
            get { return _current; }
            private set
            {
                _current = value;
                OnProgressChanged(EventArgs.Empty);
            }
        }
        public IList RemovedItems
        {
            get { return null; }
        }
        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;
                OnMessageChanged(EventArgs.Empty);
            }
        }
        public void CancelAsync()
        {
            _isCancelationPending = true;
        }

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        private void worker_DoWork_XML(object sender, DoWorkEventArgs e)
        {
            try
            {
                Total = _selectedItems.Length;

                foreach (XElement node in _selectedItems)
                {

                    //exit if the user cancels
                    if (_isCancelationPending == true)
                        return;

                    Nds nds = new Nds();
                    nds.Title = Util.GetElementValue(node, "Title");
                    nds.BarCode = Util.GetElementValue(node, "BarCode");
                    nds.Comments = Util.GetElementValue(node, "Comments");
                    nds.Description = Util.GetElementValue(node, "Description");
                    nds.FileName = Util.GetElementValue(node, "FileName");
                    nds.FilePath = Util.GetElementValue(node, "FilePath");

                    DateTime dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "AddedDate"), out dateValue) == true)
                        nds.AddedDate = dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "ReleaseDate"), out dateValue) == true)
                        nds.ReleaseDate = dateValue;

                    #region Bool
                    bool boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsComplete"), out boolValue) == true)
                        nds.IsComplete = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsDeleted"), out boolValue) == true)
                        nds.IsDeleted = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsTested"), out boolValue) == true)
                        nds.Watched = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsWhish"), out boolValue) == true)
                        nds.IsWhish = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "ToBeDeleted"), out boolValue) == true)
                        nds.ToBeDeleted = boolValue;

                    #endregion
                    #region Long
                    long longValue;

                    if (long.TryParse(Util.GetElementValue(node, "Rated"), out longValue) == true)
                        nds.Rated = longValue.ToString(CultureInfo.InvariantCulture);

                    #endregion

                    int intValue;

                    if (int.TryParse(Util.GetElementValue(node, "Rating"), out intValue) == true)
                        nds.MyRating = intValue;

                    #region Media
                    var query = from item in node.Descendants("Media")
                                select item;

                    XElement[] mediaNode = query.ToArray();

                    foreach (XElement media in mediaNode)
                    {
                        Media newMedia = MediaServices.Get(Util.GetElementValue(media, "Name"), true);
                        newMedia.Path = Util.GetElementValue(media, "Path");
                        nds.Media = newMedia;
                    }
                    #endregion
                    #region Links
                    query = from item in node.Descendants("Link")
                            select item;

                    XElement[] linksNode = query.ToArray();

                    foreach (XElement link in linksNode)
                        LinksServices.AddLinks(Util.GetElementValue(link, "Path"), nds, true);

                    #endregion
                    #region Types
                    query = from item in node.Descendants("Type")
                            select item;

                    XElement[] typekNode = query.ToArray();

                    foreach (XElement type in typekNode)
                        GenreServices.AddGenres(new[] { Util.GetElementValue(type, "RealName") }, nds,true);
                    #endregion
                    #region Image
                    query = from item in node.Descendants("Ressource")
                            select item;

                    XElement[] imagesNode = query.ToArray();

                    foreach (XElement images in imagesNode)
                    {
                        if (Util.GetElementValue(images, "ResourcesType") == "Image")
                        {
                            bool isDefault = bool.Parse(Util.GetElementValue(images, "IsDefault"));
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddImage(cover, nds, isDefault);
                        }
                        if (Util.GetElementValue(images, "ResourcesType") == "Background")
                        {
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddBackground(cover, nds);
                        }
                    }
                    #endregion
                    #region Editor
                    query = from item in node.Descendants("Editor")
                            select item;

                    XElement[] editorNode = query.ToArray();

                    foreach (XElement editor in editorNode)
                    {
                        bool isNew;
                        nds.Publisher = PublisherServices.GetPublisher(Util.GetElementValue(editor, "Name"), out isNew, "App_Editor");
                        if (isNew == true)
                            Dal.GetInstance.AddPublisher("App_Editor", nds.Publisher);
                    }
                    #endregion
                    #region Language
                    query = from item in node.Descendants("Language")
                            select item;

                    XElement[] languageNode = query.ToArray();

                    foreach (XElement languages in languageNode)
                        nds.Language = LanguageServices.GetLanguage(Util.GetElementValue(languages, "DisplayName"),true);
                    #endregion
                    Dal.GetInstance.AddNds(nds);
                    _intAddedItem++;

                    Current++;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnComplete(EventArgs.Empty);
        }

        private void OnProgressTotalChanged(EventArgs e)
        {
            if (ProgressTotalChanged != null)
                ProgressTotalChanged(this, e);
        }
        private void OnProgressChanged(EventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }
        private void OnComplete(EventArgs e)
        {
            if (Complete != null)
                Complete(this, e);
        }
        private void OnMessageChanged(EventArgs e)
        {
            if (MessageChanged != null)
                MessageChanged(this, e);
        }
    }
}