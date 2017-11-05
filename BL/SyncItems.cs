
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL
{
    class SyncItems : IProgressOperation, IDisposable
    {
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private readonly IList _selectedItems;
        private readonly IList _removedItems;
        private int _total;
        private string _message;

        public SyncItems(IList mediaItems)
        {
            _current = 1;
            _selectedItems = mediaItems;
            _removedItems = new List<IMyCollectionsData>();
        }

        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;


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

        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
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

        public IList RemovedItems
        {
            get { return _removedItems; }
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

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (_selectedItems != null)
                {
                    Total = _selectedItems.Count;
                    foreach (IMyCollectionsData selectedItem in _selectedItems)
                    {
                        //Fix Since V2.5.5.0
                        if (string.IsNullOrWhiteSpace(selectedItem.FileName) == false)
                        {
                            string path = Path.Combine(selectedItem.FilePath, selectedItem.FileName);
                            if (File.Exists(path) == false && Directory.Exists(path) == false)
                            {
                                _removedItems.Add(selectedItem);
                                _intAddedItem++;
                            }
                        }
                        Current++;
                    }
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
    }
}
