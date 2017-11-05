using System.Collections.Generic;
using System.Windows;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using System;
using myCollections.Utils;
using System.Windows.Input;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for ManageData.xaml
    /// </summary>
    public partial class ManageCleanTitle
    {
        public ManageCleanTitle()
        {
            try
            {
                InitializeComponent();
                DataContext = CleanTitleServices.GetAll();
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

            IList<CleanTitle> objSource = CleanTitleServices.GetAll();
            IList<CleanTitle> objTarget = (IList<CleanTitle>)dtgData.DataContext;

            foreach (CleanTitle item in objSource)
            {
                if (!objTarget.Contains(item))
                    CleanTitleServices.Delete(item);
            }
            foreach (CleanTitle item in objTarget)
            {
                if (!objSource.Contains(item))
                    CleanTitleServices.Add(item);
            }
            Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}