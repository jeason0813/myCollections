using System.Threading.Tasks;
using System.Windows;
using myCollections.Pages;
using myCollections.Utils;

namespace myCollections.UserControls.Common
{
    /// <summary>
    /// Interaction logic for ucBuyLinkxaml.xaml
    /// </summary>
    public partial class UcBuyLink
    {
        public UcBuyLink()
        {
            InitializeComponent();
        }

        private void lnkToBuy_Click(object sender, RoutedEventArgs e)
        {
            string url = lnkToBuy.NavigateUri.ToString();
            Task.Factory.StartNew(() => Util.NotifyEvent("ToBuy :" + url));
            Browser browser = new Browser(url);
            browser.Show();
        }
    }
}
