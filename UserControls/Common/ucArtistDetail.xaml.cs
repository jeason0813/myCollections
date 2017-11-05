using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System.Windows.Input;

namespace myCollections.UserControls.Common
{
    public partial class UcArtistDetail
    {
        public Artist Artist { get; set; }
        public EntityType EntityType { get; set; }

        public UcArtistDetail()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            if (Artist !=null  &&string.IsNullOrWhiteSpace(Artist.FulleName) == false)
            {
                string name = Artist.FulleName;
                Task.Factory.StartNew(() => Util.NotifyEvent("ArtistDetail:" + name));
            }

            IList<string> lstBooks = ArtistServices.GetOwnedBooks(Artist);
            IList<string> lstMusic = ArtistServices.GetOwnedMusic(Artist);
            IList<string> lstMovies = ArtistServices.GetOwnedMovies(Artist);
            IList<string> lstSeries = ArtistServices.GetOwnedSeries(Artist);
            IList<string> lstXxx = ArtistServices.GetOwnedXxx(Artist);

            if (Artist != null && Artist.IsOld==false)
            {
                if (Artist.ArtistCredits == null || Artist.ArtistCredits.Any()==false)
                    Artist.ArtistCredits = ArtistServices.GetCredits(Artist);

                lblName.Text = Artist.FulleName;
                if (Artist.BirthDay.HasValue)
                {
                    lblBirth.Text = Artist.BirthDay.Value.ToLongDateString();
                    txtAge.Text = (DateTime.Now.Year - Artist.BirthDay.Value.Year).ToString(CultureInfo.InvariantCulture);
                    lblBorn.Visibility = Visibility.Visible;
                    txtAge.Visibility = Visibility.Visible;
                    lblAge.Visibility = Visibility.Visible;
                }
                else
                {
                    lblBorn.Visibility = Visibility.Collapsed;
                    txtAge.Visibility = Visibility.Collapsed;
                    lblAge.Visibility = Visibility.Collapsed;
                }

               ShowPicture();

                if (string.IsNullOrWhiteSpace(Artist.Ethnicity) == false)
                    lblEthnicity.Text = Artist.Ethnicity;

                if (string.IsNullOrWhiteSpace(Artist.Breast) == false)
                    lblBreast.Text = Artist.Breast;

                if (string.IsNullOrWhiteSpace(Artist.Bio) == false)
                    lblBio.Text = Artist.Bio;
                else
                    lblBio.Text = "No Bio !";

                if (string.IsNullOrWhiteSpace(Artist.PlaceBirth) == false)
                {
                    lblBirthPlace.Text = Artist.PlaceBirth;
                    lblIn.Visibility = Visibility.Visible;
                    lblBorn.Visibility = Visibility.Visible;
                }
                else
                    lblIn.Visibility = Visibility.Collapsed;

                if (string.IsNullOrWhiteSpace(Artist.WebSite) == false)
                    lblSites.Text = Artist.WebSite;
            }

            if (Artist != null && string.IsNullOrWhiteSpace(Artist.Aka) == false)
                lblAka.Text = Artist.Aka;

            wrpOwnItems.Children.Clear();

               if (lstBooks != null)
                foreach (string item in lstBooks)
                {
                    Label objLabel = new Label();
                    objLabel.Content = item;
                    wrpOwnItems.Children.Add(objLabel);
                }

            if (lstMovies != null)
                foreach (string item in lstMovies)
                {
                    Label objLabel = new Label();
                    objLabel.Content = item;
                    wrpOwnItems.Children.Add(objLabel);
                }

            if (lstMusic != null)
                foreach (string item in lstMusic)
                {
                    Label objLabel = new Label();
                    objLabel.Content = item;
                    wrpOwnItems.Children.Add(objLabel);
                }

            if (lstSeries != null)
                foreach (string item in lstSeries)
                {
                    Label objLabel = new Label();
                    objLabel.Content = item;
                    wrpOwnItems.Children.Add(objLabel);
                }

