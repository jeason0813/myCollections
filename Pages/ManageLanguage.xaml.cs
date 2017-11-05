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
    /// Interaction logic for ManageData.xaml
    /// </summary>
    public partial class ManageLanguage
    {
      
        public ManageLanguage()
        {
            try
            {
                InitializeComponent();
                DataContext = LanguageServices.GetAllLanguages();
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

            IList<Language> objSource = LanguageServices.GetAllLanguages();
            IList<Language> objTarget = (IList<Language>)dtgData.DataContext;

            foreach (Language item in objSource)
            {
                if (!objTarget.Contains(item))
                    LanguageServices.Delete(item);
            }
            foreach (Language item in objTarget)
            {
                if (!objSource.Contains(item))
                    LanguageServices.Add(item);
            }
            Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}