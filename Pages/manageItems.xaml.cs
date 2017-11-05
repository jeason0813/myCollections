using System.Collections;
using System.Collections.Generic;
using System.Windows;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;

namespace myCollections.Pages
{
    public partial class ManageItems
    {
        private readonly EntityType _itemsType;
        private readonly List<string> _added = new List<string>();

        public ManageItems(EntityType itemsType)
        {
            InitializeComponent();
            _itemsType = itemsType;
            DataContext = GenreServices.GetGenres(_itemsType);
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            IList objSource = GenreServices.GetGenres(_itemsType);
            IList objTarget = (IList)dtgData.DataContext;

            if (objSource != null)
                foreach (Genre item in objSource)
                {
                    Genre type = item;
                    if (!objTarget.Contains(type))
                        GenreServices.DeleteGenre(item, _itemsType);
                }
            foreach (Genre item in objTarget)
            {
                if (string.IsNullOrWhiteSpace(item.RealName) == false || string.IsNullOrWhiteSpace(item.DisplayName) == false)
                {
                    if (string.IsNullOrWhiteSpace(item.RealName))
                        item.RealName = item.DisplayName;

                    if (string.IsNullOrWhiteSpace(item.RealName) == false && _added.Contains(item.RealName) == false)
                    {
                        new GenreServices().AddGenre(item, _itemsType);
                        _added.Add(item.RealName);
                    }
                }
            }

            Close();

        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }
    }
}
