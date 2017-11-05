using System.Text;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using System.Windows.Controls;
using System.Windows.Documents;
using myCollections.Utils;
using myCollections.Pages;
using System.Threading.Tasks;

namespace myCollections.UserControls
{
    public partial class UcSeriesDetail
    {
        private SeriesSeason _objItem;

        public UcSeriesDetail(string strId)
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
            _objItem = new SerieServices().Get(Id) as SeriesSeason;
            if (_objItem != null)
            {
                DataContext = _objItem;
                Bind();
            }
        }
        private void Bind()
        {
            lblTitle.Text = _objItem.Title + " - Season " + _objItem.Season;

            StringBuilder strType = new StringBuilder();
            foreach (Genre itemType in _objItem.Genres)
            {
                if (itemType != null)
                {
                    strType.Append(itemType.DisplayName);
                    strType.Append(" ");
                }
            }
            lblType.Text = strType.ToString().Trim();

            lstLinks.Items.Clear();

            foreach (Links item in _objItem.Links)
            {
                if (lstLinks.Items.IndexOf(item.Path) == -1)
                    lstLinks.Items.Add(item.Path.Trim());
            }

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
                        artist.TargetName = item.Id;
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
            Artist artist = ArtistServices.GetById(link);
            if (artist != null)
            {
                ArtistDetail page = new ArtistDetail(artist, EntityType.Series);
                page.ShowDialog();
            }
        }
    }
}