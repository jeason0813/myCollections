using System.Text;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using System;
using System.Collections.Generic;

namespace myCollections.UserControls
{
    /// <summary>
    /// Interaction logic for ucBooksDetail.xaml
    /// </summary>
    public partial class UcBooksDetail
    {
        private Books _objItem;

        public UcBooksDetail(string strId)
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
            _objItem = new BookServices().Get(Id) as Books;
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
            int i = 0;

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

            strType = new StringBuilder();
            tooltip = new StringBuilder();

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

            lstLinks.Items.Clear();

            tooltip.Clear();
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

        }

    }
}