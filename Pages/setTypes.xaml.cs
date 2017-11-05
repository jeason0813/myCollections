using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using myCollections.BL.Services;
using myCollections.Data;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System.Windows.Input;

namespace myCollections.Pages
{
    public partial class SetTypes 
    {
        private readonly IList _types;
        private readonly IList _usedTypes;
        private readonly IList<ThumbItem> _items;
        readonly IServices _service;

        public SetTypes(IList lstSelectedItem, EntityType entityType)
        {
            InitializeComponent();

            _items = lstSelectedItem.Cast<ThumbItem>().ToList();
            IEnumerable<string> ids = _items.Select(t => t.Id).Distinct();
            _service = Util.GetService(entityType);

            _types = _service.GetTypesName();
            _usedTypes = _service.GetItemTypes(ids);

            AddCheckBox();
        }

        private void AddCheckBox()
        {
            foreach (string item in _types)
            {
                if (string.IsNullOrWhiteSpace(item) == false)
                {
                    CheckBox objCheckBox = new CheckBox();
                    objCheckBox.Margin = new Thickness(5, 0, 0, 0);
                    objCheckBox.Content = item;
                    objCheckBox.IsChecked = false;

                    foreach (string type in _usedTypes)
                    {
                        if (type == item)
                        {
                            objCheckBox.IsChecked = true;
                            break;
                        }
                    }
                    RootPanel.Children.Add(objCheckBox);
                }
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cmdUpdate_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            List<string> lstGenres = (from CheckBox item in RootPanel.Children 
                                      where item.IsChecked == true 
                                      select item.Content.ToString()).ToList();

            foreach (ThumbItem item in _items)
            {
                IMyCollectionsData entity = _service.Get(item.Id);
                GenreServices.UnlinkGenre(entity);
                GenreServices.AddGenres(lstGenres, entity, true);
            }

            Close();
            Cursor = null;
        }
    }
}
