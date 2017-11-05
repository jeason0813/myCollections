using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using myCollections.BL.Services;
using myCollections.Controls;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL
{
    sealed class UpdateItem : IProgressOperation
    {
        private readonly IEnumerable<ThumbItem> _objEntity;
        private int _total;
        private int _current;
        private bool _isCancelationPending;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private string _message;

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;


        public UpdateItem(IEnumerable<ThumbItem> objEntity)
        {
            _objEntity = objEntity;
            _total = 0;
            _current = 1;
            _isCancelationPending = false;
        }

        /// <summary>
        /// Starts the background operation that will export the event logs
        /// </summary>
        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Total = _objEntity.Count();
                IServices service=null;
                
                //Fix Since 2.5.5.0
                for (int i = 0; i < _objEntity.Count(); i++)
                {
                    ThumbItem item = _objEntity.ElementAt(i);

                    if (_isCancelationPending == true) break;

                    if (service == null)
                        service = Util.GetService(item.EType);

                    IMyCollectionsData currentItem = service.Get(item.Id);
                    service.GetInfoFromWeb(currentItem);
                    service.Add(currentItem);

                    Current++;
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnComplete(EventArgs.Empty);
        }

        public int Total
        {
            get
            {
                return _total;
            }
            private set
            {
                _total = value;
                OnProgressTotalChanged(EventArgs.Empty);
            }
        }
        public int AddedItem
        {
            get
            {
                return _intAddedItem;
            }
        }
        public int NotAddedItem
        {
            get { return _intNotAddedItem; }
        }
        public int Current
        {
            get
            {
                return _current;
            }
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
    }
}

