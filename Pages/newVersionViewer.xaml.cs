using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace myCollections.Pages
{
    public partial class newVersionViewer
    {
        readonly FlowDocument _document;
        public newVersionViewer(string title, FlowDocument document, Uri strLink)
        {
            InitializeComponent();
            Title = Title + " " +  title;
            _document = document;
            hylLink.NavigateUri = strLink;
            hylLink.Inlines.Add(strLink.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_document == null)
                return;
            updateFrame.Document=_document;
        }

        private void hylLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
