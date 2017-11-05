using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for ChangeMediaPath.xaml
    /// </summary>
    public partial class ChangeMediaInfo
    {
        private Media _objMedia;
        public ChangeMediaInfo()
        {
            InitializeComponent();
            cboMedia.DataContext = MediaServices.GetNames();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_objMedia != null)
                MediaServices.UpdatePath(_objMedia.Name, txtName.Text, txtPath.Text);
            Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cboMedia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(cboMedia.SelectedValue.ToString()) == false)
            {
                _objMedia = MediaServices.Get(cboMedia.SelectedValue.ToString(),false);
                txtName.Text = _objMedia.Name;
                txtPath.Text = _objMedia.Path;
                if (_objMedia.FreeSpace.HasValue)
                    lblFreeSpace.Text = _objMedia.FreeSpace.ToString() + " Gb";
                else
                    lblFreeSpace.Text = string.Empty;

                if (_objMedia.TotalSpace.HasValue)
                    lblTotalSpace.Text = _objMedia.TotalSpace.ToString() + " Gb";
                else
                    lblTotalSpace.Text = string.Empty;

                if (_objMedia.LastUpdate.HasValue)
                    lblLastUpdate.Text = _objMedia.LastUpdate.Value.ToShortDateString();
                else
                    lblLastUpdate.Text = string.Empty;

                if (_objMedia.FreeSpace < _objMedia.TotalSpace / 10)
                    lblFreeSpace.Foreground = new SolidColorBrush(Colors.Red);

                else if (_objMedia.FreeSpace < _objMedia.TotalSpace / 15)
                    lblFreeSpace.Foreground = new SolidColorBrush(Colors.Orange);
                else
                    lblFreeSpace.Foreground = lblLastUpdate.Foreground;

            }
        }
    }
}
