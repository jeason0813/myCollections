using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using myCollections.BL.Services;
using myCollections.Utils;
using System.Windows.Input;

namespace myCollections.Pages
{
   
    public partial class PreviewTvix
    {
        private readonly ImageSource _objImage;
        private string _strOutputPath;
        public PreviewTvix(ImageSource objImage, string strOutputPath)
        {
            InitializeComponent();
            _objImage = objImage;
            imgPreview.Source = _objImage;
            _strOutputPath = strOutputPath;
        }
       
        private void CatchException(Exception ex)
        {
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Wait;
                _strOutputPath  = ThemeServices.CreatePreviewTvix((BitmapSource)_objImage, _strOutputPath,DevicesServices.GetDevice());
                new MessageBoxYesNo("Layout generated in : " + _strOutputPath, false, false).ShowDialog();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            finally
            {
                Cursor = null;
            }
        }
    }
}
