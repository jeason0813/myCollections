using System;
using System.Collections.Generic;
using System.Text;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using myCollections.Pages;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace myCollections.UserControls
{

    public partial class UcXxxDetail
    {
        private XXX _objItem;

        public UcXxxDetail(string strId)
        {
            InitializeComponent();
            Refresh(strId);
        }
        public string Id { get; set; }
        public void Refresh(string strId)
        {
            Id = strId;
            LoadItem();
        }
        private void LoadItem()
        {
            _objItem = new XxxServices().Get(Id) as XXX;
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

            titleType.ToolTip = tooltip.ToString().Trim();
            lblType.Text = strType.ToString().Trim();
            lblType.ToolTip = titleType.ToolTip;

            tooltip.Clear();

            lstLinks.Items.Clear();

            foreach (Links item in _objItem.Links)
            {
                if (lstLinks.Items.IndexOf(item.Path) == -1)
                {
                    lstLinks.Items.Add(item.Path.Trim());
                    tooltip.Append(item.Path.Trim() + Environment.NewLine);
                }
            }
            lstLinks.ToolTip = tooltip.ToString().Trim();
            titleLinks.ToolTip = lstLinks.ToolTip;

            #region Cast
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

            #endregion
        }
        void artist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string link = ((Hyperlink)sender).TargetName;
            Task.Factory.StartNew(() => Util.NotifyEvent("ArtistDetail:" + link));
            ArtistDetail page = new ArtistDetail(link, EntityType.XXX);
            page.ShowDialog();
        }
    }
}