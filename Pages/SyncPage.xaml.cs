using System.Windows;
using myCollections.BL.Services;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for SyncPage.xaml
    /// </summary>
    public partial class SyncPage
    {
        public SyncPage()
        {
            InitializeComponent();
        }

        private void mniSelect_Click(object sender, RoutedEventArgs e)
        {
            lstResults.SelectAll();
        }

        private void mniUnselect_Click(object sender, RoutedEventArgs e)
        {
            lstResults.UnselectAll();
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            CommonServices.DeleteSelectedListBoxItem(lstResults.SelectedItems);
            Close();
        }
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