            if (lstXxx != null)
                foreach (string item in lstXxx)
                {
                    Label objLabel = new Label();
                    objLabel.Content = item;
                    if (Artist.ArtistCredits != null)
                    {
                        ArtistCredits credit = Artist.ArtistCredits.FirstOrDefault(x => x.Title.ToUpper() == item.ToUpper());

                        if (credit != null && string.IsNullOrEmpty(credit.Notes) == false)
                            objLabel.ToolTip = credit.Notes;
                    }

                    wrpOwnItems.Children.Add(objLabel);
                }

            wrpToBuyItems.Children.Clear();
            if (Artist.ArtistCredits != null)
            {
                IEnumerable<ArtistCredits> credits = Artist.ArtistCredits.Distinct(new ArtistCreditComparer()).OrderBy(x=>x.Title);
                foreach (ArtistCredits item in credits)
                {
                    if ((lstBooks == null || lstBooks.Contains(item.Title, StringComparer.InvariantCultureIgnoreCase) == false) &&
                        (lstMovies == null || lstMovies.Contains(item.Title, StringComparer.InvariantCultureIgnoreCase) == false) &&
                        (lstMusic == null || lstMusic.Contains(item.Title, StringComparer.InvariantCultureIgnoreCase) == false) &&
                        (lstSeries == null || lstSeries.Contains(item.Title, StringComparer.InvariantCultureIgnoreCase) == false) &&
                        (lstXxx == null || lstXxx.Contains(item.Title, StringComparer.InvariantCultureIgnoreCase) == false))
                    {
                        UcBuyLink objLabel = new UcBuyLink();
                        objLabel.lblTitle.Text = item.Title;

                        if (item.ReleaseDate.HasValue)
                            objLabel.lblTitle.ToolTip = (item.ReleaseDate.Value.Year + " " + item.Notes).Trim();
                        else if (item.Notes != null)
                            objLabel.lblTitle.ToolTip = item.Notes.Trim();

                        if (item.BuyLink != null && string.IsNullOrWhiteSpace(item.BuyLink)==false)
                        {
                            objLabel.lnkToBuy.NavigateUri = new Uri(item.BuyLink);
                            objLabel.lblToBuy.ToolTip = item.BuyLink;
                        }
                        else
                            objLabel.lblToBuy.Visibility = Visibility.Collapsed;

                        wrpToBuyItems.Children.Add(objLabel);
                    }
                }
            }
        }
        private void ShowPicture()
        {
            if (Artist.Picture != null)
                imgPicture.Source = Util.CreateImage(Artist.Picture);
            else
            {
                BitmapImage objImage = new BitmapImage();
                objImage.BeginInit();
                objImage.UriSource = new Uri("pack://application:,,,/myCollections;component/Images/elvis64.png");
                objImage.DecodePixelHeight = 50;
                objImage.EndInit();
                objImage.Freeze();
                imgPicture.Source = objImage;
            }
        }

        private void mniDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (Artist.Picture != null)
            {
                Artist.Picture = null;
                ShowPicture();
            }

        }
        private void mniReplaceImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog objDialog = new OpenFileDialog();
            objDialog.InitialDirectory = "c:\\";
            objDialog.Filter = "JPEG files (*.jpg)|*.jpg|Png files (*.png)|*.png|All files (*.*)|*.*";
            objDialog.FilterIndex = 2;
            objDialog.RestoreDirectory = true;

            if (objDialog.ShowDialog() == true)
                if (string.IsNullOrEmpty(objDialog.FileName) == false)
                {
                    Artist.Picture = Util.LoadImageData(objDialog.FileName);
                    ShowPicture();
                }
        }

        private void mniDelete_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            ArtistServices.DeleteCredits(Artist);
            wrpToBuyItems.Children.Clear();
            Cursor = null;
        }

        private void mniDeleteBio_Click(object sender, RoutedEventArgs e)
        {
            Artist.Bio = string.Empty;
            lblBio.Text = string.Empty;
        }

    }
}
