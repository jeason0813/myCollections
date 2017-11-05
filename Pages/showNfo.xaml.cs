using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace myCollections.Pages
{
    public partial class showNfo
    {
        private readonly string _strPath;
        public showNfo(string strPath)
        {
            InitializeComponent();
            _strPath = strPath;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtTitle.Text = _strPath;

            TextRange objRange = new TextRange(rtbNfoViewer.Document.ContentStart, rtbNfoViewer.Document.ContentEnd);
            FileStream objStream = new FileStream(_strPath, FileMode.OpenOrCreate);
            objRange.Load(objStream, DataFormats.Text);
            objStream.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}