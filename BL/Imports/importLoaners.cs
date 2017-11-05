using System;
using myCollections.Controls;
using myCollections.Utils;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using myCollections.Data.SqlLite;

namespace myCollections.BL.Imports
{
    internal sealed class ImportLoaners : IProgressOperation, IDisposable
    {
        private readonly List<string> _selectedItems;
        private int _current;
        private int _intAddedItem;
        private int _intNotAddedItem;
        private bool _isCancelationPending;
        private int _total;
        private string _message;


        public ImportLoaners(IList selectedItems)
        {
            _current = 1;
            _selectedItems = selectedItems as List<string>;

        }

        #region IProgressOperation Members

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;


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
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Total = _selectedItems.Count;

                foreach (string item in _selectedItems)
                {
                    //exit if the user cancels
                    if (_isCancelationPending == true)
                        return;


                    if (string.IsNullOrWhiteSpace(item) == false)
                    {
                        string[] attributes = item.Split(';');

                        if (string.IsNullOrWhiteSpace(attributes[0]) == false)
                        {
                            Friends friend = new Friends();
                            if (attributes.Length >= 9)
                            {
                                friend.Adresse = attributes[6] + " " + attributes[7] + " " + attributes[8] + " " +
                                                 attributes[9];
                                friend.Alias = attributes[0];

                                DateTime birtdate;
                                if (DateTime.TryParse(attributes[3], out birtdate))
                                    friend.BirthDate = birtdate;

                                friend.EMail = attributes[5];
                                friend.FirstName = attributes[1];
                                friend.LastName = attributes[2];
                                friend.PhoneNumber = attributes[10];

                                if (attributes[4].ToUpper() == "M")
                                    friend.Sex = true;
                                else
                                    friend.Sex = false;

                                Friends bExist = Dal.GetInstance.GetFriendByName(friend.Alias);
                                if (bExist == null)
                                {
                                    Dal.GetInstance.AddFriend(friend);
                                    _intAddedItem++;
                                }
                                else
                                {
                                    _intNotAddedItem++;
                                }
                            }
                        }
                    }
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

