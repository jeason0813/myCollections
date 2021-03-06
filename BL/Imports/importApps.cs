﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL.Imports
{
    class ImportApps: IProgressOperation, IDisposable
    {
        private readonly XNode[] _selectedItems;
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private string _message;

        public ImportApps(XElement[] nodes)
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

                    Apps apps = new Apps();
                    apps.Title = Util.GetElementValue(node, "Title");
                    apps.BarCode = Util.GetElementValue(node, "BarCode");
                    apps.Comments = Util.GetElementValue(node, "Comments");
                    apps.Description = Util.GetElementValue(node, "Description");
                    apps.FileName = Util.GetElementValue(node, "FileName");
                    apps.FilePath = Util.GetElementValue(node, "FilePath");
                    apps.Version = Util.GetElementValue(node, "Version");

                    #region DateTime
                    DateTime dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "AddedDate"), out dateValue) == true)
                        apps.AddedDate = dateValue;

                    if (DateTime.TryParse(Util.GetElementValue(node, "ReleaseDate"), out dateValue) == true)
                        apps.ReleaseDate = dateValue;
                    #endregion
                    #region Bool
                    bool boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsComplete"), out boolValue) == true)
                        apps.IsComplete = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsDeleted"), out boolValue) == true)
                        apps.IsDeleted = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsTested"), out boolValue) == true)
                        apps.Watched = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "IsWhish"), out boolValue) == true)
                        apps.IsWhish = boolValue;

                    if (bool.TryParse(Util.GetElementValue(node, "ToBeDeleted"), out boolValue) == true)
                        apps.ToBeDeleted = boolValue;
                    #endregion
                    #region Long
                    int longValue;

                    if (int.TryParse(Util.GetElementValue(node, "Rating"), out longValue) == true)
                        apps.MyRating = longValue;
                    #endregion
                    #region Media
                    var query = from item in node.Descendants("Media")
                                select item;

                    XElement[] bookNode = query.ToArray();

                    foreach (XElement media in bookNode)
                    {
                        Media newMedia = MediaServices.Get(Util.GetElementValue(media, "Name"),true);
                        newMedia.Path = Util.GetElementValue(media, "Path");
                        apps.Media = newMedia;
                    }
                    #endregion
                    #region Publisher
                    query = from item in node.Descendants("Editor")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement editor in bookNode)
                    {
                        bool isNew;
                        apps.Publisher = PublisherServices.GetPublisher(Util.GetElementValue(editor, "Name"), out isNew, "App_Editor");
                        if (isNew == true)
                            Dal.GetInstance.AddPublisher("App_Editor", apps.Publisher);
                    }
                    #endregion
                    #region Language
                    query = from item in node.Descendants("Language")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement languages in bookNode)
                        apps.Language = LanguageServices.GetLanguage(Util.GetElementValue(languages, "DisplayName"),true);
                    #endregion
                    #region Links
                    query = from item in node.Descendants("Link")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement link in bookNode)
                        LinksServices.AddLinks(Util.GetElementValue(link, "Path"), apps,true);

                    #endregion
                    #region Types
                    query = from item in node.Descendants("Type")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement type in bookNode)
                        GenreServices.AddGenres(new [] {Util.GetElementValue(type, "RealName")}, apps,true);
                    #endregion
                    #region Image
                    query = from item in node.Descendants("Ressource")
                            select item;

                    bookNode = query.ToArray();

                    foreach (XElement images in bookNode)
                    {
                        if (Util.GetElementValue(images, "ResourcesType") == "Image")
                        {
                            bool isDefault = bool.Parse(Util.GetElementValue(images, "IsDefault"));
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddImage(cover, apps, isDefault);
                        }
                        if (Util.GetElementValue(images, "ResourcesType") == "Background")
                        {
                            byte[] cover = Convert.FromBase64String(Util.GetElementValue(images, "Value"));

                            if (cover.Length > 0)
                                RessourcesServices.AddBackground(cover, apps);
                        }
                    }
                    #endregion

                    Dal.GetInstance.AddApps(apps);
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