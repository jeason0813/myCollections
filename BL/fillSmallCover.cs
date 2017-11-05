using System;
using System.Collections;
using System.ComponentModel;
using myCollections.Controls;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.BL
{
    class FillSmallCover : IProgressOperation
    {
        private readonly IList _objEntity;
        private readonly EntityType _entityType;
        private int _total;
        private int _current;
        private bool _isCancelationPending;
        private string _message;

        public event EventHandler ProgressTotalChanged;
        public event EventHandler ProgressChanged;
        public event EventHandler Complete;
        public event EventHandler MessageChanged;


        public FillSmallCover(IList objEntity, EntityType strEntityType)
        {
            _objEntity = objEntity;
            _entityType = strEntityType;

            _total = 0;
            _current = 0;
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
                Total = _objEntity.Count;
                
                switch (_entityType)
                {
                    #region Apps
                    case EntityType.Apps:
                        foreach (Apps item in _objEntity)
                        {
                            if (_isCancelationPending) break;

                            byte[] bigCover = Dal.GetInstance.GetBigCover("Apps_Ressources","Apps_Id", item.Id);

                            if (bigCover != null)
                            {
                                item.Cover = Util.CreateSmallCover(bigCover, Util.ThumbHeight, Util.ThumbWidth);
                                Dal.GetInstance.AddApps(item);
                            }

                            Current++;
                        }
                        break;
                    #endregion
                    #region Books
                    case EntityType.Books:
                        foreach (Books item in _objEntity)
                        {
                            if (_isCancelationPending) break;

                            byte[] bigCover = Dal.GetInstance.GetBigCover("Books_Ressources", "Books_Id", item.Id);

                            if (bigCover != null)
                            {
                                item.Cover = Util.CreateSmallCover(bigCover, Util.ThumbHeight, Util.ThumbWidth);
                                Dal.GetInstance.AddBook(item);
                            }

                            Current++;
                        }
                        break;
                    #endregion
                    #region Gamez
                    case EntityType.Games:
                        foreach (Gamez item in _objEntity)
                        {
                            if (_isCancelationPending) break;

                            byte[] bigCover = Dal.GetInstance.GetBigCover("Gamez_Ressources", "Gamez_Id", item.Id);

                            if (bigCover != null)
                            {
                                item.Cover = Util.CreateSmallCover(bigCover, Util.ThumbHeight, Util.ThumbWidth);
                                Dal.GetInstance.AddGame(item);
                            }

                            Current++;
                        }
                        break;
                    #endregion
                    #region Movies
                    case EntityType.Movie:
                        foreach (Movie item in _objEntity)
                        {
                            if (_isCancelationPending) break;
                            
                            byte[] bigCover = Dal.GetInstance.GetBigCover("Gamez_Ressources", "Gamez_Id", item.Id);

                            if (bigCover != null)
                            {
                                item.Cover = Util.CreateSmallCover(bigCover, Util.ThumbHeight, Util.ThumbWidth);

                                Dal.GetInstance.AddMovie(item);
                            }

                            Current++;
                        }
                        break;
                    #endregion
                    #region Music
                    case EntityType.Music:
                        foreach (Music item in _objEntity)
                        {
                            if (_isCancelationPending) break;

                            byte[] bigCover = Dal.GetInstance.GetBigCover("Music_Ressources", "Music_Id", item.Id);

                            if (bigCover != null)
                            {
                                item.Cover = Util.CreateSmallCover(bigCover, Util.ThumbHeight, Util.ThumbWidth);
                                Dal.GetInstance.AddMusic(item);
                            }

                            Current++;
                        }
                        break;
                    #endregion
                    #region Nds
                    case EntityType.Nds:
                        foreach (Nds item in _objEntity)
                        {
                            if (_isCancelationPending) break;

                            byte[] bigCover = Dal.GetInstance.GetBigCover("Nds_Ressources", "Nds_Id", item.Id);

                            if (bigCover != null)
                            {
                                item.Cover = Util.CreateSmallCover(bigCover, Util.ThumbHeight, Util.ThumbWidth);
                                Dal.GetInstance.AddNds(item);
                            }

                            Current++;
                        }
                        break;
                    #endregion
                    #region Series
                    case EntityType.Series:
                        foreach (SeriesSeason item in _objEntity)
                        {
                            if (_isCancelationPending) break;

                            byte[] bigCover = Dal.GetInstance.GetDefaultRessourceValue(item.Id, "SeriesSeason_Ressources", "SeriesSeason_Id");

                            if (bigCover != null)
                            {
                                item.Cover = Util.CreateSmallCover(bigCover, Util.ThumbHeight, Util.ThumbWidth);
                                Dal.GetInstance.AddSeriesSeason(item);
                            }

                            Current++;
                        }
                        break;
                    #endregion
                    #region XXX
                    case EntityType.XXX:
                        foreach (XXX item in _objEntity)
                        {
                            if (_isCancelationPending) break;

                            byte[] bigCover = Dal.GetInstance.GetDefaultRessourceValue(item.Id, "XXX_Ressources", "XXX_Id");

                            if (bigCover != null)
                            {
                                item.Cover = Util.CreateSmallCover(bigCover, Util.ThumbHeight, Util.ThumbWidth);
                                Dal.GetInstance.AddXxx(item);
                            }

                            Current++;
                        }
                        break;
                        #endregion
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

        public int AddedItem { get; private set; }
        public int NotAddedItem { get; private set; }
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
        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;
                OnMessageChanged(EventArgs.Empty);
            }
        }
        public IList RemovedItems
        {
            get { return null; }
        }

        private void OnProgressTotalChanged(EventArgs e)
        {
            if (ProgressTotalChanged != null)
            {
                ProgressTotalChanged(this, e);
            }
        }
        private void OnProgressChanged(EventArgs e)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, e);
            }
        }
        private void OnComplete(EventArgs e)
        {
            if (Complete != null)
            {
                Complete(this, e);
            }
        }
        private void OnMessageChanged(EventArgs e)
        {
            if (MessageChanged != null)
                MessageChanged(this, e);
        }

        /// <summary>
        /// Requests cancelation of the event log exporting
        /// </summary>
        public void CancelAsync()
        {
            _isCancelationPending = true;
        }
    }
}

