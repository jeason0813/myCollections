using System.Linq;
using System.Windows;
using System.Windows.Controls;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Pages;

namespace myCollections.UserControls.Common
{
    /// <summary>
    /// Interaction logic for ucGenres.xaml
    /// </summary>
    public partial class UcSeriesGenres
    {
        private SeriesSeason _objEntity;
        private bool _bLoaded;

        public UcSeriesGenres()
        {
            InitializeComponent();
        }

        public SeriesSeason CurrentEntity
        {
            get { return _objEntity; }
            set { _objEntity = value; }
        }

        public void Refresh()
        {

            if (_bLoaded == false)
                AddCheckBox();

            if (_objEntity != null)
            {
                if (_objEntity.Genres != null)
                    foreach (Genre itemGenre in _objEntity.Genres)
                    {
                        if (itemGenre != null && itemGenre.IsOld == false)
                        {

                            bool bExist = false;
                            foreach (CheckBox item in RootPanel.Children)
                            {
                                if (string.IsNullOrWhiteSpace(itemGenre.DisplayName) == false)
                                    if (item.Content.ToString() == itemGenre.DisplayName)
                                    {
                                        item.IsChecked = true;
                                        bExist = true;
                                        break;
                                    }
                            }

                            if (bExist == false)
                            {
                                if (string.IsNullOrWhiteSpace(itemGenre.DisplayName) == false)
                                {
                                    CheckBox objCheckBox = new CheckBox();
                                    objCheckBox.Margin = new Thickness(5, 0, 0, 0);

                                    objCheckBox.Content = itemGenre.DisplayName;
                                    objCheckBox.IsChecked = true;
                                    RootPanel.Children.Add(objCheckBox);
                                }
                            }
                        }
                    }
            }
        }
        private void AddCheckBox()
        {

            _bLoaded = true;
            foreach (string item in new MovieServices().GetTypesName())
            {
                if (string.IsNullOrWhiteSpace(item) == false)
                {
                    CheckBox objCheckBox = new CheckBox();
                    objCheckBox.Margin = new Thickness(5, 0, 0, 0);
                    objCheckBox.Content = item;
                    objCheckBox.IsChecked = false;

                    if (_objEntity != null)
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
                GenreServices.GetGenre(newtype,_objEntity.ObjectType);
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