using System.Linq;
using System.Windows;
using System.Windows.Controls;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Pages;

namespace myCollections.UserControls.Common
{
    public partial class UcGameType
    {
        private Gamez _objEntity;
        private bool _bLoaded;

        public UcGameType()
        {
            InitializeComponent();
        }

        public Gamez CurrentEntity
        {
            get { return _objEntity; }
            set { _objEntity = value; }
        }

        public void Refresh()
        {
            if (_bLoaded == false)
                AddCheckBox();

            foreach (Genre itemType in _objEntity.Genres)
            {
                if (itemType != null && itemType.IsOld == false)
                {

                    bool bExist = false;
                    foreach (CheckBox item in RootPanel.Children)
                    {

                        if (string.IsNullOrWhiteSpace(itemType.DisplayName) == false)
                            if (item.Content.ToString() == itemType.DisplayName)
                            {
                                item.IsChecked = true;
                                bExist = true;
                                break;
                            }
                    }

                    if (bExist == false)
                    {
                        if (string.IsNullOrWhiteSpace(itemType.DisplayName) == false)
                        {
                            CheckBox objCheckBox = new CheckBox();
                            objCheckBox.Margin = new Thickness(5, 0, 0, 0);

                            objCheckBox.Content = itemType.DisplayName;
                            objCheckBox.IsChecked = true;
                            RootPanel.Children.Add(objCheckBox);
                        }
                    }
                }
            }
        }
        private void AddCheckBox()
        {
            _bLoaded = true;
            foreach (string item in new GameServices().GetTypesName())
            {
                if (string.IsNullOrWhiteSpace(item) == false)
                {
                    CheckBox objCheckBox = new CheckBox();
                    objCheckBox.Margin = new Thickness(5, 0, 0, 0);
                    objCheckBox.Content = item;
                    objCheckBox.IsChecked = false;

                    if (_objEntity.Genres != null)
                    {
                        foreach (Genre itemType in _objEntity.Genres)
                        {
                            if (itemType != null)
                                if (itemType.DisplayName == item)
                                {
                                    objCheckBox.IsChecked = true;
                                    break;
                                }
                        }
                    }
                    RootPanel.Children.Add(objCheckBox);
                }
            }
        }

        private void addCustomeType_Click(object sender, RoutedEventArgs e)
        {
            AddLink addlink = new AddLink();
            addlink.ShowDialog();
            if (string.IsNullOrWhiteSpace(addlink.Link) == false)
            {
                string newtype = addlink.Link;
                GenreServices.GetGenre(newtype, _objEntity.ObjectType);
                bool bExist = RootPanel.Children.Cast<CheckBox>().Any(item => item.Content.ToString() == newtype);

                if (bExist == false)
                {
                    CheckBox objCheckBox = new CheckBox();
                    objCheckBox.Margin = new Thickness(5, 0, 0, 0);

                    objCheckBox.Content = newtype;
                    objCheckBox.IsChecked = true;
                    RootPanel.Children.Add(objCheckBox);
                }
            }
        }
    }
}