using System.Diagnostics;
using System.Windows;
using myCollections.BL.Services;

namespace myCollections.Pages
{
    public partial class DonatePage 
    {
        public DonatePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4UB72VRPLLZ8G");
            DatabaseServices.UpdateDonate();
        }

    }
}
