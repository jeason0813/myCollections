using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for manageMetaData.xaml
    /// </summary>
    public partial class ManageMetaData
    {
        public ManageMetaData()
        {
            try
            {

                InitializeComponent();
                DataContext = MetaDataServices.GetAll();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
        private void CatchException(Exception ex)
        {
            Cursor = null;
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            IList<MetaData> objSource = MetaDataServices.GetAll();
            IList<MetaData> objTarget = (IList<MetaData>)dtgData.DataContext;

            foreach (MetaData item in objSource)
            {
                if (!objTarget.Contains(item))
                    MetaDataServices.Delete(item);
            }
            foreach (MetaData item in objTarget)
            {
                if (!objSource.Contains(item) && string.IsNullOrWhiteSpace(item.Name) == false)
                    MetaDataServices.Add(item);
            }
            Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
