using System;
using System.Text;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using System.Collections.Generic;
using System.Windows.Documents;
using myCollections.Utils;
using System.Windows.Controls;
using myCollections.Pages;
using System.Threading.Tasks;

namespace myCollections.UserControls
{
    public partial class UcMovieDetail
    {
        private Movie _objItem;

        public UcMovieDetail(string strId)
        {
            InitializeComponent();
            Refresh(strId);
        }
        private string Id { get; set; }

        private void Refresh(string strId)
        {
            Id = strId;
            LoadItem();
        }
        private void LoadItem()
        {
            _objItem = new MovieServices().Get(Id) as Movie;
            if (_objItem != null)
            {
                DataContext = _objItem;
                Bind();
            }
        }
        private void Bind()
        {
            StringBuilder strType = new StringBuilder();
            StringBuilder tooltip = new StringBuilder();
            List<string> addedType = new List<string>();

            #region Genre
            if (_objItem != null)
            {
                foreach (Genre itemType in _objItem.Genres)
                {
                    if (itemType != null)
                        if (string.IsNullOrWhiteSpace(itemType.DisplayName) == false)
                            if (addedType.Contains(itemType.DisplayName) == false)
                            {
                                strType.Append(itemType.DisplayName);
                                strType.Append(" ");
                                tooltip.Append(itemType.DisplayName + Environment.NewLine);
                                addedType.Add(itemType.DisplayName);
                            }
                }
            }

            titleType.ToolTip = tooltip.ToString().Trim();
            lblType.Text = strType.ToString().Trim();
            lblType.ToolTip = titleType.ToolTip;
            #endregion
            #region Cast
            if (_objItem != null && _objItem.Artists !=null)
            {
                foreach (Artist item in _objItem.Artists)
                {
                    if (item != null)
                    {
                        if (string.IsNullOrWhiteSpace(item.FulleName) == false)
                        {
                            TextBlock paragraph = new TextBlock();
                            Hyperlink artist = new Hyperlink();
                            artist.Click += artist_Click;
                            artist.TargetName = item.FulleName;
                            artist.Inlines.Add(item.FulleName);
                            paragraph.Inlines.Add(artist);
                            if (item.Picture != null)
                            {
                                Image picture = new Image();
                                picture.Height = 200;
                                picture.Source = Util.CreateImage(item.Picture);
                                ToolTipService.SetToolTip(artist, picture);
                            }
                            stcCast.Children.Add(paragraph);

                        }
                    }
                }
            }

            #endregion

            tooltip.Clear();
            lstLinks.Items.Clear();

            if (_objItem != null)
            {
                foreach (Links item in _objItem.Links)
                {
                    if (lstLinks.Items.IndexOf(item.Path) == -1)
                    {
                        lstLinks.Items.Add(item.Path.Trim());
                        tooltip.Append(item.Path.Trim() + Environment.NewLine);
                    }
                }
            }
            lstLinks.ToolTip = tooltip.ToString().Trim();
            titleLinks.ToolTip = lstLinks.ToolTip;
        }

        void artist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string link = ((Hyperlink)sender).TargetName;
            ArtistDetail page = new ArtistDetail(link, EntityType.Movie);
            page.ShowDialog();
        }
    }
}