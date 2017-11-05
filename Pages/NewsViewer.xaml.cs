using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace myCollections.Pages
{
    public partial class NewsViewer
    {
        public NewsViewer()
        {
            InitializeComponent();
        }

        private void HylLink_OnClick(object sender, RoutedEventArgs e)
        {
            Hyperlink objLink = e.OriginalSource as Hyperlink;
            if (objLink != null)
                Process.Start(objLink.NavigateUri.ToString());
        }
    }
}
