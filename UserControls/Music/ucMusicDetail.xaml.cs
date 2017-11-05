using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using System.Text;
using System.Collections.Generic;
using System;

namespace myCollections.UserControls
{
    public partial class UcMusicDetail
    {
        private Music _objItem;

        public UcMusicDetail(string id)
        {
            InitializeComponent();
            Refresh(id);
        }
        public string Id { get; set; }
        public void Refresh(string strId)
        {

            Id = strId;
            LoadItem();
        }
        private void LoadItem()
        {
            _objItem = new MusicServices().Get(Id) as Music;
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

            tooltip.Clear();
            strType.Clear();

            int i = 0;
            if (_objItem.Artists != null)
            {
                foreach (Artist item in _objItem.Artists)
                {
                    if (item.IsOld == false)
                    {
                        strType.Append(item.FirstName + " " + item.LastName);
                        tooltip.Append(item.FirstName + " " + item.LastName + Environment.NewLine);

                        i++;
                        if (i < _objItem.Artists.Count)
                            strType.Append(", ");
                    }
                }
            }
            titleAuthor.ToolTip = tooltip.ToString().Trim();
            lblAuthor.Text = strType.ToString().Trim();
            lblAuthor.ToolTip = tooltip.ToString().Trim();
        }
    }
}