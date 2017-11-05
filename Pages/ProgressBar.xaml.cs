using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using myCollections.Controls;

namespace myCollections.Pages
{
   
    public partial class ProgressBar : INotifyPropertyChanged
    {
        private readonly IProgressOperation _operation;
        private Visibility _progressVisibility = Visibility.Visible;
        private Visibility _waitVisibility = Visibility.Hidden;
        public ProgressBar(IProgressOperation operation)
        {
            _operation = operation;
            _operation.ProgressChanged += _operation_ProgressChanged;
            _operation.ProgressTotalChanged += _operation_TotalChanged;
            _operation.Complete += _operation_Complete;
            _operation.MessageChanged += _operation_MessageChanged;


            InitializeComponent();

            Loaded += ProgressWindow_Loaded;
        }

        public int Current
        {
            get { return _operation.Current; }
        }
        public Visibility ProgresseBarVisibility
        {
            get { return _progressVisibility; }
        }
        public Visibility WaitVisibility
        {
            get { return _waitVisibility; }
        }

        public string Message
        {
            get { return _operation.Message; }
        }
        public IList RemovedItems
        {
            get { return _operation.RemovedItems; }
        }

        public int Total
        {
            get { return _operation.Total; }
        }

        public int AddedItem
        {
            get { return _operation.AddedItem; }
        }
        public int NotAddedItem
        {
            get { return _operation.NotAddedItem; }
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        private void ProgressWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _operation.Start();
        }

        private void _operation_Complete(object sender, EventArgs e)
        {
            Close();
        }

        private void _operation_ProgressChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Current");
        }

        private void _operation_TotalChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Total");
        }

        void _operation_MessageChanged(object sender, EventArgs e)
        {
            _progressVisibility=Visibility.Hidden;
            OnPropertyChanged("ProgresseBarVisibility");
            _waitVisibility = Visibility.Visible;
            OnPropertyChanged("WaitVisibility");
            OnPropertyChanged("Message");
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            _operation.CancelAsync();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var p = PropertyChanged;
            if (p != null) 
                p(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}