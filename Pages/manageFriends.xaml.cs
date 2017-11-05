using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Pages
{
    public partial class ManageFriends
    {

        public ManageFriends()
        {
            InitializeComponent();
            Bind();
            lstUsers.DisplayMemberPath = "Alias";
            cboSex.Items.Add("Man");
            cboSex.Items.Add("Woman");
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                Friends objFriend = FriendServices.Get(txtAlias.Text);
                if (objFriend == null)
                    objFriend = new Friends();

                objFriend.Adresse = txtAdress.Text;
                objFriend.Alias = txtAlias.Text;

                if (string.IsNullOrEmpty(txtBirhtDate.Text) == false)
                {
                    DateTime datTemp;
                    if (DateTime.TryParse(txtBirhtDate.Text, out datTemp))
                        objFriend.BirthDate = datTemp;
                    else
                        objFriend.BirthDate = null;
                }
                else
                    objFriend.BirthDate = null;

                objFriend.Comments = txtComments.Text;
                objFriend.EMail = txtEmail.Text;

                string firstName;
                string lastName;
                FriendServices.SplitName(txtFullName.Text, out firstName, out lastName);
                objFriend.FirstName = firstName;
                objFriend.LastName = lastName;

                if (string.IsNullOrEmpty(txtMaxLoan.Text) == false)
                {
                    int lgnTemp;
                    if (int.TryParse(txtMaxLoan.Text, out lgnTemp))
                        objFriend.NbrMaxLoan = lgnTemp;
                    else
                        objFriend.NbrMaxLoan = 0;
                }
                else
                    objFriend.NbrMaxLoan = 0;

                objFriend.PhoneNumber = txtPhone.Text;
                objFriend.Picture = Util.SaveImageData(imgPhotos.Source);
                if (cboSex.Text == "Man")
                    objFriend.Sex = true;
                else
                    objFriend.Sex = false;

                FriendServices.Update(objFriend);

                Close();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            Cursor = null;
        }
        private static void CatchException(Exception ex)
        {
            Util.LogException(ex);
            new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
        }
        private void txtAlias_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableUpdate();
        }

        private void EnableUpdate()
        {
            if (string.IsNullOrEmpty(txtAlias.Text) == true)
                Ok.IsEnabled = false;
            else
                Ok.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableUpdate();
        }

        private void Bind()
        {
            lstUsers.ItemsSource = FriendServices.Gets();
        }

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Friends objFriend = lstUsers.SelectedValue as Friends;
            if (objFriend != null)
            {
                txtAdress.Text = objFriend.Adresse;
                txtAlias.Text = objFriend.Alias;
                if (objFriend.BirthDate != null)
                {
                    txtBirhtDate.Text = objFriend.BirthDate.Value.ToShortDateString();
                }
                txtComments.Text = objFriend.Comments;
                txtEmail.Text = objFriend.EMail;
                txtFullName.Text = objFriend.FirstName + " " + objFriend.LastName;
                txtMaxLoan.Text = objFriend.NbrMaxLoan.ToString(CultureInfo.InvariantCulture);
                txtPhone.Text = objFriend.PhoneNumber;

                if (objFriend.Sex == true)
                    cboSex.Text = "Man";
                else if (objFriend.Sex == false)
                    cboSex.Text = "Woman";

                imgPhotos.Source = Util.CreateImage(objFriend.Picture);
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                FriendServices.Delete(lstUsers.SelectedValue as Friends);
                Clean();
                Bind();
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
            Cursor = null;
        }

        private void Clean()
        {
            txtAdress.Text = string.Empty;
            txtAlias.Text = string.Empty;
            txtBirhtDate.Text = string.Empty;
            txtComments.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtFullName.Text = string.Empty;
            txtMaxLoan.Text = string.Empty;
            txtPhone.Text = string.Empty;
            cboSex.Text = string.Empty;
            imgPhotos.Source = new BitmapImage(new Uri("pack://application:,,,/myCollections;component/Images/monroe64.png"));
            cboSex.Text = string.Empty;
            lstUsers.SelectedItem = null;
        }

        private void cmdAdd_Click(object sender, RoutedEventArgs e)
        {
            Clean();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void mniReplaceImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog objDialog = new OpenFileDialog();
                objDialog.InitialDirectory = "c:\\";
                objDialog.Filter = "JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*";
                objDialog.FilterIndex = 2;
                objDialog.RestoreDirectory = true;

                if (objDialog.ShowDialog() == true)
                {
                    if (string.IsNullOrEmpty(objDialog.FileName) == false)
                    {
                        byte[] objImage = Util.LoadImageData(objDialog.FileName);
                        imgPhotos.Source = Util.CreateImage(objImage);
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }

        }
    }
}