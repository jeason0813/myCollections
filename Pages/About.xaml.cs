using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About
    {
        public About()
        {
            InitializeComponent();
            lblVersion.Text = lblVersion.Text + " " + Utils.Util.GetAppVersion();
        }

        private void hypMail_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink objLink = e.OriginalSource as Hyperlink;
            if (objLink != null) Process.Start(objLink.NavigateUri.ToString());
        }
    }
}