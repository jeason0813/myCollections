using System.Text;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using System;
using System.Collections.Generic;

namespace myCollections.UserControls
{
    public partial class UcGamezDetail
    {
        private Gamez _objItem;

        public UcGamezDetail(string strId)
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
            _objItem = new GameServices().Get(Id) as Gamez;
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

            //Fix since 2.5.5.0
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

            tooltip.Clear();
            lstLinks.Items.Clear();

            //Fix since 2.5.5.0
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
    }
}