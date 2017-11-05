using System;
using System.Windows;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for LoanTo.xaml
    /// </summary>
    public partial class LoanTo
    {
        private readonly EntityType _selectedItems;
        private readonly string _strId;
        public LoanTo(ThumbItem thumbitem)
        {
            InitializeComponent();

            if (thumbitem == null) return;

            _strId = thumbitem.Id;
            _selectedItems = thumbitem.EType;

            cboAlias.ItemsSource = FriendServices.Gets();
            cboAlias.DisplayMemberPath = "Alias";
            imgTitle.Source = Util.CreateSmallImage(thumbitem.Cover);
            lblTitle.Text = thumbitem.Name;

            Loan objLoan = LoanServices.Get(_strId);
            if (objLoan != null)
            {
                txtEndDate.Text = objLoan.EndDate.ToShortDateString();

                //Fix since 2.7.12.0
                if (objLoan.Friend != null)
                    cboAlias.Text = objLoan.Friend.Alias;

                txtStartDate.Text = objLoan.StartDate.ToShortDateString();
            }
            else
            {
                txtStartDate.Text = DateTime.Now.ToShortDateString();
                txtEndDate.Text = DateTime.Now.AddDays(10).ToShortDateString();
            }
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

        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Loan objLoan = LoanServices.Get(_strId);
                if (objLoan == null)
                    objLoan = new Loan();

                DateTime objDate;

                if (DateTime.TryParse(txtEndDate.Text, out objDate) == true)
                    objLoan.EndDate = objDate;
                else
                {
                    new MessageBoxYesNo("End Date is not valide", false, true).ShowDialog();
                    txtEndDate.Focus();
                    return;
                }

                if (DateTime.TryParse(txtStartDate.Text, out objDate) == true)
                    objLoan.StartDate = objDate;
                else
                {
                    new MessageBoxYesNo("Start Date is not valide", false, true).ShowDialog();
                    txtStartDate.Focus();
                    return;
                }
                string strFriends;
                if (string.IsNullOrEmpty(cboAlias.Text) == false)
                    strFriends = cboAlias.Text;
                else
                {
                    new MessageBoxYesNo("Please Choose a friend", false, true).ShowDialog();
                    cboAlias.Focus();
                    return;
                }

                objLoan.ItemId = _strId;

                LoanServices.Update(objLoan, strFriends, _selectedItems);
                Close();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }
    }
}